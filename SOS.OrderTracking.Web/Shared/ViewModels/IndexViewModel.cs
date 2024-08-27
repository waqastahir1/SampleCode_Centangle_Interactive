using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class IndexViewModel<TListViewModel>
    {
        public int TotalRows { get; set; }

        public List<TListViewModel> Items { get; set; }

        [Obsolete("Use multi arg constructor")]
        public IndexViewModel()
        {

        }

        public IndexViewModel(IEnumerable<TListViewModel> items, int totalRows)
        {
            Items = items.ToList();
            TotalRows = totalRows;
        }
    }

    public class BaseIndexModel : INotifyPropertyChanged
    {
        private int _currentIndex;

        public int CurrentIndex
        {
            get { return _currentIndex = _currentIndex < 1 ? 1 : _currentIndex; }
            set
            {
                _currentIndex = value;
                NotifyPropertyChanged();
            }
        }

        private int _rowsPerPage;

        public int RowsPerPage
        {
            get { return _rowsPerPage = _rowsPerPage < 1 ? 15 : _rowsPerPage; }
            set
            {
                _rowsPerPage = value;
                NotifyPropertyChanged();
            }
        }

        public int? RegionId { get; set; }

        public int? SubRegionId { get; set; }

        public int? StationId { get; set; }

        private string _sortColumn;
        public string SortColumn
        {
            get { return _sortColumn; }
            set
            {
                _sortColumn = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime? Offset { get; set; }
        public string AdditionalParams { get; set; }
        public DateTime? FilterDate { get; set; }

        public BaseIndexModel()
        {
            RegionId = 0;
        }
        public virtual string ToQueryString()
        {
            var regionId = RegionId.GetValueOrDefault() == 0 ? null : RegionId;
            var subRegionId = SubRegionId.GetValueOrDefault() == 0 ? null : SubRegionId;
            var stationId = StationId.GetValueOrDefault() == 0 ? null : StationId;

            return $"rowsPerPage={RowsPerPage}" +
                    $"&currentIndex={CurrentIndex}" +
                    $"&RegionId={regionId}&SubRegionId={subRegionId}&StationId={stationId}" +
                    $"&FilterDate={FilterDate?.ToString("o")}&Offset={Offset?.ToString("o")}{AdditionalParams}" +
                    $"&SortColumn={SortColumn}";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
