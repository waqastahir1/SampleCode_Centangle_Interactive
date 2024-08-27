using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System.Collections.Concurrent;
using System.Web;

namespace SOS.OrderTracking.Web.Server.Services
{
    public class NotificationAgent
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;
        private readonly FirebaseMessaging fcm;

        public NotificationAgent(IServiceScopeFactory serviceScopeFactory,
            ILogger<NotificationAgent> logger,
            UserCacheService userCache,
            IWebHostEnvironment env)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            var path = env.ContentRootPath;
            path += "/google-auth.json";
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
                logger.LogWarning(ex.Message);
                app = FirebaseApp.GetInstance("com.sos");
            }
            fcm = FirebaseMessaging.GetMessaging(app);

        }

        public static BlockingCollection<int> WebPushNotificationsQueue { get; set; } = new BlockingCollection<int>();
        public static BlockingCollection<int> ShipmentIds { get; set; } = new BlockingCollection<int>();

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

                                        logger.LogInformation($"Sent notification to {user.UserName}");
                                    }
                                    catch (Exception ex)
                                    {
                                        notification.NotificationStatus = Shared.Enums.NotificationStatus.Error;
                                        notification.TransactionId = ex.ToString().Substring(0, 450);

                                        logger.LogError(ex.Message);
                                    }
                                }
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });
        }

        /// <summary>
        /// Adds id to refreshqueue if this id is not already present
        /// </summary>
        /// <param name="id">Shipment Id to be refreshed</param>
        public int QueueShipmentIdToRefresh(int id)
        {
            if (!ShipmentIds.Contains(id))
            {
                logger.LogDebug($"Added {id} to refresh queue");
                if (!ShipmentIds.Contains(id))
                {
                    ShipmentIds.Add(id);
                }
            }

            return ShipmentIds.Count;
        }

        public void StartShipmentCacheUpdateService()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var id = ShipmentIds.Take();
                        using var scope = serviceScopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var workOrderService = scope.ServiceProvider.GetRequiredService<ConsignmentService>();
                        var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                        var shipment = await workOrderService.GetShipment(id);
                        await shipmentsCacheService.SetShipment(shipment.Id, shipment);
                        await PubSub.Hub.Default.PublishAsync(shipment);
                        logger.LogInformation($"Refreshed {id}, queue length {ShipmentIds.Count}");
                        await Task.Delay(300);
                    }
                    catch { }
                }
            });

            Task.Run(async () =>
            {

                while (true)
                {
                    try
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var id = WebPushNotificationsQueue.Take();
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var webPushNotificationService = scope.ServiceProvider.GetRequiredService<WebPushNotificationService>();
                        var notification = await context.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                        if (notification == null)
                            continue;

                        //notification.Description = JsonConvert.SerializeObject(message); 
                        NotificationSubscription subscription = null;
                        try
                        {
                            PubSub.Hub.Default.Publish(notification);
                            notification.NotificationStatus = NotificationStatus.Sent;
                            notification.SentAt = DateTime.UtcNow;
                            await context.SaveChangesAsync();

                            try
                            {
                                subscription = await context.NotificationSubscriptions.FirstOrDefaultAsync(e => e.UserId == notification.ReceiverUserName);
                                if (subscription == null)
                                {
                                    throw new Exception($"Notification Reciever is not defined for {notification.Id}");
                                }

                                await webPushNotificationService.SendNotificationAsync(subscription, notification.Title, notification.Description);
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning($"{ex.Message} - {subscription?.Id}, {subscription?.UserId}");

                                if (notification != null)
                                {
                                    notification.TransactionId = ex.Message;
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
                            logger.LogError(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
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
                    await Task.Delay(600000);
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


        public void StartPasswordExpiryNotificationCreationService()
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

                            var users = await (from u in context.Users
                                               join ur in context.UserRoles on u.Id equals ur.UserId
                                               from n in context.Notifications.Where(x => x.NotificationMedium == NotificationMedium.Email && x.Title == "Password Expired" && x.ReceiverUserName == u.UserName && x.CreatedAt > u.ExpireDate.Value.AddDays(-3)).DefaultIfEmpty()
                                               where (u.ExpireDate.HasValue ? u.ExpireDate.Value < MyDateTime.Today.AddDays(3) && u.ExpireDate.Value > MyDateTime.Today : false) && (n == null) && (ur.RoleId == "BankBranch" || ur.RoleId == "BankBranchManager" || ur.RoleId == "BankCPC" || ur.RoleId == "BankCPCManager" || ur.RoleId == "BankHybrid" || ur.RoleId == "BANK" || ur.RoleId == "BankGaurding")
                                               select new
                                               {
                                                   u.UserName,
                                                   u.ExpireDate
                                               }).ToListAsync();
                            foreach (var user in users)
                            {
                                var notification = new Common.Data.Models.Notification()
                                {
                                    Id = context.Sequences.GetNextNotificationSequence(),
                                    ReceiverUserName = user.UserName,
                                    CreatedAt = MyDateTime.Now,
                                    NotificationType = NotificationType.New,
                                    NotificationCategory = NotificationCategory.CIT,
                                    NotificationMedium = NotificationMedium.Email,
                                    Title = "Password Expired",
                                    NotificationStatus = NotificationStatus.Undelivered
                                };
                                await context.Notifications.AddAsync(notification);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                    }
                    await Task.Delay(TimeSpan.FromHours(8));
                }
            });
        }
        public void StartPasswordExpiryNotificationPushService()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    string baseAddress;
                recheckBaseAddress:
                    if (string.IsNullOrEmpty(Portal.Helpers.UrlHelper.BaseAddress))
                        goto recheckBaseAddress;
                    else baseAddress = Portal.Helpers.UrlHelper.BaseAddress;

                    try
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var notification = await context.Notifications.Where(x => x.NotificationMedium == NotificationMedium.Email && x.Title == "Password Expired" && x.NotificationStatus == NotificationStatus.Undelivered).FirstOrDefaultAsync();
                            if (notification != null)
                            {
                                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                                var emailManager = scope.ServiceProvider.GetService<SmtpEmailManager>();

                                var applicationUser = await userManager.FindByNameAsync(notification.ReceiverUserName);
                                string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                                token = HttpUtility.UrlEncode(token);
                                string? emailConfirmationLink = $"{baseAddress}Account/ConfirmEmail?userId={applicationUser.Id}&token={token}";
                                var emailBody = HTMLEmailFormats.PasswordExpiryFormat(applicationUser.UserName, emailConfirmationLink, applicationUser.ExpireDate.Value.ToString("dd-MM-yyyy hh:mm tt"));
                                await emailManager.SendEmail(applicationUser.Email, emailBody, "Password Expiring Soon");
                                notification.NotificationStatus = NotificationStatus.Delivered;
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
    }
}
