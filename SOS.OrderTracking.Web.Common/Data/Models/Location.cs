using NetTopologySuite.Geometries;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class Location
    {
        public Location()
        {
            LocationRelationshipsFromLocation = new HashSet<LocationRelationship>();
            LocationRelationshipsToLocation = new HashSet<LocationRelationship>();
            OrderFulfilmentsDestinationLocation = new HashSet<ConsignmentDelivery>();
            OrderFulfilmentsPickupLocation = new HashSet<ConsignmentDelivery>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(250)]
        public string Abbrevation { get; set; }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        public LocationType Type { get; set; }

        public Point Geolocation { get; set; }

        public byte Status { get; set; }

        public DateTimeOffset? LastSync { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public int ExternalId { get; set; }

        public virtual ICollection<LocationRelationship> LocationRelationshipsFromLocation { get; set; }
        public virtual ICollection<LocationRelationship> LocationRelationshipsToLocation { get; set; }
        public virtual ICollection<ConsignmentDelivery> OrderFulfilmentsDestinationLocation { get; set; }
        public virtual ICollection<ConsignmentDelivery> OrderFulfilmentsPickupLocation { get; set; }
    }
}
