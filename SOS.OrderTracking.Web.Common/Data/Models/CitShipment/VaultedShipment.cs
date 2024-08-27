using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models.CitShipment
{
    public class VaultedShipment
    {
        [Key]
        public int Id { get; set; }
        public int? ShipmentId { get; set; }
        public int? ManualShipmentId { get; set; }
        public int VaultId { get; set; }
        public int? CrewInId { get; set; }
        public int? CrewOutId { get; set; }
        public int? VehicleInId { get; set; }
        public int? VehicleOutId { get; set; }
        public int? ChiefCrewInId { get; set; }
        public int? ChiefCrewOutId { get; set; }
        public int? FromBranchId { get; set; }
        public int? ToBranchId { get; set; }
        public int BagsIn { get; set; }
        public int? BagsOut { get; set; }
        public decimal AmountIn { get; set; }
        public decimal? AmountOut { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime RequiredTimeOut { get; set; }
        public string VaultedBy { get; set; }
        public bool IsVaulted { get; set; }
        public virtual ICollection<VaultedSeal> VaultedSeals { get; set; }

    }
}
