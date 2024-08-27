using BoldReports.Writer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Portal.Pages.CIT.Shipments;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System.Security.Claims;

namespace SOS.OrderTracking.Web.Portal.Pages.CIT.Reports
{
    public partial class ShipmentsVaultReport
    {
        public int BranchId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ThruDate { get; set; }


        //[Inject]
        //public AppDbContext  context { get; set; }

        [Inject]
        private IWebHostEnvironment _hostingEnvironment { get; set; }

        private List<SelectListItem> _consignmentStateSummarizedTypes;
        private List<SelectListItem> ConsignmentStateSummarizedTypes
        {
            get
            {
                return _consignmentStateSummarizedTypes ??= new List<SelectListItem>()
                {
                    ToSelectListItem( ConsignmentDeliveryState.All),
                    //ToSelectListItem( ConsignmentDeliveryState.Created),
                    //ToSelectListItem( ConsignmentDeliveryState.CrewAssigned),
                    //ToSelectListItem( ConsignmentDeliveryState.ReachedPickup),
                    ToSelectListItem( ConsignmentDeliveryState.InTransit),
                    ToSelectListItem( ConsignmentDeliveryState.ReachedDestination),
                    ToSelectListItem( ConsignmentDeliveryState.Delivered),
                    ToSelectListItem( ConsignmentDeliveryState.InVault),
                };
            }
        }

        private ConsignmentDeliveryState _consignmentDeliveryState;

