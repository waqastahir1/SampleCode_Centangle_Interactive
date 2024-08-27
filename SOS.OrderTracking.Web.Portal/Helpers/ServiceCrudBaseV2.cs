using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SOS.OrderTracking.Web.Portal
{
    public abstract class ServiceCrudBaseV2<TFormViewModel, TListViewModel, TKey, TBaseIndexModel, TDataService> : ComponentBase, INotifyPropertyChanged
     where TBaseIndexModel : BaseIndexModel, new()
     where TDataService : ICrudService<TFormViewModel, TListViewModel, TKey, TBaseIndexModel>
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
                _viewModel = value;
                NotifyPropertyChanged();
            }
        }

        [Inject]
        public TDataService DataService { get; set; }

        [Inject]
        public IServiceScopeFactory ScopeFactory { get; set; }

        protected TBaseIndexModel BaseIndexModel;

        [Inject]
        protected ILogger<TFormViewModel> Logger { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        //  public abstract string ApiControllerName { get; }
        //   public abstract APICrudInterface<TFormViewModel, TListViewModel, TKey> APICrudInterface { get; }

        #region Index Properties

        public int TotalPages { get; set; }


        public int TotalRows { get; set; }

        #endregion

        public bool IsTableBusy { get; set; }

        public bool IsModalBusy { get; set; }

        private TFormViewModel _selectedItem;
        public TFormViewModel SelectedItem
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

        private List<TListViewModel> _items;
        protected List<TListViewModel> Items
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


        protected string? Error { get; set; }

        public string? ValidationError { get; set; }


        public ServiceCrudBaseV2()
        {
            BaseIndexModel = new TBaseIndexModel();


            PropertyChanged += async (p, q) =>
            {

                if (q.PropertyName == nameof(OrganizationalUnit))
                {
                    if (OrganizationalUnit != null &&
                    (BaseIndexModel.RegionId.GetValueOrDefault() != OrganizationalUnit.RegionId.GetValueOrDefault()
                    || BaseIndexModel.SubRegionId.GetValueOrDefault() != OrganizationalUnit.SubRegionId.GetValueOrDefault()
                    || BaseIndexModel.StationId.GetValueOrDefault() != OrganizationalUnit.StationId.GetValueOrDefault()))
                    {
                        BaseIndexModel.RegionId = OrganizationalUnit.RegionId;
                        Logger.LogInformation($"RegionId = {BaseIndexModel.RegionId}");
                        BaseIndexModel.SubRegionId = OrganizationalUnit.SubRegionId;
                        Logger.LogInformation($"SubRegionId = {BaseIndexModel.SubRegionId}");
                        BaseIndexModel.StationId = OrganizationalUnit.StationId;
                        Logger.LogInformation($"StationId = {BaseIndexModel.StationId}");
                        await LoadItems(true);
                        OnFilterApplied?.Invoke();
                    }
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
        }

        protected async Task UnBlockTableUI()
        {
            IsTableBusy = false;
            await InvokeAsync(() => StateHasChanged());
        }

        //static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        protected Action<TFormViewModel> OnSelectedItemCreated { get; set; }

        protected Action OnFilterApplied { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PropertyChanged += (p, q) =>
             {
                 Logger?.LogInformation($"Property Changed {q.PropertyName}");

                 if (q.PropertyName == nameof(SelectedItem) && SelectedItem != null)
                 {
                     OnSelectedItemCreated?.Invoke(SelectedItem);
                 }

             };

            BaseIndexModel.PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(BaseIndexModel.SortColumn)
                     || q.PropertyName == nameof(BaseIndexModel.RowsPerPage)
                     || q.PropertyName == nameof(BaseIndexModel.CurrentIndex))
                {
                    await LoadItems(true);
                    StateHasChanged();
                }
            };
            if ((Items?.Count).GetValueOrDefault() == 0)
            {
                await LoadItems(true);
            }
            await base.OnInitializedAsync();
        }

        protected async Task OnItemClicked(TKey id)
        {
            Error = null;
            ValidationError = null;
            SelectedItem = CreateSelectedItem();
            if (id?.ToString() != "0" && !string.IsNullOrEmpty(id?.ToString()))
            {
                using var scope = ScopeFactory.CreateScope();
                var dataService = scope.ServiceProvider.GetRequiredService<TDataService>();

                SelectedItem = await DataService.GetAsync(id);
            }
        }

        protected async Task<TViewModel> GetViewModel<TViewModel>(string controller, TKey id, string action = "get")
        {
            TViewModel viewModel = default;
            IsModalBusy = true;
            try
            {

                //viewModel = await Http.GetFromJsonAsync<TViewModel> ($"v1/{controller}/{action}?id={id}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Error = ex.ToString();
            }
            IsModalBusy = false;
            return viewModel;
        }

        /// <summary>
        /// Created SelectedItem with default values.
        /// Override this method if want to create and enforce specific rules on ComposeViewModel,
        /// </summary> 
        protected virtual TFormViewModel CreateSelectedItem()
        {
            return (TFormViewModel)Activator.CreateInstance(typeof(TFormViewModel));
        }
        public virtual void OnCancelClicked()
        {
            SelectedItem = default;
            ValidationError = null;
        }

        public virtual async Task<TKey> OnFormSubmit()
        {
            TKey val = default;
            IsModalBusy = true;
            try
            {
                using var scope = ScopeFactory.CreateScope();
                var dataService = scope.ServiceProvider.GetRequiredService<TDataService>();
                val = await dataService.PostAsync(SelectedItem);

                SelectedItem = default;

                OnFormSubmitted?.Invoke(val);
                await LoadItems(true);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex.Message);
                ValidationError = ex.Message;
            }
            IsModalBusy = false;
            return val;
        }

        protected Action<TKey> OnFormSubmitted;

        [Obsolete]
        protected async Task<TKey> PostViewModel<TViewModel>(TViewModel viewModel, string controller, string action = "post")
        {
            Error = null;
            IsModalBusy = true;
            TKey val = default;
            try
            {
                ////var response = await Http.PostAsJsonAsync ($"v1/{controller}/{action}", viewModel);

                //////it produces error when as response is not serializeable
                //////  var resposneString = await response.Content.ReadAsStringAsync();

                ////if (!response.IsSuccessStatusCode)
                ////    throw new Exception(await response.Content.ReadAsStringAsync());

                ////try
                ////{
                ////    val = JsonSerializer.Deserialize<TKey>(await response.Content.ReadAsStringAsync());
                ////}
                ////catch (JSException)
                ////{
                ////    Error = "Cannot connect to server, please check your internet connection";
                ////}
                ////catch
                ////{
                ////    // silent exception for APIs which do not return Key on saving record
                ////}

                ValidationError = null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ValidationError = ex.Message;
            }
            IsModalBusy = false;
            return val;
        }

        [Obsolete]
        public virtual string AdditionalParams { get; set; }

        protected virtual async Task LoadItems(bool blockUI = false,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Logger?.LogInformation($"Load Items -> {memberName}");
                if (BaseIndexModel.RegionId == null)
                {
                    await InvokeAsync(() =>
                    {
                        Error = "Select Region to show data";
                        Items = new List<TListViewModel>();
                        TotalRows = 0;
                        TotalRows = 1;
                    });
                }
                else
                {
                    await InvokeAsync(() =>
                    {
                        IsTableBusy = blockUI;
                        Error = null;
                        StateHasChanged();
                        BaseIndexModel.AdditionalParams = AdditionalParams;
                    });

                    using var scope = ScopeFactory.CreateScope();
                    var dataService = scope.ServiceProvider.GetRequiredService<TDataService>();
                    var data = await dataService.GetPageAsync(BaseIndexModel);
                    if (data == null || data.Items == null)
                    {
                        await InvokeAsync(() =>
                        {
                            Items = new List<TListViewModel>();
                            TotalRows = 0;
                            TotalRows = 1;
                        });


                    }
                    else
                    {
                        await InvokeAsync(() =>
                        {
                            Items = data.Items;
                            TotalRows = data.TotalRows;
                            TotalPages = TotalRows < BaseIndexModel.RowsPerPage ? 1 : Convert.ToInt32(Math.Ceiling(TotalRows / (double)BaseIndexModel.RowsPerPage));
                            if (BaseIndexModel.CurrentIndex > TotalPages)
                            {
                                BaseIndexModel.CurrentIndex = 1;
                            }
                        });
                    }

                }
                OnItemsLoaded();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                await InvokeAsync(() =>
                {
                    Items = new List<TListViewModel>();
                    Error = ex.Message.ToString() +
                        ex.InnerException?.Message?.ToString() +
                        ex.InnerException?.InnerException?.Message?.ToString();
                });
            }
            await InvokeAsync(() => { IsTableBusy = false; StateHasChanged(); });
        }

        protected virtual Task OnItemsLoaded()
        {
            return Task.CompletedTask;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
