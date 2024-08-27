using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data.Models;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class WebPushNotificationService
    {
        private readonly AppDbContext context;
        private readonly ILogger<UserService> logger;

        public WebPushNotificationService(AppDbContext context, ILogger<UserService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task SendNotificationAsync(NotificationSubscription subscription, string message, string url)
        {
            // For a real application, generate your own
            var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:asad@sos.com", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            var payload = JsonSerializer.Serialize(new
            {
                message,
                url = url,
            });
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
        }
    }
}
