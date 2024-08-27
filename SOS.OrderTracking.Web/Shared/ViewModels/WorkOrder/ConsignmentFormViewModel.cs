using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ConsignmentFormViewModel : ViewModelBase
    {
        public ConsignmentFormViewModel()
        {
            ExchangeRate = 1;
        }
        public int Id
        {
            get; set;
        }

        public string ConsignmentNo
        {
            get; set;
        }

        [Required(ErrorMessage = "Please select Collection Branch")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Collection Branch")]
        public int FromPartyId
        {
            get
            {
                return _fromPartyId;
            }
            set
            {
                _fromPartyId = value;
                NotifyPropertyChanged();
            }
        }
        private int _fromPartyId;

        public string FromPartyName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select Bill Branch")]
        public int? BillBranchId { get; set; }

        public string BillBrannchName { get; set; }


        [StringLength(maximumLength: 20)]
        [RegularExpression("^A{0,1}([0-9]){6}$", ErrorMessage = "Shipment number should be six digits, with or without Capital 'A'. No other charachters are allowed.")]
        public string ManualShipmentCode { get; set; }

        private int _toPartyId;
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


        private int _amount;
        [Range(0, 300000000, ErrorMessage = "Amount shall not exceed 300,000,000")]
        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private decimal _exchangeRate;
        [Range(0.001, 15000, ErrorMessage = "Exchange Rate should be between 0.001 - 15,000")]
        public decimal ExchangeRate
        {
            get
            {
                return _exchangeRate;
            }
            set
            {
                if (_exchangeRate != value)
                {
                    _exchangeRate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Valueables { get; set; }
        public ShipmentExecutionType Type
        {
            get; set;
        }

        public ConsignmentApprovalState ApprovalState { get; set; }



    }
}
