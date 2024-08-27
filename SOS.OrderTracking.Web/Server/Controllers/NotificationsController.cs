using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Notification;
using WebPush;
using NotificationSubscription = SOS.OrderTracking.Web.Shared.ViewModels.Notification.NotificationSubscription;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ILogger<NotificationsController> logger;
        private UserManager<ApplicationUser> userManager;
        private readonly WebPushNotificationService webPushNotificationService;
        private readonly SequenceService sequenceService;
        public NotificationsController(AppDbContext appDbContext,
             UserManager<ApplicationUser> userManager,
             WebPushNotificationService webPushNotificationService,
            ILogger<NotificationsController> logger, SequenceService sequenceService)
        {
            this.userManager = userManager;
            this.webPushNotificationService = webPushNotificationService;
            context = appDbContext;
            this.logger = logger;
            this.sequenceService = sequenceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] BaseIndexModel vm)
        {
            try
            {
                var query = (from n in context.Notifications
                             join p in context.Parties on n.ReceiverId equals p.Id
                             join r in context.PartyRelationships on p.Id equals r.FromPartyId
                             join c in context.Parties on r.ToPartyId equals c.Id
                             where vm.RegionId == c.RegionId
                             && (vm.SubRegionId == null || vm.SubRegionId == c.SubregionId)
                             && (vm.StationId == null || vm.StationId == c.StationId)
                             && r.FromPartyRole == Shared.Enums.RoleType.CheifCrewAgent
                             && r.StartDate <= Shared.MyDateTime.Today
                             && (r.ThruDate == null || r.ThruDate >= Shared.MyDateTime.Today)
                             orderby n.CreatedAt descending
                             select new NotificationsListViewModel()
                             {
                                 ReceiverId = n.ReceiverId,
                                 OrganizationName = c.FormalName,
                                 ReceiverUserName = n.ReceiverUserName,
                                 TransactionId = n.TransactionId,
                                 Description = n.Description,
                                 NotificationCategory = n.NotificationCategory,
                                 NotificationMedium = n.NotificationMedium,
                                 NotificationStatus = n.NotificationStatus,
                                 NotificationType = n.NotificationType,
                                 Title = n.Title,
                                 CreatedAt = n.CreatedAt,
                                 ReceivedAt = n.ReceivedAt,
                                 SentAt = n.SentAt
                             });

                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();
               

                return Ok(new IndexViewModel<NotificationsListViewModel>( items, totalRows));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<NotificationSubscription> Subscribe(NotificationSubscription subscription)
        {
            // We're storing at most one subscription per user, so delete old ones.
            // Alternatively, you could let the user register multiple subscriptions from different browsers/devices.
            var userId = GetUserId();
            var oldSubscriptions = context.NotificationSubscriptions.Where(e => e.UserId == userId || (e.Url == subscription.Url && e.Auth == subscription.Auth));
            context.NotificationSubscriptions.RemoveRange(oldSubscriptions);

            // Store new subscription
            subscription.UserId = userId;
            context.NotificationSubscriptions.Add(new Common.Data.Models.NotificationSubscription()
            {
                Id = context.Sequences.GetNextNotificationSequence(),
                P256dh = subscription.P256dh,
                Auth = subscription.Auth,
                UserId = subscription.UserId,
                Url = subscription.Url
            });

            await context.SaveChangesAsync();
            return subscription;
        }
          
        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}