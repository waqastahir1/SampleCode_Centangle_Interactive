using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Maps;
using Google.Maps.DistanceMatrix;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.GBMS;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using static Google.Maps.DistanceMatrix.DistanceMatrixResponse;
using static SOS.OrderTracking.Web.Shared.Constants.Roles;
using ConsignmentState = SOS.OrderTracking.Web.Common.Data.Models.ConsignmentState;
using Location = SOS.OrderTracking.Web.Common.Data.Models.Location;
using Point = SOS.OrderTracking.Web.Shared.ViewModels.Point;
namespace SOS.OrderTracking.Web.Server.Controllers.Operations
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ShipmentSchedulesController : ControllerBase, IShipmentSchedulesService
    {
        private readonly ConsignmentService workOrderService;

        private readonly IHubContext<ConsignmentHub> consignmentHub; 
        private readonly NotificationService notificationService;
        private readonly SequenceService sequenceService;
        private readonly PartiesCacheService cache;
        private readonly PartiesCacheService partiesCache;
        private readonly AppDbContext context;
        private readonly ILogger<ShipmentSchedulesController> logger;
        public ShipmentSchedulesController(AppDbContext context,
           ConsignmentService workOrderService,
           IHubContext<ConsignmentHub> consignmentHub, 
          NotificationService notificationService,
          ILogger<ShipmentSchedulesController> logger,
          SequenceService sequenceService,
          PartiesCacheService distributedCache,
          PartiesCacheService partiesCache
            )
        {
            this.workOrderService = workOrderService;
            this.consignmentHub = consignmentHub; 
            this.notificationService = notificationService;
            this.logger = logger;
            this.sequenceService = sequenceService;
            cache = distributedCache;
            this.partiesCache = partiesCache;
            this.context = context;
        }

        #region Crud Methods

        public async Task<IndexViewModel<ShipmentScheduleListViewModel>> GetPageAsync([FromQuery] BaseIndexModel vm)
        {
            try
            {
                List<int> customers = null;
                if (User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN))
                {

                }
                else if (User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) || User.IsInRole(BANK_BRANCH_MANAGER))
                {
                    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    customers = new List<int>() { user.PartyId };
                } 
                else if (User.IsInRole(BANK))
                {
                    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    customers = await context.PartyRelationships.Where(x => x.ToPartyId == user.PartyId && x.FromPartyRole == RoleType.ChildOrganization)
                        .Select(x => x.FromPartyId).ToListAsync();
                }
                var consignmentType = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_BRANCH_MANAGER) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;

                var query = workOrderService.GetConsignmentsQuery(vm.RegionId.GetValueOrDefault(), vm.SubRegionId.GetValueOrDefault(),
                vm.StationId.GetValueOrDefault(), customers, User.Identity.Name, ShipmentExecutionType.Recurring, ConsignmentApprovalState.Approved,  ConsignmentApprovalState.ReApprove, consignmentType);


               
                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage)
                    .Take(vm.RowsPerPage).ToListAsync();

                var scheduledConsignments = context.ScheduledConsignments.Where(x => items.Select(p => p.Id).Contains(x.ConsignmentId));

                List<ShipmentScheduleListViewModel> item2 = new List<ShipmentScheduleListViewModel>();

                foreach (var c in items)
                {
                    c.FromPartyCode = await cache.GetCode(c.FromPartyId);
                    c.FromPartyName = await cache.GetName(c.FromPartyId);

                    c.ToPartyCode = await cache.GetCode(c.ToPartyId);
                    c.ToPartyName = await cache.GetName(c.ToPartyId);

                    c.FromPartyStationName = await cache.GetName(c.CollectionStationId);
                    c.ToPartyStationName = await cache.GetName(c.DeliveryStationId);
                    c.BillingRegion = await cache.GetName(c.BillBranchId);

                    c.FromPartyGeolocation = await cache.GetGeoCoordinate(c.FromPartyId);
                    c.ToPartyGeolocation = await cache.GetGeoCoordinate(c.ToPartyId);

                    c.FromPartyGeoStatus = await cache.GetGeoStatus(c.FromPartyId);
                    c.ToPartyGeoStatus = await cache.GetGeoStatus(c.ToPartyId);
                    c.Denomination ??= new CitDenominationViewModel();

                    var r = JsonConvert.DeserializeObject<ShipmentScheduleListViewModel>(JsonConvert.SerializeObject(c));

                    r.ScheduleViewModel = new ScheduleViewModel()
                    {
                        MondayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Monday)?.ScheduleStatus == ScheduleStatus.Enable,
                        MondayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Monday), DayOfWeek.Monday),
                        TuesdayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Tuesday)?.ScheduleStatus == ScheduleStatus.Enable,
                        TuesdayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Tuesday), DayOfWeek.Tuesday),
                        WednesdayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Wednesday)?.ScheduleStatus == ScheduleStatus.Enable,
                        WednesdayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Wednesday), DayOfWeek.Wednesday),
                        ThursdayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Thursday)?.ScheduleStatus == ScheduleStatus.Enable,
                        ThursdayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Thursday), DayOfWeek.Thursday),
                        FridayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Friday)?.ScheduleStatus == ScheduleStatus.Enable,
                        FridayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Friday), DayOfWeek.Friday),
                        SaturdayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Saturday)?.ScheduleStatus == ScheduleStatus.Enable,
                        SaturdayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Saturday), DayOfWeek.Saturday),
                        SundayScheduleExist = scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Sunday)?.ScheduleStatus == ScheduleStatus.Enable,
                        SundayTime = GetValidTime(scheduledConsignments.FirstOrDefault(x => x.ConsignmentId == c.Id && x.DayOfWeek == DayOfWeek.Sunday), DayOfWeek.Sunday),
                    };

                    item2.Add(r);
                }

                logger.LogInformation($"totalRows-> {totalRows}, currentRows-> {items.Count}");
                return new IndexViewModel<ShipmentScheduleListViewModel>(item2, totalRows);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
         
        [HttpGet]
        public async Task<ShipmentScheduleFormViewModel> GetAsync(int id)
        {
            var order = await (from c in context.Consignments
                               join f in context.Parties on c.FromPartyId equals f.Id
                               join t in context.Parties on c.ToPartyId equals t.Id
                               join b in context.Parties on c.BillBranchId equals b.Id
                               where c.Id == id
                               select new ShipmentScheduleFormViewModel()
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
                                   ConsignmentStatus = c.ConsignmentStatus
                               }).FirstAsync();
            order.Comments = order.Comments != null ? JsonConvert.DeserializeObject<List<ShipmentComment>>(order.Comments).FirstOrDefault().Description : null;
            return order;
        }

        public async Task<int> PostAsync([FromBody] ShipmentScheduleFormViewModel selectedItem)
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
                    consignment = new Consignment()
                    {
                        Id = newConsignmentSequence,
                        ShipmentCode = $"CIT/{MyDateTime.Now.Year}/" + newConsignmentSequence.ToString("D4"),
                        ManualShipmentCode = selectedItem.ManualShipmentCode,
                        ApprovalState =   ConsignmentApprovalState.Approved,
                        Type = ShipmentExecutionType.Recurring,
                        CreatedAt = MyDateTime.Now,
                        ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                        CreatedBy = User.Identity.Name,
                        ServiceType = ServiceType.Unknown,
                        ShipmentType = ShipmentType.Unknown,
                        ConsignmentStatus = ConsignmentStatus.TobePosted
                    };

                    context.Consignments.Add(consignment);

                    //linking Denomination with Consignment for preserving later
                    Denomination denom = new Denomination
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
                        PlanedPickupTime = MyDateTime.Now,
                        PlanedDropTime = MyDateTime.Now.AddHours(1),
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
                try
                {
                    var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                    consignment.Distance = distanceInfo.Item1;
                    consignment.DistanceStatus = distanceInfo.Item2;

                }
                catch { }

                await context.SaveChangesAsync();

                await PublishUpdates(selectedItem.Id > 0 ? "UPDATE" : "NEW", consignment);

                if (consignment.ApprovalState.HasFlag(ConsignmentApprovalState.Draft))
                {
                    var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                    notificationIds.ForEach(x => NotificationAgent.WebPushNotificationsQueue.Add(x));
                }
                else
                {
                    var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);
                }
                if (selectedItem.Id > 0)
                { 
                   await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                }
                return consignment.Id;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
            }
        }
        #endregion


        #region Extra Methods
        public async Task<int> PostConsignmentDelivery([FromBody] DeliveryFormViewModel deliveryFormViewModel)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Location location = null;

            if (deliveryFormViewModel.LocationId == null)
            {
                if (!string.IsNullOrEmpty(deliveryFormViewModel.LocationName) || deliveryFormViewModel.Latitude > 0 || deliveryFormViewModel.Longitude > 0)
                {
                    location = await context.Locations.FirstOrDefaultAsync(x => x.Name.Equals(deliveryFormViewModel.LocationName));
                    if (location != null)
                        throw new BadRequestException("This location is already exist!");
                    if (deliveryFormViewModel.Latitude <= 0 || deliveryFormViewModel.Longitude <= 0)
                    {
                        throw new BadRequestException("Please provide correct values for Latitued/Longitude");
                    }
                    location = new Location()
                    {
                        Id = sequenceService.GetNextPartiesSequence(),
                        Name = deliveryFormViewModel.LocationName,
                        Description = deliveryFormViewModel.LocationName,
                        Geolocation = geometryFactory.CreatePoint(new Coordinate(deliveryFormViewModel.Longitude, deliveryFormViewModel.Latitude)),
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
            var lastDelivery = await context.ConsignmentDeliveries
                .Include(x => x.Consignment)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.ConsignmentId == deliveryFormViewModel.ConsignmentId);

            if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                throw new BadRequestException("You need to assign this consignment to one of crews before including additional crews");

            //   var dummyLocation = context.Locations.First();
            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var newDelivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                ParentId = lastDelivery.Id,
                ConsignmentId = deliveryFormViewModel.ConsignmentId,
                CrewId = deliveryFormViewModel.CrewId.GetValueOrDefault(),
                DestinationLocationId = location.Id,
                PickupLocationId = location.Id,
                FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                ToPartyId = lastDelivery.ToPartyId,
                PlanedPickupTime = deliveryFormViewModel.PlanedPickupTime.GetValueOrDefault(),
                PlanedDropTime = lastDelivery.PlanedDropTime,
                PickupCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Handover",
                DropoffCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Dropoff",
                DeliveryState = Shared.Enums.ConsignmentDeliveryState.CrewAssigned
            };

            context.ConsignmentDeliveries.Add(newDelivery);

            lastDelivery.ToPartyId = deliveryFormViewModel.CrewId.GetValueOrDefault();
            lastDelivery.DestinationLocationId = location.Id;
            lastDelivery.PlanedDropTime = deliveryFormViewModel.PlanedPickupTime.GetValueOrDefault();
            lastDelivery.DropoffCode = newDelivery.PickupCode;

            await context.SaveChangesAsync();
            await PublishUpdates();
            await notificationService.CreateFirebaseNotification(lastDelivery.Id, lastDelivery.ConsignmentId, lastDelivery.CrewId,
                lastDelivery.Consignment.ShipmentCode, NotificationType.UpdatedDropoff, NotificationCategory.CIT);

            await notificationService.CreateFirebaseNotification(newDelivery.Id, newDelivery.ConsignmentId, newDelivery.CrewId,
                newDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);
            return deliveryId;
        }

        [HttpPost]
        public async Task<int> AssignCrew([FromBody] DeliveryCrewFormViewModel selectedItem)
        {
            using (var trx = await context.Database.BeginTransactionAsync())
            {
                try
                {

                    await workOrderService.AssignCrewAsync(selectedItem.DeliveryId, selectedItem.CrewId, false);
                    await workOrderService.LogConsignmentState(selectedItem.ConsignmenId, Shared.Enums.ConsignmentDeliveryState.CrewAssigned, User.Identity.Name);
                    await trx.CommitAsync();

                    await PublishUpdates("", null);

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
                               }).FirstAsync();
            return denom;
        }

        public async Task<IActionResult> PublishUpdates(string message = "", Consignment consignment = null)
        {
            if (consignment != null)
            {
                var users = context.Users.Where(x => x.PartyId == consignment.FromPartyId
                 || x.PartyId == consignment.ToPartyId).Select(x => x.UserName).ToList();

                foreach (var user in users.Where(x => x != User.Identity.Name))
                {
                    await consignmentHub.Clients.All.SendAsync("RefreshCITConsignments", user, message);
                }
            }
            await consignmentHub.Clients.All.SendAsync("RefreshCITConsignments", "", "");
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
                    party.Orgnization.LocationStatus = vm.LocationStatus;

                    if (vm.Latitude > 0 && vm.Longitude > 0)
                    {
                        if (party.Orgnization.Geolocation?.X != vm.Longitude.Value || party.Orgnization.Geolocation?.Y != vm.Latitude.Value)
                        {
                            party.Orgnization.Geolocation = geometryFactory.CreatePoint(new Coordinate(vm.Longitude.Value, vm.Latitude.Value));
                            party.Orgnization.GeolocationUpdateAt = DateTime.Now;
                            party.Orgnization.GeolocationVersion++;

                            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE IntraPartyDistances SET DistanceStatus = '0' WHERE DistanceStatus = '1' AND (FromPartyId = {party.Id} OR ToPartyId = {party.Id})");
                            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND (FromPartyId = {party.Id} OR ToPartyId = {party.Id})");

                        }
                    }
                    await context.SaveChangesAsync();
                    await cache.SetGeoStatus(party.Orgnization.Id, vm.LocationStatus);
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
                               where r.FromPartyRole == RoleType.Crew || r.FromPartyRole == RoleType.Vault
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
                               select new SelectListItem(o.Id, p.ShortName, p.FormalName, "-")).Distinct()
             .ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.ToString() + ex.InnerException?.ToString());
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
                .OrderByDescending(x => x.DistanceStatus)
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
            return new Tuple<double, DataRecordStatus>(intraPartyDistance.Distance, intraPartyDistance.DistanceStatus);
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


        [HttpGet]
        public Task<ScheduleViewModel> GetSchedule(int consignmentId)
        {
            ScheduleViewModel schedule1 = new()
            {
                ConsignmentId = consignmentId,
            };
            var schedules = context.ScheduledConsignments.Where(x => x.ConsignmentId == consignmentId);
            if (schedules != null)
            {
                foreach (var schedule in schedules)
                {
                    if (schedule.DayOfWeek == DayOfWeek.Monday)
                    {
                        schedule1.MondayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.MondayTime = GetValidTime(schedule, DayOfWeek.Monday);
                    }
                    if (schedule.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        schedule1.TuesdayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.TuesdayTime = GetValidTime(schedule, DayOfWeek.Tuesday);
                    }
                    if (schedule.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        schedule1.WednesdayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.WednesdayTime = GetValidTime(schedule, DayOfWeek.Wednesday);
                    }
                    if (schedule.DayOfWeek == DayOfWeek.Thursday)
                    {
                        schedule1.ThursdayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.ThursdayTime = GetValidTime(schedule, DayOfWeek.Thursday);
                    }
                    if (schedule.DayOfWeek == DayOfWeek.Friday)
                    {
                        schedule1.FridayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.FridayTime = GetValidTime(schedule, DayOfWeek.Friday);
                    }
                    if (schedule.DayOfWeek == DayOfWeek.Saturday)
                    {
                        schedule1.SaturdayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.SaturdayTime = GetValidTime(schedule, DayOfWeek.Saturday);
                    }
                    if (schedule.DayOfWeek == DayOfWeek.Sunday)
                    {
                        schedule1.SundayScheduleExist = schedule.ScheduleStatus == ScheduleStatus.Enable;
                        schedule1.SundayTime = GetValidTime(schedule, DayOfWeek.Sunday);
                    }
                }
            }
            return Task.FromResult(schedule1);
        }
        public static DateTime? GetValidTime(ScheduledConsignment sc, DayOfWeek dayOfWeek)
        {
            return sc?.DayOfWeek == dayOfWeek ? Convert.ToDateTime($"{sc?.Hour}:{sc?.Minute}") : null;
        }
        [HttpPost]
        public async Task<int> SaveSchedule(ScheduleViewModel viewModel)
        {
            try
            {
                if (viewModel.MondayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Monday, viewModel.MondayScheduleExist, viewModel.MondayTime, viewModel.ConsignmentId);
                }
                if (viewModel.TuesdayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Tuesday, viewModel.TuesdayScheduleExist, viewModel.TuesdayTime, viewModel.ConsignmentId);
                }
                if (viewModel.WednesdayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Wednesday, viewModel.WednesdayScheduleExist, viewModel.WednesdayTime, viewModel.ConsignmentId);
                }
                if (viewModel.ThursdayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Thursday, viewModel.ThursdayScheduleExist, viewModel.ThursdayTime, viewModel.ConsignmentId);
                }
                if (viewModel.FridayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Friday, viewModel.FridayScheduleExist, viewModel.FridayTime, viewModel.ConsignmentId);
                }
                if (viewModel.SaturdayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Saturday, viewModel.SaturdayScheduleExist, viewModel.SaturdayTime, viewModel.ConsignmentId);
                }
                if (viewModel.SundayScheduleExist)
                {
                    await AddSchedule(DayOfWeek.Sunday, viewModel.SundayScheduleExist, viewModel.SundayTime, viewModel.ConsignmentId);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }
        public async Task AddSchedule(DayOfWeek dayOfWeek, bool scheduleExist, DateTime? Time, int consignmentId)
        {
            var schedule = await context.ScheduledConsignments.FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId && x.DayOfWeek == dayOfWeek);
            if (schedule == null)
            {
                schedule = new ScheduledConsignment()
                {
                    ConsignmentId = consignmentId,
                    DayOfWeek = dayOfWeek,

                };
                context.ScheduledConsignments.Add(schedule);
            }
            schedule.ScheduleStatus = scheduleExist ? ScheduleStatus.Enable : ScheduleStatus.Disable;
            schedule.Hour = (byte)Time.GetValueOrDefault().TimeOfDay.Hours;
            schedule.Minute = (byte)Time.GetValueOrDefault().TimeOfDay.Minutes;
            schedule.UpdatedAt = DateTime.Now;
            schedule.UpdatedBy = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            await context.SaveChangesAsync();
        }
        [HttpPost]
        public async Task<int> PostConsignmentStatus(ConsignmentStatusViewModel ConsignmentStatusViewModel)
        {
            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == ConsignmentStatusViewModel.ConsignmentId);

            try
            {
                consignment.ConsignmentStatus = ConsignmentStatusViewModel.ConsignmentStatus;
                List<ShipmentComment> listOfComments = new();
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                if (ConsignmentStatusViewModel.Comments != null)
                {
                    listOfComments.Add(new ShipmentComment()
                    {
                        Description = ConsignmentStatusViewModel.Comments + "," + ConsignmentStatusViewModel.ConsignmentStatus,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ViewedAt = MyDateTime.Now,
                        ViewedBy = User.Identity.Name
                    });


                    consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                }
                context.Consignments.Update(consignment);
                await context.SaveChangesAsync();

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

            try
            {
                consignment.ApprovalState = ConsignmentStatusViewModel.ApprovalState;
                //  consignment.Comments = ConsignmentStatusViewModel.Comments;
                List<ShipmentComment> listOfComments = new List<ShipmentComment>();
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                if (ConsignmentStatusViewModel.Comments != null)
                {
                    listOfComments.Add(new ShipmentComment()
                    {
                        Description = ConsignmentStatusViewModel.Comments,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ViewedAt = MyDateTime.Now,
                        ViewedBy = User.Identity.Name
                    });


                    consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                }
                context.Consignments.Update(consignment);
                await context.SaveChangesAsync();

                var notificationId1 = await notificationService.CreateShipmentApprovedNotification(consignment.Id, NotificationType.ShipmentApproved, consignment.ShipmentCode);
                NotificationAgent.WebPushNotificationsQueue.Add(notificationId1);

                var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);
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

            distance.Distance = model.Distance * 1000;
            distance.DistanceStatus = model.DistanceStatus;
            distance.UpdateAt = DateTime.Now;
            distance.UpdatedBy = User.Identity.Name; 

            await context.SaveChangesAsync();
            var distanceUpdateResult = new DistanceUpdateResult();
            distanceUpdateResult.Repushed = await context.Consignments.FromSqlInterpolated($"Select * FROM Consignments WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))")
                .Select(x => new SelectListItem(x.Id, x.ShipmentCode)).ToListAsync();

            distanceUpdateResult.Updated = await context.Consignments.FromSqlInterpolated($"Select * FROM Consignments WHERE (ConsignmentStatus = '0' OR ConsignmentStatus = '3' OR ConsignmentStatus = '5') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))")
                .Select(x => new SelectListItem(x.Id, x.ShipmentCode)).ToListAsync();

            var v1 = await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))");
            var v2 = await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET Distance = {model.Distance * 1000}, DistanceStatus = {(byte)model.DistanceStatus} WHERE (ConsignmentStatus = '0' OR ConsignmentStatus = '3' OR ConsignmentStatus = '5') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))");

            return distanceUpdateResult;

        }

        public Task<int> FinalizeShipment(ShipmentAdministrationViewModel model)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
