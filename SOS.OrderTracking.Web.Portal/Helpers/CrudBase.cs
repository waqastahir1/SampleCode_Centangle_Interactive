using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Portal.Components;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace SOS.OrderTracking.Web.Portal
{
    public abstract class CrudBase<TFormViewModel, TListModel, TKey> : ComponentBase, INotifyPropertyChanged
        where TFormViewModel : class, new()
        where TListModel : class, new()

    {

        private OrganizationUitViewModel _viewModel;

        [CascadingParameter]
        public OrganizationUitViewModel OrganizationalUnit
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if (value != null && (_viewModel != value))
                {
                    _viewModel = value;
                    NotifyPropertyChanged();
                }

                if (_viewModel.RegionId != RegionId)
                {
                    RegionId = _viewModel.RegionId.GetValueOrDefault();
                }

                if (_viewModel.SubRegionId != SubRegionId)
                {
                    SubRegionId = _viewModel.SubRegionId.GetValueOrDefault();
                }
                if (_viewModel.StationId != StationId)
                {
                    StationId = _viewModel.StationId.GetValueOrDefault();
                }

            }
        }

        private int _regionId;

        public int RegionId
        {
            get { return _regionId; }
            set
            {
                if (_regionId != value)
                {
                    _regionId = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private int _stationId;

        public int StationId
        {
            get { return _stationId; }
            set
            {
                if (_stationId != value)
                {
                    _stationId = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private int _subRegionId;

        public int SubRegionId
        {
            get { return _subRegionId; }
            set
            {
                if (_subRegionId != value)
                {
                    _subRegionId = value; NotifyPropertyChanged();
                }
            }
        }


        internal ClaimsPrincipal _user;
        [CascadingParameter]
        internal ClaimsPrincipal User
        {
            get { return _user; }
            set
            {
                if (_user != value && value != null)
                {
                    _user = value;
                    NotifyPropertyChanged();
                }
            }
        }


        [Inject]
        protected IServiceScopeFactory? scopeFactory { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        #region Index Properties

        public int TotalPages { get; set; }


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
        [Inject]
        protected ILogger<TFormViewModel> Logger { get; set; }
        public int TotalRows { get; set; }

        #endregion

        protected Action<TFormViewModel> OnSelectedItemCreated { get; set; }

        protected Action<TKey> OnSelectedItemSaved { get; set; }


        public string ValidationError { get; set; }

        protected string ModalCss { get; set; }

        protected string BackDrop { get; set; }
        public bool IsModalBusy { get; set; }
        protected string ButtonClasses { get; set; }

        public bool IsTableBusy { get; set; }

        public string Sorting { get; set; }

        //protected ComposeViewModel SelectedItem { get; private set; }
        private TFormViewModel? _selectedItem;
        public TFormViewModel? SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }

        private List<TListModel> _items;
        protected List<TListModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }

        protected PaginationStripModel PaginationStrip { get; set; }

        [CascadingParameter]
        protected string Error { get; set; }



        public CrudBase()
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
                    await LoadItems(true);
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
                if (q.PropertyName == nameof(RegionId)
                || q.PropertyName == nameof(SubRegionId)
                || q.PropertyName == nameof(StationId))
                {
                    await LoadItems(true);
                    OnFilterApplied?.Invoke();
                }
            };



        }
        protected async Task BlockFormUI()
        {
            IsModalBusy = true;
            await InvokeAsync(() => StateHasChanged());
        }

        protected async Task UnBlockFormUI()
        {
            IsModalBusy = false;
            await InvokeAsync(() => StateHasChanged());
        }
        protected async Task BlockTableUI()
        {
            IsTableBusy = true;
            await InvokeAsync(() => StateHasChanged());
            await Task.Delay(20);
        }
        protected async Task UnBlockTableUI()
        {
            IsTableBusy = false;
            await InvokeAsync(() => StateHasChanged());
            await Task.Delay(20);
        }
        protected abstract Task LoadSelectLists(AppDbContext context, IServiceProvider serviceProvider);
        protected Action OnFilterApplied { get; set; }


        bool pageLoaded;
        protected override async Task OnParametersSetAsync()
        {
            if (!pageLoaded)
            {
                if (_user != null && OrganizationalUnit != null)
                {
                    pageLoaded = true;
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await LoadSelectLists(context, scope.ServiceProvider);
                    }
                    await LoadItems(true);
                }
            }
        }

        protected async Task OnItemClicked(TKey id)
        {
            await BlockFormUI();
            ValidationError = null;
            Error = null;
            ShowModal();
            IsModalBusy = true;
            await Task.Delay(10);
            try
            {
                if (id?.ToString() == "0" || string.IsNullOrEmpty(id?.ToString()))
                {
                    SelectedItem = CreateSelectedItem();
                }
                else
                {
                    await BlockFormUI();
                    using var scope = scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    SelectedItem = await InitSelectedItem(id, context);
                    await context.DisposeAsync();
                    await UnBlockFormUI();
                }

                if (SelectedItem != null)
                {
                    OnSelectedItemCreated?.Invoke(SelectedItem);
                }


            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            IsModalBusy = false;
        }

        /// <summary>
        /// Created SelectedItem with default values.
        /// Override this method if want to create and enforce specific rules on ComposeViewModel,
        /// </summary> 
        protected virtual TFormViewModel CreateSelectedItem()
        {
            return new TFormViewModel();
        }

        protected abstract Task<TFormViewModel> InitSelectedItem(TKey id, AppDbContext context);

        //protected abstract void RemoveSelectedItem();

        protected async Task OnFormSubmit()
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await OnFormSubmit(context, scope);
            await context.DisposeAsync();
        }

        protected async Task OnFormSubmit(AppDbContext context, IServiceScope scope)
        {
            await BlockFormUI();

            try
            {
                ButtonClasses = "btn btn-primary pull-right kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light";

                var id = await PersistSelectedItem(context, scope);
                await context.SaveChangesAsync();
                await LoadItems();
                HideModal();

                //   var selectedItemClone = JsonConvert.DeserializeObject<ComposeViewModel>(JsonConvert.SerializeObject(selectedItem));

                OnSelectedItemSaved?.Invoke(id);
                SelectedItem = null;
                await JSRuntime.InvokeVoidAsync("toast.showMessage", "Data Saved Successfully");
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            await UnBlockFormUI();
        }
        public bool UsePagination = true;
        protected async Task LoadItems(bool showBusy = false)
        {
            if (showBusy)
                await BlockTableUI();
            try
            {
                shouldRender = false;
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var query = GetQuery(context);

                if (!string.IsNullOrEmpty(SortColumn))
                {
                    query = query.OrderBy(SortColumn);
                }

                if (UsePagination)
                {
                    TotalRows = GetCountQuery();

                    if (TotalRows == -1)
                        TotalRows = query.Count();

                    TotalPages = Convert.ToInt32(Math.Ceiling(TotalRows / (double)PaginationStrip.RowsPerPage));
                    if (TotalPages != 0 && TotalPages < PaginationStrip.CurrentIndex) PaginationStrip.CurrentIndex = 1;

                }
                if (query is IAsyncEnumerable<TListModel>)
                {
                    Items = await (UsePagination ? query.Skip((PaginationStrip.CurrentIndex - 1) * PaginationStrip.RowsPerPage).Take(PaginationStrip.RowsPerPage).ToListAsync() : query.ToListAsync());
                }
                else
                {
                    Items = UsePagination ? query.Skip((PaginationStrip.CurrentIndex - 1) * PaginationStrip.RowsPerPage).Take(PaginationStrip.RowsPerPage).ToList() : query.ToList();
                }

                Items = await OnItemsLoaded(Items, scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>(), scope);
                shouldRender = true;
                await context.DisposeAsync();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            await UnBlockTableUI();

        }

        protected virtual int GetCountQuery()
        {
            return -1;
        }

        bool shouldRender = true;
        protected override bool ShouldRender()
        {
            return shouldRender;
        }
        protected abstract IQueryable<TListModel> GetQuery(AppDbContext context);

        protected virtual Task<List<TListModel>> OnItemsLoaded(List<TListModel> items, AppDbContext context, IServiceScope scope)
        {
            return Task.FromResult(items);
        }

        /// <summary>
        /// This method shall transform SelectedItem object to Data Models and add/update them to Database
        /// </summary>
        /// <returns></returns>
        protected abstract Task<TKey> PersistSelectedItem(AppDbContext context, IServiceScope scope);

        protected void HideModal()
        {
            ModalCss = null;
            BackDrop = "";
            ButtonClasses = null;
            SelectedItem = null;
        }

        protected void ShowModal()
        {
            ButtonClasses = "btn btn-primary pull-right";
            ModalCss = "offcanvas-on";
            BackDrop = "offcanvas-overlay";
        }

        protected T GetScopedService<T>()
        {
            if (scopeFactory == null)
                throw new InvalidOperationException();

            var scopedObject = scopeFactory
                                         .CreateAsyncScope()
                                         .ServiceProvider
                                         .GetRequiredService<T>();
            if (scopedObject == null)
                throw new InvalidOperationException();

            return scopedObject;

        }

        public async Task ShowNotification(string content)
        {
            await JSRuntime.InvokeVoidAsync("toast.showMessage", content);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

