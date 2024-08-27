using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class Party
    {
        public Party()
        {
            Deliveries = new HashSet<ConsignmentDelivery>();
            PartyRelationshipsFromParty = new HashSet<PartyRelationship>();
            PartyRelationshipsToParty = new HashSet<PartyRelationship>();
            FromPartyOrders = new HashSet<Consignment>();
            ToPartyOrders = new HashSet<Consignment>();
            CustomerOrders = new HashSet<Consignment>();
            MainCustomerOrders = new HashSet<Consignment>();
            BillBranchOrders = new HashSet<Consignment>();
            AssetAllocations = new HashSet<AssetAllocation>();

            FromPartyDeliveries = new HashSet<ConsignmentDelivery>();
            ToPartyDeliveries = new HashSet<ConsignmentDelivery>();
            IsActive = true;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Branch Code or Organization Abbriviation etc
        /// </summary>
        [StringLength(400)]
        public string ShortName { get; set; }

        [StringLength(400)]
        public string FormalName { get; set; }

        [StringLength(400)]
        public string Abbrevation { get; set; }

        public PartyType PartyType { get; set; }

        [StringLength(127)]
        public string PersonalContactNo { get; set; }

        [StringLength(127)]
        public string OfficialContactNo { get; set; }

        [StringLength(127)]
        public string PersonalEmail { get; set; }

        [StringLength(127)]
        public string OfficialEmail { get; set; }

        [StringLength(450)]
        public string Address { get; set; }

        public int? RegionId { get; set; }

        public int? SubregionId { get; set; }

        public int? StationId { get; set; }
        public int? GaurdingRegionId { get; set; }
        public int? GaurdingSubRegionId { get; set; }
        public int? GaurdingStationId { get; set; }

        public virtual Organization Orgnization { get; set; }

        public virtual PartyAttribute PartyAttributes { get; set; }

        public virtual Person People { get; set; }


        public DateTime? UpdatedAtErp { get; set; }

        public DateTime? UpdateAt { get; set; }


        public int ExternalId { get; set; }

        public byte SycStatus { get; set; }

        [StringLength(450)]
        public string ImageLink { get; set; }

        [StringLength(400)]
        public string ParentCode { get; set; }

        [StringLength(450)]
        public string JsonData { get; set; }

        public virtual ICollection<ConsignmentDelivery> Deliveries { get; set; }
        public virtual ICollection<PartyRelationship> PartyRelationshipsFromParty { get; set; }
        public virtual ICollection<PartyRelationship> PartyRelationshipsToParty { get; set; }
        public virtual ICollection<Consignment> MainCustomerOrders { get; set; }

        public virtual ICollection<Consignment> CustomerOrders { get; set; }
        public virtual ICollection<Consignment> BillBranchOrders { get; set; }
        public virtual ICollection<Consignment> FromPartyOrders { get; set; }
        public virtual ICollection<Consignment> ToPartyOrders { get; set; }

        public virtual ICollection<ConsignmentDelivery> FromPartyDeliveries { get; set; }
        public virtual ICollection<ConsignmentDelivery> ToPartyDeliveries { get; set; }

        public virtual ICollection<AssetAllocation> AssetAllocations { get; set; }
        public virtual ICollection<AllocatedBranch> AllocatedBranches { get; set; }

        public DateTimeOffset LastSync { get; set; }
        public bool IsActive { get; set; }
    }
}
