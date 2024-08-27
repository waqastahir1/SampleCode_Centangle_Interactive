using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class AllocatedBranch
    {
        [Key]
        public int Id { get; set; }
        public int PartyId { get; set; } //branch Id
        [ForeignKey("PartyId")]
        public Party Party { get; set; } //branch Party
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public bool IsEnabled { get; set; }
    }
}
