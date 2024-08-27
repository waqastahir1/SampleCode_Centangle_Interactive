using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notification = SOS.OrderTracking.Web.Common.Data.Models.Notification;

namespace SOS.OrderTracking.Web.Common.Services
{
    public class NotificationService
    {
        private readonly AppDbContext context;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(AppDbContext context, ILogger<NotificationService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        #region Firebase Notification

        public async Task CreateShipmentNotificationForMobile(int consignmentId, NotificationType type, NotificationCategory category)
        {
            var deliveries = await context.ConsignmentDeliveries.Include(x => x.Consignment).Where(x => x.ConsignmentId == consignmentId).ToArrayAsync();
            foreach (var item in deliveries)
            {
                await CreateFirebaseNotification(item.Id, item.ConsignmentId, item.CrewId, item.Consignment.ShipmentCode, type, category);
            }

        }
        public async Task CreateFirebaseNotification(int deliveryId, int consignmentId, int? teamId, string shipmentCode, NotificationType type, NotificationCategory category, bool forceCreate = false)
        {
            try
            {
                if (teamId > 0)
                {

                    var receiverIds = await (from x in context.PartyRelationships
                                             join u in context.Users on x.FromPartyId equals u.PartyId
                                             where (x.ToPartyId == teamId || u.PartyId == teamId) && x.IsActive && u.FCMToken != null
                                             select x.FromPartyId).ToListAsync();

                    if (category == NotificationCategory.CIT)
                    {
                        receiverIds.Add(teamId.GetValueOrDefault());
                    }

                    foreach (var receiverId in receiverIds.Distinct())
                    {
                        var title = $"{type} {category} {shipmentCode}";
                        //if (!context.Notifications.Any(x=> 
                        //        x.NotificationType == type &&
                        //        x.NotificationCategory == category &&
                        //        x.ReceiverId == receiverId &&
                        //        x.Title == title) || forceCreate)
                        {
                            var notificationId = context.Sequences.GetNextNotificationSequence();
                            Message message = new Message()
                            {
                                Notification = new FirebaseAdmin.Messaging.Notification
                                {
                                    Title = $"{type}",
                                    Body = $"{type} {category} No {shipmentCode}"
                                },
                                Data = new Dictionary<string, string>()
                                {
                                    { "Type", type.ToString() },
                                    { "Category", category.ToString() },
                                    { "ConsignmentId", consignmentId.ToString()  },
                                    { "deliveryId", deliveryId.ToString() },
                                    {"notificationId",notificationId.ToString() }
                                }
                            };

                            var notification = new Notification()
                            {
                                Id = notificationId,
                                CreatedAt = DateTime.UtcNow,
                                NotificationType = type,
                                NotificationCategory = category,
                                NotificationStatus = NotificationStatus.Waiting,
                                NotificationMedium = NotificationMedium.Firebase,
                                ReceiverId = receiverId,
                                Title = title,
                                Description = JsonConvert.SerializeObject(message)
                            };
                            context.Notifications.Add(notification);
                            await context.SaveChangesAsync();
                            FcmNotificationAgent.FcmNotificationsQueue.Add(notificationId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        #endregion


        private async Task<int> CreateWebNotification(int shipmentId, string receiverUserId, NotificationType type, string notificationTitle, NotificationCategory category)
        {
            var notificationId = context.Sequences.GetNextNotificationSequence();
            var notification = new Notification()
            {
                Id = notificationId,
                CreatedAt = DateTime.UtcNow,
                NotificationType = type,
                NotificationCategory = category,
                NotificationStatus = NotificationStatus.Waiting,
                NotificationMedium = NotificationMedium.Web,
                ReceiverId = 1,
                ReceiverUserName = receiverUserId,
                Description = shipmentId.ToString(),
                Title = notificationTitle
            };
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
            return notification.Id;
        }

        public async Task<List<int>> CreateApprovalPromptNotification(int consignmentId, NotificationType type, string shipmentCode)
        {
            var receivers = await (from u in context.Users
                                   join c in context.Consignments on u.UserName equals c.CreatedBy
                                   join b in context.Parties on u.PartyId equals b.Id
                                   join approver in context.Users on b.Id equals approver.PartyId
                                   join ur in context.UserRoles on approver.Id equals ur.UserId
                                   where Constants.Roles.Supervisors.Contains(ur.RoleId) && c.Id == consignmentId
                                   && (c.ApprovedBy == null || c.ApprovedBy == approver.Id)
                                   select new { b.Id, UserId = approver.Id }).ToListAsync();

            if (receivers == null)
            {
                throw new Exception("Cannot locate notification receiver for shipment approval");
            }
            List<int> notificationIds = new List<int>();
            foreach (var userId in receivers)
            {
                var id = await CreateWebNotification(consignmentId, userId.UserId, type, $"Approval Required for {shipmentCode}", NotificationCategory.CIT);
                notificationIds.Add(id);
            }
            return notificationIds;
        }

        public async Task<int> CreateShipmentApprovedNotification(int consignmentId, NotificationType type, string shipmentCode, bool IsUpdateRouteNotification = false)
        {

            var receiver = await (from u in context.Users
                                  join c in context.Consignments on u.UserName equals c.CreatedBy
                                  where c.Id == consignmentId
                                  select new { Id = u.PartyId, UserId = u.Id }).FirstOrDefaultAsync();

            return await CreateWebNotification(consignmentId, receiver.UserId, type,
                (IsUpdateRouteNotification ? "Destination update request for " : "") +
                $"{shipmentCode} has been Approved",
                 NotificationCategory.CIT);
        }

        public async Task<int> CreateShipmentDeclinedNotification(int consignmentId, string shipmentCode)
        {

            var receiver = await (from u in context.Users
                                  join c in context.Consignments on u.UserName equals c.CreatedBy
                                  where c.Id == consignmentId
                                  select new { Id = u.PartyId, UserId = u.Id }).FirstOrDefaultAsync();

            return await CreateWebNotification(consignmentId, receiver.UserId, NotificationType.Declined,
                $"Sorry, We are unable to collect your shipment {shipmentCode}",
                 NotificationCategory.CIT);
        }
        public async Task<int> CreateShipmentRefusedNotification(int consignmentId, string shipmentCode, bool IsUpdateRouteNotification = false)
        {

            var receiver = await (from u in context.Users
                                  join c in context.Consignments on u.UserName equals c.CreatedBy
                                  where c.Id == consignmentId
                                  select new { Id = u.PartyId, UserId = u.Id }).FirstOrDefaultAsync();
            return await CreateWebNotification(consignmentId, receiver.UserId, NotificationType.Declined,
                (IsUpdateRouteNotification ? "Destination update request for " : "") +
            $"{shipmentCode} has been Refused",
             NotificationCategory.CIT);

            return await CreateWebNotification(consignmentId, receiver.UserId, NotificationType.Declined,
                $"{shipmentCode} has been Refused",
                 NotificationCategory.CIT);
        }

        public async Task<int> CreateCrewAssignedNotification(int consignmentId, string shipmentCode)
        {

            var receiver = await (from u in context.Users
                                  join c in context.Consignments on u.UserName equals c.CreatedBy
                                  join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                                  join crew in context.Parties on d.CrewId equals crew.Id
                                  where c.Id == consignmentId
                                  select new { Id = u.PartyId, UserId = u.Id, crew.FormalName }).FirstOrDefaultAsync();

            return await CreateWebNotification(consignmentId, receiver.UserId, NotificationType.Declined,
                $" Your order# {shipmentCode} is placed successfuly, {receiver.FormalName} is on its way for collection",
                 NotificationCategory.CIT);
        }


        public async Task<int> CreateCrewReachedNotification(int consignmentId, NotificationType type, NotificationCategory category, string shipmentCode)
        {

            var receiver = await (from u in context.Users
                                  join c in context.Consignments on u.UserName equals c.CreatedBy
                                  where c.Id == consignmentId
                                  select new { Id = u.PartyId, UserId = u.Id }).FirstOrDefaultAsync();

            return await CreateWebNotification(consignmentId, receiver.UserId, type,
                 $"The crew has reached the collection point, please finalize the shipment {shipmentCode}",
                 NotificationCategory.CIT);
        }


        public async Task<int> CreateNewShipmentNotification(int shipmentId, NotificationType type, string shipmentCode)
        {
            var receiver = await (from c in context.Consignments
                                  join fromParty in context.Parties on c.FromPartyId equals fromParty.Id
                                  join s in context.Parties on fromParty.SubregionId equals s.Id
                                  join sr in context.PartyRelationships on s.Id equals sr.ToPartyId
                                  join sh in context.Users on sr.FromPartyId equals sh.PartyId
                                  where c.Id == shipmentId
                                  && sr.FromPartyRole == RoleType.SubRegionalHead
                                  && sr.ToPartyRole == RoleType.SubRegionalOrg
                                  && sr.IsActive
                                  select new { sh.PartyId, UserId = sh.Id }).FirstOrDefaultAsync();

            if (receiver == null)
                return -1;

            return await CreateWebNotification(shipmentId, receiver.UserId, type, $"New Shipment {shipmentCode}", NotificationCategory.CIT);
        }

        public async Task<List<int>> CreateUnassignedShipmentAlert(int shipmentId, NotificationType type, string shipmentCode)
        {
            var admins = (from u in context.Users
                          join r in context.UserRoles on u.Id equals r.UserId
                          where r.RoleId == "SOS-Admin" && u.UserName.Contains("admin")
                          //where r.RoleId == "SOS-Admin"
                          select u.Id);
            var notificationIds = new List<int>();
            foreach (var admin in admins.ToArray())
            {
                notificationIds.Add(await CreateWebNotification(shipmentId, admin, type, $"{shipmentCode} is waiting for crew assignment", NotificationCategory.CIT));
            }
            return notificationIds;
        }
    }
}
