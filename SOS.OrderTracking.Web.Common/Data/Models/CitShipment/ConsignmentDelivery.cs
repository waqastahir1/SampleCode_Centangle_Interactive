using NetTopologySuite.Geometries;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class ConsignmentDelivery
    {
        public ConsignmentDelivery()
        {

            Childern = new HashSet<ConsignmentDelivery>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public int PickupLocationId { get; set; }

        public int FromPartyId { get; set; }

        public int DestinationLocationId { get; set; }

        public int ToPartyId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PlanedPickupTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActualPickupTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PlanedDropTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActualDropTime { get; set; }

        public int? CauseOfPickupTimeDifference { get; set; }

        public int? CauseOfDropTimeDifference { get; set; }

        public int ConsignmentId { get; set; }

        public int? CrewId { get; set; }

        [MaxLength(450)]
        public string PickupCode { get; set; }

        [MaxLength(450)]
        public string DropoffCode { get; set; }

        public Shared.Enums.ConsignmentDeliveryState DeliveryState { get; set; }

        public TemporalState TemporalState { get; set; }

        public byte SerialNo { get; set; }

        public bool IsVault { get; set; }

        public virtual Party Crew { get; set; }

        public virtual Location DestinationLocation { get; set; }

        public virtual Consignment Consignment { get; set; }

        public virtual Location PickupLocation { get; set; }

        public virtual Party FromParty { get; set; }

        public virtual Party ToParty { get; set; }

        public Point CollectionPoint { get; set; }

        public Point DeliveryPoint { get; set; }

        public byte CollectionMode { get; set; }

        public byte DeliveryMode { get; set; }

        public DataRecordStatus CollectionPointStatus { get; set; }

        public DataRecordStatus DeliveryPointStatus { get; set; }

        public virtual ConsignmentDelivery Parent { get; set; }

        public virtual ICollection<ConsignmentDelivery> Childern { get; set; }
    }
}
