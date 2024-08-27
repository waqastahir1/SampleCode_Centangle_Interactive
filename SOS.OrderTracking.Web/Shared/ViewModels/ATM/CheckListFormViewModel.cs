namespace SOS.OrderTracking.Web.Shared.ViewModels.ATM
{
    public class CheckListFormViewModel
    {
        public int checkListId { get; set; }
        // [Required]
        public string Feature { get; set; }
        public int featureId { get; set; }
        public string BankId { get; set; }
        public bool isActive { get; set; }

    }
}
