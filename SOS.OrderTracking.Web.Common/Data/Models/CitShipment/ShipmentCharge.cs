using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    [Table(name: "ShipmentCharges")]
    public partial class ShipmentCharge
    {

        public int ChargeTypeId { get; set; }

        public int ConsignmentId { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public int Status { get; set; }


        /// <summary>
        /// Can be used to store related information like SealNo/Code
        /// </summary>
        [MaxLength(450)]
        public string Tag { get; set; }

        public virtual ShipmentChargeType ChargeType { get; set; }

        /// <summary>
        /// Parent
        /// </summary>
        public virtual Consignment Consignment { get; set; }
    }
}
