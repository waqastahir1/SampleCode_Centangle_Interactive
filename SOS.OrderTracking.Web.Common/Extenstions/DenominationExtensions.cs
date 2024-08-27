using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Common.Extensions
{
    static public class DenominationExtensions
    {
        public static CitDenominationViewModel ToViewModel(this Denomination d, string shipmentCode)
        {
            if (d == null)
                return null;

            var viewModel = new CitDenominationViewModel
            {
                Id = d.Id,
                Type = d.DenominationType,
                ConsignmentId = d.ConsignmentId,
                ShipmentCode = shipmentCode,
                Currency1x = d.Currency1x,
                Currency2x = d.Currency2x,
                Currency5x = d.Currency5x,
                Currency10x = d.Currency10x,
                Currency20x = d.Currency20x,
                Currency50x = d.Currency50x,
                Currency75x = d.Currency75x,
                Currency100x = d.Currency100x,
                Currency500x = d.Currency500x,
                Currency1000x = d.Currency1000x,
                Currency5000x = d.Currency5000x,

                Currency200x = d.Currency200x,
                Currency750x = d.Currency750x,
                Currency1500x = d.Currency1500x,
                Currency7500x = d.Currency7500x,
                Currency15000x = d.Currency15000x,
                Currency25000x = d.Currency25000x,
                Currency40000x = d.Currency40000x,

                PrizeMoney100x=d.PrizeMoney100x,
                PrizeMoney200x=d.PrizeMoney200x,
                PrizeMoney750x=d.PrizeMoney750x,
                PrizeMoney1500x=d.PrizeMoney1500x,
                PrizeMoney7500x=d.PrizeMoney7500x,
                PrizeMoney15000x=d.PrizeMoney15000x,
                PrizeMoney25000x=d.PrizeMoney25000x,
                PrizeMoney40000x=d.PrizeMoney40000x
            };
            return viewModel;
        }
    }
}
