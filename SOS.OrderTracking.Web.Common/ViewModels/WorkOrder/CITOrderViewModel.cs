using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class CITOrderViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Shipment Code is Required")]
        public string ConsignmentNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select Customer")]
        public int CustomerId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please select Pickup")]
        public int FromPartyId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select Dropoff")]
        public int ToPartyId { get; set; }


        [Required(ErrorMessage = "Select Currency Type")]
        public CurrencySymbol CurrencySymbol { get; set; }

        public int Amount { get; set; }

        public ShipmentExecutionType Type { get; set; }

    }
}
