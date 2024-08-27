using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ComplaintStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public Shared.Enums.ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
