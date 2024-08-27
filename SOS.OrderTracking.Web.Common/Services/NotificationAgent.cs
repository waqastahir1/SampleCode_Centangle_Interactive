using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Services
{
    public class FcmNotificationAgent
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;
        private readonly FirebaseMessaging fcm;
        public FcmNotificationAgent(IServiceScopeFactory serviceScopeFactory,
            ILogger<FcmNotificationAgent> logger,
            IWebHostEnvironment env)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
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
        public static BlockingCollection<int> FcmNotificationsQueue { get; set; } = new BlockingCollection<int>();

        public void StartFcmSerrvice()
        {
            // FCM Service
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var id = FcmNotificationsQueue.Take();
                        {
                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                                var notification = await context.Notifications.FirstOrDefaultAsync(x => x.Id == id);


                                Message message = JsonConvert.DeserializeObject<Message>(notification.Description);
                                var user = await context.Users.FirstOrDefaultAsync(x => x.PartyId == notification.ReceiverId);
                                if (user == null)
                                {
                                    notification.NotificationStatus = NotificationStatus.UndefinedUser;
                                }
                                else
                                {
                                    message.Token = user.FCMToken;
                                    notification.ReceiverUserName = user.UserName;
                                    notification.SentAt = MyDateTime.Now;
                                    try
                                    {
                                        var responseId = await fcm.SendAsync(message);
                                        if (string.IsNullOrEmpty(responseId))
                                        {
                                            notification.NotificationStatus = NotificationStatus.FirbaseError;
                                        }
                                        else
                                        {
                                            notification.NotificationStatus = NotificationStatus.Sent;
                                            notification.TransactionId = responseId;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        notification.NotificationStatus = NotificationStatus.Error;
                                        notification.TransactionId = ex.Message;

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

    }
}
