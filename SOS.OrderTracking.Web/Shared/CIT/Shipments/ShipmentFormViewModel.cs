using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{

    public class ShipmentFormViewModel : ConsignmentFormViewModel
    {
        [Required(ErrorMessage = "Please select Bill Branch")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Bill Branch")]
        public int? BillBranchId { get; set; }

        public string BillBrannchName { get; set; }


        [StringLength(maximumLength: 20)]
        [RegularExpression("^A{0,1}([0-9]){6,9}$", ErrorMessage = "Shipment number should be six to nine digits, with or without Capital 'A'. No other charachters are allowed.")]
        public string ManualShipmentCode { get; set; }

        private int _toPartyId;
        [Required(ErrorMessage = "Please select Dropoff")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Dropoff")]
        public int ToPartyId
        {
            get
            {
                return _toPartyId;
            }
            set
            {
                _toPartyId = value;
                NotifyPropertyChanged();
            }
        }

        public string ToPartyName { get; set; }

        public int OriginPartyId { get; set; }

        [Required(ErrorMessage = "Select Currency Type")]
        public CurrencySymbol CurrencySymbol
        {
            get; set;
        }
        [Required(ErrorMessage = "Please select Consignment status")]
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Comments { get; set; }

        public string Initiator { get; set; }

        public byte TransactionMode { get; set; }

        public bool SealedBags { get; set; }

        public bool IncludeCashProcessing { get; set; }
        public int AmountPKR { get; set; }
        public byte[] ConsignmentImage { get; set; }
        public string ConsignmentImageType { get; set; }

        public ShipmentFormViewModel()
        {
            CurrencySymbol = CurrencySymbol.PKR;
            ServiceType = ServiceType.ByRoad;
        }
    }

    public class CrewShipmentFormViewModel : ConsignmentFormViewModel
    {
        [Required(ErrorMessage = "Please select Bill Branch")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Bill Branch")]
        public int? BillBranchId { get; set; }

        public string BillBrannchName { get; set; }

        [Required(ErrorMessage = "Manual Shipment Code is Required")]
        [StringLength(maximumLength: 20)]
        [RegularExpression("^A{0,1}([0-9]){6,9}$", ErrorMessage = "Shipment number should be six to nine digits, with or without Capital 'A'. No other charachters are allowed.")]
        public string ManualShipmentCode { get; set; }

        private int _toPartyId;
        [Required(ErrorMessage = "Please select Dropoff")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Dropoff")]
        public int ToPartyId
        {
            get
            {
                return _toPartyId;
            }
            set
            {
                _toPartyId = value;
                NotifyPropertyChanged();
            }
        }

        public string ToPartyName { get; set; }

        public int OriginPartyId { get; set; }

        [Required(ErrorMessage = "Select Currency Type")]
        public CurrencySymbol CurrencySymbol
        {
            get { return CitDenominationViewModel.CurrencySymbol; }
            set { CitDenominationViewModel.CurrencySymbol = value; }
        }
        [Required(ErrorMessage = "Please select Consignment status")]
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Comments { get; set; }

        public bool SealedBags { get; set; }

        public CitDenominationViewModel CitDenominationViewModel { get; set; }

        public CrewShipmentFormViewModel()
        {
            CitDenominationViewModel = new CitDenominationViewModel() { CurrencySymbol = CurrencySymbol.PKR, FinalizeShipment = true };
            CurrencySymbol = CurrencySymbol.PKR;
            ServiceType = ServiceType.ByRoad;
        }
    }
}
