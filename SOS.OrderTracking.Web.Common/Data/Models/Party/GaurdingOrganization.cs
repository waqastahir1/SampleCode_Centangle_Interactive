using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class GaurdingOrganization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int NoOfShifts { get; set; }
        public int TotalNoOfGaurdsRequired { get; set; }
        public int NoOfGaurdsAppointed { get; set; }
        public int BranchId { get; set; }
        public int SupervisorId { get; set; }

    }
}
