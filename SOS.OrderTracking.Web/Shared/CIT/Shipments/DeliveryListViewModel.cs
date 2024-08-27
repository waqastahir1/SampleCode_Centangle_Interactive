using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class DeliveryListViewModel
    {
        /// <summary>
        /// Delivery Id
        /// </summary>
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public int ConsignmentId { get; set; }

        public int? CrewId { get; set; }

        //public ConsignmentState ConsignmentState { get; set; }

        public ConsignmentDeliveryState DeliveryState { get; set; }


        public string CrewName { get; set; }

        public OrganizationType? OrganizationType { get; set; }

        public DateTime? PlanedCollectionTime { get; set; }

        public DateTime? PlanedDeliveryTime { get; set; }

        public string PickupFrom { get; set; }

        public IEnumerable<string> CrewMembers { get; set; }


        [MaxLength(450)]
        public string PickupCode { get; set; }

        [MaxLength(450)]
        public string DropoffCode { get; set; }

        public TemporalState TemporalState { get; set; }

        public byte SerialNo { get; set; }

        public bool IsVault { get; set; }


        public Point CollectionPoint { get; set; }

        public Point DeliveryPoint { get; set; }

        public byte CollectionMode { get; set; }

        public byte DeliveryMode { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public object CollectionPoint_ { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public object DeliveryPoint_ { get; set; }

        public DataRecordStatus CollectionPointStatus { get; set; }

        public DataRecordStatus DeliveryPointStatus { get; set; }
    }

}
