using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class CPCTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int CPCServiceId { get; set; }

        public int CrPartyId { get; set; }

        public RoleType CrPartyRole { get; set; }

        [StringLength(50)]
        public string CrPartRole_ { get; set; }

        public int DrPartyId { get; set; }

        public RoleType DrPartyRole { get; set; }

        [StringLength(50)]
        public string DrPartRole_ { get; set; }

        public CPCTransactionReason CPCTransactionReason { get; set; }

        [StringLength(450)]
        public string CPCTransactionReasons { get; set; }

        public CashNature CashNature { get; set; }

        public CashType CashType { get; set; }

        public int Amount { get; set; }

        public int Currency10x { get; set; }

        public int Currency20x { get; set; }

        public int Currency50x { get; set; }
        public int Currency75x { get; set; }

        public int Currency100x { get; set; }

        public int Currency500x { get; set; }

        public int Currency1000x { get; set; }

        public int Currency5000x { get; set; }

        [StringLength(450)]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }


        [StringLength(450)]
        public string Particulars { get; set; }



    }
}
