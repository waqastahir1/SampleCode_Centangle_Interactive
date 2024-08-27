using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.CPC.Consignments
{
    public class CpcConsignmentFormViewModel : ConsignmentFormViewModel
    {
        [Required(ErrorMessage = "Select Currency Type")]
        public CurrencySymbol CurrencySymbol
        {
            get; set;
        }

        public string Comments { get; set; }
        public string Initiator { get; set; }

        public CpcConsignmentFormViewModel()
        {
            CurrencySymbol = CurrencySymbol.PKR;
        }
    }
}
