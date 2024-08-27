using BoldReports.Writer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Portal.Services;
using SOS.OrderTracking.Web.Portal.Services.Customers;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.StaticClasses;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using static SOS.OrderTracking.Web.Shared.UserInfoViewModel;

namespace SOS.OrderTracking.Web.Portal.Pages.CIT.Shipments
{
    public enum ConsignmentCards
    {
        Consignment,
        Denomination,
        Charges,
        DeliveryStates
    }

    public partial class LiveShipmentsPage
    {
        #region Properties


        [Inject]
        public IServiceScopeFactory scopeFactory { get; set; }

        [Inject]
        private IWebHostEnvironment _hostingEnvironment { get; set; }
        [Inject]
        private SmtpEmailManager emailManager { get; set; }

        private string _shipmentsView;
        [Parameter]
        public string ShipmentsView
        {
            get
            {
                return _shipmentsView;
            }
            set
            {
                if (_shipmentsView != value)
                {
                    _shipmentsView = value;
                    NotifyPropertyChanged(nameof(ShipmentsView));
                }
            }
        }

        [Parameter]
        public int FakeId { get; set; }


        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                BaseIndexModel.StartDate = value.GetValueOrDefault();
                NotifyPropertyChanged();
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                BaseIndexModel.EndDate = value.GetValueOrDefault();
                NotifyPropertyChanged();
            }
        }
        private int rating;

        public int Rating
        {
            get { return rating; }
            set
            {
                rating = value;
                BaseIndexModel.Rating = value;
                NotifyPropertyChanged();
            }
        }


        public BulkShipmentsViewModel BulkShipmentsViewModel { get; set; }
        //[Inject]
        //public ApiService HttpService { get; set; }


        private string FormTitle { get; set; }

        [Range(1, 300000000, ErrorMessage = "Amount cannot be greater then three hundred million.")]
        public int? TotalAmount { get; set; }

        [Range(1, 300000000, ErrorMessage = "Amount cannot be greater then three hundred million.")]
        public int AmountPKR { get; set; }

        public string AmountInWords { get; set; }
        public int Amount { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        private List<SelectListItem> _consignmentStateSummarizedTypes;
        private List<SelectListItem> ConsignmentStateSummarizedTypes
        {
            get
            {
                return _consignmentStateSummarizedTypes ??= new List<SelectListItem>()
                {
                    ToSelectListItem( ConsignmentDeliveryState.All),
                    ToSelectListItem( ConsignmentDeliveryState.Created),
                    ToSelectListItem( ConsignmentDeliveryState.CrewAssigned),
                    ToSelectListItem( ConsignmentDeliveryState.ReachedPickup),
                    ToSelectListItem( ConsignmentDeliveryState.InTransit),
                    ToSelectListItem( ConsignmentDeliveryState.Clubbed),
                    ToSelectListItem( ConsignmentDeliveryState.ReachedDestination),
                    ToSelectListItem( ConsignmentDeliveryState.Delivered),
                    ToSelectListItem( ConsignmentDeliveryState.InVault),
                };
            }
        }

        private ConsignmentDeliveryState _consignmentStateSummarized;

        public ConsignmentDeliveryState ConsignmentStateSummarized
        {
            get { return _consignmentStateSummarized; }
            set
            {
                BaseIndexModel.ConsignmentStateSummarized = value;
                _consignmentStateSummarized = value;
                NotifyPropertyChanged();
            }
        }


        private List<SelectListItem> _consignmentStatusTypes;
        private List<SelectListItem> ConsignmentStatusTypes
        {
            get
            {
                return _consignmentStatusTypes ??= new List<SelectListItem>()
                {
                    ToSelectListItem( ConsignmentStatus.All),
                    ToSelectListItem( ConsignmentStatus.TobePosted),
                    ToSelectListItem( ConsignmentStatus.RePush),
                    ToSelectListItem( ConsignmentStatus.Pushing),
                    ToSelectListItem( ConsignmentStatus.Pushed),
                    ToSelectListItem( ConsignmentStatus.DuplicateSeals),
                    ToSelectListItem( ConsignmentStatus.DistanceIssue),
                    ToSelectListItem( ConsignmentStatus.PushingFailed),
                    ToSelectListItem( ConsignmentStatus.Declined),
                    ToSelectListItem( ConsignmentStatus.Cancelled)
            };
            }
        }

        private ConsignmentStatus _consignmentStatus;

        private ConsignmentStatus ConsignmentStatus
        {
            get { return _consignmentStatus; }
            set
            {
                if (_consignmentStatus != value)
                {
                    _consignmentStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ShipmentType _shipmentType;
        public ShipmentType ShipmentType
        {
            get
            {
                return _shipmentType;
            }
            set
            {
                _shipmentType = value;
                NotifyPropertyChanged();
            }
        }

        private int ConsignmentId { get; set; }

        [SupplyParameterFromQuery]
        [Parameter]
        public string SearchKey { get; set; }

        private IEnumerable<SelectListItem> BillBranches { get; set; }

        private ConsignmentStatusViewModel ConsignmentStatusViewModel { get; set; }

        public ConsignmentApprrovalViewModel ConsignmentApprrovalViewModel { get; set; }

        public bool ShowAllCrews { get; set; } = true;

        private IEnumerable<CrewWithLocation> Crews { get; set; }

        public BranchFormViewModel BranchFormViewModel { get; set; }

        public CitDenominationViewModel _CitDenominationViewModel { get; set; }
        private CitDenominationViewModel CitDenominationViewModel
        {
            get
            {
                return _CitDenominationViewModel;
            }
            set
            {
                _CitDenominationViewModel = value;
                NotifyPropertyChanged();
            }
        }

        private DeliveryChargesViewModel DeliveryChargesModel { get; set; }
        private IEnumerable<ShowConsignmentsViewModel> ShowConsignmentsViewModel { get; set; }

        private DeliveryCrewFormViewModel DeliveryCrewFormViewModel { get; set; }
        private string _crewSearchKey;

        public string CrewSearchKey
        {
            get { return _crewSearchKey; }
            set
            {
                _crewSearchKey = value;
                NotifyPropertyChanged();
            }
        }
        private bool _searchCrewGlobal;

        public bool SearchCrewGlobal
        {
            get { return _searchCrewGlobal; }
            set
            {
                _searchCrewGlobal = value;
                NotifyPropertyChanged();
            }
        }
        private DeliveryCrewFormViewModel VaultOutCrewFormViewModel { get; set; }

        IEnumerable<CrewMemberListModel> CrewMembersViewModel { get; set; }
        public IEnumerable<CrewMemberListModel> CrewMembersDetailViewModel { get; set; }

        public DomesticShipmentViewModel? DomesticShipmentViewModel { get; set; }

        public DeliveryFormViewModel? DeliveryFormViewModel { get; set; }

        public DenominationChangeAmountViewModel DenominationChangeAmountViewModel { get; set; }
        public ShipmentAdministrationViewModel ShipmentAdministrationViewModel { get; set; }
        public TransitTimeViewModel TransitTimeViewModel { get; set; }
        public UserInfo userInfo { get; set; }
        private int TabIndex { get; set; }
        int minute = DateTime.Now.Minute;


        public IEnumerable<SelectListItem> MainCustomers { get; set; } = new List<SelectListItem>();
        private List<SelectListItem> DedicatedVehicles { get; set; }
        private int DedicatedVehicle { get; set; }
        private bool TableView;
        #endregion

        public DotNetObjectReference<LiveShipmentsPage> DotNetRef;

        public LiveShipmentsPage()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            ConsignmentStatus = ConsignmentStatus.All;

            ConsignmentStateSummarized = ConsignmentDeliveryState.All;
            BaseIndexModel.RowsPerPage = 18;
#if DEBUG
            BaseIndexModel.RowsPerPage = 6;
#endif 
            PropertyChanged += async (p, q) =>
            {
                Logger?.LogInformation(q.PropertyName);
                if (q.PropertyName == nameof(StartDate) ||
                    q.PropertyName == nameof(EndDate) ||
                    q.PropertyName == nameof(ConsignmentStatus) ||
                    q.PropertyName == nameof(ConsignmentStateSummarized)
                   || q.PropertyName == nameof(Rating)
                   || q.PropertyName == nameof(Sorting)
                   || q.PropertyName == nameof(MainCustomerId)
                   || q.PropertyName == nameof(ShipmentType)
                   || q.PropertyName == nameof(ShipmentsView))
                {

                    BaseIndexModel.StartDate = StartDate.GetValueOrDefault();
                    BaseIndexModel.EndDate = EndDate.GetValueOrDefault();
                    BaseIndexModel.ConsignmentStatus = ConsignmentStatus;
                    BaseIndexModel.ConsignmentStateSummarized = ConsignmentStateSummarized;
                    BaseIndexModel.Rating = Rating;
                    BaseIndexModel.Sorting = Sorting;
                    BaseIndexModel.MainCustomerId = MainCustomerId;
                    BaseIndexModel.ShipmentType = ShipmentType;
                    BaseIndexModel.ConsignmentType = (ShipmentsView?.ToLower() == "scheduled" ? ShipmentExecutionType.Scheduled : ShipmentExecutionType.Live);
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());

                    Logger.LogInformation($"{q.PropertyName} changed");
                }
                else if (q.PropertyName == nameof(CitDenominationViewModel) && CitDenominationViewModel != null)
                {
                    CitDenominationViewModel.PropertyChanged += (p, q) =>
                    {
                        if (q.PropertyName == nameof(CitDenominationViewModel.Currency1x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency2x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency5x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency10x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency100x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency1000x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency20x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency50x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency75x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency500x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency5000x))
                        {
                            if (CitDenominationViewModel.Type == DenominationType.Leafs)
                                TotalAmount = CalculateAmount(CitDenominationViewModel, DenominationType.Leafs);
                            else if (CitDenominationViewModel.Type == DenominationType.Packets)
                                TotalAmount = CalculateAmount(CitDenominationViewModel, DenominationType.Packets);
                            else if (CitDenominationViewModel.Type == DenominationType.Bundles)
                                TotalAmount = CalculateAmount(CitDenominationViewModel, DenominationType.Bundles);

                        }
                    };
                }

                if (q.PropertyName == nameof(CrewSearchKey) || q.PropertyName == nameof(SearchCrewGlobal))
                {
                    IsModalBusy = true;

                    var id = DeliveryCrewFormViewModel?.ConsignmenId;
                    if (id == null)
                        id = VaultOutCrewFormViewModel?.ConsignmenId;

                    if (!string.IsNullOrWhiteSpace(CrewSearchKey) && CrewSearchKey.Length > 2)
                    {
                        if (!SearchCrewGlobal)
                        {
                            Crews = (await DataService.GetCrewsWithLocationMatrix(id.GetValueOrDefault()))?.Where(x => x.Text.ToLower().Contains(CrewSearchKey.ToLower()));
                        }
                        else
                            Crews = (await DataService.GetCrewsWithLocationMatrix(id.GetValueOrDefault(), true, CrewSearchKey))?.Where(x => x.Text.ToLower().Contains(CrewSearchKey.ToLower()));

                    }
                    if (string.IsNullOrEmpty(CrewSearchKey) && !SearchCrewGlobal)
                        Crews = await DataService.GetCrewsWithLocationMatrix(id.GetValueOrDefault());

                    IsModalBusy = false;

                    await InvokeAsync(() => StateHasChanged());
                }
            };

            OnSelectedItemCreated += async (x) =>
            {

                if (SelectedItem.ToPartyId > 0 && SelectedItem.FromPartyId > 0)
                {

                    BillBranches = await DataService.GetSiblingBranches(SelectedItem.ToPartyId, SelectedItem.FromPartyId);

                    await InvokeAsync(() => StateHasChanged());//
                }

                SelectedItem.PropertyChanged += async (p, q) =>
                {
                    if (q.PropertyName == nameof(SelectedItem.ToPartyId) || q.PropertyName == nameof(SelectedItem.FromPartyId))
                    {
                        if (SelectedItem.ToPartyId > 0 && SelectedItem.FromPartyId > 0)
                        {
                            BillBranches = await DataService.GetSiblingBranches(SelectedItem.ToPartyId, SelectedItem.FromPartyId);

                            await InvokeAsync(() => StateHasChanged());//
                        }
                    }
                    if (SelectedItem.ExchangeRate == 0)
                        SelectedItem.ExchangeRate = 1;
                };

            };
        }



        public DeliveryTimeViewModel DeliveryTimeViewModel { get; private set; }


        public ShipmentStatusViewModel? ShipmentStatusViewModel { get; private set; }
        private SortBy _sortBy;

        public SortBy Sorting
        {
            get { return _sortBy; }
            set
            {
                _sortBy = value;
                NotifyPropertyChanged();
            }
        }

        public bool ScheduleThisShipment { get; set; }
        private string searchKey;
        
        public VaultNowViewModel VaultNowViewModel { get; private set; }

        public bool HideButtons { get; private set; } = true;
        public RatingCategoriesViewModel RatingCategoriesViewModel { get; private set; }
        public RatingControlViewModel RatingControlViewModel { get; private set; }
        public ShipmentCommentsViewModel ShipmentCommentsViewModel { get; private set; }

        private int _mainCustomerId;
        public int MainCustomerId
        {
            get { return _mainCustomerId; }
            set
            {
                _mainCustomerId = value;
                NotifyPropertyChanged();
            }
        }

        SOS.OrderTracking.Web.Portal.Components.Select2Ajax<int>[] select2Ajax = new Components.Select2Ajax<int>[10];
        protected override async Task LoadItems(bool blockUI = false,
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
                        Items = new List<ConsignmentListViewModel>();
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
                    var apiService = scope.ServiceProvider.GetRequiredService<CitCardsService>();
                    if (authenticationStateTask == null)
                        throw new Exception("Waiting for security protocols");

                    apiService.User = (await authenticationStateTask).User;
                    var data = await apiService.GetPageAsync(BaseIndexModel);
                    if (data == null || data.Items == null)
                    {
                        await InvokeAsync(() =>
                        {
                            Items = new List<ConsignmentListViewModel>();
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
            }
            catch (JSException)
            {
                await InvokeAsync(() =>
                {
                    Error = "Cannot connect to server, please check your internet connection";
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                await InvokeAsync(() =>
                {
                    Items = new List<ConsignmentListViewModel>();
                    Error = ex.Message.ToString() +
                        ex.InnerException?.Message?.ToString() +
                        ex.InnerException?.InnerException?.Message?.ToString();
                });
            }
            {
                await InvokeAsync(() => { IsTableBusy = false; StateHasChanged(); });
            }
        }


        protected override async Task OnInitializedAsync()
        {
            FormTitle = "Create CIT Consignment";
            Crews = new List<CrewWithLocation>();
            if (NavigationManager.Uri.ToLower().Contains("live"))
            {
                BaseIndexModel.ConsignmentType = ShipmentExecutionType.Live;
            }
            else if (NavigationManager.Uri.ToLower().Contains("schduled"))
            {
                BaseIndexModel.ConsignmentType = ShipmentExecutionType.Scheduled;
            }

            try
            {
                var CommonApiService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<CommonApiService>();
                var partiesService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<PartiesService>();

                var User = (await authenticationStateTask).User;
                userInfo = await CommonApiService.GetUserInfo(User);
                //CPCCustomers = await ApiService.GetCPCBranches(userInfo.PartyId);
                MainCustomers = await partiesService.GetOrganizationsByTypeAsync(OrganizationType.MainCustomer);

                PubSub.Hub.Default.Subscribe<WebNotification>((Func<WebNotification, Task>)(async message =>
                {
                    //await InvokeAsync(async () => await LoadItems());

                    if (message.Source == WebNotification.NotificationSource.OnNewShipment)
                    {
                        if (string.IsNullOrEmpty(SearchKey) || ConsignmentStateSummarized == ConsignmentDeliveryState.All)

                            if ((userInfo.Roles.Contains("BankBranch") || userInfo.Roles.Contains("BankBranchManager"))
                            && (message.FromPartyId == userInfo.PartyId || message.ToPartyId == userInfo.PartyId)
                            || OrganizationalUnit.Regions.Any<SelectListItem>(x => x.IntValue == message.CollectionRegionId) || OrganizationalUnit.Regions.Any<SelectListItem>(x => x.IntValue == message.DeliveryRegionId))
                            {
                                var target = Items.FirstOrDefault<ConsignmentListViewModel>(x => x.Id == message.Id);
                                if (target == null)
                                {
                                    var baseIndexModel = JsonConvert.DeserializeObject<CitCardsAdditionalValueViewModel>(JsonConvert.SerializeObject(BaseIndexModel));
                                    baseIndexModel.Id = message.Id;
                                    Logger.LogInformation($"message-> new {message}");
                                    baseIndexModel.CurrentIndex = 1;
                                    var data = await DataService.GetPageAsync(baseIndexModel);
                                    var item = data.Items.FirstOrDefault<ConsignmentListViewModel>(x => x.Id == message.Id);
                                    if (item != null)
                                    {
                                        Items.Insert(0, item);
                                        if (Items.Count >= BaseIndexModel.RowsPerPage)
                                            Items.RemoveAt(Items.Count - 1);

                                        await InvokeAsync(() => StateHasChanged());
                                    }
                                }
                            }
                    }
                    else if (message.Source == WebNotification.NotificationSource.OnShipmentUpdated)
                    {
                        var target = Items.FirstOrDefault<ConsignmentListViewModel>(x => x.Id == message.Id);
                        if (target != null)
                        {
                            var data = await DataService.GetShipmentFromCache(message.Id);
                            if (data != null)
                            {
                                var i = Items.IndexOf(target);
                                Items[i] = data;
                                await InvokeAsync(() => StateHasChanged());
                            }
                        }
                    }

                }));

                PubSub.Hub.Default.Subscribe<ConsignmentListViewModel>(async shipment =>
                {
                    var target = Items.FirstOrDefault(x => x.Id == shipment.Id);
                    if (target != null)
                    {
                        //await LoadItems(false);
                        var i = Items.IndexOf(target);
                        Items[i] = shipment;
                        await InvokeAsync(() => StateHasChanged());
                    };
                });

                PubSub.Hub.Default.Subscribe<Notification>(async p =>
                {

                    if (p.ReceiverUserName == userInfo.UserName || p.ReceiverUserName == userInfo.UserId)
                    {
                        await LoadItems(false);
                        //if (!p.Title.StartsWith("New Shipment"))
                        {
                            await JSRuntime.InvokeVoidAsync("toast.show", p.Title);
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                // this exception will appear in browser console.
                Logger?.LogError(ex.ToString());
                Error = ex.ToString() + ex.InnerException?.ToString() + ex.InnerException?.InnerException?.ToString();
            }
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DotNetRef = DotNetObjectReference.Create(this);
                //await JSRuntime.InvokeVoidAsync("loadItems.init", DotNetRef);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        //[JSInvokable("loadItems")]
        public async Task loadItems()
        {
            await LoadItems();
        }


        private async Task SearchShipment(string searchKey)
        {
            SearchKey = searchKey;
            BaseIndexModel.SearchKey = searchKey;
            await LoadItems(true);
        }

        protected override ShipmentFormViewModel CreateSelectedItem()
        {
            return new ShipmentFormViewModel();
        }

        [Inject]
        public AppDbContext context { get; set; }

        public override async Task<int> OnFormSubmit()
        {
            IsModalBusy = true;
            //bool getDenomination = true;
            try
            {
                //if (!string.IsNullOrEmpty(SelectedItem.Valueables)) getDenomination = false;

                SelectedItem.Type = ShipmentExecutionType.Live;
                var val = await DataService.PostAsync(SelectedItem);

                if (SelectedItem.IncludeCashProcessing) await DataService.CreateCpcShipment(SelectedItem, val);


                SelectedItem = default;

                OnFormSubmitted?.Invoke(val);

                CitDenominationViewModel = await DataService.GetDenomination(val);
                CitDenominationViewModel.NewShipment = true;
                // await GetViewModel<CitDenominationViewModel>
                //  (ApiControllerName, consignmentId, "GetDenomination");

                FormTitle = CitDenominationViewModel.ShipmentCode;
                //if (getDenomination)
                //{

                //}
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString() + ex.InnerException?.ToString());
                ValidationError = ex.Message;
            }
            IsModalBusy = false;
            return ConsignmentId;
        }

        private void CreateShipment()
        {
            Error = null;
            ValidationError = null;
            SelectedItem = new ShipmentFormViewModel()
            {
                Initiator = userInfo.UserName,
                TransactionMode = 3
            };
        }

        private void SendMoney()
        {
            Error = null;
            ValidationError = null;
            SelectedItem = new ShipmentFormViewModel()
            {
                FromPartyId = OrganizationalUnit.PartyId.GetValueOrDefault(),
                Initiator = userInfo.UserName,
                TransactionMode = 1
            };
        }

        private void ReceiveMoney()
        {
            Error = null;
            ValidationError = null;
            SelectedItem = new ShipmentFormViewModel()
            {
                ToPartyId = OrganizationalUnit.PartyId.GetValueOrDefault(),
                Initiator = userInfo.UserName,
                TransactionMode = 2
            };
        }
        public void OnBulkShipmentClicked()
        {
            BulkShipmentsViewModel = new BulkShipmentsViewModel()
            {
                FromPartyDic = new Dictionary<int, int>(),
                ToPartyDic = new Dictionary<int, int>(),
                BillBranchDic = new Dictionary<int, int>(),
                PickupTimeDic = new Dictionary<int, DateTime?>(),
                DropoffTimeDic = new Dictionary<int, DateTime?>(),
                Keys = new List<KeyHelper>(),
                FaultyKeys = new List<int>()
            };
            for (int i = 0; i < 10; i++)
            {
                BulkShipmentsViewModel.Keys.Add(new KeyHelper() { Id = i });
            }
        }
        public void AddFromParty(int key, int val)
        {
            if (BulkShipmentsViewModel.FromPartyDic.ContainsKey(key)) BulkShipmentsViewModel.FromPartyDic[key] = val;
            else BulkShipmentsViewModel.FromPartyDic.Add(key, val);
        }
        public void AddToParty(int key, int val)
        {
            if (!BulkShipmentsViewModel.BillBranchDic.ContainsKey(key) || ((BulkShipmentsViewModel.BillBranchDic.ContainsKey(key) && BulkShipmentsViewModel.ToPartyDic.ContainsKey(key) && BulkShipmentsViewModel.BillBranchDic[key] == BulkShipmentsViewModel.ToPartyDic[key])))
                select2Ajax[key].ChangeFromParent(context.Parties.Where(x => x.Id == val).Select(x => x.ShortName + "-" + x.FormalName).FirstOrDefault(), val);

            if (BulkShipmentsViewModel.ToPartyDic.ContainsKey(key)) BulkShipmentsViewModel.ToPartyDic[key] = val;
            else BulkShipmentsViewModel.ToPartyDic.Add(key, val);
        }
        public void AddBillBranch(int key, int val)
        {
            if (BulkShipmentsViewModel.BillBranchDic.ContainsKey(key)) BulkShipmentsViewModel.BillBranchDic[key] = val;
            else BulkShipmentsViewModel.BillBranchDic.Add(key, val);
        }
        public void AddPickupTime(int key, DateTime? val)
        {
            if (BulkShipmentsViewModel.PickupTimeDic.ContainsKey(key)) BulkShipmentsViewModel.PickupTimeDic[key] = val;
            else BulkShipmentsViewModel.PickupTimeDic.Add(key, val);
        }
        public void AddDopoffTime(int key, DateTime? val)
        {
            if (BulkShipmentsViewModel.DropoffTimeDic.ContainsKey(key)) BulkShipmentsViewModel.DropoffTimeDic[key] = val;
            else BulkShipmentsViewModel.DropoffTimeDic.Add(key, val);
        }
        public async Task SaveBulkShipments()
        {
            await BlockFormUI();
            try
            {
                ValidationError = null;
                BulkShipmentsViewModel.FaultyKeys = new List<int>();

                for (int i = 0; i < 10; i++)
                {
                    if (BulkShipmentsViewModel.FromPartyDic.ContainsKey(i) && BulkShipmentsViewModel.ToPartyDic.ContainsKey(i) && BulkShipmentsViewModel.BillBranchDic.ContainsKey(i))
                    {
                        if (BulkShipmentsViewModel.FromPartyDic[i] > 0 && BulkShipmentsViewModel.ToPartyDic[i] > 0 && BulkShipmentsViewModel.BillBranchDic[i] > 0)
                        {
                            bool isFaulty = false;
                            if (userInfo.Roles.Contains("BankBranch") || userInfo.Roles.Contains("BankHybrid"))
                            {
                                if (!(BulkShipmentsViewModel.FromPartyDic[i] == userInfo.PartyId || BulkShipmentsViewModel.ToPartyDic[i] == userInfo.PartyId))
                                {
                                    isFaulty = true;
                                }
                            }
                            if ((BulkShipmentsViewModel.PickupTimeDic.ContainsKey(i) && BulkShipmentsViewModel.PickupTimeDic[i].HasValue && BulkShipmentsViewModel.PickupTimeDic[i].Value < MyDateTime.Now) || (BulkShipmentsViewModel.DropoffTimeDic.ContainsKey(i) && BulkShipmentsViewModel.DropoffTimeDic[i].HasValue && BulkShipmentsViewModel.DropoffTimeDic[i].Value < MyDateTime.Now))
                            {
                                isFaulty = true;
                            }

                            if (BulkShipmentsViewModel.PickupTimeDic.ContainsKey(i) && BulkShipmentsViewModel.PickupTimeDic[i].HasValue && BulkShipmentsViewModel.PickupTimeDic[i].Value >= ((BulkShipmentsViewModel.DropoffTimeDic.ContainsKey(i) && BulkShipmentsViewModel.DropoffTimeDic[i].HasValue) ? BulkShipmentsViewModel.DropoffTimeDic[i].Value : MyDateTime.Now))
                            {
                                isFaulty = true;
                            }

                            if (userInfo.Roles.Contains("BankBranch") || userInfo.Roles.Contains("BankHybrid"))
                            {
                                if (isFaulty)
                                {
                                    ValidationError = $"some shipments doesn't contain either Pickup or DropOff branch from your assigned branches or contains previous Pickup or DropOff Time or Pickup Time is greater than Dropoff Time";
                                    BulkShipmentsViewModel.FaultyKeys.Add(i);
                                }
                            }
                            else
                            {
                                if (isFaulty)
                                {
                                    ValidationError = $"some shipments contains previous Pickup or DropOff Time or Pickup Time is greater than Dropoff Time";
                                    BulkShipmentsViewModel.FaultyKeys.Add(i);
                                }
                            }
                        }
                    }
                }


                if (ValidationError != null)
                {
                    await UnBlockFormUI();
                    return;
                }

                //if (BulkShipmentsViewModel.PickupTimeDic.Any(x => x.Value.HasValue && x.Value.Value < MyDateTime.Now) || BulkShipmentsViewModel.DropoffTimeDic.Any(x => x.Value.HasValue && x.Value.Value < MyDateTime.Now))
                //{
                //    ValidationError = "You cannot select previous Pickup or Dropoff Time";
                //    await UnBlockFormUI();
                //    return;
                //}
                //if (BulkShipmentsViewModel.PickupTimeDic.Any(x => x.Value.HasValue && x.Value.Value >= (BulkShipmentsViewModel.DropoffTimeDic.Where(y => y.Key == x.Key).Select(x => x.Value.HasValue ? x.Value.Value : MyDateTime.Now).FirstOrDefault())))
                //{
                //    ValidationError = "DropOff Time should be greater then Pickup Time!";
                //    await UnBlockFormUI();
                //    return;
                //}

                var resp = await DataService.PostBulkShipments(BulkShipmentsViewModel);
                if (resp > 0)
                {
                    ValidationError = null;
                    BulkShipmentsViewModel = default;
                    await LoadItems();
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            await UnBlockFormUI();
        }

        private void ApproveShipment(int consignmentId, ConsignmentApprovalState approvalState)
        {

            ConsignmentApprrovalViewModel = new ConsignmentApprrovalViewModel
            {
                ConsignmentId = consignmentId,
                ApprovalState = approvalState
            };

        }

        private void OnConsignmentStatusChangeClicked(int consignmentId, ConsignmentStatus consignmentStatus)
        {

            ConsignmentStatusViewModel = new ConsignmentStatusViewModel
            {
                ConsignmentId = consignmentId,
                ConsignmentStatus = consignmentStatus
            };
        }

        private async Task ApproveConsignmentStatus()
        {
            try
            {
                await BlockFormUI();
                var response = await DataService.ApproveConsignmentStatus(ConsignmentApprrovalViewModel);
                if (response > 0)
                {
                    ConsignmentApprrovalViewModel = default;
                    await loadItems();
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message);
            }
            finally
            {
                await UnBlockFormUI();
            }

        }
        private async Task PostConsignmentStatus()
        {
            try
            {
                await BlockFormUI();
                var response = await DataService.PostConsignmentStatus(ConsignmentStatusViewModel);
                if (response > 0)
                {
                    ConsignmentStatusViewModel = default;
                    await loadItems();
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message);
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        public SealCodeFormViewModel SealCodes { get; set; }
        private async Task EditDenominationModel(int id, bool finalizeShipment, bool enableSkip)
        {
            await BlockFormUI();
            try
            {
                SealCodes = new SealCodeFormViewModel();

                DedicatedVehicles = await DataService.GetDedicatedVehicles(id);
                if (DedicatedVehicles != null && DedicatedVehicles.Count > 0)
                    DedicatedVehicles.Insert(0, new SelectListItem(0, "Non Dedicated Vehicle"));

                ValidationError = default;
                CitDenominationViewModel = await DataService.GetDenomination(id);
                if (CitDenominationViewModel != null)
                {
                    FormTitle = CitDenominationViewModel.ShipmentCode;
                    CitDenominationViewModel.FinalizeShipment = finalizeShipment;
                    CitDenominationViewModel.EnableSkip = enableSkip;
                }

            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message);
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        public void NewAmount(int prevAmount, int totalAmount)
        {
            AmountPKR = prevAmount;
            TotalAmount = totalAmount;
        }
        private async Task EditDeliveryTime(int consignmentId, bool newShipment)
        {

            ValidationError = default;
            CitDenominationViewModel = null;
            if (newShipment)
            {
                await BlockFormUI();
                DedicatedVehicles = await DataService.GetDedicatedVehicles(consignmentId);
                if (DedicatedVehicles != null && DedicatedVehicles.Count > 0)
                    DedicatedVehicles.Insert(0, new SelectListItem(0, "Non Dedicated Vehicle"));
                //else DedicatedVehicles = new List<SelectListItem>() { (new SelectListItem(0, "Non Dedicated Vehicle")) };
                DeliveryTimeViewModel = await DataService.GetDeliveryTime(consignmentId);
                await UnBlockFormUI();
            }
        }

        private async Task OpenShipmentStatusForm(int consignmentId)
        {

            ValidationError = default;
            await BlockFormUI();
            ShipmentStatusViewModel = (from c in context.Consignments
                                       where c.Id == consignmentId
                                       select new ShipmentStatusViewModel()
                                       {
                                           ConsignmentId = c.Id,
                                           ConsignmentState = c.ConsignmentStateType,
                                           PickupTime = MyDateTime.Now
                                       }).FirstOrDefault();
            await UnBlockFormUI();
        }

        private async Task OnShipmentStatusFormSubmit()
        {
            await BlockFormUI();
            try
            {
                var s = (from c in context.ConsignmentDeliveries.Include(x => x.Consignment)
                         where c.ConsignmentId == ShipmentStatusViewModel.ConsignmentId
                         select c).FirstOrDefault();

                s.Consignment.ConsignmentStateType = ShipmentStatusViewModel.ConsignmentState;
                s.DeliveryState = ShipmentStatusViewModel.ConsignmentState;


                var ds = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == ShipmentStatusViewModel.ConsignmentId
                && x.DeliveryId == s.Id
                && x.ConsignmentStateType == ShipmentStatusViewModel.ConsignmentState);
                if (ds == null)
                {
                    ds = new ConsignmentState()
                    {
                        ConsignmentId = s.ConsignmentId,
                        DeliveryId = s.Id,
                        CreatedBy = userInfo.UserName,
                        ConsignmentStateType = ShipmentStatusViewModel.ConsignmentState
                    };
                }
                ds.Status = StateTypes.Confirmed;
                ds.TimeStamp = ShipmentStatusViewModel.PickupTime.GetValueOrDefault();

                await context.SaveChangesAsync();

                var scope = scopeFactory.CreateScope();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                await shipmentsCacheService.SetShipment(ShipmentStatusViewModel.ConsignmentId, null);
                ShipmentStatusViewModel = null;
                await LoadItems(true);

            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            await UnBlockFormUI();
        }

        private async Task RepushShipment(int id)
        {
            await BlockFormUI();
            try
            {
                var c = await context.Consignments.FirstOrDefaultAsync(x => x.Id == id);
                c.ConsignmentStatus = ConsignmentStatus.Pushing;
                await context.SaveChangesAsync();

                var scope = scopeFactory.CreateScope();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                await shipmentsCacheService.SetShipment(id, null);
                await LoadItems(true);

            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            await UnBlockFormUI();
        }

        private async Task SaveDenomination()
        {
            var citShipmentsService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<CitCardsService>();

            citShipmentsService.User = DataService.User;
            IsModalBusy = true;
            var newShipment = false;
            if (CitDenominationViewModel.DedicatedVehicle > 0)
            {
                var Error = await DataService.AssignCrewFromAssetId(CitDenominationViewModel.DedicatedVehicle, CitDenominationViewModel.ConsignmentId);
                if (!string.IsNullOrEmpty(Error))
                {
                    ValidationError = Error;
                    await UnBlockFormUI();
                    return;
                }
                DedicatedVehicle = 0;
            }
            if (CitDenominationViewModel.FinalizeShipment && SealCodes != null && !SealCodes.IsPosted)
            {
                int noOfBags;
                try
                {
                    noOfBags = Convert.ToInt32(SealCodes.NoOfBags);
                }
                catch
                {
                    noOfBags = 0;
                }

                if (noOfBags < 0)
                {
                    ValidationError = "No of bags can't be a negative number";
                    IsModalBusy = false;
                    return;
                }

                if (await CheckSealCodeDuplication(CitDenominationViewModel.ConsignmentId))
                {
                    IsModalBusy = false;
                    return;
                }
                else
                {
                    ValidationError = await DataService.PostSealCodesAndNoOfBags(SealCodes.SealCodes, CitDenominationViewModel.ConsignmentId, noOfBags);
                    if (!string.IsNullOrEmpty(ValidationError))
                    {
                        IsModalBusy = false;
                        return;
                    }
                }
                SealCodes.IsPosted = true;

            }
            try
            {
                ConsignmentId = CitDenominationViewModel.ConsignmentId;

                newShipment = CitDenominationViewModel.NewShipment;

                var showSchdule = CitDenominationViewModel.EnableSkip;
                await BlockFormUI();
                if (CitDenominationViewModel.CurrencySymbol == CurrencySymbol.Other || CitDenominationViewModel.CurrencySymbol == CurrencySymbol.MixCurrency)
                    TotalAmount = AmountPKR;
                if (CitDenominationViewModel.TotalAmount != TotalAmount)
                {
                    if (!CitDenominationViewModel.SaveNewAmount)
                    {
                        DenominationChangeAmountViewModel = new DenominationChangeAmountViewModel
                        {
                            prevAmount = CitDenominationViewModel.AmountPKR,
                            newAmount = AmountPKR
                        };
                    }
                    else
                    {
                        CitDenominationViewModel.TotalAmount = TotalAmount.GetValueOrDefault();
                        CitDenominationViewModel.AmountPKR = AmountPKR;
                        var id = await citShipmentsService.PostDenomination(CitDenominationViewModel);
                        if (id > 0)
                        {
                            CitDenominationViewModel = null;
                        }
                    }
                }
                else
                {
                    var id = await citShipmentsService.PostDenomination(CitDenominationViewModel);
                    if (id > 0)
                    {
                        CitDenominationViewModel = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            IsModalBusy = false;

            if (newShipment)
                if (CitDenominationViewModel == null)// && showSchdule)
                {
                    await EditDeliveryTime(ConsignmentId, newShipment);
                }
        }
        private async Task<bool> CheckSealCodeDuplication(int consignmentId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                foreach (var item in SealCodes.SealCodes)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(item.SealCode) || Convert.ToInt32(item.SealCode) < 1)
                            item.Error = "Invalid seal code";

                        //else if (context.ShipmentSealCodes.Any(x => x.SealCode == item.SealCode && x.ConsignmentId == consignmentId))
                        //    item.Error = "Seal code already exists";

                        else if (SealCodes.SealCodes.Where(x => x.SealCode == item.SealCode).Count() > 1)
                            item.Error = "Duplicate seal code";

                        else item.Error = null;
                    }
                    catch
                    {
                        item.Error = "Invalid seal code";
                    }
                }
                StateHasChanged();
            }
            return SealCodes.SealCodes.Any(x => !string.IsNullOrEmpty(x.Error));
        }

        private async Task SaveCharges()
        {
            await BlockFormUI();
            try
            {
                var id = await DataService.PostServiceCharges(DeliveryChargesModel);
                if (id > 0)
                {
                    DeliveryChargesModel = null;
                    await LoadItems();
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message);
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        private async Task SaveCrewAssignment()
        {
            await BlockFormUI();
            try
            {
                var id = await DataService.AssignCrew(DeliveryCrewFormViewModel);
                if (id > 0)
                {
                    DeliveryCrewFormViewModel = null;
                    await LoadItems();
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message);
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        private async Task AssignCrewClicked(int consignmentId, int deliveryId, int? crewId)
        {
            await BlockFormUI();
            DeliveryCrewFormViewModel = new DeliveryCrewFormViewModel()
            {
                DeliveryId = deliveryId,
                ConsignmenId = consignmentId,
                CrewId = crewId
            };
            try
            {
                Crews = await DataService.GetCrewsWithLocationMatrix(consignmentId);
                //Crews = Crews.OrderBy(x => x.PickeupStats_);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
                ValidationError = ex.Message;
            }
            finally
            {
                await UnBlockFormUI();
            }
        }


        private async Task OnScheduleVaulButtonClicked(int consignmentId, int deliveryId)
        {


        }
        private async Task LoadVaults(OrganizationType t)
        {
            searchKey = "";
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            VaultNowViewModel.VaultId = 0;
            if (t == OrganizationType.Unknown) //for all vaults
            {
                VaultNowViewModel.Crews = await (from o in context.Parties
                                                 join org in context.Orgnizations on o.Id equals org.Id
                                                 join region in context.Parties on o.RegionId equals region.Id
                                                 from subRegion in context.Parties.Where(x => x.Id == o.SubregionId).DefaultIfEmpty()
                                                 from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                                                 where (org.OrganizationType == OrganizationType.Vault || org.OrganizationType == OrganizationType.VaultOnWheels)

                                                 orderby region.FormalName
                                                 select new VaultFormListItem()
                                                 {
                                                     Id = o.Id,
                                                     Name = o.FormalName,
                                                     RegionName = region.FormalName,
                                                     SubRegionName = subRegion.FormalName,
                                                     StationName = station.FormalName
                                                 }).ToListAsync();
            }
            else
            {
                VaultNowViewModel.Crews = await (from o in context.Parties
                                                 join org in context.Orgnizations on o.Id equals org.Id
                                                 join region in context.Parties on o.RegionId equals region.Id
                                                 from subRegion in context.Parties.Where(x => x.Id == o.SubregionId).DefaultIfEmpty()
                                                 from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                                                 where (org.OrganizationType == t)
                                                 && (BaseIndexModel.RegionId == 0 || o.RegionId == BaseIndexModel.RegionId)
                                                 && (BaseIndexModel.SubRegionId == 0 || o.SubregionId == BaseIndexModel.SubRegionId)
                                                 && (BaseIndexModel.StationId == 0 || o.StationId == BaseIndexModel.StationId)

                                                 orderby region.FormalName
                                                 select new VaultFormListItem()
                                                 {
                                                     Id = o.Id,
                                                     Name = o.FormalName,
                                                     RegionName = region.FormalName,
                                                     SubRegionName = subRegion.FormalName,
                                                     StationName = station.FormalName
                                                 }).ToListAsync();
            }
            VaultNowViewModel.VaultType = t;
        }
        private async Task OnVaultOutButtonClicked(int consignmentId, int deliveryId)
        {
            await BlockFormUI();
            VaultOutCrewFormViewModel = new DeliveryCrewFormViewModel()
            {
                DeliveryId = deliveryId,
                ConsignmenId = consignmentId
            };
            try
            {
                Crews = await DataService.GetCrewsWithLocationMatrix(consignmentId);
                Crews = Crews.OrderBy(x => x.ConsignmentDistance_);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
                ValidationError = ex.Message;
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        private async Task SaveVaultOutCrew()
        {
            await BlockFormUI();
            try
            {
                await DataService.VaultOutShipment(VaultOutCrewFormViewModel);
                VaultOutCrewFormViewModel = null;
                await LoadItems(true);

            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message);
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        private async Task OnVaultNowButtonClicked(int consignmentId, int deliveryId)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var crews = await (from o in context.Parties
                               join org in context.Orgnizations on o.Id equals org.Id
                               join region in context.Parties on o.RegionId equals region.Id
                               from subRegion in context.Parties.Where(x => x.Id == o.SubregionId).DefaultIfEmpty()
                               from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                               where (org.OrganizationType == OrganizationType.Vault)
                                  && (BaseIndexModel.RegionId == 0 || o.RegionId == BaseIndexModel.RegionId)
                               && (BaseIndexModel.SubRegionId == 0 || o.SubregionId == BaseIndexModel.SubRegionId)
                               && (BaseIndexModel.StationId == 0 || o.StationId == BaseIndexModel.StationId)
                               orderby region.FormalName
                               select new VaultFormListItem()
                               {
                                   Id = o.Id,
                                   Name = o.FormalName,
                                   RegionName = region.FormalName,
                                   SubRegionName = subRegion.FormalName,
                                   StationName = station.FormalName
                               }).ToListAsync();

            VaultNowViewModel = new VaultNowViewModel()
            {
                ConsignmentId = consignmentId,
                DeliveryId = deliveryId,
                Crews = crews,
                VaultType = OrganizationType.Vault
            };
        }

        private async Task OnVaultNowSubmit()
        {
            await BlockFormUI();
            try
            {
                if (VaultNowViewModel.VaultId == 0)
                    throw new Exception("Please Select Where To Vault");
                var resp = await DataService.AssignVault(VaultNowViewModel);
                if (resp > 0)
                {
                    await LoadItems();
                    VaultNowViewModel = default;
                    ValidationError = default;
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            await UnBlockFormUI();
        }


        private async Task ShowCrew(int? crewId, bool getCrewPic = false)
        {
            await BlockFormUI();
            var response = await DataService.GetCrewMembers(crewId.GetValueOrDefault());

            if (getCrewPic)
                CrewMembersDetailViewModel = response.Items;
            else
                CrewMembersViewModel = response.Items;

            await UnBlockFormUI();
        }
        private async Task ShowConsigments(int? crewId)
        {
            await BlockFormUI();
            if (crewId.GetValueOrDefault() > 0)
            {
                ShowConsignmentsViewModel = await DataService.GetConsignments(crewId.GetValueOrDefault());
            }
            IsModalBusy = false;
        }
        private void OnShipmentPlanClicked(ConsignmentListViewModel item)
        {
            ValidationError = null;
            DomesticShipmentViewModel = new DomesticShipmentViewModel()
            {
                Deliveries = item.Deliveries,
                ConsignmentId = item.Id,
                PickupBranchName = $"{item.FromPartyCode} {item.FromPartyName}",
                PickupBranchAddress = item.FromPartyAddress,
                DropoffBranchName = $"{item.ToPartyCode} {item.ToPartyName}",
                DropoffBranchAddress = item.ToPartyAddress
            };
        }

        private async Task OnConsignmentDeliveryClicked(int consignmentId)
        {
            await BlockFormUI();
            try
            {
                ValidationError = null;
                DeliveryFormViewModel = new DeliveryFormViewModel()
                {
                    ConsignmentId = consignmentId,
                    Crews = await DataService.GetCrews(),
                    Locations = await DataService.GetLocations(LocationType.Address),
                    PlanedPickupTime = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
                ValidationError = ex.Message;
            }
            finally
            {
                await UnBlockFormUI();
            }
        }


        private async Task SaveConsignmentDelivery()
        {
            await BlockFormUI();
            try
            {
                var id = await DataService.PostConsignmentDelivery(DeliveryFormViewModel);
                if (id > 0)
                {
                    await LoadItems();
                    OnShipmentPlanClicked(Items.FirstOrDefault(x => x.Id == DeliveryFormViewModel.ConsignmentId));
                    DeliveryFormViewModel = null;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
                ValidationError = ex.Message;
            }
            finally
            {
                await UnBlockFormUI();
            }
        }

        private async Task FinalizeShipment()
        {
            try
            {
                await BlockFormUI();
                var distanceUpdateResult = await DataService.UpdateShipmentDistance(ShipmentAdministrationViewModel);

                Logger.LogWarning($"distanceUpdateResult is null {distanceUpdateResult == null}");
                Logger.LogWarning($"distanceUpdateResult.Repushed is null {distanceUpdateResult.Repushed == null}");
                Logger.LogWarning($"distanceUpdateResult.Updated is null {distanceUpdateResult.Updated == null}");


                if (distanceUpdateResult.Repushed?.Count > 0)
                {
                    await JSRuntime.InvokeVoidAsync("toast.show", "Following shipments will be re-pushed due to distance update", $"{string.Join("<br>", (distanceUpdateResult.Repushed.Select(x => x.Text)))}", "warning");
                }
                if (distanceUpdateResult.Updated?.Count > 0)
                {
                    await JSRuntime.InvokeVoidAsync("toast.show", "Distance updated for following Shipments", $"{string.Join("<br>", (distanceUpdateResult.Updated.Select(x => x.Text)))}", "success");
                }
                ShipmentAdministrationViewModel = null;
                await LoadItems(false);
            }
            finally
            {
                await UnBlockFormUI();
                await InvokeAsync(() => StateHasChanged());
            }
        }

        private int? CalculateAmount(CitDenominationViewModel CitDenominationViewModel, DenominationType denominationType)
        {

            int mf = 1;
            if (denominationType == DenominationType.Packets)
            {
                mf = 100;
            }
            else if (denominationType == DenominationType.Bundles)
            {
                mf = 1000;
            }

            TotalAmount =
                            (CitDenominationViewModel.Currency1x.GetValueOrDefault() * 1 * mf)
                            + (CitDenominationViewModel.Currency2x.GetValueOrDefault() * 2 * mf)
                            + (CitDenominationViewModel.Currency5x.GetValueOrDefault() * 5 * mf)
                            + (CitDenominationViewModel.Currency10x.GetValueOrDefault() * 10 * mf)
                            + (CitDenominationViewModel.Currency20x.GetValueOrDefault() * 20 * mf)
                            + (CitDenominationViewModel.Currency50x.GetValueOrDefault() * 50 * mf)
                            + (CitDenominationViewModel.Currency75x.GetValueOrDefault() * 75 * mf)
                            + (CitDenominationViewModel.Currency100x.GetValueOrDefault() * 100 * mf)
                            + (CitDenominationViewModel.Currency500x.GetValueOrDefault() * 500 * mf)
                            + (CitDenominationViewModel.Currency1000x.GetValueOrDefault() * 1000 * mf)
                            + (CitDenominationViewModel.Currency5000x.GetValueOrDefault() * 5000 * mf);


            return TotalAmount.GetValueOrDefault();

        }
        public byte[] Export(string writerFormat, int consignmentId)
        {

            //var consignmentId = Convert.ToInt32(reportOption.ControlId.Split('-').Last());
            #region Actual Data
            var model = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join b in context.Parties on c.BillBranchId equals b.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         from n in context.Denominations.Where(x => x.ConsignmentId == c.Id).DefaultIfEmpty()
                         from cr in context.Parties.Where(x => x.Id == d.CrewId).DefaultIfEmpty()
                         where c.Id == consignmentId
                         select new ConsignmentReportViewModel()
                         {
                             AC_Code = c.Id.ToString(),
                             Amount = c.CurrencySymbol == CurrencySymbol.PrizeBond ? c.AmountPKR.ToString("N0") : c.Amount.ToString("N0"),
                             AmountInWords = c.CurrencySymbol.ToString() + " " + ((c.CurrencySymbol == CurrencySymbol.MixCurrency || c.CurrencySymbol == CurrencySymbol.PrizeBond || c.CurrencySymbol == CurrencySymbol.Other) ? c.AmountPKR.ToString() : CurrencyHelper.AmountInWords(c.Amount)),
                             AmountInWordsPKR = ((c.CurrencySymbol == CurrencySymbol.MixCurrency || c.CurrencySymbol == CurrencySymbol.PrizeBond || c.CurrencySymbol == CurrencySymbol.Other) ? "" : "PKR ") + CurrencyHelper.AmountInWords(c.AmountPKR),
                             AmountType = n.DenominationType.ToString(),
                             ExchangeRate = c.ExchangeRate,
                             Date = c.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                             //Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                             PickupBranch = f.ShortName + " - " + f.FormalName,
                             DeliveryBranch = t.ShortName + " - " + t.FormalName,
                             FirstCopyName = f.ShortName,
                             ThirdCopyName = t.ShortName,
                             CurrencySymbol = c.CurrencySymbol.ToString(),
                             CurrencySymbol_ = c.CurrencySymbol,
                             //ConsignedByName1 = c.CreatedBy,
                             //ConsignedByName2 = c.ApprovedBy,
                             ConsignedByName1 = context.Users.Where(x => x.UserName == c.CreatedBy).Select(x => x.Name).FirstOrDefault() ?? c.CreatedBy,
                             ConsignedByName2 = context.Users.Where(x => x.UserName == c.ApprovedBy).Select(x => x.Name).FirstOrDefault() ?? c.ApprovedBy,
                             AcceptedByName1 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId
                                                && r.FromPartyRole == RoleType.CheifCrewAgent && r.IsActive
                                                select p).FirstOrDefault().FormalName,
                             AcceptedByName2 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId && r.IsActive
                                                && r.FromPartyRole == RoleType.AssistantCrewAgent
                                                select p).FirstOrDefault().FormalName,
                             PickupTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                             DeliveryTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                             SealNos = context.ShipmentSealCodes.Where(x => x.ConsignmentId == c.Id).Select(x => x.SealCode).ToList(),
                             VehicleNo = context.AssetAllocations.FirstOrDefault(x => x.PartyId == d.CrewId).Asset.Description,
                             CustomerToBeBilledName = b.ShortName + " - " + b.FormalName,
                             CustomerToBeBilled = b.ShortName + " - " + b.FormalName,
                             ShipmentRecieptNo = c.ShipmentCode,
                             Currency5000x = n.Currency5000x,
                             Currency1000x = n.Currency1000x,
                             Currency500x = n.Currency500x,
                             Currency100x = n.Currency100x,
                             Currency75x = n.Currency75x,
                             Currency50x = n.Currency50x,
                             Currency20x = n.Currency20x,
                             Currency10x = n.Currency10x,
                             Currency5x = n.Currency5x,
                             Currency2x = n.Currency2x,
                             Currency1x = n.Currency1x,

                             Currency40000x = n.Currency40000x,
                             Currency15000x = n.Currency15000x,
                             Currency25000x = n.Currency25000x,
                             Currency7500x = n.Currency7500x,
                             Currency1500x = n.Currency1500x,
                             Currency750x = n.Currency750x,
                             Currency200x = n.Currency200x,

                             PrizeMoney40000x = n.PrizeMoney40000x,
                             PrizeMoney15000x = n.PrizeMoney15000x,
                             PrizeMoney25000x = n.PrizeMoney25000x,
                             PrizeMoney7500x = n.PrizeMoney7500x,
                             PrizeMoney1500x = n.PrizeMoney1500x,
                             PrizeMoney750x = n.PrizeMoney750x,
                             PrizeMoney200x = n.PrizeMoney200x,
                             PrizeMoney100x = n.PrizeMoney100x,

                             Currency5000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5000x, 5000, n.DenominationType),
                             Currency1000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1000x, 1000, n.DenominationType),
                             Currency500xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency500x, 500, n.DenominationType),
                             Currency100xAmount = c.CurrencySymbol == CurrencySymbol.PrizeBond ? CurrencyHelper.PrizeBondFormula(100, n.Currency100x, n.PrizeMoney100x, n.DenominationType) : CurrencyHelper.CalculateAmountOneByOne(n.Currency100x, 100, n.DenominationType),
                             Currency50xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency50x, 50, n.DenominationType),
                             Currency75xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency75x, 75, n.DenominationType),
                             Currency20xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency20x, 20, n.DenominationType),
                             Currency10xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency10x, 10, n.DenominationType),
                             Currency5xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5x, 5, n.DenominationType),
                             Currency2xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency2x, 2, n.DenominationType),
                             Currency1xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1x, 1, n.DenominationType),

                             Currency40000xAmount = CurrencyHelper.PrizeBondFormula(40000, n.Currency40000x, n.PrizeMoney40000x, n.DenominationType),
                             Currency25000xAmount = CurrencyHelper.PrizeBondFormula(25000, n.Currency25000x, n.PrizeMoney25000x, n.DenominationType),
                             Currency15000xAmount = CurrencyHelper.PrizeBondFormula(15000, n.Currency15000x, n.PrizeMoney15000x, n.DenominationType),
                             Currency7500xAmount = CurrencyHelper.PrizeBondFormula(7500, n.Currency7500x, n.PrizeMoney7500x, n.DenominationType),
                             Currency1500xAmount = CurrencyHelper.PrizeBondFormula(1500, n.Currency1500x, n.PrizeMoney1500x, n.DenominationType),
                             Currency750xAmount = CurrencyHelper.PrizeBondFormula(750, n.Currency750x, n.PrizeMoney750x, n.DenominationType),
                             Currency200xAmount = CurrencyHelper.PrizeBondFormula(200, n.Currency200x, n.PrizeMoney200x, n.DenominationType),

                             Comments = c.Comments,
                             Valueables = c.Valueables,
                             NoOfBags = c.NoOfBags,
                             SealedBags = c.SealedBags,

                         }).FirstOrDefault();



            var comments = model.Comments != null ? JsonConvert.DeserializeObject<List<ShipmentComment>>(model.Comments).ToList() : new List<ShipmentComment>();
            StringBuilder concatedComment = new();
            foreach (var comment in comments)
            {
                concatedComment.Append(comment.Description + ",");
            }
            model.Comments = concatedComment.ToString();
            var reportName = @"\Reports\ConsignmentReceipt.rdlc";
            switch (model.CurrencySymbol_)
            {
                case CurrencySymbol.USD:
                case CurrencySymbol.EURO:
                    model.AmountInWords += $"\n{model.AmountInWordsPKR} \n\rExchange Rate:{model.ExchangeRate} PKR/{model.CurrencySymbol}";
                    break;

                case CurrencySymbol.MixCurrency:
                case CurrencySymbol.Other:
                    model.AmountInWords += $"\n{model.AmountInWordsPKR}";
                    reportName = @"\Reports\ConsignmentReceiptMulti.rdlc";
                    break;
                case CurrencySymbol.PrizeBond:
                    model.AmountInWords = model.AmountInWordsPKR;
                    reportName = @"\Reports\ConsignmentReceiptPrizebond.rdlc";
                    break;
            }

            string basePath = _hostingEnvironment.WebRootPath;
            // Here, we have loaded the Product List.rdlc sample report from application the Resources folder.
            FileStream reportStream = new FileStream(basePath + reportName, FileMode.Open, FileAccess.Read);
            BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter(reportStream);
            writer.ReportProcessingMode = (BoldReports.Writer.ProcessingMode)BoldReports.ReportViewerEnums.ProcessingMode.Local;


            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var sealNo in model.SealNos)
            {
                index++;
                if (index == model.SealNos.Count())
                    sb.Append(sealNo);
                else
                    sb.Append(sealNo + ",");
            }
            model.SealNo = sb.ToString();
            model.NoOfSeals = model.SealNos.Count();
            #endregion

            List<ConsignmentReportViewModel> datasource = new List<ConsignmentReportViewModel>();
            datasource.Add(model);

            // Pass the dataset collection for report
            writer.DataSources.Clear();
            writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "ConsignmentDataSet", Value = datasource });

            string fileName = null;
            WriterFormat format;
            string type = null;

            fileName = $"CR-{model.ShipmentRecieptNo}.pdf";
            type = "pdf";
            format = WriterFormat.PDF;

            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            // Download the generated export document to the client side.
            memoryStream.Position = 0;
            memoryStream.Position = 0;
            return memoryStream.ToArray();

        }



        public async Task SaveAs(int id, string shipmentCode)
        {
            await BlockTableUI();
            await Task.Delay(2);
            byte[] data = null;
            try
            {
                data = Export("pdf", id);

            }
            catch (Exception ex)
            {
                Logger.LogError($"Failer while downloading file {ex}");
            }
            try
            {
                if (data != null)
                {
                    await JSRuntime.InvokeVoidAsync(
                         "saveAsFile",
                         shipmentCode.Replace("/", "_") + ".pdf",
                         Convert.ToBase64String(data));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failer while rendering file {ex}");
            }
            finally
            {
                await UnBlockTableUI();
            }
        }
        public async Task EditBranch(int fromPartyId, int toPartyId, int consignmentId)
        {
            await BlockFormUI();
            BranchFormViewModel = new BranchFormViewModel();
            Logger?.LogInformation($"fromPartyId = {fromPartyId} , toPartyId = {toPartyId} ,consignmentId = {consignmentId}");
            if (fromPartyId > 0)
                BranchFormViewModel = await DataService.GetBranchData(fromPartyId);
            else
                BranchFormViewModel = await DataService.GetBranchData(toPartyId);
            BranchFormViewModel.ConsignmentId = consignmentId;
            await UnBlockFormUI();
        }
        private ChangeDropoffFormViewModel changeDropoffFormViewModel;
        public async Task ChangeDropoffBranch(int toPartyId, string toPartyCode, string toPartyName, int? billBranchId, int consignmentId)
        {
            changeDropoffFormViewModel = new ChangeDropoffFormViewModel()
            {
                PreviousDropoffBranchId = toPartyId,
                DropoffBranchId = toPartyId,
                DropOffBrannchName = toPartyCode + "-" + toPartyName,
                BillBranchId = billBranchId,
                ConsignmentId = consignmentId,
                Message = (toPartyId == billBranchId) ? "Previous Dropoff and bill branch are same, Changing the dropoff branch will also change the billing branch." : null
            };
        }
        private async Task SubmitDeliveryTimeForm()
        {
            await BlockFormUI();
            try
            {
                if (DedicatedVehicle > 0)
                {
                    var Error = await DataService.AssignCrewFromAssetId(DedicatedVehicle, DeliveryTimeViewModel.ConsignmentId);
                    if (!string.IsNullOrEmpty(Error))
                    {
                        ValidationError = Error;
                        await UnBlockFormUI();
                        return;
                    }
                    DedicatedVehicle = 0;
                }
                if (ScheduleThisShipment)
                {
                    var resp = await DataService.PostDeliveryTime(DeliveryTimeViewModel);
                    if (resp > 0)
                    {
                        DeliveryTimeViewModel = null;
                        ValidationError = null;
                        await loadItems();
                        ScheduleThisShipment = false;
                        HideButtons = true;
                    }
                }
                else
                {
                    DeliveryTimeViewModel = null;
                    ValidationError = null;
                    await loadItems();
                    ScheduleThisShipment = false;
                    HideButtons = true;
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            await UnBlockFormUI();
        }

        private SelectListItem ToSelectListItem(ConsignmentStatus status)
        {
            return new SelectListItem(status.ToString(), EnumHelper.GetDisplayValue(status));
        }

        private SelectListItem ToSelectListItem(ConsignmentDeliveryState status)
        {
            return new SelectListItem(status.ToString(), EnumHelper.GetDisplayValue(status));
        }

        public int RatingValue { get; set; }
        public MixedCurrencyViewModel MixedCurrencyViewModel { get; private set; }

        public async Task ChangeBranchInfo()
        {
            await BlockFormUI();
            int Response = 0;
            try
            {
                Response = await DataService.ChangeBranchData(BranchFormViewModel);
                await LoadItems();
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            if (string.IsNullOrEmpty(ValidationError) || Response > 0)
            {
                BranchFormViewModel = default;
                ValidationError = null;
            }
            await UnBlockFormUI();
        }
        public async Task ChangeDropOffBranch()
        {
            await BlockFormUI();

            try
            {
                ValidationError = null;
                if (changeDropoffFormViewModel.PreviousDropoffBranchId == changeDropoffFormViewModel.DropoffBranchId)
                    ValidationError = "There is no change in dropoff branch";
                else
                    ValidationError = await DataService.ChangeDropoffBranch(changeDropoffFormViewModel);

            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }
            if (string.IsNullOrEmpty(ValidationError))
            {
                changeDropoffFormViewModel = default;
                await LoadItems();
            }
            await UnBlockFormUI();
        }
        private async Task OnChange(int value, int consignmentId)
        {

            Logger.LogInformation($"Rating with 5 stars value changed to {value}");
            if (value > 0)
            {
                RatingValue = value;
                RatingControlViewModel = new RatingControlViewModel()
                {
                    ConsignmentId = consignmentId,
                    RatingValue = value
                };
                if (value <= 2)
                {
                    await GetRatings(consignmentId);
                }
                else
                {
                    RatingCategoriesViewModel = default;
                }
                await BlockFormUI();
                try
                {
                    var resp = await DataService.PostRatings(RatingControlViewModel);
                }
                catch (Exception ex)
                {
                    ValidationError = ex.Message;
                    Logger.LogInformation(ex.Message + ex.InnerException);
                }
                await UnBlockFormUI();
            }

        }
        private async Task GetRatings(int consignmentId)
        {
            await BlockFormUI();
            RatingCategoriesViewModel = await DataService.GetRatingCategories(consignmentId) ?? new RatingCategoriesViewModel();
            RatingCategoriesViewModel.RatingValue = RatingValue;
            RatingCategoriesViewModel.ConsignmentId = consignmentId;
            await UnBlockFormUI();
        }
        private async Task OnAddCommentsClicked(int consignmentId)
        {
            await BlockFormUI();
            try
            {
                ShipmentCommentsViewModel = await DataService.GetComments(consignmentId);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
            }
            await UnBlockFormUI();
        }
        private async Task AddComment()
        {
            await BlockFormUI();
            try
            {
                int resp = await DataService.PostComment(ShipmentCommentsViewModel);
                if (resp > 0)
                {
                    ShipmentCommentsViewModel = default;
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
            }

            await UnBlockFormUI();
        }
        private async Task PostRating()
        {
            await BlockFormUI();
            try
            {
                var resp = await DataService.PostRatingCategories(RatingCategoriesViewModel);
                if (resp > 0)
                {
                    ValidationError = default;
                    RatingCategoriesViewModel = default;
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message + ex.InnerException);
            }
            await UnBlockFormUI();
        }
        private async Task GetMixedCurrency(int consigmentId, bool finalize)
        {
            await BlockFormUI();
            try
            {
                DedicatedVehicles = await DataService.GetDedicatedVehicles(consigmentId);
                if (DedicatedVehicles != null && DedicatedVehicles.Count > 0)
                    DedicatedVehicles.Insert(0, new SelectListItem(0, "Non Dedicated Vehicle"));

                MixedCurrencyViewModel = await DataService.GetMixCurrency(consigmentId) ?? new MixedCurrencyViewModel()
                {
                    ConsignmentId = consigmentId
                };

                MixedCurrencyViewModel.Finalize = finalize;
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message + ex.InnerException);
            }
            await UnBlockFormUI();
        }
        private async Task UpdateMixCurrency()
        {
            await BlockFormUI();
            try
            {
                if (DedicatedVehicle > 0)
                {
                    var Error = await DataService.AssignCrewFromAssetId(DedicatedVehicle, MixedCurrencyViewModel.ConsignmentId);
                    if (!string.IsNullOrEmpty(Error))
                    {
                        ValidationError = Error;
                        await UnBlockFormUI();
                        return;
                    }
                    DedicatedVehicle = 0;
                }
                var resp = await DataService.UpdateMixCurrency(MixedCurrencyViewModel);
                if (resp > 0)
                {
                    ValidationError = default;
                    MixedCurrencyViewModel = default;
                    await LoadItems(true);
                }
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message + ex.InnerException);
            }
            await UnBlockFormUI();
        }
        private async Task InTransitClick(int consignmentId)
        {
            await BlockFormUI();
            try
            {
                TransitTimeViewModel = await DataService.GetTransitTime(consignmentId);
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message + ex.InnerException);
            }
            await UnBlockFormUI();
        }
    }
}
