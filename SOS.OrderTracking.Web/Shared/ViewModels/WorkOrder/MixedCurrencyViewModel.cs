using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class MixedCurrencyViewModel
    {
        public int ConsignmentId { get; set; }
        public string CurrencyType { get; set; }
        [Required]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter net worth")]
        public int AmountPKR { get; set; }

        public bool Finalize { get; set; }

        public bool IsFinalized { get; set; }
    }
}
