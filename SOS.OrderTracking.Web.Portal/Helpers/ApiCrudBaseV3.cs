using Microsoft.AspNetCore.Components;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Portal.Components;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Runtime.CompilerServices;

namespace SOS.OrderTracking.Web.Portal
{

    public abstract class APICrudBaseV3<TFormViewModel, TListViewModel, TKey, TBaseIndexModel, TApiService> :
        ServiceCrudBaseV2<TFormViewModel, TListViewModel, TKey, TBaseIndexModel, TApiService>
        where TBaseIndexModel : BaseIndexModel, new()
     where TApiService : ICrudService<TFormViewModel, TListViewModel, TKey, TBaseIndexModel>
    {
        [Inject]
        protected ILogger<TFormViewModel> Logger { get; set; }

        [Inject] protected AppDbContext context { get; set; }

        protected PaginationStripModel PaginationStrip { get; set; }

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

        public APICrudBaseV3()
        {
            PaginationStrip = new PaginationStripModel()
            {
                CurrentIndex = 1,
                RowsPerPage = 15
            };

            PaginationStrip.PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(PaginationStrip.RowsPerPage)
                || q.PropertyName == nameof(PaginationStrip.CurrentIndex)
                || q.PropertyName == nameof(SortColumn))
                {
                    await LoadItems();
                    if (PaginationStrip.CurrentIndex < 0)
                    {
                        PaginationStrip.CurrentIndex = 1;
                    }
                    else if (PaginationStrip.CurrentIndex > TotalPages)
                    {
                        PaginationStrip.CurrentIndex = TotalPages;

                    }
                    StateHasChanged();
                }
            };

            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(SortColumn))
                {
                    await LoadItems();
                    StateHasChanged();
                }
            };

        }


        protected override async Task LoadItems(bool blockUI = false,
                 [CallerMemberName] string memberName = "",
                 [CallerFilePath] string sourceFilePath = "",
                 [CallerLineNumber] int sourceLineNumber = 0)
        {
            var query = GetQuery();

            if (!string.IsNullOrEmpty(SortColumn))
            {
                // query = query.OrderBy(SortColumn);
            }

            TotalRows = query.Count();

            TotalPages = Convert.ToInt32(Math.Ceiling(TotalRows / (double)PaginationStrip.RowsPerPage));

            //   Items = await query.Skip((PaginationStrip.CurrentIndex - 1) * PaginationStrip.RowsPerPage).Take(PaginationStrip.RowsPerPage).ToListAsync();
            Items = query.Skip((PaginationStrip.CurrentIndex - 1) * PaginationStrip.RowsPerPage).Take(PaginationStrip.RowsPerPage).ToList();

            ItemsLoaded?.Invoke();
            await InvokeAsync(() => StateHasChanged());
        }


        protected Action ItemsLoaded;


        protected abstract IQueryable<TListViewModel> GetQuery();
    }
}
