using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class LocationRelationship
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public LocationType FromLocationType { get; set; }
        public LocationType ToLocationType { get; set; }

        public virtual Location FromLocation { get; set; }
        public virtual Location ToLocation { get; set; }

        public DateTimeOffset LastSync { get; set; }

        public byte Status { get; set; }
    }
}
