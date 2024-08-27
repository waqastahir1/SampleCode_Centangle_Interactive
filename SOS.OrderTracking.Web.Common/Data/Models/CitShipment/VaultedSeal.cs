using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models.CitShipment
{
    public class VaultedSeal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SealCode { get; set; }

        public int VaultedShipmentId { get; set; }
        [ForeignKey("VaultedShipmentId")]
        public virtual VaultedShipment VaultedShipment { get; set; }
    }
}
