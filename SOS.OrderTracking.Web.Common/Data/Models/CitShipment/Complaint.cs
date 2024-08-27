using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class Complaint
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? ConsignmentId { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
