using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class IntraPartyDistanceHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int FromPartyId { get; set; }
        public int ToPartyId { get; set; }
        public double Distance { get; set; }
        public int AverageTravelTime { get; set; }
        public DataRecordStatus DistanceStatus { get; set; }
        public DistanceSource DistanceSource { get; set; }
        // public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
