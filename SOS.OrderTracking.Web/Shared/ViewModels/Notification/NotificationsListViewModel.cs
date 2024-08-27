using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Notification
{
    public class NotificationsListViewModel
    {
        public int ReceiverId { get; set; }

        [StringLength(450)]
        public string ReceiverUserName { get; set; }

        public string OrganizationName { get; set; }

        [Required]
        [StringLength(450)]
        public string Title { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        public NotificationType NotificationType { get; set; }

        public NotificationMedium NotificationMedium { get; set; }

        public NotificationCategory NotificationCategory { get; set; }

        public NotificationStatus NotificationStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        [StringLength(450)]
        public string TransactionId { get; set; }

    }
}