        public ConsignmentDeliveryState ConsignmentDeliveryState
        {
            get { return _consignmentDeliveryState; }
            set { _consignmentDeliveryState = value; }
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
                    ToSelectListItem( ConsignmentStatus.Pushing),
                    ToSelectListItem( ConsignmentStatus.Pushed),
                    ToSelectListItem( ConsignmentStatus.PushingFailed),
                    ToSelectListItem( ConsignmentStatus.Declined),
                    ToSelectListItem( ConsignmentStatus.Cancelled)
            };
            }
        }

        private ConsignmentStatus ConsignmentStatus
        {
            get { return _consignmentStatus; }
            set
            {
                _consignmentStatus = value;
                NotifyPropertyChanged();
            }
        }

        private ConsignmentStatus _consignmentStatus;

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

        public IEnumerable<SelectListItem> MainCustomers { get; set; } = new List<SelectListItem>();
        private IEnumerable<SelectListItem> VaultStatus { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem(0,"All"),
            new SelectListItem(1,"Vault-In"),
            new SelectListItem(2,"Vault-Out"),
        };
        private int VaultStatusId { get; set; }

        public ShipmentsVaultReport()
        {
            FromDate = DateTime.Today;
            ThruDate = DateTime.Today;
            ConsignmentDeliveryState = ConsignmentDeliveryState.All;
            //PropertyChanged += async (p, q) =>
            //{
            //    Logger?.LogInformation(q.PropertyName);
            //    if (q.PropertyName == nameof(MainCustomerId))
            //    {
            //        await LoadItems(true);
            //        Logger.LogInformation($"{q.PropertyName} changed");
            //    }
            //};
        }


        private async Task ShowData()
        {
            await LoadItems(true);
        }
        private SelectListItem ToSelectListItem(ConsignmentStatus status)
        {
            return new SelectListItem(status.ToString(), Enum.GetName(typeof(ConsignmentStatus), status));
        }
        private SelectListItem ToSelectListItem(ConsignmentDeliveryState status)
        {
            return new SelectListItem(status.ToString(), Enum.GetName(typeof(ConsignmentDeliveryState), status));
        }

        private async Task DownloadPdf(string fileType)
        {
            IsTableBusy = true;
            try
            {
                var vm = await Export(fileType);
                await JSRuntime.InvokeVoidAsync(
            "saveAsFile", "Shipment Vault Report." + fileType,
            Convert.ToBase64String(vm));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Error = ex.Message;
            }
            IsTableBusy = false;
        }


        public async Task<byte[]> Export(string fileType)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string basePath = _hostingEnvironment.WebRootPath;
            // Here, we have loaded the Product List.rdl sample report from application the Resources folder.
            FileStream reportStream;
            //if (User.HasSOSRole())
            //    reportStream = new FileStream(basePath + @"\Reports\ShipmentsVaultReport.rdl", FileMode.Open, FileAccess.Read);
            //else
            reportStream = new FileStream(basePath + @"\Reports\ShipmentsVaultReport.rdl", FileMode.Open, FileAccess.Read);
            ReportWriter writer = new ReportWriter(reportStream);
            writer.ReportProcessingMode = ProcessingMode.Local;
            #region Actual Data
            var query = GetConsignments(context);

            var datasource = await query.ToListAsync();
            datasource.ForEach(p =>
            {
                if (p.CollectionDelivery != null)
                {
                    if (p.CollectionDelivery?.ActualPickupTime.GetValueOrDefault().Year > 2000)
                        p.PickupTime = p.CollectionDelivery?.ActualPickupTime.GetValueOrDefault().ToString();
                    p.CollectionVehicle = p.CollectionDelivery?.Vehicle;
                    p.VaultInCrew = p.CollectionDelivery?.Crew;
                }

                if (p.DroppoffDelivery != null && p.ConsignmentState == ConsignmentDeliveryState.Delivered)
                {
                    if (p.DroppoffDelivery?.ActualDropTime.GetValueOrDefault().Year > 2000)
                        p.DeliveryTime = p.DroppoffDelivery?.ActualDropTime.GetValueOrDefault().ToString();
                    p.DropoffVehicle = p.DroppoffDelivery?.Vehicle;
                }

                p.Station = p.CollectionDelivery.Station;

            });

            #endregion
            writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "CustomerDataSet", Value = datasource });

            string fileName = null;
            WriterFormat format;
            string type = null;

            if (fileType == "pdf")
            {
                fileName = $"CR-{"Shipment Vault Report"}.pdf";
                type = "pdf";
                format = WriterFormat.PDF;
            }
            else if (fileType == "csv")
            {
                fileName = $"CR-{"Shipment Vault Report"}.csv";
                type = "csv";
                format = WriterFormat.CSV;
            }
            else
            {
                fileName = "Shipment Vault Report.xls";
                type = "xls";
                format = WriterFormat.Excel;
            }

            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        protected override IQueryable<ShipmentVaultReportListViewModel> GetQuery(AppDbContext context)
        {
            if (FromDate == null || ThruDate == null)
            {
                FromDate = DateTime.Now;
                ThruDate = DateTime.Now;
            }
            return GetConsignments(context);
        }

        private IQueryable<ShipmentVaultReportListViewModel> GetConsignments(AppDbContext context)
        {
            DateTime fromDate = FromDate.GetValueOrDefault();
            DateTime thruDate = ThruDate.GetValueOrDefault();

            if (User.IsAlive())
            {
                var user = context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                List<int> cIds = null;
                if (User.HasSOSRole() && MainCustomerId > 0)
                {
                    cIds = context.PartyRelationships.Where(x => x.ToPartyId == MainCustomerId && x.FromPartyRole == RoleType.ChildOrganization)
                   .Select(x => x.FromPartyId).ToList();
                }
                else if (User.IsInRole(Constants.Roles.BANK))
                {
                    cIds = context.AllocatedBranches.Where(x => x.UserId == user.Id && x.IsEnabled).Select(x => x.PartyId).ToList();
                }
                else if (User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_BRANCH_MANAGER) || User.IsInRole(Constants.Roles.BANK_CPC) || User.IsInRole(Constants.Roles.BANK_CPC_MANAGER) || User.IsInRole(Constants.Roles.BANK_HYBRID))
                {
                    cIds = new List<int>() { user.PartyId };
                }
                if (BranchId > 0)
                {
                    cIds = new List<int>() { BranchId };
                    //cIds.Add(billBranchId);
                }

                //var tp = context.ConsignmentDeliveries.Include(x => x.Consignment).ThenInclude(x => x.ShipmentSealCodes).Include(x => x.Childern).Where(x => x.Id == 1000428715).FirstOrDefault();
                var dueTime = MyDateTime.Now.AddHours(1);
                var endTime = thruDate.Date.AddDays(1).AddSeconds(-1);
                var query = (from c in context.ConsignmentDeliveries.Include(x => x.Crew).ThenInclude(x => x.Orgnization).Include(x => x.Consignment).ThenInclude(x => x.ShipmentSealCodes).Include(x => x.Childern)
                             from vaultout in context.ConsignmentDeliveries.Where(x => x.ParentId == c.Id).DefaultIfEmpty()

                             join cb in context.Parties on c.Consignment.FromPartyId equals cb.Id
                             join cd in context.Parties on c.Consignment.ToPartyId equals cd.Id

                             join f in context.Parties on c.FromPartyId equals f.Id
                             join fa in context.AssetAllocations on f.Id equals fa.PartyId
                             join fv in context.Assets on fa.AssetId equals fv.Id

                             from t in context.Parties.Where(x => c.ToPartyId > 0 && x.Id == c.ToPartyId).DefaultIfEmpty()
                             from ta in context.AssetAllocations.Where(x => x.PartyId == t.Id).Take(1).DefaultIfEmpty()
                             from tv in context.Assets.Where(x => x.Id == ta.AssetId).Take(1).DefaultIfEmpty()
                             from trcc in context.PartyRelationships.Include(x => x.FromParty).Where(x => t == null ? false : (x.ToPartyId == t.Id && x.FromPartyRole == RoleType.CheifCrewAgent && x.StartDate.Date <= c.PlanedPickupTime.Value.Date && (x.ThruDate == null || x.ThruDate.Value.Date >= c.PlanedPickupTime.Value.Date))).Take(1).DefaultIfEmpty()
                             from trac in context.PartyRelationships.Include(x => x.FromParty).Where(x => t == null ? false : (x.ToPartyId == t.Id && x.FromPartyRole == RoleType.AssistantCrewAgent && x.StartDate.Date <= c.PlanedPickupTime.Value.Date && (x.ThruDate == null || x.ThruDate.Value.Date >= c.PlanedPickupTime.Value.Date))).Take(1).DefaultIfEmpty()

                             from url in context.ShipmentAttachments.Where(x => x.Id == c.ConsignmentId).Select(x => x.Url).DefaultIfEmpty()

                             where (VaultStatusId == 1 ? c.Consignment.ConsignmentStateType == ConsignmentDeliveryState.InVault : VaultStatusId == 2 ? c.Consignment.ConsignmentStateType != ConsignmentDeliveryState.InVault : true)
                             && (cIds == null || cIds.Contains(c.Consignment.FromPartyId) || cIds.Contains(c.Consignment.ToPartyId) || cIds.Contains(c.Consignment.BillBranchId) || c.Consignment.CreatedBy == User.Identity.Name)
                             && (c.Consignment.ApprovalState == ConsignmentApprovalState.Approved || c.Consignment.ApprovalState == ConsignmentApprovalState.ReApprove)
                             && (c.Consignment.Type == ShipmentExecutionType.Live || c.Consignment.Type == ShipmentExecutionType.Scheduled && c.Consignment.DueTime > dueTime)
                             && c.PlanedPickupTime >= fromDate.Date && c.PlanedPickupTime <= endTime
                             && c.IsVault
                             orderby c.ConsignmentId, c.PlanedPickupTime
                             select new ShipmentVaultReportListViewModel()
                             {
                                 ShipmentCode = c.Consignment.ShipmentCode,
                                 AmountStr = $"{c.Consignment.CurrencySymbol} {c.Consignment.Amount}",
                                 Amount = c.Consignment.Amount,
                                 PickupBranch = cb.ShortName,
                                 DeliveryBranch = c.Consignment.IsToChangedPartyVerified ? context.Parties.Where(x => x.Id == c.Consignment.ToChangedPartyId).Select(x => x.ShortName).FirstOrDefault() + ", " + cd.ShortName : cd.ShortName,
                                 CurrentDate = DateTime.Now.ToString("dd/MM/yyyy"),
                                 //Station = s.FormalName,
                                 CollectionDelivery = (from x in context.ConsignmentDeliveries

                                                       join a in context.AssetAllocations on x.CrewId equals a.PartyId
                                                       join v in context.Assets on a.AssetId equals v.Id
                                                       join cr in context.Parties on x.CrewId equals cr.Id

                                                       from frcc in context.PartyRelationships.Include(x => x.FromParty).Where(x => x.ToPartyId == cr.Id && x.FromPartyRole == RoleType.CheifCrewAgent && x.StartDate.Date <= c.PlanedPickupTime.Value.Date && (x.ThruDate.HasValue ? x.ThruDate.Value.Date >= c.PlanedPickupTime.Value.Date : true)).Take(1).DefaultIfEmpty()
                                                       from frac in context.PartyRelationships.Include(x => x.FromParty).Where(x => x.ToPartyId == cr.Id && x.FromPartyRole == RoleType.AssistantCrewAgent && x.StartDate.Date <= c.PlanedPickupTime.Value.Date && (x.ThruDate.HasValue ? x.ThruDate.Value.Date >= c.PlanedPickupTime.Value.Date : true)).Take(1).DefaultIfEmpty()

                                                       join s in context.Parties on cr.StationId equals s.Id
                                                       orderby x.Id ascending
                                                       where x.ConsignmentId == c.ConsignmentId
                                                       select new ConsignmentDeliveryModel
                                                       {
                                                           Id = x.Id,
                                                           CrewId = x.CrewId,
                                                           RegionId = cr.RegionId ?? 0,
                                                           SubRegionId = cr.SubregionId ?? 0,
                                                           StationId = cr.StationId ?? 0,
                                                           DropoffCode = x.DropoffCode,
                                                           IsVault = x.IsVault,
                                                           PickupCode = x.PickupCode,
                                                           ConsignmentId = x.ConsignmentId,
                                                           ActualDropTime = x.ActualDropTime,
                                                           ActualPickupTime = x.ActualPickupTime,
                                                           CollectionMode = x.CollectionMode,
                                                           DeliveryMode = x.DeliveryMode,
                                                           Station = s.FormalName,
                                                           Vehicle = v.Description,
                                                           Crew = $"{frcc.FromParty.ShortName}-CC-{frcc.FromParty.FormalName}, {frac.FromParty.ShortName}-ACC-{frac.FromParty.FormalName}",
                                                       }).FirstOrDefault(),
                                 DroppoffDelivery = c.Consignment.ConsignmentStateType == ConsignmentDeliveryState.Delivered ? (from x in context.ConsignmentDeliveries
                                                                                                                                join a in context.AssetAllocations on x.CrewId equals a.PartyId
                                                                                                                                join v in context.Assets on a.AssetId equals v.Id
                                                                                                                                join cr in context.Parties on x.CrewId equals cr.Id
                                                                                                                                join s in context.Parties on cr.StationId equals s.Id
                                                                                                                                orderby x.Id descending
                                                                                                                                where x.ConsignmentId == c.ConsignmentId
                                                                                                                                select new ConsignmentDeliveryModel
                                                                                                                                {
                                                                                                                                    Id = x.Id,
                                                                                                                                    CrewId = x.CrewId,
                                                                                                                                    RegionId = cr.RegionId ?? 0,
                                                                                                                                    SubRegionId = cr.SubregionId ?? 0,
                                                                                                                                    StationId = cr.StationId ?? 0,
                                                                                                                                    DropoffCode = x.DropoffCode,
                                                                                                                                    IsVault = x.IsVault,
                                                                                                                                    PickupCode = x.PickupCode,
                                                                                                                                    ConsignmentId = x.ConsignmentId,
                                                                                                                                    ActualDropTime = x.ActualDropTime,
                                                                                                                                    ActualPickupTime = x.ActualPickupTime,
                                                                                                                                    CollectionMode = x.CollectionMode,
                                                                                                                                    DeliveryMode = x.DeliveryMode,
                                                                                                                                    Station = s.FormalName,
                                                                                                                                    Vehicle = v.Description
                                                                                                                                }).FirstOrDefault() : null,
                                 SealCount = c.Consignment.ShipmentSealCodes.Count(),
                                 BagsCount = c.Consignment.NoOfBags,
                                 ShipmentScan = url,
                                 VaultType = c.Crew.Orgnization.OrganizationType == OrganizationType.Vault ? "Fixed Vault" : "Vault On Wheels",
                                 Client = $"{c.Consignment.MainCustomer.ShortName}-{c.Consignment.MainCustomer.FormalName}",
                                 VaultOutCrew = t == null ? "" : $"{trcc.FromParty.ShortName}-CC-{trcc.FromParty.FormalName}, {trac.FromParty.ShortName}-ACC-{trac.FromParty.FormalName}",
                                 VehicleIn = fv.Description,
                                 VehicleOut = tv.Description,
                                 VaultInTime = c.PlanedPickupTime.HasValue ? c.PlanedPickupTime.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                                 VaultOutTime = vaultout != null ? vaultout.PlanedPickupTime.HasValue ? vaultout.PlanedPickupTime.Value.ToString("dd/MM/yyyy hh:mm tt") : "" : "",
                                 ConsignmentStatus = c.Consignment.ConsignmentStatus,
                                 ConsignmentState = c.Consignment.ConsignmentStateType,
                             });

                if (ConsignmentStatus > ConsignmentStatus.All)
                {
                    query = query.Where(x => x.ConsignmentStatus == ConsignmentStatus);
                }
                if (ConsignmentDeliveryState != ConsignmentDeliveryState.All)
                {
                    query = query.Where(x => x.ConsignmentState == ConsignmentDeliveryState);
                }


                if (User.HasSOSRole())
                {
                    // for control rooms, show live shiipments + scheduled due in next hour
                    var almostNow = DateTime.Now.AddHours(1);
                    //query = query.Where(x => x.PlannedCollectionTime == null || x.PlannedCollectionTime <= almostNow);
                }
                if (StationId > 0) query = query.Where(x => x.CollectionDelivery.StationId == StationId);
                else if (SubRegionId > 0) query = query.Where(x => x.CollectionDelivery.SubRegionId == SubRegionId);
                else if (RegionId > 0) query = query.Where(x => x.CollectionDelivery.RegionId == RegionId);
                return query;
            }
            else
            {
                return Array.Empty<ShipmentVaultReportListViewModel>().AsQueryable();
            }
        }


        protected override Task<ShipmentVaultReportFormViewModel> InitSelectedItem(int id, AppDbContext context)
        {
            throw new NotImplementedException();
        }

        protected override async Task LoadSelectLists(AppDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                var partiesService = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<PartiesService>();

                if (User.HasSOSRole())
                    MainCustomers = await partiesService.GetOrganizationsByTypeAsync(OrganizationType.MainCustomer);
            }
            catch (Exception ex)
            {
                // this exception will appear in browser console.
                Logger?.LogError(ex.ToString());
                Error = ex.ToString() + ex.InnerException?.ToString() + ex.InnerException?.InnerException?.ToString();
            }
        }

        protected override Task<int> PersistSelectedItem(AppDbContext context, IServiceScope scope)
        {
            throw new NotImplementedException();
        }
    }
}
