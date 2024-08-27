using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class CrewFormViewModel : ViewModelBase
    {
        public int Id { get; set; }

        /// <summary>
        /// For internal use
        /// </summary>
        public int RelationshipId { get; set; }

        [Required(ErrorMessage = "Please Enter Name.")]
        public string Name { get; set; }

        private int _vehicleId;
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Vehicle")]
        public int VehicleId
        {
            get { return _vehicleId; }
            set
            {
                SetField(ref _vehicleId, value);
            }
        }

        private int? _stationId;
        [Required(ErrorMessage = "Please Select Station")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Station")]
        public int? StationId
        {
            get { return _stationId; }
            set
            {
                SetField(ref _stationId, value);
            }
        }


        private int? _regionId;
        [Required(ErrorMessage = "please select Region")]
        public int? RegionId
        {
            get { return _regionId; }
            set
            {
                SetField(ref _regionId, value);
            }
        }


        private int? _subRegionId;
        [Required(ErrorMessage = "please select SubRegion")]
        public int? SubRegionId
        {
            get { return _subRegionId; }
            set
            {
                SetField(ref _subRegionId, value);
            }
        }


        [Required(ErrorMessage = "Please Select StartDate")]
        public DateTime? StartDate { get; set; }

        public DateTime? ThruDate { get; set; }

        public MemoryStream MemoryStream { get; set; }

        public bool Uploaded { get; set; }
    }
}
