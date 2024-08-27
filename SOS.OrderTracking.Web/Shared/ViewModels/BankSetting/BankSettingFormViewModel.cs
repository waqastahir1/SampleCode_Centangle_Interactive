namespace SOS.OrderTracking.Web.Shared.ViewModels.BankSetting
{
    public class BankSettingFormViewModel
    {
        public int Id { get; set; }
        public bool SkipQRCodeOnCollection { get; set; }
        public bool SkipQRCodeOnDelivery { get; set; }
        public bool EnableManualShipmentNo { get; set; }
        public bool SkipQRForSBP { get; set; } 
    }
}
