using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models.CitShipment
{
    public class DeletedConsignment
    {
        [Key]
        public int Id { get; set; }
        public int ConsignmentId { get; set; }
        public string CreatedBy { get; set; }
    }
}
