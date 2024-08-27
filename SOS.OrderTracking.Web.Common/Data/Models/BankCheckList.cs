using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class BankCheckList
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int CheckListTypeId { get; set; }
        public bool isActive { get; set; }

    }
}
