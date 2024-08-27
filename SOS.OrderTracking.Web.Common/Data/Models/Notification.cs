using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ReceiverId { get; set; }

        [StringLength(450)]
        public string ReceiverUserName { get; set; }

        public int SenderId { get; set; }

        [StringLength(450)]
        public string SenderUserName { get; set; }

        [Required]
        [StringLength(450)]
        public string Title { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        public string Url { get; set; }

        public string P256dh { get; set; }

        public string Auth { get; set; }

        public NotificationType NotificationType { get; set; }

        public NotificationMedium NotificationMedium { get; set; }

        public NotificationCategory NotificationCategory { get; set; }

        public NotificationStatus NotificationStatus { get; set; }

        public string TransactionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public DateTime? SentAt { get; set; }

    }
}
