using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext context;

        public NotificationsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Notification vm)
        {
            var notification = await context.Notifications.FindAsync(vm.Id);
            notification.NotificationStatus = Shared.Enums.NotificationStatus.Delivered;
            notification.ReceivedAt = MyDateTime.Now;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var notification = await context.Notifications
                .Where(x => x.ReceiverUserName == User.Identity.Name && x.NotificationStatus.HasFlag(NotificationStatus.Undelivered))
                .Select(x => new NotificationDetail()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = JsonConvert.DeserializeObject<Message>(x.Description)

                }).ToArrayAsync();

            return Ok(notification);
        }
    }

    public class Notification
    {
        public int Id { get; set; }
    }

    public class NotificationDetail : Notification
    {
         
        public string Title { get; set; }
         
        public Message Description { get; set; } 
    }
}
