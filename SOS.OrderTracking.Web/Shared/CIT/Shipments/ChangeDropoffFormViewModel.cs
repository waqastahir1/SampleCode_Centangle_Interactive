using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class ChangeDropoffFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select Dropoff")]
        public int DropoffBranchId { get; set; }
        public int PreviousDropoffBranchId { get; set; }
        public string DropOffBrannchName { get; set; }

        public int? BillBranchId { get; set; }
        public string Message { get; set; }
        public int ConsignmentId{ get; set; }
    }
}
