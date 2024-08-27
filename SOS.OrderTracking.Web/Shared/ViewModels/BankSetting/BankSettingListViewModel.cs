namespace SOS.OrderTracking.Web.Shared.ViewModels.BankSetting
{
    public class BankSettingListViewModel
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
        public string FormalName { get; set; }

        public string JsonData { get; set; }

        public BankSettingFormViewModel BankSetting { get; set; }


    }
}
