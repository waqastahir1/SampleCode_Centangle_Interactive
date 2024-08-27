using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class OrganizationUitViewModel : ViewModelBase
    {
        public OrganizationUitViewModel()
        {
            RegionId = 0;
            SubRegionId = 0;
            StationId = 0;
        }

        private IEnumerable<SelectListItem> _regions;

        public IEnumerable<SelectListItem> Regions

        {
            get { return _regions; }
            set
            {
                _regions = value;
                NotifyPropertyChanged();
            }
        }



        private IEnumerable<SelectListItem> _subRegions;

        public IEnumerable<SelectListItem> SubRegions
        {
            get { return _subRegions; }
            set
            {
                _subRegions = value;
                NotifyPropertyChanged();
            }
        }

        private IEnumerable<SelectListItem> _stations;

        public IEnumerable<SelectListItem> Stations
        {
            get { return _stations; }
            set
            {
                _stations = value;
                NotifyPropertyChanged();
            }
        }

        private int? _regionId;

        [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Select Region")]
        [Required(ErrorMessage = "Select Region")]
        public int? RegionId
        {
            get { return _regionId; }
            set
            {
                _regionId = value;
                NotifyPropertyChanged();
            }
        }


        private int? _subRegionId;

        public int? SubRegionId
        {
            get { return _subRegionId; }
            set
            {
                _subRegionId = value;
                NotifyPropertyChanged();
            }
        }

        private int? _stationId;

        public int? StationId
        {
            get { return _stationId; }
            set
            {
                _stationId = value;
                NotifyPropertyChanged();
            }
        }


        public int? PartyId { get; set; }

        public string PartyName { get; set; }
    }
}
