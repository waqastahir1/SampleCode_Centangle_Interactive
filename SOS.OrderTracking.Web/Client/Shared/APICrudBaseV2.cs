using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Net.Http;
using SOS.OrderTracking.Web.Client.Components;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SOS.OrderTracking.Web.Client.Services;
using System.Drawing;
using System.Threading;
using SOS.OrderTracking.Web.Client.Shared;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Client
{
    public abstract class APICrudBaseV2<TFormViewModel, TListViewModel, TKey, TBaseIndexModel, TApiService> : ComponentBase, INotifyPropertyChanged
     where TBaseIndexModel : BaseIndexModel, new()
     where TApiService : ICrudService<TFormViewModel, TListViewModel, TKey, TBaseIndexModel>
    {

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
        public TApiService ApiService { get; set; }

        private OrganizationUitViewModel _viewModel;


        protected TBaseIndexModel BaseIndexModel;

        [Inject]
        protected HttpClient Http { get; set; }

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

        private IList<TListViewModel> _items;
        protected IList<TListViewModel> Items
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


        protected string Error { get; set; }

        public string ValidationError { get; set; }

        protected virtual string GetRequestFilter { get; }

        public APICrudBaseV2()
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
            IsTableBusy = true;
            await InvokeAsync(() => StateHasChanged());
        }

        //static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        protected Action<TFormViewModel> OnSelectedItemCreated { get; set; }

        protected Action OnFilterApplied { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogWarning($"OnInitializedAsync -> ViewModel is initialized {OrganizationalUnit != null}");

           PropertyChanged += (p, q) =>
            {
                Logger?.LogWarning($"Property Changed {q.PropertyName}");

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
            if((Items?.Count).GetValueOrDefault()== 0)
            {
                await LoadItems(true);
            }
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            Logger.LogWarning($"OnParametersSetAsync -> ViewModel is initialized {OrganizationalUnit != null}");

            await base.OnParametersSetAsync();
        }


        protected async Task OnItemClicked(TKey id)
        {
            Error = null;
            ValidationError = null;
            SelectedItem = CreateSelectedItem();
            if (id?.ToString() != "0" && !string.IsNullOrEmpty(id?.ToString()))
            {
                SelectedItem = await ApiService.GetAsync(id);
                // SelectedItem = await GetViewModel<TFormViewModel>(ApiControllerName, id);
            }
        }

        protected async Task<TViewModel> GetViewModel<TViewModel>(string controller, TKey id, string action = "get")
        {
            TViewModel viewModel = default;
            IsModalBusy = true;
            try
            {
                viewModel = await Http.GetFromJsonAsync<TViewModel>
                     ($"v1/{controller}/{action}?id={id}");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
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
                val = await ApiService.PostAsync(SelectedItem); //await PostViewModel(SelectedItem, ApiControllerName);

                //Type type = typeof(TKey);
                //if (type == typeof(int))
                //{
                //    var value = val as int?;
                //    if (value.GetValueOrDefault() > 0)
                //    {
                //        ValidationError = null;
                //        SelectedItem = default;
                //    }
                //}
                //else if (type == typeof(string))
                //{
                //    var value = val as string;
                //    if (string.IsNullOrEmpty(value))
                //    {
                //        ValidationError = null;
                //        SelectedItem = default;
                //    }
                //}
                //if (ValidationError == null)
                //{
                    SelectedItem = default;
                //}
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

        protected async Task<TKey> PostViewModel<TViewModel>(TViewModel viewModel, string controller, string action = "post")
        {
            Error = null;
            IsModalBusy = true;
            TKey val = default;
            try
            {
                var response = await Http.PostAsJsonAsync
                        ($"v1/{controller}/{action}", viewModel);

                //it produces error when as response is not serializeable
                //  var resposneString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                try
                {
                    val = JsonSerializer.Deserialize<TKey>(await response.Content.ReadAsStringAsync());
                }
                catch (JSException)
                {
                    Error = "Cannot connect to server, please check your internet connection";
                }
                catch
                {
                    // silent exception for APIs which do not return Key on saving record
                }

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

        public virtual string AdditionalParams { get; set; }

        protected async Task LoadItems(bool blockUI = false,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                Logger?.LogWarning($"Load Items -> {memberName} -- {sourceFilePath} -- {sourceLineNumber}");
                //if (BaseIndexModel.RegionId == null)
                //{
                //    Error = "Select Region to show data";
                //    Logger?.LogError(Error);
                //    Items = new List<TListViewModel>();
                //    TotalRows = 0;
                //    TotalRows = 1;
                //}
                //else
                {
                    IsTableBusy = blockUI;

                    Error = null;
                    StateHasChanged();
                    BaseIndexModel.AdditionalParams = AdditionalParams;
                    var data = await ApiService.GetPageAsync(BaseIndexModel); 
                    // var data = await ApiService.GetFromJsonAsync<IndexViewModel<TListViewModel>>(url);

                    if (data == null || data.Items == null)
                    {
                        Items = new List<TListViewModel>();
                        TotalRows = 0;
                        TotalRows = 1;
                        
                    }
                    else
                    {
                        Items = data.Items;
                        TotalRows = data.TotalRows;
                        TotalPages = TotalRows < BaseIndexModel.RowsPerPage ? 1 : Convert.ToInt32(Math.Ceiling(TotalRows / (double)BaseIndexModel.RowsPerPage));
                        if(BaseIndexModel.CurrentIndex > TotalPages)
                        {
                            BaseIndexModel.CurrentIndex = 1;
                        }
                    }
                }
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                Error = "Redirecting to login page";
            }
            catch (JSException)
            {
                Error = "Cannot connect to server, please check your internet connection";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Items = Array.Empty<TListViewModel>();
                Error = ex.Message.ToString() + 
                    ex.InnerException?.Message?.ToString() + 
                    ex.InnerException?.InnerException?.Message?.ToString();
            }
            {
                await InvokeAsync(() => { IsTableBusy = false; StateHasChanged(); });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
