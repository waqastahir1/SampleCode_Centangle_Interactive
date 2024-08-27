using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultViewModel : ViewModelBase
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Please Enter Name.")]
        public string Name { get; set; }

        private int? _regionId;
        [Required(ErrorMessage = "Please Select Region")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Region")]
        public int? RegionId
        {
            get { return _regionId; }
            set
            {
                _regionId = value;
                NotifyPropertyChanged();
            }
        }


        private int? _subregionId;
        [Required(ErrorMessage = "Please Select Sub-Region")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Sub-Region")]
        public int? SubRegionId
        {
            get { return _subregionId; }
            set
            {
                _subregionId = value;
                NotifyPropertyChanged();
            }
        }

        public int? _stationId;
        [Required(ErrorMessage = "Please Select Station")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Station")]
        public int? StationId
        {
            get { return _stationId; }
            set
            {
                _stationId = value;
                NotifyPropertyChanged();
            }
        }


        public OrganizationType OrganizationType { get; set; }

        private int? _vehicleId;
        [Required(ErrorMessage = "Please Select Vault Type")]
        public int? VehicleId
        {
            get { return _vehicleId; }
            set
            {
                _vehicleId = value;
                NotifyPropertyChanged();
            }
        }


        //[Required]
        //public DateTime? StartDate { get; set; }

        //public DateTime? ThruDate { get; set; }
    }
}
