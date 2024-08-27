using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class AssetAllocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int AssetId { get; set; }

        public int PartyId { get; set; }

        public DateTime AllocatedFrom { get; set; }

        public DateTime? AllocatedThru { get; set; }

        public string AllocatedBy { get; set; }

        public DateTime AllocatedAt { get; set; }

        public Party Party { get; set; }

        public Asset Asset { get; set; }

        public bool IsActive { get; set; }

    }
}
