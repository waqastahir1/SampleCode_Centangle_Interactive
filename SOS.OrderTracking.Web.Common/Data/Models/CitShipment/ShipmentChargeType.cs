using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class ShipmentChargeType
    {
        public ShipmentChargeType()
        {
            DeliveryCharges = new HashSet<ShipmentCharge>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        public virtual ICollection<ShipmentCharge> DeliveryCharges { get; set; }
    }
}
