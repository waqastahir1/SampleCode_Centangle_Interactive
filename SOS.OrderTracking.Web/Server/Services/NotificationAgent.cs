using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Services
{
    public class NotificationAgent
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;
        private readonly IHubContext<ConsignmentHub> consignmentHub;
        private readonly UserCacheService userCache;
        private readonly FirebaseMessaging fcm;
        public NotificationAgent(IServiceScopeFactory serviceScopeFactory,
            ILogger<NotificationAgent> logger,
            IHubContext<ConsignmentHub> consignmentHub,
            UserCacheService userCache,
            IWebHostEnvironment env)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            this.consignmentHub = consignmentHub;
            this.userCache = userCache;
            var path = env.ContentRootPath;
            path += "\\google-auth.json";
            if (!File.Exists(path))
                throw new Exception("Google Credentials file is missing");

            FirebaseApp app;
            try
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path)
                }, "com.sos");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.ToString());
                app = FirebaseApp.GetInstance("com.sos");
            }
            fcm = FirebaseMessaging.GetMessaging(app);
        }

        public static BlockingCollection<int> WebPushNotificationsQueue { get; set; } = new BlockingCollection<int>();
       
        [Obsolete]
        public void StartFcmSerrvice()
        {
            // FCM Service
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var notification = await context.Notifications.FirstOrDefaultAsync(x => x.NotificationStatus == NotificationStatus.Waiting && x.NotificationMedium == NotificationMedium.Firebase);
                            if (notification == null)
                            {
                                await Task.Delay(4000);
                                continue;
                            }
                            if (notification.NotificationMedium == Shared.Enums.NotificationMedium.Firebase)
                            {
                                Message message = JsonConvert.DeserializeObject<Message>(notification.Description);
                                var user = await context.Users.FirstOrDefaultAsync(x => x.PartyId == notification.ReceiverId);
                                if (user == null)
                                {
                                    notification.NotificationStatus = Shared.Enums.NotificationStatus.UndefinedUser;
                                }
                                else
                                {
                                    message.Token = user.FCMToken;
                                    //notification.Description = JsonConvert.SerializeObject(message);
                                    notification.ReceiverUserName = user.UserName;
                                    notification.SentAt = DateTime.UtcNow;
                                    try
                                    {
                                        var responseId = await fcm.SendAsync(message);
                                        if (string.IsNullOrEmpty(responseId))
                                        {
                                            notification.NotificationStatus = Shared.Enums.NotificationStatus.FirbaseError;
                                        }
                                        else
                                        {
                                            notification.NotificationStatus = Shared.Enums.NotificationStatus.Sent;
                                            notification.TransactionId = responseId;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        notification.NotificationStatus = Shared.Enums.NotificationStatus.Error;
                                        notification.TransactionId = ex.ToString().Substring(0, 450);

                                        logger.LogError(ex.ToString());
                                    }
                                }
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });
        }

        public void StartWebPushService()
        {  
            Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
             
                while (true)
                {
                    try
                    {
                        var id = WebPushNotificationsQueue.Take(); 
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var webPushNotificationService = scope.ServiceProvider.GetRequiredService<WebPushNotificationService>();
                        var notification = await context.Notifications.FirstOrDefaultAsync(x => x.Id == id);

                        //notification.Description = JsonConvert.SerializeObject(message); 
                        NotificationSubscription subscription = null;
                        try
                        {
                            subscription = await context.NotificationSubscriptions.FirstOrDefaultAsync(e => e.UserId == notification.ReceiverUserName);
                            if (subscription == null)
                            {
                                throw new Exception($"Notification Reciever is not defined for {notification.Id}");
                            }

                            try
                            {
                                await webPushNotificationService.SendNotificationAsync(subscription, notification.Title, notification.Description);
                            }
                            catch(Exception ex)
                            {
                                logger.LogWarning($"{ex.Message} - {subscription.Id}, {subscription.UserId}");

                                if (notification != null)
                                { 
                                    notification.TransactionId = ex.Message;
                                    await context.SaveChangesAsync();
                                }
                            }

                            var connectionId = await userCache.GetHubConnectionId(notification.ReceiverUserName);
                            if (connectionId != null)
                            {
                                var client = consignmentHub.Clients.Client(connectionId);
                                if (client != null)
                                {
                                    await client.SendAsync("Notification", notification.Title); 
                                    notification.NotificationStatus = NotificationStatus.Sent;
                                    notification.SentAt = DateTime.UtcNow;
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (notification != null)
                            {
                                notification.NotificationStatus = NotificationStatus.Error;
                                notification.TransactionId = ex.Message;
                                await context.SaveChangesAsync();
                            }
                            logger.LogError(ex.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });
        }

        public void StartLocationService()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var crews = await (from o in context.Orgnizations
                                               where o.OrganizationType == OrganizationType.Crew
                                               select o
                                              ).ToListAsync();

                            foreach (var crew in crews)
                            {
                                try
                                {
                                    var crewLocation = await context.PartyLocations.Where(x => x.PartyId == crew.Id)
                                                       .OrderByDescending(x => x.TimeStamp)
                                                       .Select(x => x.Geolocation).FirstOrDefaultAsync();
                                    if (crew.Geolocation != crewLocation)
                                    {
                                        crew.Geolocation = crewLocation;
                                        await context.SaveChangesAsync();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }

                    await Task.Delay(30000);
                }
            });
        }

        public void StartEscalationNotificationService()
        {

            Task.Run(async () =>
            {
            while (true)
                {
                    try
                    {
                        Console.WriteLine("EscalationNotificationService is running");
                        DateTime dateTime = MyDateTime.Now.AddMinutes(-10);
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var notificationAgent = scope.ServiceProvider.GetRequiredService<NotificationService>();

                            var shipments = await (from o in context.Consignments
                                                   join d in context.ConsignmentDeliveries on o.Id equals d.ConsignmentId
                                                   where dateTime > o.DueTime && (!o.EscalationStatus.HasFlag(EscalationStatus.MainControlRoom))
                                                   && d.CrewId == null
                                                   select o
                                              ).ToListAsync();

                            foreach (var shipment in shipments)
                            {
                                try
                                {
                                    shipment.EscalationStatus = EscalationStatus.MainControlRoom | shipment.EscalationStatus;
                                    var notificationIds = await notificationAgent.CreateUnassignedShipmentAlert(shipment.Id, NotificationType.ShipmentAlert, shipment.ShipmentCode);
                                    notificationIds.ForEach(x => WebPushNotificationsQueue.Add(x));
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                    await Task.Delay(60000);
                }
            });
             
        }
    }
}
