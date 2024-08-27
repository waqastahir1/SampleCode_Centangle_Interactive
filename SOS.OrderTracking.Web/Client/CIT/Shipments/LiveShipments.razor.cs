using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SOS.OrderTracking.Web.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System.Security.Claims;
using static SOS.OrderTracking.Web.Shared.UserInfoViewModel;
using SOS.OrderTracking.Web.Shared.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations;
using Radzen;
using System.Threading;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings;
using System.Linq.Expressions;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SOS.OrderTracking.Web.Client.CIT.Shipments
{
    public enum ConsignmentCards
    {
        Consignment,
        Denomination,
        Charges,
        DeliveryStates
    }

    public partial class LiveShipments
    {
        [Parameter]
        public string Type { get; set; }

        #region Properties

        private HubConnection hubConnection;
        public string HubConnectionStatusColour { get; set; }
        public bool AddLocationClicked { get; set; }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
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

                NotifyPropertyChanged();
            }
        }
        private ShipmentExecutionType _consignmentType;

        [Parameter]
        public ShipmentExecutionType ConsignmentType
        {
            get { return _consignmentType; }
            set
            {
                if (_consignmentType != value)
                {
                    _consignmentType = value;

                    NotifyPropertyChanged();
                }
            }

        }


        public BulkShipmentsViewModel BulkShipmentsViewModel { get; set; }
        [Inject]
        public ApiService HttpService { get; set; }


        public bool Sender { get; set; }

        private string FormTitle { get; set; }

        [Range(1, 300000000, ErrorMessage = "Amount cannot be greater then three hundred million.")]
        public int? TotalAmount { get; set; }
        [Range(1, 300000000, ErrorMessage = "Amount cannot be greater then three hundred million.")]
        public int AmountPKR { get; set; }
        public string AmountInWords { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        private List<SelectListItem> _consignmentStateSummarizedTypes;
        private List<SelectListItem> ConsignmentStateSummarizedTypes
        {
            get
            {
                return _consignmentStateSummarizedTypes ??= new List<SelectListItem>()
                {
                    ToSelectListItem( ConsignmentStateSummarized.All),
                    ToSelectListItem( ConsignmentStateSummarized.Created),
                    ToSelectListItem( ConsignmentStateSummarized.CrewAssigned),
                    ToSelectListItem( ConsignmentStateSummarized.ReachedPickup),
                    ToSelectListItem( ConsignmentStateSummarized.InTransit),
                    ToSelectListItem( ConsignmentStateSummarized.ReachedDestination),
                    ToSelectListItem( ConsignmentStateSummarized.Delivered),

                };
            }
        }

        private ConsignmentStateSummarized _consignmentStateSummarized;

        public ConsignmentStateSummarized ConsignmentStateSummarized
        {
            get { return _consignmentStateSummarized; }
            set
            {
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
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.All),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.TobePosted),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.RePush),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Pushing),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Pushed),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.DuplicateSeals),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.DistanceIssue),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.PushingFailed),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Declined),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Cancelled)
            };
            }
        }


        private string _consignmentStatus;

        private string ConsignmentStatus
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
        private int ConsignmentId { get; set; }
        private string _searchKey;

        [Parameter]
        public string SearchKey { get; set; }

        private IEnumerable<SelectListItem> BillBranches { get; set; }

        private IEnumerable<SelectListItem> CPCCustomers { get; set; }

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

        IEnumerable<CrewMemberListModel> CrewMembersViewModel { get; set; }
        public IEnumerable<CrewMemberListModel> CrewMembersDetailViewModel { get; set; }

        public DomesticShipmentViewModel DomesticShipmentViewModel { get; set; }

        public DeliveryFormViewModel DeliveryFormViewModel { get; set; }

        public DenominationChangeAmountViewModel DenominationChangeAmountViewModel { get; set; }
        public ShipmentAdministrationViewModel ShipmentAdministrationViewModel { get; set; }
        public TransitTimeViewModel TransitTimeViewModel { get; set; }
        public UserInfo userInfo { get; set; }
        private int TabIndex { get; set; }
        int minute = DateTime.Now.Minute;
        #endregion

        public override string AdditionalParams
        {
            get => AdditionalParams = $"&SearchKey={SearchKey}&ConsignmentStatus={ConsignmentStatus}&ConsignmentStateSummarized={ConsignmentStateSummarized}&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}&ConsignmentType={ConsignmentType}&Rating={Rating}&Sorting={Sorting}";
            set => base.AdditionalParams = value;
        }

        public DotNetObjectReference<LiveShipments> DotNetRef;

        public LiveShipments()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            ConsignmentStatus = "All";
            ConsignmentType = ShipmentExecutionType.Live;
            ConsignmentStateSummarized = ConsignmentStateSummarized.All;
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
                    q.PropertyName == nameof(ConsignmentStateSummarized) ||
                    q.PropertyName == nameof(ConsignmentType)
                   || q.PropertyName == nameof(Rating)
                   || q.PropertyName == nameof(Sorting))
                {
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

            };

            OnSelectedItemCreated += async (x) =>
            {

                SelectedItem.OriginPartyId = userInfo.PartyId;

                if (SelectedItem.ToPartyId > 0 && SelectedItem.FromPartyId > 0)
                {

                    BillBranches = await ApiService.GetSiblingBranches(SelectedItem.ToPartyId, SelectedItem.FromPartyId);

                    await InvokeAsync(() => StateHasChanged());//
                }

                SelectedItem.PropertyChanged += async (p, q) =>
                {
                    if (q.PropertyName == nameof(SelectedItem.ToPartyId) || q.PropertyName == nameof(SelectedItem.FromPartyId))
                    {
                        if (SelectedItem.ToPartyId > 0 && SelectedItem.FromPartyId > 0)
                        {
                            BillBranches = await ApiService.GetSiblingBranches(SelectedItem.ToPartyId, SelectedItem.FromPartyId);

                            await InvokeAsync(() => StateHasChanged());//
                        }
                    }
                    if (SelectedItem.ExchangeRate == 0)
                        SelectedItem.ExchangeRate = 1;
                };

            };
        }

        [Inject]
        public IAccessTokenProvider AccessTokenProvider { get; set; }
        public DeliveryTimeViewModel DeliveryTimeViewModel { get; private set; }
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
        public DeliveryVaultViewModel DeliveryVaultViewModel { get; private set; }
        public bool HideButtons { get; private set; } = true;
        public RatingCategoriesViewModel RatingCategoriesViewModel { get; private set; }
        public RatingControlViewModel RatingControlViewModel { get; private set; }
        public ShipmentCommentsViewModel ShipmentCommentsViewModel { get; private set; }

        public class ShipmentMesage {

            public int Id { get; set; }
            public int FromPartyId { get; set; }
            public int ToPartyId { get; set; }

            public int? CollectionRegionId { get; set; }

            public int? CollectionSubRegionId { get; set; }

            public int? CollectionStationId { get; set; }


            public int? DeliveryRegionId { get; set; }

            public int? DeliverySubRegionId { get; set; }

            public int? DeliveryStationId { get; set; }

        }

        protected override async Task OnInitializedAsync()
        {
            FormTitle = "Create CIT Consignment";
            Crews = new List<CrewWithLocation>();
            try
            {
                userInfo = await ApiService.ApiService.GetFromJsonAsync<UserInfo>($"v1/Common/GetUserInfo");
                CPCCustomers = await ApiService.GetCPCBranches(userInfo.PartyId);
                #region ini signal R
                try
                {
                    hubConnection = new HubConnectionBuilder()
                     .WithUrl(NavigationManager.ToAbsoluteUri("/ConsignmentHub"), options =>
                     {

                         options.AccessTokenProvider = async () =>
                       {
                           var accessTokenResult = await AccessTokenProvider.RequestAccessToken();
                           accessTokenResult.TryGetToken(out var accessToken); 
                           return accessToken.Value;
                       };
                     })
                     .WithAutomaticReconnect(new RandomRetryPolicy())
                     .Build();

                    hubConnection.On<ShipmentMesage>("OnNewShipment", async ( message) =>
                    { 
                        if(string.IsNullOrEmpty(SearchKey) || ConsignmentStateSummarized == ConsignmentStateSummarized.All )
                        if ((userInfo.Roles.Contains("BankBranch") || userInfo.Roles.Contains("BankBranchManager"))  && (message.FromPartyId == userInfo.PartyId || message.ToPartyId == userInfo.PartyId)
                        ||  OrganizationalUnit.Regions.Any(x=>x.IntValue == message.CollectionRegionId) || OrganizationalUnit.Regions.Any(x => x.IntValue == message.DeliveryRegionId))
                        {
                            var target = Items.FirstOrDefault(x => x.Id == message.Id);
                            if (target == null)
                            {
                                var baseIndexModel = JsonConvert.DeserializeObject<CitCardsAdditionalValueViewModel>(JsonConvert.SerializeObject(BaseIndexModel));
                                baseIndexModel.Id = message.Id;
                                Logger.LogInformation($"message-> new {message}");
                                baseIndexModel.CurrentIndex = 1;
                                var data = await ApiService.GetPageAsync(baseIndexModel);
                                var item = data.Items.FirstOrDefault(x => x.Id == message.Id);
                                if (item != null)
                                {
                                    Items.Insert(0, item);
                                        if (Items.Count >= BaseIndexModel.RowsPerPage)
                                            Items.RemoveAt(Items.Count - 1);

                                    await InvokeAsync(() => StateHasChanged());
                                }
                            }
                        }
                    });

                    hubConnection.On<string>("OnShipmentUpdated", async ( message) =>
                    {
                        Logger.LogInformation($"message-> update {message}");
                        if (int.TryParse(message, out int shipmentId))
                        {
                            var target = Items.FirstOrDefault(x => x.Id == shipmentId);
                            if (target != null)
                            {
                                var data = await ApiService.GetShipmentFromCache(shipmentId);
                                if (data != null)
                                {
                                    var i = Items.IndexOf(target);
                                    Items[i] = data;
                                    await InvokeAsync(() => StateHasChanged());
                                }
                            }
                        }
                    });

                    hubConnection.On<string>("Notification", async (message) =>
                    {
                        Logger.LogWarning($"Notification -> {message}");
                        if (!message.StartsWith("New Shipment"))
                        {
                            await JSRuntime.InvokeVoidAsync("toast.show", message);
                        }
                    });

                    await hubConnection.StartAsync();

                    HubConnectionStatusColour = "#1bc58a";

                    Logger.LogInformation("connection is started");
                    hubConnection.Closed += async (error) =>
                    {
                        HubConnectionStatusColour = "red";
                        Logger.LogInformation("connection is closed");
                        PubSub.Hub.Default.Publish<LiveShipments>(this);
                        //await Task.Delay(new Random().Next(0, 5) * 1000);
                        //await hubConnection.StartAsync();
                    };
                    hubConnection.Reconnecting += error =>
                    {
                        HubConnectionStatusColour = "orange";
                        //  Debug.Assert(hubConnection.State == HubConnectionState.Reconnecting);

                        // Notify users the connection was lost and the client is reconnecting.
                        // Start queuing or dropping messages.
                        Logger.LogInformation("connection is reconnecting");
                        PubSub.Hub.Default.Publish<LiveShipments>(this);
                        return Task.CompletedTask;
                    };
                    hubConnection.Reconnected += connectionId =>
                    {
                        HubConnectionStatusColour = "#1bc58a";
                        // Debug.Assert(hubConnection.State == HubConnectionState.Connected);
                        Logger.LogInformation("connection is reconnected");
                        PubSub.Hub.Default.Publish<LiveShipments>(this);
                        // Notify users the connection was reestablished.
                        // Start dequeuing messages queued while reconnecting if any.
                        return Task.CompletedTask;
                    };
                    PubSub.Hub.Default.Publish<LiveShipments>(this);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex.ToString());
                }
                #endregion

            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                Error = ex.ToString() + ex.InnerException?.ToString() + ex.InnerException?.InnerException?.ToString();
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
                await JSRuntime.InvokeVoidAsync("loadItems.init", DotNetRef);
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
            await LoadItems(true);
        }

        protected override ShipmentFormViewModel CreateSelectedItem()
        {
            return new ShipmentFormViewModel();
        }
        public override async Task<int> OnFormSubmit()
        {
            IsModalBusy = true;
            bool getDenomination = true;
            try
            {
                if (!String.IsNullOrEmpty(SelectedItem.Valueables))
                {
                    getDenomination = false;
                }
                var val = await ApiService.PostAsync(SelectedItem);
                SelectedItem = default;

                OnFormSubmitted?.Invoke(val);

                if (getDenomination)
                {
                    CitDenominationViewModel = await ApiService.GetDenomination(val);
                    CitDenominationViewModel.NewShipment = true;
                    // await GetViewModel<CitDenominationViewModel>
                    //  (ApiControllerName, consignmentId, "GetDenomination");

                    FormTitle = CitDenominationViewModel.ShipmentCode;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
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
                Keys = new List<KeyHelper>(),
            };
            for (int i = 0; i < 10; i++)
            {
                BulkShipmentsViewModel.Keys.Add(new KeyHelper() { Id = i });
            }
        }
        public void AddFromParty(int key, int val)
        {
            if (BulkShipmentsViewModel.FromPartyDic.ContainsKey(key))
            {
                BulkShipmentsViewModel.FromPartyDic[key] = val;
            }
            else
            {
                BulkShipmentsViewModel.FromPartyDic.Add(key, val);
            }
        }
        public void AddToParty(int key, int val)
        {
            if (BulkShipmentsViewModel.ToPartyDic.ContainsKey(key))
            {
                BulkShipmentsViewModel.ToPartyDic[key] = val;
            }
            else
            {
                BulkShipmentsViewModel.ToPartyDic.Add(key, val);
            }

        }
        public void AddBillBranch(int key, int val)
        {
            if (BulkShipmentsViewModel.BillBranchDic.ContainsKey(key))
            {
                BulkShipmentsViewModel.BillBranchDic[key] = val;
            }
            else
            {
                BulkShipmentsViewModel.BillBranchDic.Add(key, val);
            }

        }
        public async Task SaveBulkShipments()
        {
            await BlockFormUI();
            try
            {
                var resp = await ApiService.PostBulkShipments(BulkShipmentsViewModel);
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
        private async Task EditViewModel(int consignmentId)
        {
            if (TabIndex == 0)
            {
                await OnItemClicked(consignmentId);
            }
            else if (TabIndex == 1)
            {
                CitDenominationViewModel = await ApiService.GetDenomination(consignmentId);
                if (CitDenominationViewModel != null)
                {
                    FormTitle = CitDenominationViewModel.ShipmentCode;
                }
            }
            else if (TabIndex == 4)
            {
                DeliveryChargesModel = await GetViewModel<DeliveryChargesViewModel>
                    ("DeliveryCharges", consignmentId);
            }
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
                var response = await ApiService.ApproveConsignmentStatus(ConsignmentApprrovalViewModel);
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
                var response = await ApiService.PostConsignmentStatus(ConsignmentStatusViewModel);
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
        private async Task EditDenominationModel(int id, bool finalizeShipment, bool enableSkip)
        {
            await BlockFormUI();
            try
            {
                ValidationError = default;
                CitDenominationViewModel = await ApiService.GetDenomination(id);
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
                DeliveryTimeViewModel = await ApiService.GetDeliveryTime(consignmentId);
                await UnBlockFormUI();
            }
        }
        private async Task SaveDenomination()
        {
            IsModalBusy = true;
            var newShipment = false;
            try
            {
                ConsignmentId = CitDenominationViewModel.ConsignmentId;

                newShipment = CitDenominationViewModel.NewShipment;

                var showSchdule = CitDenominationViewModel.EnableSkip;
                await BlockFormUI();
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
                        //if (CitDenominationViewModel.CurrencySymbol == CurrencySymbol.PKR)
                        CitDenominationViewModel.TotalAmount = TotalAmount.GetValueOrDefault();
                        CitDenominationViewModel.AmountPKR = AmountPKR;
                        var id = await PostViewModel(CitDenominationViewModel, "denomination");
                        if (id > 0)
                        {
                            CitDenominationViewModel = null;
                        }
                    }
                }
                else
                {
                    var id = await PostViewModel(CitDenominationViewModel, "denomination");
                    if (id > 0)
                    {
                        CitDenominationViewModel = null;
                    }
                }
            }
            catch { }
            IsModalBusy = false;

            if (newShipment)
                if (CitDenominationViewModel == null)// && showSchdule)
                {
                    await EditDeliveryTime(ConsignmentId, newShipment);
                }
        }

        private async Task SaveCharges()
        {
            await BlockFormUI();
            try
            {
                var id = await PostViewModel(DeliveryChargesModel, "DeliveryCharges");
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
                var id = await ApiService.AssignCrew(DeliveryCrewFormViewModel);
                if (id > 0)
                {
                    DeliveryCrewFormViewModel = null;
                   // await LoadItems();
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
        private async Task VaultItClicked(int consignmentId, int deliveryId)
        {

            DeliveryVaultViewModel = new DeliveryVaultViewModel()
            {
                ConsignmentId = consignmentId,
                DeliveryId = deliveryId,
                Crews = await ApiService.GetCrews(),
            };

        }
        private async Task AssignVault()
        {
            await BlockFormUI();
            try
            {
                var resp = await ApiService.AssignVault(DeliveryVaultViewModel);
                if (resp > 0)
                {
                    await LoadItems();
                    OnShipmentPlanClicked(Items.FirstOrDefault(x => x.Id == DeliveryVaultViewModel.ConsignmentId));
                    DeliveryVaultViewModel = default;
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
            var response = await ApiService.ApiService.GetFromJsonAsync<IndexViewModel<CrewMemberListModel>>
                    ($"v1/crewmembers/GetPage?rowsPerPage=15&currentIndex=1&crewId={crewId}&OnlyActive=true");

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
                ShowConsignmentsViewModel = await ApiService.GetConsignments(crewId.GetValueOrDefault());
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
                    Crews = await ApiService.GetCrews(),
                    Locations = await ApiService.GetLocations(LocationType.Address),
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
                Crews = await ApiService.GetCrewsWithLocationMatrix(consignmentId);
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

        private async Task SaveConsignmentDelivery()
        {
            await BlockFormUI();
            try
            {
                var id = await ApiService.PostConsignmentDelivery(DeliveryFormViewModel);
                if (id > 0)
                {
                    await LoadItems();
                    OnShipmentPlanClicked(Items.FirstOrDefault(x => x.Id == DeliveryFormViewModel.ConsignmentId));
                    DeliveryFormViewModel = null;
                    AddLocationClicked = false;
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
                var distanceUpdateResult = await ApiService.UpdateShipmentDistance(ShipmentAdministrationViewModel);

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

        public async Task SaveAs(int id, string shipmentCode)
        {
            await BlockFormUI();
            FileViewModel data = null;
            try
            {
                data = await HttpService.GetFromJsonAsync<FileViewModel>($"pages/ConsignmentReceipt/Export?writerFormat=PDF&consignmentId={id}");
                Logger.LogError($"Downloaded {data?.Data?.Length} bytes");
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
                         Convert.ToBase64String(data.Data));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failer while rendering file {ex}");
            }
            finally
            {
                await UnBlockFormUI();
            }
        }
        public async Task EditBranch(int fromPartyId, int toPartyId, int consignmentId)
        {
            await BlockFormUI();
            BranchFormViewModel = new BranchFormViewModel();
            Logger?.LogInformation($"fromPartyId = {fromPartyId} , toPartyId = {toPartyId} ,consignmentId = {consignmentId}");
            if (fromPartyId > 0)
                BranchFormViewModel = await ApiService.GetBranchData(fromPartyId);
            else
                BranchFormViewModel = await ApiService.GetBranchData(toPartyId);
            BranchFormViewModel.ConsignmentId = consignmentId;
            await UnBlockFormUI();
        }
        private async Task SaveDeliveryTime()
        {
            await BlockFormUI();
            try
            {
                var resp = await ApiService.PostDeliveryTime(DeliveryTimeViewModel);
                if (resp > 0)
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

        private SelectListItem ToSelectListItem(ConsignmentStateSummarized status)
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
                Response = await ApiService.ChangeBranchData(BranchFormViewModel);
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
                    var resp = await ApiService.PostRatings(RatingControlViewModel);
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
            RatingCategoriesViewModel = await ApiService.GetRatingCategories(consignmentId) ?? new RatingCategoriesViewModel();
            RatingCategoriesViewModel.RatingValue = RatingValue;
            RatingCategoriesViewModel.ConsignmentId = consignmentId;
            await UnBlockFormUI();
        }
        private async Task OnAddCommentsClicked(int consignmentId)
        {
            await BlockFormUI();
            try
            {
                ShipmentCommentsViewModel = await ApiService.GetComments(consignmentId);
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
                int resp = await ApiService.PostComment(ShipmentCommentsViewModel);
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
                var resp = await ApiService.PostRatingCategories(RatingCategoriesViewModel);
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
                MixedCurrencyViewModel = await ApiService.GetMixCurrency(consigmentId) ?? new MixedCurrencyViewModel()
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
                var resp = await ApiService.UpdateMixCurrency(MixedCurrencyViewModel);
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
                TransitTimeViewModel = await ApiService.GetTransitTime(consignmentId);
            }
            catch (Exception ex)
            {
                ValidationError = ex.Message;
                Logger.LogInformation(ex.Message + ex.InnerException);
            }
            await UnBlockFormUI();
        }

    }

    public class DomesticShipmentViewModel
    {
        public int ConsignmentId { get; set; }

        public IEnumerable<DeliveryListViewModel> Deliveries { get; set; }

        public string PickupBranchName { get; set; }

        public string PickupBranchAddress { get; set; }

        public string DropoffBranchName { get; set; }

        public string DropoffBranchAddress { get; set; }
    }

    public class RandomRetryPolicy : IRetryPolicy
    {
        private readonly Random _random = new Random();

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            // If we've been reconnecting for less than 60 seconds so far,
            // wait between 0 and 10 seconds before the next reconnect attempt.
            //if (retryContext.ElapsedTime < TimeSpan.FromSeconds(60))
            //{
            // return TimeSpan.FromSeconds(_random.NextDouble() * 10);
            return TimeSpan.FromSeconds(5);

            //  }
            //else
            //{
            //    // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
            //    return null;
            //}
        }
    }

}
