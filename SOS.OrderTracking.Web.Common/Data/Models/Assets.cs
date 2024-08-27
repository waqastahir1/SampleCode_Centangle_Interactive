using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class Asset
    {
        public Asset()
        {
            AssetAllocations = new HashSet<AssetAllocation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? RegionId { get; set; }

        public int? SubregionId { get; set; }

        public int StationId { get; set; }


        [Required]
        [MaxLength(450)]
        public string Code { get; set; }

        public string Description { get; set; }

        public DateTime AcquisitionDate { get; set; }

        public AssetType AssetType { get; set; }

        public string UpdateBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<AssetAllocation> AssetAllocations { get; set; }
    }
}
