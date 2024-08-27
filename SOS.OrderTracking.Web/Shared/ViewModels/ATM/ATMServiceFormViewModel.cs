using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{

    public class ATMServiceFormViewModel : ViewModelBase
    {
        public int Id
        {
            get; set;
        }

        public string ConsignmentNo
        {
            get; set;
        }

        [Range(1, int.MaxValue, ErrorMessage = "Please select Target ATM")]
        public int ATMId
        {
            get
            {
                return _atmId;
            }
            set
            {
                _atmId = value;
                NotifyPropertyChanged();
            }
        }
        private int _atmId;

        public int? CashSourceBranchId
        {
            get; set;
        }

        [Required(ErrorMessage = "Select ATMR request type")]
        public ATMRServiceType ATMRServiceType
        {
            get; set;
        }

        public int Currency500x { get; set; }

        public int Currency1000x { get; set; }

        public int Currency5000x { get; set; }

        public DateTime? Deadline
        {
            get; set;
        }

    }
}
