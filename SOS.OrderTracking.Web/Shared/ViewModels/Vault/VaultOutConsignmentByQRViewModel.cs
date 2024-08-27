using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{


    public class VaultOutConsignmentByQRViewModel
    {
        public VaultOutConsignmentByQRViewModel()
        {
            Seals = new List<string>();
        }

        public int Id { get; set; }
        public int? ShipmentId { get; set; }
        public string Shipment { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Crew")]
        public int CrewOutId { get; set; }
        public string CrewIn { get; set; }
        public int BagsIn { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Bags Required")]
        public int BagsOut { get; set; }
        public decimal AmountIn { get; set; }
        public decimal AmountOut { get; set; }
        public DateTime TimeOut { get; set; } = DateTime.Now;
        public bool OpenSecondForm { get; set; }
        public List<string> Seals { get; set; }
    }


    public class VaultOutManualByQRViewModel : INotifyPropertyChanged
    {
        public VaultOutManualByQRViewModel()
        {
            SealsIn = new List<string>();
            SealsOut = new List<string>();
        }

        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Shipment No Required")]
        public int ShipmentNumber { get { return _shipmentNumber; } set { _shipmentNumber = value; NotifyPropertyChanged(); } }
        private int _shipmentNumber;


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select To Chief Crew")]
        public int ChiefCrewId { get; set; }
        public string ChiefCrew { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select To Branch")]
        public int ToBranchId { get; set; }
        public string ToBranch { get; set; }

        public int BagsIn { get; set; }
        //[Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Bags Required")]
        //public int BagsOut { get; set; }

        public decimal AmountIn { get; set; }
        //[Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Amount Required")]
        //public decimal AmountOut { get; set; }

        public List<string> SealsIn { get; set; }
        public List<string> SealsOut { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
