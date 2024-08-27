using SOS.OrderTracking.Web.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class ShipmentScheduleFormViewModel : ShipmentFormViewModel
    {


        [Required(ErrorMessage = "Shipment Code cannot by empty, type \"Auto\" in textbox if you want to generate shipment code by system")]
        public string ShipmentCode { get; set; }

        public ShipmentScheduleFormViewModel()
        {
            CurrencySymbol = CurrencySymbol.PKR;
            ServiceType = ServiceType.ByRoad;
            ShipmentCode = "Auto";
        }
    }
}