using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Shared
{
    public interface APICrudInterface<TFormViewModel, TListViewModel, TKey>
    {
        public Task<IndexViewModel<TListViewModel>> GetPageAsync(int RowsPerPage,int CurrentIndex,string AdditionalParams,int? RegionId,int? SubRegionId,int? StationId,string SortColumn);
       
        public Task<TFormViewModel> Get(TKey id);

        public Task<int> Post(TFormViewModel selectedItem);
    }
}
