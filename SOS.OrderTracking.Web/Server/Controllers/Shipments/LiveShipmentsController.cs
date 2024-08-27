using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading.Tasks;
using BoldReports.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings;
using static SOS.OrderTracking.Web.Shared.Constants.Roles;
using ConsignmentState = SOS.OrderTracking.Web.Common.Data.Models.ConsignmentState;
using Location = SOS.OrderTracking.Web.Common.Data.Models.Location;
using Point = SOS.OrderTracking.Web.Shared.ViewModels.Point;

namespace SOS.OrderTracking.Web.Server.Controllers.Operations
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class LiveShipmentsController : ControllerBase, ICitCardsService
    {
        private readonly ConsignmentService workOrderService;

        private readonly IHubContext<ConsignmentHub> consignmentHub;
        private readonly NotificationService notificationService;
        private readonly SequenceService sequenceService;
        private readonly PartiesCacheService partiesCache;
        private readonly ShipmentsCacheService shipmentsCacheService;
        private readonly UserCacheService userCache;
        private readonly IWebHostEnvironment env;
        private readonly AppDbContext context;
        private readonly ILogger<LiveShipmentsController> logger;
        public LiveShipmentsController(AppDbContext context,
           ConsignmentService workOrderService,
           IHubContext<ConsignmentHub> consignmentHub,
          NotificationService notificationService,
          ILogger<LiveShipmentsController> logger,
          SequenceService sequenceService,
          PartiesCacheService partiesCache,
          ShipmentsCacheService shipmentsCacheService,
          UserCacheService userCache
            )
        {
            this.workOrderService = workOrderService;
            this.consignmentHub = consignmentHub;
            this.notificationService = notificationService;
            this.logger = logger;
            this.sequenceService = sequenceService;
            this.partiesCache = partiesCache;
            this.shipmentsCacheService = shipmentsCacheService;
            this.userCache = userCache;
            this.context = context;

        }

        #region CRUD Methods
        [HttpGet]
        public async Task<IndexViewModel<ConsignmentListViewModel>> GetPageAsync([FromQuery] CitCardsAdditionalValueViewModel vm)
        {
            try
            { 
                List<int> customers = null;
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                if (User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN))
                {

                }
                else if (User.IsInititorOrSupervisor())
                {
                    vm.ConsignmentStatus = ConsignmentStatus.All;
                    vm.ConsignmentStateSummarized = ConsignmentDeliveryState.All; 
                    customers = new List<int>() { user.PartyId };
                }
                else if (User.IsInRole(BANK))
                {
                    vm.ConsignmentStatus = ConsignmentStatus.All;
                    vm.ConsignmentStateSummarized = ConsignmentDeliveryState.All; 
                    customers = await context.PartyRelationships.Where(x => x.ToPartyId == user.PartyId && x.FromPartyRole == RoleType.ChildOrganization)
                        .Select(x => x.FromPartyId).ToListAsync();
                }

                var approvalState = User.IsInititorOrSupervisor() ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;

                var query = workOrderService.GetConsignmentsIds(vm.RegionId.GetValueOrDefault(),
                    vm.SubRegionId.GetValueOrDefault(), vm.StationId.GetValueOrDefault(), customers, 
                    User.Identity.Name, vm.ConsignmentStateSummarized, vm.ShipmentType, ConsignmentApprovalState.Approved, ConsignmentApprovalState.ReApprove, approvalState);

                if (vm.Rating > 0)
                {
                    query = query.Where(x => x.Rating == vm.Rating);
                }
                if (User.IsSOSAdmin())
                {
                    // for control rooms, show live shiipments + scheduled due in next hour
                    var almostNow = DateTime.Now.AddHours(1);
                    if (vm.ConsignmentType == ShipmentExecutionType.Live)
                    {
                        query = query.Where(x => x.PlanedCollectionTime == null || x.PlanedCollectionTime <= almostNow);
                    }
                    else
                    {
                        query = query.Where(x => x.PlanedCollectionTime != null);
                    }
                }
                if (vm.Id > 0)
                {
                    query = query.Where(x => x.Id == vm.Id);
                }
                else if (!string.IsNullOrEmpty(vm.SearchKey))
                    query = query.Where(x => x.ShipmentCode.ToLower().Contains(vm.SearchKey.ToLower()) || x.ManualShipmentCode.ToLower().Contains(vm.SearchKey.ToLower()));

                else
                {
                    query = query.Where(x => (vm.StartDate.Date <= x.DueTime.Date && vm.EndDate.Date >= x.DueTime.Date));

                    if (vm.ConsignmentStatus > ConsignmentStatus.All)
                    {
                        query = query.Where(x => x.ConsignmentStatus == vm.ConsignmentStatus);
                    }
                    if (vm.ConsignmentStateSummarized >= ConsignmentDeliveryState.Created)
                    {
                        query = query.Where(x => x.ConsignmentStateType == (Shared.Enums.ConsignmentDeliveryState)(byte)vm.ConsignmentStateSummarized);
                    }
                }

                switch (vm.Sorting)
                {
                    case SortBy.CreationDateAsc:
                        query = query.OrderBy(x => x.CreatedAt);
                        break;
                    case SortBy.CreationDateDesc:
                        query = query.OrderByDescending(x => x.CreatedAt);
                        break;
                    case SortBy.DueDateAsc:
                        query = query.OrderBy(x => x.DueTime);
                        break;
                    case SortBy.DueDateDesc:
                        query = query.OrderByDescending(x => x.DueTime);
                        break;
                    case SortBy.DeliveryDateAsc:
                        query = query.OrderBy(x => x.ActualDeliveryTime);
                        break;
                    case SortBy.DeliveryDateDesc:
                        query = query.OrderByDescending(x => x.ActualDeliveryTime);
                        break;
                    default:
                        break;
                }
                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage)
                    .Take(vm.RowsPerPage).ToListAsync();


                var shipments = new List<ConsignmentListViewModel>(items.Count);
                foreach (var item in items)
                {
                    try
                    {
                        var shipment = await shipmentsCacheService.GetShipment(item.Id);
                        shipments.Add(shipment);
                    }
                    catch
                    {
                        var shipment = await workOrderService.GetShipment(item.Id);
                        await shipmentsCacheService.SetShipment(shipment.Id, shipment);
                        shipments.Add(shipment);
                    }
                }
 

                shipments = shipments.Where(x => x.ApprovalState == ConsignmentApprovalState.Approved || ((x.ApprovalState ==  ConsignmentApprovalState.Draft || x.ApprovalState ==  ConsignmentApprovalState.ReApprove)
                && x.OriginPartyId == user.PartyId)).ToList();

                logger.LogInformation($"totalRows-> {totalRows}, currentRows-> {items.Count}");
                return new IndexViewModel<ConsignmentListViewModel>(shipments, totalRows);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IndexViewModel<ConsignmentListViewModel>> GetPageAsyncV1([FromQuery] CitCardsAdditionalValueViewModel vm)
        {
            try
            {
                List<int> customers = null;
                if (User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN))
                {

                }
                else if (User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) || User.IsInRole(BANK_BRANCH_MANAGER) || User.IsInRole(BANK_CPC_MANAGER) || User.IsInRole(BANK_HYBRID))
                {
                    vm.ConsignmentStatus = ConsignmentStatus.All;
                    vm.ConsignmentStateSummarized = ConsignmentDeliveryState.All;
                    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    customers = new List<int>() { user.PartyId };
                }
                else if (User.IsInRole(BANK))
                {
                    vm.ConsignmentStatus = ConsignmentStatus.All;
                    vm.ConsignmentStateSummarized = ConsignmentDeliveryState.All;
                    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    customers = await context.PartyRelationships.Where(x => x.ToPartyId == user.PartyId && x.FromPartyRole == RoleType.ChildOrganization)
                        .Select(x => x.FromPartyId).ToListAsync();
                }

                var consignmentType = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;

                var query = workOrderService.GetConsignmentsQuery(vm.RegionId.GetValueOrDefault(),
                    vm.SubRegionId.GetValueOrDefault(), vm.StationId.GetValueOrDefault(), customers, User.Identity.Name,
                    ShipmentExecutionType.Live, vm.ShipmentType, ConsignmentApprovalState.Approved, ConsignmentApprovalState.ReApprove, consignmentType);

                if (vm.Rating > 0)
                {
                    query = query.Where(x => x.Rating == vm.Rating);
                }
                if (User.IsInRole(SUPER_ADMIN) || User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN))
                {
                    // for control rooms, show live shiipments + scheduled due in next hour
                    var almostNow = DateTime.Now.AddHours(1);
                    if (vm.ConsignmentType == ShipmentExecutionType.Live)
                    {
                        query = query.Where(x => x.PlanedCollectionTime == null || x.PlanedCollectionTime <= almostNow);
                    }
                    else
                    {
                        query = query.Where(x => x.PlanedCollectionTime != null);
                    }
                }
                if (vm.Id > 0)
                {
                    query = query.Where(x => x.Id == vm.Id);
                }
                else if (!string.IsNullOrEmpty(vm.SearchKey))
                    query = query.Where(x => x.ShipmentCode.ToLower().Contains(vm.SearchKey.ToLower()) || x.ManualShipmentCode.ToLower().Contains(vm.SearchKey.ToLower()));

                else
                {
                    query = query.Where(x => (vm.StartDate.Date <= x.DueTime.Date && vm.EndDate.Date >= x.DueTime.Date));

                    if (vm.ConsignmentStatus > ConsignmentStatus.All)
                    {
                        query = query.Where(x => x.ConsignmentStatus == vm.ConsignmentStatus);
                    }
                    if (vm.ConsignmentStateSummarized >= ConsignmentDeliveryState.Created)
                    {
                        query = query.Where(x => x.ConsignmentStateType == (Shared.Enums.ConsignmentDeliveryState)(byte)vm.ConsignmentStateSummarized);
                    }
                }



                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage)
                    .Take(vm.RowsPerPage).ToListAsync();

                switch (vm.Sorting)
                {
                    case SortBy.CreationDateAsc:
                        items = items.OrderBy(x => x.CreatedAt).ToList();
                        break;
                    case SortBy.CreationDateDesc:
                        items = items.OrderByDescending(x => x.CreatedAt).ToList();
                        break;
                    case SortBy.DueDateAsc:
                        items = items.OrderBy(x => x.DueTime).ToList();
                        break;
                    case SortBy.DueDateDesc:
                        items = items.OrderByDescending(x => x.DueTime).ToList();
                        break;
                    case SortBy.DeliveryDateAsc:
                        items = items.OrderBy(x => x.ActualDeliveryTime).ToList();
                        break;
                    case SortBy.DeliveryDateDesc:
                        items = items.OrderByDescending(x => x.ActualDeliveryTime).ToList();
                        break;
                    default:
                        break;
                }
                items = items.Where(x => x.Deliveries.Count() > 0).ToList();
                foreach (var x in items)
                {
                    x.FromPartyCode = await partiesCache.GetCode(x.FromPartyId);
                    x.FromPartyName = await partiesCache.GetName(x.FromPartyId);

                    x.ToPartyCode = await partiesCache.GetCode(x.ToPartyId);
                    x.ToPartyName = await partiesCache.GetName(x.ToPartyId);

                    x.FromPartyStationName = await partiesCache.GetName(x.CollectionStationId);
                    x.ToPartyStationName = await partiesCache.GetName(x.DeliveryStationId);
                    x.BillingRegion = await partiesCache.GetName(x.BillBranchId);

                    x.FromPartyGeolocation = await partiesCache.GetGeoCoordinate(x.FromPartyId);
                    x.ToPartyGeolocation = await partiesCache.GetGeoCoordinate(x.ToPartyId);

                    x.FromPartyGeoStatus = await partiesCache.GetGeoStatus(x.FromPartyId);
                    x.ToPartyGeoStatus = await partiesCache.GetGeoStatus(x.ToPartyId);
                    x.Denomination ??= new CitDenominationViewModel();

                    foreach (var d in x.Deliveries)
                    {
                        if (d.CollectionPoint_ != null)
                        {
                            d.CollectionPoint = new Point(((NetTopologySuite.Geometries.Point)d.CollectionPoint_).Y,
                              ((NetTopologySuite.Geometries.Point)d.CollectionPoint_).X);
                        }


                        if (d.DeliveryPoint_ != null)
                        {
                            d.DeliveryPoint = new Point(((NetTopologySuite.Geometries.Point)d.DeliveryPoint_).Y,
                                ((NetTopologySuite.Geometries.Point)d.DeliveryPoint_).X);
                        }

                        d.CollectionPoint_ = null;
                        d.DeliveryPoint_ = null;
                    }
                }

                logger.LogInformation($"totalRows-> {totalRows}, currentRows-> {items.Count}");
                return new IndexViewModel<ConsignmentListViewModel>(items.ToList(), totalRows);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
         
        [HttpGet]
        public async Task<ShipmentFormViewModel> GetAsync(int id)
        {
            var order = await (from c in context.Consignments
                               join f in context.Parties on c.FromPartyId equals f.Id
                               join t in context.Parties on c.ToPartyId equals t.Id
                               join b in context.Parties on c.BillBranchId equals b.Id
                               where c.Id == id
                               select new ShipmentFormViewModel()
                               {
                                   Id = c.Id,
                                   ConsignmentNo = c.ShipmentCode,
                                   ManualShipmentCode = c.ManualShipmentCode,
                                   BillBranchId = c.BillBranchId,
                                   FromPartyId = c.FromPartyId,
                                   ToPartyId = c.ToPartyId,
                                   ExchangeRate = c.ExchangeRate,
                                   Valueables = c.Valueables,
                                   AmountPKR = c.AmountPKR,
                                   BillBrannchName = b.ShortName + "-" + b.FormalName,
                                   FromPartyName = f.ShortName + "-" + f.FormalName,
                                   ToPartyName = t.ShortName + "-" + t.FormalName,
                                   Type = c.Type,
                                   CurrencySymbol = c.CurrencySymbol,
                                   Amount = c.Amount,
                                   ServiceType = c.ServiceType,
                                   Comments = c.Comments,
                                   ConsignmentStatus = c.ConsignmentStatus,
                                    OriginPartyId = c.OriginPartyId
                               }).FirstAsync();
            order.Comments = order.Comments != null ? JsonConvert.DeserializeObject<List<ShipmentComment>>(order.Comments).FirstOrDefault().Description : null;
            return order;
        }
        [HttpPost]
        public async Task<int> PostAsync([FromBody] ShipmentFormViewModel selectedItem)
        {
            try
            {
                //todo: replace with actual location
                var dummyLocation = context.Locations.First();
                int newConsignmentSequence = 0;
                Consignment consignment = null;
                if (selectedItem.Id > 0)
                {
                    consignment = context.Consignments.Find(selectedItem.Id);
                }

                if (consignment == null)
                { 
                    newConsignmentSequence = context.Sequences.GetNextCitOrdersSequence();
                    var now = MyDateTime.Now;
                    consignment = new Consignment()
                    {
                        Id = newConsignmentSequence,
                        ShipmentCode = $"CIT/{now.Year}/" + newConsignmentSequence.ToString("D4"),
                        ManualShipmentCode = selectedItem.ManualShipmentCode,
                        ApprovalState = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved,
                        CreatedAt = now,
                        ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                        CreatedBy = User.Identity.Name,
                        ServiceType = ServiceType.Unknown,
                        ShipmentType = ShipmentType.Unknown,
                        ConsignmentStatus = ConsignmentStatus.TobePosted,
                        Type = ShipmentExecutionType.Live,
                        IsFinalized = User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN),
                        DueTime = now,
                        SealedBags = selectedItem.SealedBags
                    };

                    context.Consignments.Add(consignment);

                    //linking Denomination with Consignment for preserving later
                    Denomination denom = new()
                    {
                        Id = context.Sequences.GetNextDenominationSequence(),
                        ConsignmentId = consignment.Id,
                        DenominationType = DenominationType.Leafs
                    };
                    context.Denominations.Add(denom);

                    var deliveryId = context.Sequences.GetNextDeliverySequence();
                    var delivery = new ConsignmentDelivery()
                    {
                        Id = deliveryId,
                        CrewId = null,
                        ConsignmentId = consignment.Id,
                        FromPartyId = selectedItem.FromPartyId,
                        ToPartyId = selectedItem.ToPartyId,
                        DestinationLocationId = dummyLocation.Id,
                        PickupLocationId = dummyLocation.Id,
                        PickupCode = $"{consignment.ShipmentCode}{deliveryId}-Pickup",
                        DropoffCode = $"{consignment.ShipmentCode}{deliveryId}-Dropoff"
                    };
                    context.ConsignmentDeliveries.Add(delivery);

                    foreach (var item in context.ShipmentChargeType.ToList())
                    {
                        context.ShipmentCharges.Add(new ShipmentCharge()
                        {
                            ChargeTypeId = item.Id,
                            ConsignmentId = delivery.ConsignmentId,
                            Amount = 0,
                            Status = 1
                        });
                    }

                    context.ConsignmentStates.Add(new ConsignmentState()
                    {
                        ConsignmentId = delivery.ConsignmentId,
                        CreatedBy = User.Identity.Name,
                        ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                        TimeStamp = MyDateTime.Now,
                        Status = StateTypes.Confirmed
                    });

                    var states = Enum.GetValues(typeof(Shared.Enums.ConsignmentDeliveryState));
                    for (int i = 1; i < states.Length; i++)
                    {
                        context.ConsignmentStates.Add(new ConsignmentState()
                        {
                            ConsignmentId = delivery.ConsignmentId,
                            CreatedBy = User.Identity.Name,
                            ConsignmentStateType = (Shared.Enums.ConsignmentDeliveryState)states.GetValue(i),
                            Status = StateTypes.Waiting
                        });
                    }

                }

                if (consignment.ConsignmentStateType >= Shared.Enums.ConsignmentDeliveryState.InTransit)
                {
                    throw new BadRequestException("You cannot edit consignment details once consignment is picked up by crew.");
                }

                if (selectedItem.Id > 0)
                {
                    var firstDelivery = await context.ConsignmentDeliveries.FirstOrDefaultAsync(x => x.ConsignmentId == selectedItem.Id && x.ParentId == null);
                    firstDelivery.FromPartyId = selectedItem.FromPartyId;

                    var lastDelivery = await context.ConsignmentDeliveries.FirstOrDefaultAsync(x => x.ConsignmentId == selectedItem.Id && x.Childern.Count == 0);
                    lastDelivery.ToPartyId = selectedItem.ToPartyId;

                }
                var parties = await context.Parties.Where(x => x.Id == selectedItem.BillBranchId || x.Id == selectedItem.FromPartyId || x.Id == selectedItem.ToPartyId)
                    .ToListAsync();
                var collectionBranch = parties.FirstOrDefault(x => x.Id == selectedItem.FromPartyId);
                var deliveryBranch = parties.FirstOrDefault(x => x.Id == selectedItem.ToPartyId);
                var billingBranch = parties.FirstOrDefault(x => x.Id == selectedItem.BillBranchId);

                consignment.CustomerId = selectedItem.BillBranchId.GetValueOrDefault();

                consignment.FromPartyId = selectedItem.FromPartyId;
                consignment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
                consignment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
                consignment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

                consignment.ToPartyId = selectedItem.ToPartyId;
                consignment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
                consignment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
                consignment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


                consignment.BillBranchId = selectedItem.BillBranchId.GetValueOrDefault();
                consignment.BillingRegionId = billingBranch.RegionId.GetValueOrDefault();
                consignment.BillingSubRegionId = billingBranch.SubregionId.GetValueOrDefault();
                consignment.BillingStationId = billingBranch.StationId.GetValueOrDefault();

                consignment.MainCustomerId = context.PartyRelationships
                    .First(x => x.FromPartyId == consignment.CustomerId && x.ToPartyRole == RoleType.ParentOrganization)
                    .ToPartyId;

                consignment.CurrencySymbol = selectedItem.CurrencySymbol;
                consignment.Amount = selectedItem.Amount;
                consignment.AmountPKR = selectedItem.AmountPKR;
                consignment.ExchangeRate = selectedItem.ExchangeRate;
                consignment.Valueables = selectedItem.Valueables;
                consignment.ServiceType = selectedItem.ServiceType;
                consignment.ConsignmentStatus = selectedItem.ConsignmentStatus;

                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                consignment.OriginPartyId = user.PartyId;
                consignment.CounterPartyId = consignment.OriginPartyId == consignment.FromPartyId ? consignment.ToPartyId : consignment.OriginPartyId;
                 
                List<ShipmentComment> listOfComments = new List<ShipmentComment>();
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                if (selectedItem.Comments != null)
                {
                    listOfComments.Add(new ShipmentComment()
                    {
                        Description = selectedItem.Comments,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ViewedAt = MyDateTime.Now,
                        ViewedBy = User.Identity.Name
                    });

                    consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                }
                try
                {
                    var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                    consignment.Distance = distanceInfo.Item1;
                    consignment.DistanceStatus = distanceInfo.Item2;

                }
                catch { }
                await context.SaveChangesAsync();

                await PublishUpdates(consignment);

                if (consignment.ApprovalState == ConsignmentApprovalState.Approved && consignment.Type != ShipmentExecutionType.Live)
                {
                    var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);
                }
                else if (User.IsInRole(BANK_CPC) || User.IsInRole(BANK_BRANCH))
                {
                    var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                    notificationIds.ForEach(x => NotificationAgent.WebPushNotificationsQueue.Add(x));
                }

                if (selectedItem.Id > 0)
                {
                    await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                }
                return consignment.Id;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.Message);
            }
        }
        #endregion

        #region Extra Methods

        [HttpGet]
        public async Task<ConsignmentListViewModel> GetShipmentFromCache(int id)
        {
            return await shipmentsCacheService.GetShipment(id);
        }

        public async Task<int> PostConsignmentDelivery([FromBody] DeliveryFormViewModel deliveryFormViewModel)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Location location = null;

            if (deliveryFormViewModel.LocationId == null)
            {
                if (!string.IsNullOrEmpty(deliveryFormViewModel.LocationName))
                {
                    location = await context.Locations.FirstOrDefaultAsync(x => x.Name.Equals(deliveryFormViewModel.LocationName));
                    if (location != null)
                        throw new BadRequestException("This location is already exist!");

                    location = new Location()
                    {
                        Id = sequenceService.GetNextPartiesSequence(),
                        Name = deliveryFormViewModel.LocationName,
                        Description = deliveryFormViewModel.LocationName,
                        Geolocation = (deliveryFormViewModel.Latitude > 0 || deliveryFormViewModel.Longitude > 0) ?
                        geometryFactory.CreatePoint(new Coordinate(deliveryFormViewModel.Longitude, deliveryFormViewModel.Latitude)) : null,
                        UpdatedAt = DateTime.Now,
                        Type = LocationType.Address,
                        Code = deliveryFormViewModel.LocationName.Substring(0, 3),
                    };
                    context.Locations.Add(location);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new BadRequestException("Please select location!");
                }
            }
            else
            {
                location = await context.Locations.FirstOrDefaultAsync(x => x.Id == deliveryFormViewModel.LocationId);
            }

            int deliveryId = await AddDelivery(deliveryFormViewModel.ConsignmentId,
                  deliveryFormViewModel.CrewId.GetValueOrDefault(),
                  location.Id,
                  deliveryFormViewModel.PlanedPickupTime.GetValueOrDefault(),
                  deliveryFormViewModel.PlanedPickupTime.GetValueOrDefault());
            await shipmentsCacheService.SetShipment(deliveryFormViewModel.ConsignmentId, null);
            return deliveryId;
        }
        private async Task<int> AddDelivery(int consignmentId, int crewId, int locationId, DateTime? pickupTime, DateTime? dropoffTime)
        {
            var lastDelivery = await context.ConsignmentDeliveries
                .Include(x => x.Consignment)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId);

            if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                throw new BadRequestException("You need to assign this consignment to one of crews before including additional crews or vault");

            if (lastDelivery.CrewId.GetValueOrDefault() == crewId)
                throw new BadRequestException("You cannot assign two Consective deliveries to same crew or vault");

            //   var dummyLocation = context.Locations.First();
            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var newDelivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                ParentId = lastDelivery.Id,
                ConsignmentId = consignmentId,
                CrewId = crewId,
                DestinationLocationId = locationId,
                PickupLocationId = locationId,
                FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                ToPartyId = lastDelivery.ToPartyId,
                PlanedPickupTime = pickupTime,
                PickupCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Handover",
                DropoffCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Dropoff",
                DeliveryState = Shared.Enums.ConsignmentDeliveryState.CrewAssigned
            };

            context.ConsignmentDeliveries.Add(newDelivery);

            lastDelivery.ToPartyId = crewId;
            lastDelivery.DestinationLocationId = locationId;
            lastDelivery.PlanedDropTime = dropoffTime;
            lastDelivery.DropoffCode = newDelivery.PickupCode;

            await context.SaveChangesAsync();
            await PublishIncrementalUpdates(lastDelivery.ConsignmentId);
            await notificationService.CreateFirebaseNotification(lastDelivery.Id, lastDelivery.ConsignmentId, lastDelivery.CrewId,
                lastDelivery.Consignment.ShipmentCode, NotificationType.UpdatedDropoff, NotificationCategory.CIT);

            await notificationService.CreateFirebaseNotification(newDelivery.Id, newDelivery.ConsignmentId, newDelivery.CrewId,
                newDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);
            await shipmentsCacheService.SetShipment(consignmentId, null);
            return deliveryId;
        }

        [HttpPost]
        public async Task<int> AssignCrew([FromBody] DeliveryCrewFormViewModel selectedItem)
        {
            using (var trx = await context.Database.BeginTransactionAsync())
            {
                try
                {

                    await workOrderService.AssignCrewAsync(selectedItem.DeliveryId, selectedItem.CrewId, true);
                    await workOrderService.LogConsignmentState(selectedItem.ConsignmenId, Shared.Enums.ConsignmentDeliveryState.CrewAssigned, User.Identity.Name);
                    await trx.CommitAsync();
                    await shipmentsCacheService.SetShipment(selectedItem.ConsignmenId, null);
                    await PublishIncrementalUpdates(selectedItem.ConsignmenId);

                    var notificationId = await notificationService.CreateCrewAssignedNotification(selectedItem.ConsignmenId, selectedItem.ConsignmenId.ToString());
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId);
                    return 1;
                }
                catch (Exception ex)
                {
                    await trx.RollbackAsync();
                    throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
                }
            }
        }

        [HttpGet]
        public async Task<CitDenominationViewModel> GetDenomination(int id)
        {
            var denom = await (from d in context.Denominations
                               join c in context.Consignments on d.ConsignmentId equals c.Id
                               where d.ConsignmentId == id //&& d.CashCountType == cashCountType
                               select new CitDenominationViewModel()
                               {
                                   Id = d.Id,
                                   Type = d.DenominationType,
                                   ConsignmentId = d.ConsignmentId,
                                   ShipmentCode = c.ShipmentCode,
                                   Currency1x = d.Currency1x,
                                   Currency2x = d.Currency2x,
                                   Currency5x = d.Currency5x,
                                   Currency10x = d.Currency10x,
                                   Currency20x = d.Currency20x,
                                   Currency50x = d.Currency50x,
                                   Currency75x = d.Currency75x,
                                   Currency100x = d.Currency100x,
                                   Currency500x = d.Currency500x,
                                   Currency1000x = d.Currency1000x,
                                   Currency5000x = d.Currency5000x,
                                   TotalAmount = c.Amount,
                                   AmountPKR = c.AmountPKR,
                                   CurrencySymbol = c.CurrencySymbol,
                                   ExchangeRate = c.ExchangeRate
                               }).FirstOrDefaultAsync();
            return denom;
        }

        private async Task<IActionResult> PublishUpdates(Consignment consignment)
        {
            if (consignment != null)
            {
                var shipment = await workOrderService.GetShipment(consignment.Id);
                await shipmentsCacheService.SetShipment(shipment.Id, shipment);
                await consignmentHub.Clients.All.SendAsync("OnNewShipment", new
                {
                    shipment.Id,
                    shipment.FromPartyId,
                    shipment.ToPartyId,
                    shipment.CollectionRegionId,
                    shipment.CollectionSubRegionId,
                    shipment.CollectionStationId,
                    shipment.DeliveryRegionId,
                    shipment.DeliverySubRegionId,
                    shipment.DeliveryStationId
                });
            } 
            return Ok();
        }
        static HttpClient client = new HttpClient();

        [AllowAnonymous]
        public async Task<IActionResult> PublishIncrementalUpdates(int id)
        {
            var shipment =await workOrderService.GetShipment(id);
            await shipmentsCacheService.SetShipment(shipment.Id, shipment);

            await consignmentHub.Clients.All.SendAsync("OnShipmentUpdated", id.ToString());
           
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult QueueWebNotification(int id)
        {
            NotificationAgent.WebPushNotificationsQueue.Add(id);
              
            return Ok();
        }

        [HttpGet]
        public async Task<IEnumerable<ShowConsignmentsViewModel>> GetConsignments(int crewId)
        {
            var consignments = await (from c in context.Consignments
                                      join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                                      join f in context.Parties on c.FromPartyId equals f.Id
                                      join t in context.Parties on c.ToPartyId equals t.Id
                                      where d.CrewId == crewId
                                      && MyDateTime.Today.Date == c.CreatedAt.Date
                                      && c.ConsignmentStateType < Shared.Enums.ConsignmentDeliveryState.Delivered
                                      select new ShowConsignmentsViewModel()
                                      {
                                          PickupBranch = f.FormalName,
                                          DropoffBranch = t.FormalName,
                                          ConsignmentStatus = c.ConsignmentStateType.ToString(),
                                          Amount = c.Amount,
                                          CreatedAt = c.CreatedAt,
                                          ShipmentCode = c.ShipmentCode
                                      }).ToListAsync();
            return consignments;
        }

        [HttpGet]
        public async Task<BranchFormViewModel> GetBranchData(int branchId)
        {
            BranchFormViewModel branch = new BranchFormViewModel();
            try
            {
                branch = await (from p in context.Parties
                                join o in context.Orgnizations on p.Id equals o.Id
                                where p.Id == branchId
                                select new BranchFormViewModel()
                                {
                                    BranchId = p.Id,
                                    ShortName = p.ShortName,
                                    FormalName = p.FormalName,
                                    Address = p.Address,
                                    PersonalContactNo = p.PersonalContactNo,
                                    OfficialContactNo = p.OfficialContactNo,
                                    Latitude = o.Geolocation.Y,
                                    Longitude = o.Geolocation.X,
                                    LocationStatus = o.LocationStatus
                                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation("Exception : " + ex.ToString());
            }
            return branch;
        }

        [HttpPost]
        public async Task<int> ChangeBranchData(BranchFormViewModel vm)
        {
            try
            {
                if (vm.Latitude.GetValueOrDefault() == 0 || vm.Longitude.GetValueOrDefault() == 0)
                    throw new BadRequestException("Please provide Latitude and Longitude values");
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var party = await context.Parties.Include(x => x.Orgnization).FirstOrDefaultAsync(x => x.Id == vm.BranchId);
                if (party != null)
                {
                    party.ShortName = vm.ShortName;
                    party.FormalName = vm.FormalName;
                    party.Address = vm.Address;
                    party.PersonalContactNo = vm.PersonalContactNo;
                    party.OfficialContactNo = vm.OfficialContactNo;
                    party.Orgnization.LocationStatus = vm.LocationStatus;

                    if (vm.Latitude > 0 && vm.Longitude > 0)
                    {
                        if (party.Orgnization.Geolocation?.X != vm.Longitude.Value || party.Orgnization.Geolocation?.Y != vm.Latitude.Value)
                        {
                            party.Orgnization.Geolocation = geometryFactory.CreatePoint(new Coordinate(vm.Longitude.Value, vm.Latitude.Value));
                            party.Orgnization.GeolocationUpdateAt = DateTime.Now;
                            party.Orgnization.GeolocationVersion++;

                            await partiesCache.SetGeoCoordinate(party.Id, new Point(party.Orgnization.Geolocation.Y, party.Orgnization.Geolocation.X));

                            // reset the distance status so that it can be recalculated by distance calculation service.
                            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE IntraPartyDistances SET DistanceStatus = '0' WHERE DistanceStatus = '1' AND (FromPartyId = {party.Id} OR ToPartyId = {party.Id})");

                            // update relevent shipments accordingly.
                            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND (FromPartyId = {party.Id} OR ToPartyId = {party.Id})");

                        }
                    }
                    await context.SaveChangesAsync();
                    await partiesCache.SetGeoStatus(party.Orgnization.Id, vm.LocationStatus);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetCPCBranches(int id)
        {
            var query = await (from p in context.Parties
                               join r in context.PartyRelationships on p.Id equals r.FromPartyId
                               where r.ToPartyId == id &&
                                   r.FromPartyRole == RoleType.ChildOrganization
                                   && r.ToPartyRole == RoleType.BankCPC
                               select new SelectListItem(p.Id, p.ShortName + "-" + p.FormalName)).ToListAsync();
            return query;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetCrews()
        {
            var crews = await (from o in context.Parties
                               join r in context.PartyRelationships on o.Id equals r.FromPartyId
                               where (r.FromPartyRole == RoleType.Crew || r.FromPartyRole == RoleType.Vault)
                               && r.IsActive
                               select new SelectListItem()
                               {
                                   Value = o.Id.ToString(),
                                   Text = o.FormalName
                               }).ToListAsync();

            return crews;
        }

        [HttpGet]
        public async Task<IEnumerable<CrewWithLocation>> GetCrewsWithLocationMatrix(int consignmentId)
        {


            var consignment = await (from c in context.Consignments
                                     join f in context.Parties on c.FromPartyId equals f.Id
                                     join d in context.Parties on c.ToPartyId equals d.Id
                                     where c.Id == consignmentId
                                     select new ConsignmentLocationsViewModel()
                                     {
                                         DeliveryId = 0,
                                         //PickupPoint = new Point(c.FromParty.Orgnization.Geolocation.Y,
                                         //c.FromParty.Orgnization.Geolocation.X),
                                         PickupPartyId = c.FromPartyId,
                                         PickupStationId = f.StationId,
                                         //DropoffPoint = new Point(c.ToParty.Orgnization.Geolocation.Y,
                                         //c.ToParty.Orgnization.Geolocation.X),
                                         DropoffStationId = d.StationId,
                                         FromPartyCode = c.FromParty.ShortName,
                                         ToPartyCode = c.ToParty.ShortName,
                                         DropoffPartyId = c.ToPartyId
                                     }).FirstAsync();

            consignment.PickupPoint = await partiesCache.GetGeoCoordinate(consignment.PickupPartyId);
            consignment.DropoffPoint = await partiesCache.GetGeoCoordinate(consignment.DropoffPartyId);

            var crewsWithCommonDestination = await (from s in context.Consignments
                                              join d in context.ConsignmentDeliveries on s.Id equals d.ConsignmentId
                                              where s.DeliveryStationId == consignment.PickupStationId
                                              && d.CrewId != null
                                              && s.CreatedAt >= MyDateTime.Today && s.CreatedAt <= MyDateTime.Today.AddDays(1)
                                              select d.CrewId).ToListAsync();

            var crews = await (from p in context.Parties
                               join o in context.Orgnizations on p.Id equals o.Id
                               where o.OrganizationType == OrganizationType.Crew
                               && p.IsActive
                                && ((p.StationId == consignment.PickupStationId || p.StationId == consignment.DropoffStationId)
                                || crewsWithCommonDestination.Contains(p.Id))
                               select new CrewWithLocation()
                               {
                                   IntValue = p.Id,
                                   Text = p.FormalName,
                                   // CrewLocation = new Point(o.Geolocation.Y, o.Geolocation.X),
                               }).ToListAsync();


            double? distance = null;
            try
            {
                var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.PickupPartyId, consignment.PickupPoint, consignment.DropoffPartyId, consignment.DropoffPoint);
                distance = distanceInfo.Item1;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            var crewIds = crews.Select(x => x.IntValue).ToList();


            var allShipmentsOfPotentialCrews = context.ConsignmentDeliveries
                     .Include(x => x.Consignment).Include(x => x.ToParty.Orgnization)
                     .Where(x => crewIds.Contains(x.CrewId) && x.DeliveryState < Shared.Enums.ConsignmentDeliveryState.Delivered && x.Consignment.DueTime > MyDateTime.Today).ToList();


            foreach (var crew in crews)
            {
                crew.CrewLocation = await partiesCache.GetTemporatlGeoCoordinate(crew.IntValue.GetValueOrDefault());
                crew.ConsignmentDistance = distance.GetValueOrDefault() == 0 ? "Not Available, please check lat/long" : $"{Math.Round(distance.Value / 1000d, 1)} km consignemt distance";
                crew.ConsignmentDistance_ = distance.GetValueOrDefault() == 0 ? double.MaxValue : Math.Round(distance.Value / 1000d, 1);
                crew.PickupLocation = consignment.PickupPoint;
                try
                {
                    var result = CalculateDistanceLocally(crew.CrewLocation, consignment.PickupPoint);
                    crew.PickeupStats = $"{result}Km to reach pickup";
                    crew.PickeupStats_ = result;
                }
                catch (Exception ex)
                {
                    crew.PickeupStats = ex.Message;
                }

                var deliveries = allShipmentsOfPotentialCrews.Where(x => x.CrewId == crew.IntValue).ToList();
                crew.PickupUpConsignments = deliveries.Count == 0 ?
                "**No Shipments**" : $"{deliveries.Sum(x => x.Consignment.Amount) / 1000000}M cash, {deliveries.Count} consignments";

                var distances = new List<double>(deliveries.Count);
                foreach (var d in deliveries)
                {
                    try
                    {
                        var toLocation = new Point(d.ToParty.Orgnization.Geolocation?.Y, (d.ToParty.Orgnization.Geolocation?.X));

                        distances.Add((await GetDistanceOrCalculateUsingGoogle(d.ToPartyId, toLocation, consignment.DropoffPartyId, consignment.DropoffPoint)).Item1);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex.Message);
                    }
                }
                if (distances.Count > 0)
                {
                    crew.ClosetToDropffDistance_ = Math.Round(distances.Min() / 1000d, 1);
                    crew.ClosetToDropffDistance = $"{crew.ClosetToDropffDistance_} kms difference in destinations";

                }
                else
                {
                    crew.ClosetToDropffDistance_ = 0;
                    crew.ClosetToDropffDistance = "This crew has no destinations";
                }
            }

            return crews.OrderBy(x => x.ConsignmentDistance_);
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetLocations(LocationType? locationType)
        {
            var query = (from o in context.Locations

                         select new
                         {
                             Value = o.Id,
                             Text = o.Name,
                             o.Type
                         });

            if (locationType.HasValue)
            {
                query = query.Where(x => x.Type == locationType);
            }

            return await query.Select(x => new SelectListItem()
            {
                Text = x.Text,
                IntValue = x.Value
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetSiblingBranches(int id1, int id2)
        {
            try
            {
                var results = await (
                               from c in context.PartyRelationships
                               join r in context.PartyRelationships on c.ToPartyId equals r.ToPartyId
                               join p in context.Parties on r.FromPartyId equals p.Id
                               join o in context.Orgnizations on p.Id equals o.Id

                               where o.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
                                 && (id1 == c.FromPartyId || id2 == c.FromPartyId)
                                 && r.ToPartyRole == RoleType.ParentOrganization
                                 && c.ToPartyRole == RoleType.ParentOrganization
                                 && p.IsActive
                               select new SelectListItem(o.Id, p.ShortName, p.FormalName, "-")).Distinct()
             .ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.ToString() + ex.InnerException?.ToString());
            }
        }

        [HttpPost]
        public async Task<int> AssignVault(DeliveryVaultViewModel viewModel)
        {
            try
            {
                var isCrew = await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == viewModel.VaultId && x.OrganizationType == OrganizationType.Crew);
                if (isCrew != null)
                {
                    var relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == viewModel.VaultId
                    && x.FromPartyRole == RoleType.Vault);
                    if (relationship == null)
                    {
                        relationship = new PartyRelationship
                        {
                            Id = sequenceService.GetNextPartiesSequence(),
                            ToPartyId = 1, // Id of SOS
                            FromPartyRole = RoleType.Vault,
                            ToPartyRole = RoleType.ParentOrganization,
                            FromPartyId = viewModel.VaultId,
                            StartDate = MyDateTime.Now,
                            IsActive = true
                        };
                        context.PartyRelationships.Add(relationship);
                        await context.SaveChangesAsync();
                    }
                }
                var dummyLocation = context.Locations.First();
                await shipmentsCacheService.SetShipment(viewModel.ConsignmentId, null);
                return await AddDelivery(viewModel.ConsignmentId, viewModel.VaultId, dummyLocation.Id, null, null);
           
            }  
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
            }
        }

        private async Task<Tuple<double, DataRecordStatus>> GetDistanceOrCalculateUsingGoogle(int pickupPartyId, int dropoffPartyId)
        {
            var branches = await context.Orgnizations.Where(x => x.Id == pickupPartyId || x.Id == dropoffPartyId).ToListAsync();
            var collectionPoint = new Point(branches.First(x => x.Id == pickupPartyId).Geolocation?.Y, branches.First(x => x.Id == pickupPartyId).Geolocation?.X);
            var deliveryPoint = new Point(branches.First(x => x.Id == dropoffPartyId).Geolocation?.Y, branches.First(x => x.Id == pickupPartyId).Geolocation?.X);
            return await GetDistanceOrCalculateUsingGoogle(pickupPartyId, collectionPoint, dropoffPartyId, deliveryPoint);
        }

        private async Task<Tuple<double, DataRecordStatus>> GetDistanceOrCalculateUsingGoogle(int pickupPartyId, Point pickupPoint, int dropoffPartyId, Point dropoffPoint)
        {
            if ((pickupPoint?.Lat).GetValueOrDefault() == 0 || (dropoffPoint?.Lat).GetValueOrDefault() == 0)
                return new Tuple<double, DataRecordStatus>(0, DataRecordStatus.None);

            var intraPartyDistance = context.IntraPartyDistances
                .OrderByDescending(x=>x.DistanceStatus)
                .FirstOrDefault(x => (x.FromPartyId == pickupPartyId && x.ToPartyId == dropoffPartyId) || (x.ToPartyId == pickupPartyId && x.FromPartyId == dropoffPartyId));
            if (intraPartyDistance == null)
            {
                var result = GMapsService.ClaculateDistanceUsinGoogle(pickupPoint, dropoffPoint, string.Empty);

                if (intraPartyDistance == null)
                {
                    intraPartyDistance = new IntraPartyDistance()
                    {
                        FromPartyId = pickupPartyId,
                        ToPartyId = dropoffPartyId
                    };
                    context.IntraPartyDistances.Add(intraPartyDistance);
                }
                intraPartyDistance.AverageTravelTime = (int)result.duration.Value;
                intraPartyDistance.Distance = (int)result.distance.Value;
                intraPartyDistance.UpdatedBy = intraPartyDistance.UpdatedBy == null ? "System" : $"System at {DateTime.Now}_{intraPartyDistance.UpdatedBy}";
                intraPartyDistance.UpdateAt = DateTime.Now;
                await context.SaveChangesAsync();
            }
            return  new Tuple<double, DataRecordStatus>(intraPartyDistance.Distance, intraPartyDistance.DistanceStatus);
        }


        protected double CalculateDistanceLocally(Point p1, Point p2)
        {
            if ((p1?.Lat).GetValueOrDefault() == 0 || (p2?.Lat).GetValueOrDefault() == 0)
                throw new InvalidDataException("Geolocation data is not available");

            var d1 = p1?.Lat * (Math.PI / 180.0);
            var num1 = p1?.Lng * (Math.PI / 180.0);
            var d2 = p2?.Lat * (Math.PI / 180.0);
            var num2 = p2?.Lng * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2.Value - d1.Value) / 2.0), 2.0) + Math.Cos(d1.Value) * Math.Cos(d2.Value) * Math.Pow(Math.Sin(num2.Value / 2.0), 2.0);

            return Math.Round((6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)))) / 1000d, 1);
        }

        [HttpPost]
        public async Task<int> PostConsignmentStatus(ConsignmentStatusViewModel vm)
        {
            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == vm.ConsignmentId);
            List<ShipmentComment> listOfComments = new();
            try
            {
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                if (vm.Comments == null)
                    throw new BadRequestException("Please enter something in commention section to add!");

                listOfComments.Add(new ShipmentComment()
                {
                    Description = vm.Comments + "," + vm.ConsignmentStatus,
                    CreatedAt = MyDateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ViewedAt = MyDateTime.Now,
                    ViewedBy = User.Identity.Name
                });
                consignment.ConsignmentStatus = vm.ConsignmentStatus;
                consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                await context.SaveChangesAsync();
                await shipmentsCacheService.SetShipment(vm.ConsignmentId, null);
                if (vm.ConsignmentStatus == ConsignmentStatus.Declined || vm.ConsignmentStatus == ConsignmentStatus.Cancelled)
                {
                    var notificationId = await notificationService.CreateShipmentDeclinedNotification(consignment.Id, consignment.ShipmentCode);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return consignment.Id;
        }

        [HttpPost]
        public async Task<int> ApproveConsignmentStatus(ConsignmentApprrovalViewModel ConsignmentStatusViewModel)
        {
            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == ConsignmentStatusViewModel.ConsignmentId);

            if(!string.IsNullOrEmpty(consignment.ApprovedBy) && consignment.ApprovedBy != User.Identity.Name)
            {
                throw new BadRequestException($"Only {consignment.ApprovedBy} can supervise/authenticate this shipment");
            }

            try
            {
                consignment.ApprovalState = ConsignmentStatusViewModel.ApprovalState;
                consignment.ApprovedBy = User.Identity.Name;
                consignment.ApprovedAt = MyDateTime.Now;

                List<ShipmentComment> listOfComments = new();
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);

                listOfComments.Add(new ShipmentComment()
                {
                    Description = $"Approved by {User.Identity.Name} at {consignment.ApprovedAt?.ToString("hh:mm tt dd/MM/yyyy")} with " 
                    + (string.IsNullOrEmpty(ConsignmentStatusViewModel.Comments)? "No Comments" : $"Comments {ConsignmentStatusViewModel.Comments}") ,
                    CreatedAt = MyDateTime.Now,
                    CreatedBy = User.UserId()
                });

                consignment.Comments = JsonConvert.SerializeObject(listOfComments);

                await context.SaveChangesAsync();
                await shipmentsCacheService.SetShipment(consignment.Id, null);
                var notificationId1 = await notificationService.CreateShipmentApprovedNotification(consignment.Id, NotificationType.ShipmentApproved, consignment.ShipmentCode);
                NotificationAgent.WebPushNotificationsQueue.Add(notificationId1);

                var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);

                await PublishIncrementalUpdates(consignment.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return consignment.Id;
        }

        public async Task<DistanceUpdateResult> UpdateShipmentDistance(ShipmentAdministrationViewModel model)
        {
            var consignment = await (from c in context.Consignments where c.Id == model.ConsignmentId select c).FirstOrDefaultAsync();
            var distance = await context.IntraPartyDistances
                .FirstOrDefaultAsync(x => (x.FromPartyId == consignment.FromPartyId && x.ToPartyId == consignment.ToPartyId)
                || (x.FromPartyId == consignment.ToPartyId && x.ToPartyId == consignment.FromPartyId));
            if (distance == null)
            {
                distance = new IntraPartyDistance()
                {
                    FromPartyId = consignment.FromPartyId,
                    ToPartyId = consignment.ToPartyId
                };
                context.IntraPartyDistances.Add(distance);
            }
            else
            {
                var distanceHistory = new IntraPartyDistanceHistory()
                {
                    Id = sequenceService.GetNextIntraPartyDistanceSequence(),
                    FromPartyId = distance.FromPartyId,
                    ToPartyId = distance.ToPartyId,
                    CreatedAt = distance.UpdateAt.GetValueOrDefault(),
                    CreatedBy = distance.CreatedBy,
                    UpdatedBy = distance.UpdatedBy,
                    UpdateAt = MyDateTime.Now,
                    DistanceSource = distance.DistanceSource,
                    DistanceStatus = distance.DistanceStatus,
                    Distance = distance.Distance,
                };
                context.IntraPartyDistancesHistory.Add(distanceHistory);

            }

            distance.Distance = model.Distance * 1000;
            distance.DistanceStatus = model.DistanceStatus;
            distance.UpdateAt = DateTime.Now;
            distance.UpdatedBy = User.Identity.Name;
            await context.SaveChangesAsync();
            var distanceUpdateResult = new DistanceUpdateResult
            {
                Repushed = await context.Consignments.FromSqlInterpolated($"Select * FROM Consignments WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))")
                .Select(x => new SelectListItem(x.Id, x.ShipmentCode)).ToListAsync(),

                Updated = await context.Consignments.FromSqlInterpolated($"Select * FROM Consignments WHERE (ConsignmentStatus = '0' OR ConsignmentStatus = '3' OR ConsignmentStatus = '5') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))")
                .Select(x => new SelectListItem(x.Id, x.ShipmentCode)).ToListAsync()
            };

            var v1 = await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128' OR ConsignmentStatus = '640') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))");
            var v2 = await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET Distance = {model.Distance * 1000}, DistanceStatus = {(byte)model.DistanceStatus} WHERE (ConsignmentStatus = '0' OR ConsignmentStatus = '3' OR ConsignmentStatus = '5') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))");
            
            await partiesCache.SetGeoStatus(consignment.Id, model.DistanceStatus);

            foreach (var item in distanceUpdateResult.Updated)
            {
                await shipmentsCacheService.SetShipment(item.IntValue.GetValueOrDefault(), null);
            }

            return distanceUpdateResult;

        }

        [HttpPost]
        public async Task<int> PostBulkShipments(BulkShipmentsViewModel viewModel)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    if (viewModel.FromPartyDic.ContainsKey(i) && viewModel.ToPartyDic.ContainsKey(i) && viewModel.BillBranchDic.ContainsKey(i))
                    {
                        if (viewModel.FromPartyDic[i] > 0 && viewModel.ToPartyDic[i] > 0 && viewModel.BillBranchDic[i] > 0)
                        {
                            var consignmentId = sequenceService.GetNextCitOrdersSequence();
                            var now = MyDateTime.Now;
                            var consignment = new Consignment()
                            {
                                Id = consignmentId,
                                CreatedAt = now,
                                DueTime = now,
                                ShipmentCode = $"CIT/{MyDateTime.Now.Year}/" + consignmentId.ToString("D4"),
                                //  ManualShipmentCode = selectedItem.ManualShipmentCode,
                                Type = ShipmentExecutionType.Live,
                                ApprovalState = User.IsInRole(BANK_BRANCH) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved,
                                ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                                CreatedBy = User.Identity.Name,
                                ServiceType = ServiceType.ByRoad,
                                ShipmentType = ShipmentType.Unknown,
                                ConsignmentStatus = ConsignmentStatus.TobePosted,
                                CustomerId = viewModel.BillBranchDic[i],
                                FromPartyId = viewModel.FromPartyDic[i],
                                ToPartyId = viewModel.ToPartyDic[i],
                                BillBranchId = viewModel.BillBranchDic[i],
                                MainCustomerId = context.PartyRelationships
                                    .First(x => x.FromPartyId == viewModel.BillBranchDic[i] && x.ToPartyRole == RoleType.ParentOrganization)
                                    .ToPartyId,
                                CurrencySymbol = CurrencySymbol.PKR,

                            };

                            var parties = await context.Parties.Where(x => x.Id == viewModel.BillBranchDic[i] || x.Id == viewModel.FromPartyDic[i] || x.Id == viewModel.ToPartyDic[i])
                            .ToListAsync();
                            var collectionBranch = parties.FirstOrDefault(x => x.Id == viewModel.FromPartyDic[i]);
                            var deliveryBranch = parties.FirstOrDefault(x => x.Id == viewModel.ToPartyDic[i]);
                            var billingBranch = parties.FirstOrDefault(x => x.Id == viewModel.BillBranchDic[i]);

                            consignment.CustomerId = viewModel.BillBranchDic[i];

                            consignment.FromPartyId = viewModel.FromPartyDic[i];
                            consignment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
                            consignment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
                            consignment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

                            consignment.ToPartyId =  viewModel.ToPartyDic[i];
                            consignment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
                            consignment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
                            consignment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


                            consignment.BillBranchId = viewModel.BillBranchDic[i];
                            consignment.BillingRegionId = billingBranch.RegionId.GetValueOrDefault();
                            consignment.BillingSubRegionId = billingBranch.SubregionId.GetValueOrDefault();
                            consignment.BillingStationId = billingBranch.StationId.GetValueOrDefault();

                            consignment.MainCustomerId = context.PartyRelationships
                                .First(x => x.FromPartyId == consignment.CustomerId && x.ToPartyRole == RoleType.ParentOrganization)
                                .ToPartyId;


                            try
                            {
                                var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                                consignment.Distance = distanceInfo.Item1;
                                consignment.DistanceStatus = distanceInfo.Item2;

                            }
                            catch { }

                            context.Consignments.Add(consignment);

                            var dinomination = new Denomination()
                            {
                                ConsignmentId = consignmentId,
                                Id = context.Sequences.GetNextDenominationSequence(),
                            };
                            context.Denominations.Add(dinomination);



                            int deliveryId = context.Sequences.GetNextDeliverySequence();
                            var dummyLocation = context.Locations.First();

                            var consignmentDelivery = new ConsignmentDelivery()
                            {
                                FromPartyId = viewModel.FromPartyDic[i],
                                ToPartyId = viewModel.ToPartyDic[i],
                                DestinationLocationId = dummyLocation.Id,
                                PickupLocationId = dummyLocation.Id,
                                ConsignmentId = consignmentId,
                                Id = deliveryId,
                                PlanedPickupTime = MyDateTime.Now,
                                PlanedDropTime = MyDateTime.Now.AddHours(1),
                                // consignmentDelivery.PickupCode = $"{consignment.ShipmentCode}{deliveryId}-Pickup";
                                //consignmentDelivery.DropoffCode = $"{consignment.ShipmentCode}{deliveryId}-Dropoff";
                            };
                            context.ConsignmentDeliveries.Add(consignmentDelivery);

                            var consignmentState = new ConsignmentState()
                            {
                                ConsignmentId = consignmentId,
                                ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.Created,
                                TimeStamp = MyDateTime.Now,
                                Status = StateTypes.Confirmed,
                            };
                            context.ConsignmentStates.Add(consignmentState);


                            var states = Enum.GetValues(typeof(Web.Shared.Enums.ConsignmentDeliveryState));
                            for (int j = 1; j < states.Length; j++)
                            {
                                context.ConsignmentStates.Add(new ConsignmentState()
                                {
                                    ConsignmentId = consignmentId,
                                    CreatedBy = User.Identity.Name,
                                    ConsignmentStateType = (Shared.Enums.ConsignmentDeliveryState)states.GetValue(j),
                                    Status = StateTypes.Waiting
                                });
                            }

                            foreach (var item in context.ShipmentChargeType.ToList())
                            {
                                context.ShipmentCharges.Add(new ShipmentCharge()
                                {
                                    ChargeTypeId = item.Id,
                                    ConsignmentId = consignmentId,
                                    Amount = 0,
                                    Status = 1
                                });
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return 1;
        }

        [HttpPost]
        public async Task<int> PostDeliveryTime(DeliveryTimeViewModel deliveryTimeViewModel)
        {
            try
            {
                if (deliveryTimeViewModel.PickupTime.GetValueOrDefault() < MyDateTime.Now
                    || deliveryTimeViewModel.DropOffTime.GetValueOrDefault() < MyDateTime.Now)
                    throw new BadRequestException("You cannot select previous Pickup or Dropoff Time");

                if (deliveryTimeViewModel.PickupTime.GetValueOrDefault() >= deliveryTimeViewModel.DropOffTime.GetValueOrDefault())
                    throw new BadRequestException("DropOff Time should be greater then Pickup Time!");

                var consignmentDelivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .FirstAsync(x => x.ConsignmentId == deliveryTimeViewModel.ConsignmentId);

                consignmentDelivery.Consignment.PlanedCollectionTime = consignmentDelivery.PlanedPickupTime = deliveryTimeViewModel.PickupTime;
                consignmentDelivery.Consignment.PlanedDeliveryTime = consignmentDelivery.PlanedDropTime = deliveryTimeViewModel.DropOffTime;
                consignmentDelivery.Consignment.DueTime = deliveryTimeViewModel.PickupTime.GetValueOrDefault();
                await shipmentsCacheService.SetShipment(consignmentDelivery.ConsignmentId, null);
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex?.Message + ex?.InnerException?.Message);
            }
        }

        [HttpGet]
        public async Task<DeliveryTimeViewModel> GetDeliveryTime(int consignmentId)
        {
            DeliveryTimeViewModel deliveryTime = null;
            var delivery = await context.ConsignmentDeliveries.FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId);
            if (delivery != null)
            {
                deliveryTime = new DeliveryTimeViewModel()
                {
                    ConsignmentId = consignmentId,
                    PickupTime = delivery.PlanedPickupTime,
                    DropOffTime = delivery.PlanedDropTime,
                };
            }
            return deliveryTime;
        }
        [HttpGet]
        public async Task<ShipmentCommentsViewModel> GetComments(int consignmentId)
        {
            ShipmentCommentsViewModel viewModel = new ShipmentCommentsViewModel()
            {
                ConsignmentId = consignmentId,
                ShipmentComments = new List<ShipmentComment>(),
            };
            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == consignmentId);
            if (!string.IsNullOrEmpty(consignment?.Comments))
            {
                try
                {
                    viewModel.ShipmentComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                }
                catch
                {
                    viewModel.ShipmentComments.Add(new ShipmentComment()
                    {
                        CreatedAt = consignment.CreatedAt,
                        CreatedBy = "---",
                        Description = consignment.Comments
                    });
                }
            }
            return viewModel;
        }
        [HttpPost]
        public async Task<int> PostComment(ShipmentCommentsViewModel viewModel)
        {
            try
            {
                var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == viewModel.ConsignmentId);

                if (consignment?.Comments != null)
                {
                    try
                    {
                        viewModel.ShipmentComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                    }
                    catch
                    {
                    }
                }

                if (viewModel.CommentText == null)
                    throw new BadRequestException("Please enter something in comment section to add!");

                viewModel.ShipmentComments.Add(new ShipmentComment()
                {
                    Description = viewModel.CommentText,
                    CreatedAt = MyDateTime.Now,
                    CreatedBy = User.Identity.Name + " ("+context.Users.Where(x=>x.UserName==User.Identity.Name).Select(x=>x.Name).FirstOrDefault()+") ",
                    ViewedAt = MyDateTime.Now,
                    ViewedBy = User.Identity.Name
                });

                consignment.Comments = JsonConvert.SerializeObject(viewModel.ShipmentComments);
                await context.SaveChangesAsync();
                await shipmentsCacheService.SetShipment(consignment.Id, null);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
            }
            return 1;
        }
        #endregion

        [HttpPost]
        public async Task<int> PostRatingCategories(RatingCategoriesViewModel categoriesViewModel)
        {
            try
            {

                List<Category> categories = await (from c in context.Categories select c).ToListAsync();

                ComplaintCategory complaintCategory = default;
                Common.Data.Models.ComplaintStatus complaintStatus = default;//await context.Complaints.FirstOrDefaultAsync(x => x.ConsignmentId == categoriesViewModel.ConsignmentId);
                List<string> categoryNames = new();

                if (categoriesViewModel.IsBadBehaviour)
                    categoryNames.Add("Bad Behaviour");
                if (categoriesViewModel.IsBadQuality)
                    categoryNames.Add("Bad Quality");
                if (categoriesViewModel.IsShipmentDelayed)
                    categoryNames.Add("Shipment Delayed");

                var complaint = await context.Complaints.FirstOrDefaultAsync(x => x.ConsignmentId == categoriesViewModel.ConsignmentId);
                if (complaint == default)
                {
                    complaint = new()
                    {
                        Id = context.Complaints.OrderByDescending(x => x.Id).FirstOrDefault()?.Id == default ? 1 : context.Complaints.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        ConsignmentId = categoriesViewModel.ConsignmentId,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name
                    };
                    context.Complaints.Add(complaint);
                }
                var complaints = await context.ComplaintCategories.Where(x => categories.Select(x => x.Id).Contains(x.CategoryId)
                && x.ComplaintId == complaint.Id).ToListAsync();
                if (complaints != default)
                {
                    context.ComplaintCategories.RemoveRange(complaints);
                    await context.SaveChangesAsync();
                }
                foreach (var categoryName in categoryNames)
                {
                    var category = categories.FirstOrDefault(x => x.Name == categoryName);
                    complaintCategory = context.ComplaintCategories.FirstOrDefault(x => x.CategoryId == category.Id && x.ComplaintId == complaint.Id);
                    if (complaintCategory == default)
                    {
                        complaintCategory = new()
                        {
                            CategoryId = category.Id,
                            ComplaintId = complaint.Id
                        };
                        context.ComplaintCategories.Add(complaintCategory);
                    }
                }
                complaintStatus = context.ComplaintStatuses.FirstOrDefault(x => x.ComplaintId == complaint.Id);
                if (complaintStatus == default)
                {
                    complaintStatus = new()
                    {
                        Id = context.ComplaintStatuses.OrderByDescending(x => x.Id).FirstOrDefault()?.Id == default ? 1 : context.ComplaintStatuses.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        ComplaintId = complaint.Id,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name,
                    };
                    context.ComplaintStatuses.Add(complaintStatus);
                }

                complaint.Description = categoriesViewModel.Description;
                complaintStatus.Comments = categoriesViewModel.Description;
                complaintStatus.Status = categoriesViewModel.Status;

                Consignment consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == categoriesViewModel.ConsignmentId);
                consignment.Rating = (byte)categoriesViewModel.RatingValue;
                await shipmentsCacheService.SetShipment(consignment.Id, null);
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }

        [HttpGet]
        public async Task<RatingCategoriesViewModel> GetRatingCategories(int consignmentId)
        {
            var ratings = await (from c in context.Complaints
                                 join co in context.Consignments on c.ConsignmentId equals co.Id
                                 join s in context.ComplaintStatuses on c.Id equals s.ComplaintId
                                 where c.ConsignmentId == consignmentId
                                 select new RatingCategoriesViewModel()
                                 {
                                     ConsignmentId = consignmentId,
                                     ComplaintId = c.Id,
                                     RatingValue = co.Rating,
                                     Description = c.Description,
                                     Status = s.Status
                                 }).FirstOrDefaultAsync();

            if (ratings != default)
            {
                var categories = await (from cc in context.ComplaintCategories
                                        join cat in context.Categories on cc.CategoryId equals cat.Id
                                        where cc.ComplaintId == ratings.ComplaintId
                                        select new RatingCategoriesViewModel()
                                        {
                                            IsShipmentDelayed = cat.Name == "Shipment Delayed",
                                            IsBadBehaviour = cat.Name == "Bad Behaviour",
                                            IsBadQuality = cat.Name == "Bad Quality"
                                        }).ToListAsync();
                foreach (var cat in categories)
                {
                    if (cat.IsBadBehaviour)
                        ratings.IsBadBehaviour = true;
                    if (cat.IsBadQuality)
                        ratings.IsBadQuality = true;
                    if (cat.IsShipmentDelayed)
                        ratings.IsShipmentDelayed = true;
                }
            }

            return ratings;
        }

        [HttpPost]
        public async Task<int> PostRatings(RatingControlViewModel ratingControl)
        {
            try
            {
                Consignment consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == ratingControl.ConsignmentId);
                consignment.Rating = (byte)ratingControl.RatingValue;
                await shipmentsCacheService.SetShipment(consignment.Id, null);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }

        public async Task<MixedCurrencyViewModel> GetMixCurrency(int consignmentId)
        {
            try
            {
                var mixCurrency = await context.Consignments.Where(x => x.Id == consignmentId).Select(x => new MixedCurrencyViewModel()
                {
                    ConsignmentId = consignmentId,
                    CurrencyType = x.CurrencySymbol.ToString(),
                    Description = x.Valueables,
                    AmountPKR = x.AmountPKR
                }).FirstOrDefaultAsync();

                return mixCurrency;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        [HttpPost]
        public async Task<int> UpdateMixCurrency(MixedCurrencyViewModel mixedCurrencyViewModel)
        {
            try
            {
                var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == mixedCurrencyViewModel.ConsignmentId);
                if (consignment != default)
                {
                    consignment.Valueables = mixedCurrencyViewModel.Description;
                    consignment.AmountPKR = consignment.Amount = mixedCurrencyViewModel.AmountPKR;

                    if (mixedCurrencyViewModel.Finalize)
                    {
                        consignment.IsFinalized = mixedCurrencyViewModel.Finalize;
                        consignment.FinalizedAt = MyDateTime.Now;
                        consignment.FinalizedBy = User.Identity.Name;
                    }
                    await shipmentsCacheService.SetShipment(consignment.Id, null);
                    await context.SaveChangesAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        [HttpGet]
        public async Task<TransitTimeViewModel> GetTransitTime(int consignmentId)
        {
            TransitTimeViewModel transitTimeViewModel = new()
            {
                ListOfTransitTime = new List<TransitTime>(),
                ConsignmentId = consignmentId,
                IsCrewAssigned = context.ConsignmentStates.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.CrewAssigned
                                  && x.Status == StateTypes.Confirmed
                                  && x.ConsignmentId == consignmentId).Any(),
            };

            var times = await (from c in context.Consignments
                               join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                               join cr in context.Parties on d.CrewId equals cr.Id
                               where c.Id == consignmentId
                               select new TransitTime()
                               {
                                   CrewId = cr.Id,
                                   CrewName = cr.FormalName,
                                   ActualPickupTime = d.ActualPickupTime,
                                   DeliveryState = d.DeliveryState,
                                   DeliveryId = d.Id,
                                   CollectionPoint = new Point((d.CollectionPoint).Y,
                                                            (d.CollectionPoint).X)
        }).ToListAsync();

            transitTimeViewModel.ListOfTransitTime.AddRange(times);
            
            //else
            //{
            //transitTimeViewModel.ListOfTransitTime.Add(new TransitTime()
            //{
            //    ActualDropoffTime = DateTime.Now,
            //    DeliveryState = Shared.Enums.ConsignmentState.InTransit,
            //    CrewId = 1,
            //    CrewName = "FirstCrew"
            //});
            //transitTimeViewModel.ListOfTransitTime.Add(new TransitTime()
            //{
            //    ActualDropoffTime = DateTime.Now.AddMinutes(3),
            //    DeliveryState = Shared.Enums.ConsignmentState.ReachedDestination,
            //    CrewId = 2,
            //    CrewName = "SecondCrew"
            //});
            //transitTimeViewModel.ListOfTransitTime.Add(new TransitTime()
            //{
            //    ActualDropoffTime = DateTime.Now.AddMinutes(5),
            //    DeliveryState = Shared.Enums.ConsignmentState.ReachedPickup,
            //    CrewId = 3,
            //    CrewName = "ThirdCrew"
            //});
            //}

            return transitTimeViewModel;
        }

        public Task<IndexViewModel<CrewMemberListModel>> GetCrewMembers(int crewId)
        {
            throw new NotImplementedException();
        }
    }
}
