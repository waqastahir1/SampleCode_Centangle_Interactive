using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ATMService
    {
        public ATMService()
        {
            ATMServiceLogs = new HashSet<ATMServiceLog>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ATMId { get; set; }

        public int? CashBranchId { get; set; }

        public int CustomerId { get; set; }

        public int? TechnitianId { get; set; }

        public int? CachierId { get; set; }

        public ATMServiceState? ATMReplanishmentState { get; set; }

        public ATMRServiceType ATMRServiceType { get; set; }

        public int Currency500x { get; set; }

        public int Currency1000x { get; set; }

        public int Currency5000x { get; set; }

        public DateTime? Deadline { get; set; }
        public string CreatedBy { get; set; }

        public DateTime TimeStamp { get; set; }
        public DateTime DueTime { get; set; }

        public virtual ICollection<ATMServiceLog> ATMServiceLogs { get; set; }

        public string ShipmentCode { get; set; }

        [StringLength(64)]
        public string PickupQrCode { get; set; }

        [StringLength(64)]
        public string CITPickupQrCode { get; set; }

        [StringLength(64)]
        public string CITDropoffQrCode { get; set; }

        [StringLength(64)]
        public string ReturnCITPickupQrCode { get; set; }


        public int ShipmentId { get; set; }

        public int ReturnShipmentId { get; set; }

        public int AccessCashReturnShipmentId { get; set; }

        public int CardReturnShipmentId { get; set; }
    }
}
