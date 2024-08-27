namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class CitShipmentViewModel<TViewModel>
        where TViewModel : ShipmentFormViewModel, new()
    {
        protected TViewModel ViewModel;
        public CitShipmentViewModel()
        {
            ViewModel = new TViewModel();
        }
    }
}
