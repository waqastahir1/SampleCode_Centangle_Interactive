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
using SOS.OrderTracking.Web.Shared.CIT.Shipments;

namespace SOS.OrderTracking.Web.Client.CIT.Shipments
{

    public partial class RecurringShipments
    {
        #region Properties

        private HubConnection hubConnection;

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
                _consignmentStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _searchKey;

        [Parameter]
        public string SearchKey
        {
            get { return _searchKey; }
            set
            {
                if (_searchKey != value)
                {
                    _searchKey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IEnumerable<SelectListItem> BillBranches { get; set; }

        private IEnumerable<SelectListItem> CPCCustomers { get; set; }

        private ConsignmentStatusViewModel ConsignmentStatusViewModel { get; set; }

        public ConsignmentApprrovalViewModel ConsignmentApprrovalViewModel { get; set; }

        public bool ShowAllCrews { get; set; }

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

        public DomesticShipmentViewModel DomesticShipmentViewModel { get; set; }

        public DeliveryFormViewModel DeliveryFormViewModel { get; set; }

        public DenominationChangeAmountViewModel DenominationChangeAmountViewModel { get; set; }
        public ShipmentAdministrationViewModel ShipmentAdministrationViewModel { get; set; }
        public UserInfo userInfo { get; set; }
        private int TabIndex { get; set; }
        int minute = DateTime.Now.Minute;
        ClaimsPrincipal User = null;
        #endregion

        public DotNetObjectReference<RecurringShipments> DotNetRef;

        public RecurringShipments()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            ConsignmentStatus = "All";
            BaseIndexModel.RowsPerPage = 18;
#if DEBUG
            BaseIndexModel.RowsPerPage = 6;
#endif
            AdditionalParams = $"&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}";
            PropertyChanged += async (p, q) =>
            {
                Logger?.LogInformation(q.PropertyName);
                if (q.PropertyName == nameof(StartDate) || q.PropertyName == nameof(EndDate) || q.PropertyName == nameof(ConsignmentStatus) || q.PropertyName == nameof(SearchKey))
                {
                    AdditionalParams = $"&SearchKey={SearchKey}&ConsignmentStatus={ConsignmentStatus}&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}";
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
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency75x)
                        || q.PropertyName == nameof(CitDenominationViewModel.Currency50x)
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

            OnFormSubmitted += async (int id) =>
            {
                await EditSchedule(id);
            };
        }
        [Inject]
        public IAccessTokenProvider AccessTokenProvider { get; set; }
        private ScheduleViewModel _scheduleViewModel;

        public ScheduleViewModel ScheduleViewModel
        {
            get { return _scheduleViewModel; }
            set
            {
                _scheduleViewModel = value;
                NotifyPropertyChanged();
            }
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
                           Logger.LogInformation($"Requested token=> {accessToken?.Value}");

                           //options.Headers.Add("Authorization", $"Bearer {accessToken.Value}");

                           return accessToken.Value;
                       };
                     })
                     .Build();

                    if (authenticationStateTask != null)
                    {
                        User = (await authenticationStateTask).User;
                    }

                    hubConnection.On<string, string>("RefreshCITConsignments", async (user, message) =>
                    {
                        if (minute != DateTime.Now.Minute)
                        {
                            Logger.LogWarning($"messahe-> {user} , {message}");
                            if (user == User?.Identity?.Name && message != "UPDATE")
                            {
                                await JSRuntime.InvokeVoidAsync("toast.show", message);
                            }
                            await LoadItems(false);
                            await InvokeAsync(() => StateHasChanged());
                            minute = DateTime.Now.Minute;
                        }
                    });

                    hubConnection.On<string>("Notification", async (message) =>
                    {
                        Logger.LogWarning($"Notification -> {message}");
                        await JSRuntime.InvokeVoidAsync("toast.show", message);
                        await LoadItems(false);
                        await InvokeAsync(() => StateHasChanged());
                    });

                    await hubConnection.StartAsync();
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

        [JSInvokable("loadItems")]
        public async Task loadItems()
        {
            await LoadItems();
        }

        protected override ShipmentScheduleFormViewModel CreateSelectedItem()
        {
            return new ShipmentScheduleFormViewModel();
        }
        public override async Task<int> OnFormSubmit()
        {
            bool getDenomination = true;
            if (!String.IsNullOrEmpty(SelectedItem.Valueables))
            {
                getDenomination = false;
            }
            int consignmentId = await base.OnFormSubmit();

            if (getDenomination)
            {
                CitDenominationViewModel = await ApiService.GetDenomination(consignmentId);
                // await GetViewModel<CitDenominationViewModel>
                //  (ApiControllerName, consignmentId, "GetDenomination");

                FormTitle = CitDenominationViewModel.ShipmentCode;
            }
            return consignmentId;
        }

        private void CreateShipment()
        {
            Error = null;
            ValidationError = null;
            SelectedItem = new ShipmentScheduleFormViewModel()
            {
                Initiator = User.Identity.Name,
                TransactionMode = 3
            };
        }

        private void SendMoney()
        {
            Error = null;
            ValidationError = null;
            SelectedItem = new ShipmentScheduleFormViewModel()
            {
                FromPartyId = OrganizationalUnit.PartyId.GetValueOrDefault(),
                Initiator = User.Identity.Name,
                TransactionMode = 1
            };
        }



        private void ReceiveMoney()
        {
            Error = null;
            ValidationError = null;
            SelectedItem = new ShipmentScheduleFormViewModel()
            {
                ToPartyId = OrganizationalUnit.PartyId.GetValueOrDefault(),
                Initiator = User.Identity.Name,
                TransactionMode = 2
            };
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
        private async Task EditDenominationModel(int id)
        {
            await BlockFormUI();
            try
            {
                CitDenominationViewModel = await ApiService.GetDenomination(id);
                if (CitDenominationViewModel != null)
                {
                    FormTitle = CitDenominationViewModel.ShipmentCode;
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

        private async Task SaveDenomination()
        {
            await BlockFormUI();
            if (CitDenominationViewModel.TotalAmount != TotalAmount)
            {
                if (!CitDenominationViewModel.SaveNewAmount)
                {
                    DenominationChangeAmountViewModel = new DenominationChangeAmountViewModel();
                    DenominationChangeAmountViewModel.prevAmount = CitDenominationViewModel.AmountPKR;
                    DenominationChangeAmountViewModel.newAmount = AmountPKR;
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
                        await LoadItems(true);
                    }
                }
            }
            else
            {
                var id = await PostViewModel(CitDenominationViewModel, "denomination");
                if (id > 0)
                {
                    CitDenominationViewModel = null;
                    await LoadItems(true);
                }
            }
            IsModalBusy = false;
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

        private async Task ShowCrew(int? crewId)
        {
            var response = await ApiService.ApiService.GetFromJsonAsync<IndexViewModel<CrewMemberListModel>>
                    ($"v1/crewmembers/GetPage?rowsPerPage=15&currentIndex=1&crewId={crewId}");

            CrewMembersViewModel = response.Items;
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
        private void OnShipmentPlanClicked(ShipmentScheduleListViewModel item)
        {
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
                Error = ex.Message;
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
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.ToString());
                Error = ex.Message;
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
                Error = ex.Message;
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
        public void NewAmount(int prevAmount, int totalAmount)
        {
            AmountPKR = prevAmount;
            TotalAmount = totalAmount;
        }
        private async Task EditSchedule(int id)
        {
            ScheduleViewModel = await ApiService.GetSchedule(id);
        }

        private async Task SaveSchedule()
        {
            var response = await ApiService.SaveSchedule(ScheduleViewModel);
            if (response > 0)
            {

                ScheduleViewModel = null;
                ValidationError = null;
                await LoadItems(true);
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

        private SelectListItem ToSelectListItem(ConsignmentStatus status)
        {
            return new SelectListItem(status.ToString(), EnumHelper.GetDisplayValue(status));
        }

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
        public static string CalculateAmountInWords(int Amount)
        {
            bool isUK = true;
            if (Amount == 0) return "Zero";
            string and = isUK ? "and " : ""; // deals with UK or US Amounting
            //if (Amount == -2147483648) return "Minus Two Billion One Hundred " + and +
            //"Forty Seven Million Four Hundred " + and + "Eighty Three Thousand " +
            //"Six Hundred " + and + "Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (Amount < 0)
            {
                sb.Append("Minus ");
                Amount = -Amount;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Million ", "Billion " };
            num[0] = Amount % 1000;           // units
            num[1] = Amount / 1000;
            num[2] = Amount / 1000000;
            num[1] = num[1] - 1000 * num[2];  // thousands
            num[3] = Amount / 1000000000;     // billions
            num[2] = num[2] - 1000 * num[3];  // millions
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10;              // ones
                t = num[i] / 10;
                h = num[i] / 100;             // hundreds
                t = t - 10 * h;               // tens

                if (h > 0)
                    sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (i < first)  //if (h > 0 || i < first) 
                        sb.Append(and);
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0)
                    sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

    }



}
