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
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Text.Json;
using SOS.OrderTracking.Web.Client.Services;
using System.Drawing;
using System.Threading;

namespace SOS.OrderTracking.Web.Client
{
    public abstract class APICrudBase<TFormViewModel, TListViewModel, TKey> : ComponentBase, INotifyPropertyChanged

    {

        [CascadingParameter]
        public OrganizationUitViewModel ViewModel
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

        private OrganizationUitViewModel _viewModel;


        [Inject]
        protected HttpClient Http { get; set; }

        public int? RegionId { get; set; }


        public int? SubRegionId { get; set; }


        public int? StationId { get; set; }


        [Inject]
        protected ApiService ApiService { get; set; }

        [Inject]
        public ILogger<APICrudBase<TFormViewModel, TListViewModel, TKey>> Logger { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        public abstract string ApiControllerName { get; }


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

        private IEnumerable<TListViewModel> _items;
        protected IEnumerable<TListViewModel> Items
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
            get { return _rowsPerPage = _rowsPerPage <1 ? 15 : _rowsPerPage; }
            set
            {
                _rowsPerPage = value;
                NotifyPropertyChanged();
            }
        }

         

        protected string Error { get; set; }

        public string ValidationError { get; set; }

        protected virtual string GetRequestFilter { get; }

        public APICrudBase()
        {
            PropertyChanged += async (p, q) =>
            { 

                if (q.PropertyName == nameof(ViewModel))
                {

                    if (ViewModel != null &&
                    (RegionId != ViewModel.RegionId || SubRegionId != ViewModel.SubRegionId || StationId != ViewModel.StationId))
                    {
                        RegionId = ViewModel.RegionId;
                        SubRegionId = ViewModel.SubRegionId;
                        StationId = ViewModel.StationId;
                        await LoadItems(true);
                        OnFilterApplied?.Invoke();
                    }
                }
            };

        }

        //static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        protected Action<TFormViewModel> OnSelectedItemCreated { get; set; }

        protected Action OnFilterApplied { get; set; }

        protected override async Task OnInitializedAsync()
        { 
            Logger.LogWarning($"OnInitializedAsync -> ViewModel is initialized {ViewModel != null}");

            PropertyChanged += async (p, q) =>
            {
                Logger?.LogWarning($"Property Changed {q.PropertyName}");

                if (q.PropertyName == nameof(SortColumn)
                || q.PropertyName == nameof(RowsPerPage)
                || q.PropertyName == nameof(CurrentIndex)
                || q.PropertyName == nameof(SortColumn) )
                {
                    await LoadItems(true);
                    StateHasChanged();
                }
                else if (q.PropertyName == nameof(SelectedItem) && SelectedItem != null)
                {
                    OnSelectedItemCreated?.Invoke(SelectedItem);
                } 
                else if (q.PropertyName == nameof(ViewModel))
                {

                    if (ViewModel != null &&
                    (RegionId != ViewModel.RegionId || SubRegionId != ViewModel.SubRegionId || StationId != ViewModel.StationId))
                    {
                        RegionId = ViewModel.RegionId;
                        SubRegionId = ViewModel.SubRegionId;
                        StationId = ViewModel.StationId;
                        await LoadItems(true);
                        OnFilterApplied?.Invoke();
                    }
                }
            };
            await base.OnInitializedAsync();
        } 

        protected override async Task OnParametersSetAsync()
        {
            Logger.LogWarning($"OnParametersSetAsync -> ViewModel is initialized {ViewModel != null}");
  
            await base.OnParametersSetAsync();
        }


        protected async Task OnItemClicked(TKey id)
        {
            Error = null;
            SelectedItem = CreateSelectedItem();
            if (id?.ToString() != "0" && !string.IsNullOrEmpty(id?.ToString()))
            {
                SelectedItem = await GetViewModel<TFormViewModel>(ApiControllerName, id);
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
            TKey val = await PostViewModel(SelectedItem, ApiControllerName);
          
            if (ValidationError == null)
            {
                SelectedItem = default;
            }
            OnFormSubmitted?.Invoke();
            await LoadItems(true);
            return val;
        }

        protected Action OnFormSubmitted;

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
                Logger.LogInformation($"Load Items -> {memberName} -- {sourceFilePath} -- {sourceLineNumber}");
                if (RegionId == null)
                {
                    Error = "Select Region to show data";
                    Logger.LogError(Error);
                    Items = new List<TListViewModel>();
                    TotalRows = 0;
                    TotalRows = 1;
                }
                else
                {
                    IsTableBusy = blockUI;

                    Error = null;
                    StateHasChanged();
                    var url = $"v1/{ApiControllerName}/GetPage?rowsPerPage={RowsPerPage}" +
                        $"&currentIndex={CurrentIndex}{AdditionalParams}" +
                        $"&RegionId={RegionId}&SubRegionId={SubRegionId}&StationId={StationId}&SortColumn={SortColumn}";

                    var data = await ApiService.GetFromJsonAsync<IndexViewModel<TListViewModel>>(url);


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

                        TotalPages = TotalRows == 0 ? 1 : Convert.ToInt32(Math.Ceiling(TotalRows / (double)RowsPerPage));
                    }
                }
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                Error = ex.ToString() + ex.InnerException?.ToString() + ex.InnerException?.InnerException?.ToString();
            }
            catch (JSException)
            {
                Error = "Cannot connect to server, please check your internet connection";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Error = ex.ToString() + ex.InnerException?.ToString() + ex.InnerException?.InnerException?.ToString();
            }

            await InvokeAsync(() =>
            {
                IsTableBusy = false;
                StateHasChanged();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
