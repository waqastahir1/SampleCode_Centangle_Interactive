using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class DedicatedVehiclesCapacity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ToDate { get; set; }

        public byte VehicleCapacity { get; set; }

        public decimal RadiusInKm { get; set; }

        public int TripPerDay { get; set; }

        public string JsonData { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public byte SyncStatus { get; set; }

    }
}
