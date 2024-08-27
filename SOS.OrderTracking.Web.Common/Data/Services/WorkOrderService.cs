using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Index.HPRtree;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Extensions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsignmentDeliveryState = SOS.OrderTracking.Web.Shared.Enums.ConsignmentDeliveryState;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class ConsignmentListQuery
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public DateTime? PlanedCollectionTime { get; set; }
        public string ShipmentCode { get; set; }
        public string ManualShipmentCode { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public ConsignmentDeliveryState ConsignmentStateType { get; set; }
        public ConsignmentApprovalState ApprovalState { get; set; }
        public DateTime DueTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsVault { get; set; }
        public bool IsClubbed { get; set; }
        public int? ToChangedPartyId { get; set; }
        public string Valueables { get; set; }

    }

    public class ConsignmentService
    {
        private readonly AppDbContext context;
        private readonly NotificationService notificationService;
        private readonly ILogger<ConsignmentService> Logger;
        private readonly PartiesCacheService partiesCache;
        private readonly IServiceScopeFactory scopeFactory;

        public ConsignmentService(IServiceScopeFactory? scopeFactory, AppDbContext context, NotificationService notificationService, ILogger<ConsignmentService> Logger, PartiesCacheService partiesCache)
        {
            this.context = context;
            this.notificationService = notificationService;
            this.Logger = Logger;
            this.partiesCache = partiesCache;
            this.scopeFactory = scopeFactory;
        }

        public IQueryable<ConsignmentListQuery> GetConsignmentsIds(int regionId, int subRegionId, int stationId, List<int> cIds, string createdBy, ConsignmentDeliveryState state, ShipmentType shipmentType, params ConsignmentApprovalState[] consignmentApprovalStates)
        {
            if (stationId > 0)
            {
                subRegionId = 0;
            }
            if (subRegionId > 0)
            {
                regionId = 0;
            }
            var approvalState = consignmentApprovalStates.First();
            var dueTime = MyDateTime.Now.AddHours(1);
            var query = (from c in context.Consignments
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         where (regionId == 0 || c.CollectionRegionId == regionId || c.DeliveryRegionId == regionId)
                         && (subRegionId == 0 || c.CollectionSubRegionId == subRegionId || c.DeliverySubRegionId == subRegionId)
                         && (stationId == 0 || c.CollectionStationId == stationId || c.DeliveryStationId == stationId)
                         && (cIds == null || cIds.Contains(c.FromPartyId) || cIds.Contains(c.ToPartyId) || cIds.Contains(c.BillBranchId) || c.CreatedBy == createdBy)
                         && (approvalState == ConsignmentApprovalState.All || consignmentApprovalStates.Contains(c.ApprovalState))
                         && (c.Type == ShipmentExecutionType.Live || c.Type == ShipmentExecutionType.Scheduled && c.DueTime > dueTime)
                         && (shipmentType == ShipmentType.Unknown || c.ShipmentType == shipmentType)
                         orderby c.DueTime descending
                         select new ConsignmentListQuery()
                         {
                             Id = c.Id,
                             ShipmentCode = c.ShipmentCode,
                             ManualShipmentCode = c.ManualShipmentCode,
                             CreatedAt = c.CreatedAt,
                             ConsignmentStateType = c.ConsignmentStateType,
                             ConsignmentStatus = c.ConsignmentStatus,
                             PlanedCollectionTime = c.PlanedCollectionTime,
                             DueTime = c.DueTime,
                             Rating = c.Rating,
                             IsVault = c.IsVault,
                             IsClubbed = (c.ConsignmentStateType == ConsignmentDeliveryState.InTransit && state == ConsignmentDeliveryState.Clubbed) ? (context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.Id && x.CrewId.GetValueOrDefault() != 0).Count() > 1) : false,
                             ToChangedPartyId = c.ToChangedPartyId,
                             ApprovalState = c.ApprovalState,
                             Valueables = c.Valueables
                         });

            return query;
        }

        public IQueryable<ConsignmentListViewModel> GetConsignmentsQuery(int regionId, int subRegionId, int stationId, List<int> customerId, string createdBy,
            ShipmentExecutionType executionType, ShipmentType shipmentType, params ConsignmentApprovalState[] consignmentTypes)
        {
            if (stationId > 0)
            {
                subRegionId = 0;
            }
            if (subRegionId > 0)
            {
                regionId = 0;
            }
            var ctype = consignmentTypes.First();

            var query = (from c in context.Consignments
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         from a in context.ShipmentAttachments.Where(x => x.ConsignmentId == c.Id).Select(x => x.Url).DefaultIfEmpty()
                         where (regionId == 0 || c.CollectionRegionId == regionId || c.DeliveryRegionId == regionId)
                         && (subRegionId == 0 || c.CollectionSubRegionId == subRegionId || c.DeliverySubRegionId == subRegionId)
                         && (stationId == 0 || c.CollectionStationId == stationId || c.DeliveryStationId == stationId)
                         && (customerId == null || customerId.Contains(c.FromPartyId) || customerId.Contains(c.ToPartyId) || customerId.Contains(c.BillBranchId) || c.CreatedBy == createdBy)
                         && (ctype == ConsignmentApprovalState.All || consignmentTypes.Contains(c.ApprovalState))
                         && (executionType == ShipmentExecutionType.All || c.Type == executionType)
                         && (shipmentType == ShipmentType.Unknown || c.ShipmentType == shipmentType)
                         orderby c.DueTime descending
                         select new ConsignmentListViewModel()
                         {
                             Id = c.Id,
                             CustomerId = c.MainCustomerId,
                             BillBranchId = c.BillBranchId,
                             OriginPartyId = c.OriginPartyId,
                             CounterPartyId = c.CounterPartyId,
                             CustomerName = s.FormalName,
                             CustomerCode = s.ShortName,
                             CustomerLogoUrl = s.ImageLink,
                             ShipmentCode = c.ShipmentCode,
                             ManualShipmentCode = c.ManualShipmentCode,
                             PostingMessages = c.PostingMessage,
                             ApprovalState = c.ApprovalState,
                             FromPartyId = c.FromPartyId,
                             CollectionStationId = c.CollectionStationId,
                             CollectionSubRegionId = c.CollectionSubRegionId,
                             CollectionRegionId = c.CollectionRegionId,
                             Valueables = c.Valueables,
                             DeliveryRegionId = c.DeliveryRegionId,
                             DeliverySubRegionId = c.DeliverySubRegionId,
                             DeliveryStationId = c.DeliveryStationId,
                             ExchangeRate = c.ExchangeRate,
                             ToPartyId = c.ToPartyId,
                             Type = c.Type,
                             Comments = c.Comments,
                             IsFinalized = c.IsFinalized,
                             CurrencySymbol = c.CurrencySymbol,
                             Amount = c.Amount,
                             AmountPKR = c.AmountPKR,
                             CreatedAt = c.CreatedAt,
                             CreatedBy = c.CreatedBy,
                             ApprovedBy = c.ApprovedBy,
                             Distance = c.Distance,
                             DistanceStatus = c.DistanceStatus,
                             ConsignmentStateType = c.ConsignmentStateType,
                             ConsignmentStatus = c.ConsignmentStatus,
                             ShipmentType = c.ShipmentType,
                             PlanedCollectionTime = c.PlanedCollectionTime,
                             PlanedDeliveryTime = c.PlanedDeliveryTime,
                             DueTime = c.DueTime,
                             SealCodes = context.ShipmentSealCodes.Where(x => x.ConsignmentId == c.Id).Select(x => x.SealCode).ToList(), //todo: move to cache
                             Rating = c.Rating,
                             ImageUrl=a,
                             Deliveries = c.ConsignmentDeliveries.Select(x =>
                             new DeliveryListViewModel
                             {
                                 Id = x.Id,
                                 ConsignmentId = x.ConsignmentId,
                                 CrewId = x.CrewId,
                                 //ConsignmentState = c.ConsignmentStateType,
                                 PickupCode = x.PickupCode,
                                 DropoffCode = x.DropoffCode,
                                 CollectionMode = x.CollectionMode,
                                 DeliveryMode = x.DeliveryMode,
                                 PlanedCollectionTime = x.PlanedPickupTime,
                                 PlanedDeliveryTime = x.PlanedDropTime,
                                 PickupFrom = "N/A",
                                 SerialNo = x.SerialNo,
                                 TemporalState = x.TemporalState,
                                 IsVault = x.IsVault,
                                 DeliveryState = x.DeliveryState,
                                 CollectionPoint_ = x.CollectionPoint,
                                 DeliveryPoint_ = x.DeliveryPoint,
                                 CollectionPointStatus = x.CollectionPointStatus,
                                 DeliveryPointStatus = x.DeliveryPointStatus,
                                 ParentId = x.ParentId,
                             }),

                             Denomination = c.Denominations.FirstOrDefault()
                                             .ToViewModel(c.ShipmentCode),
                             Charges = Array.Empty<DeliveryChargesModel>(),

                             //Charges = c.DeliveryCharges.Select(x => new DeliveryChargesModel
                             //{
                             //    Amount = x.Amount,
                             //    ChargeType = x.ChargeType.Name,
                             //    ChargeTypeId = x.ChargeTypeId,
                             //    ConsignmentId = x.ConsignmentId,
                             //    Tag = x.Tag,
                             //    Status = x.Status
                             //}).ToList(),

                             DeliveryStates = (from s in context.ConsignmentStates
                                               where s.ConsignmentId == c.Id
                                               select new ConsignmentStateViewModel
                                               {
                                                   ConsignmentStateType = s.ConsignmentStateType,
                                                   Status = s.Status,
                                                   TimeStamp = s.TimeStamp,
                                                   Tag = s.Tag,
                                                   ConsignmentId = c.Id
                                               }).ToList(),
                         });

            return query;
        }


        public ChangedDropOff GetChangedDropoffIfExists(int consignmentId)
        {
            return (from c in context.Consignments
                    join p in context.Parties on c.ToChangedPartyId equals p.Id
                    where c.Id == consignmentId
                    select new ChangedDropOff
                    {
                        ToPartyId = p.Id,
                        ToPartyCode = p.ShortName,
                        ToPartyName = p.FormalName,
                        IsApproved = c.IsToChangedPartyVerified
                    }).FirstOrDefault();

        }
        public decimal GetExchangeRate(int consignmentId)
        {
            return (from c in context.Consignments
                    where c.Id == consignmentId
                    select c.ExchangeRate).FirstOrDefault();

        }
        public IQueryable<ConsignmentListViewModel> GetRecurringConsignmentsQuery(int regionId, int subRegionId, int stationId, List<int> customerId, string createdBy,
            ShipmentExecutionType executionType, ShipmentType shipmentType, params ConsignmentApprovalState[] consignmentTypes)
        {
            var tempcontext = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

            if (stationId > 0)
            {
                subRegionId = 0;
            }
            if (subRegionId > 0)
            {
                regionId = 0;
            }
            var ctype = consignmentTypes.First();

            var query = (from c in tempcontext.Consignments
                         join s in tempcontext.Parties on c.MainCustomerId equals s.Id
                         where (regionId == 0 || c.CollectionRegionId == regionId || c.DeliveryRegionId == regionId)
                         && (subRegionId == 0 || c.CollectionSubRegionId == subRegionId || c.DeliverySubRegionId == subRegionId)
                         && (stationId == 0 || c.CollectionStationId == stationId || c.DeliveryStationId == stationId)
                         && (customerId == null || customerId.Contains(c.FromPartyId) || customerId.Contains(c.ToPartyId) || customerId.Contains(c.BillBranchId) || c.CreatedBy == createdBy)
                         && (ctype == ConsignmentApprovalState.All || consignmentTypes.Contains(c.ApprovalState))
                         && (executionType == ShipmentExecutionType.All || c.Type == executionType)
                         && (shipmentType == ShipmentType.Unknown || c.ShipmentType == shipmentType)
                         orderby c.DueTime descending
                         select new ConsignmentListViewModel()
                         {
                             Id = c.Id,
                             CreatedAt = c.CreatedAt,

                         });

            return query;
        }


        public static DateTime? GetValidTime(ScheduledConsignment sc, DayOfWeek dayOfWeek)
        {
            return sc?.DayOfWeek == dayOfWeek ? Convert.ToDateTime($"{sc?.Hour}:{sc?.Minute}") : null;
        }
        public async Task LogConsignmentState(int consignmentId, int deliveryId,
            ConsignmentDeliveryState stateType, string user)
        {
            var consignment = await context.Consignments.FindAsync(consignmentId);

            if (consignment == null)
                throw new Exception("Consignment information is missing");

            if (consignment.ConsignmentStateType >= ConsignmentDeliveryState.InTransit && stateType == ConsignmentDeliveryState.CrewAssigned)
                throw new Exception("Consignment crew cannot be changed when consignment is in transit.");


            var consignmentState = await context.ConsignmentStates.FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId
             && x.ConsignmentStateType == stateType && x.DeliveryId == deliveryId);

            if (consignmentState == null)
            {
                consignmentState = new Models.ConsignmentState()
                {
                    ConsignmentId = consignmentId,
                    DeliveryId = deliveryId,
                    ConsignmentStateType = stateType,
                    CreatedBy = user
                };
                context.ConsignmentStates.Add(consignmentState);
            }

            consignmentState.TimeStamp = MyDateTime.Now;
            consignmentState.Status = StateTypes.Confirmed;

            /// Need to workout this logic
            if (consignment.ConsignmentStateType == ConsignmentDeliveryState.ReachedPickup && stateType == ConsignmentDeliveryState.CrewAssigned)
            {
                var status2 = await context.ConsignmentStates.FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId && x.ConsignmentStateType == ConsignmentDeliveryState.ReachedPickup && x.DeliveryId == deliveryId);
                if (status2 != null)
                {
                    status2.TimeStamp = null;
                    status2.Status = StateTypes.Waiting;
                }
            }

            consignment.ConsignmentStateType = stateType;
            await context.SaveChangesAsync();

        }

        public async Task AssignCrewAsync(int deliveryId, int? crewId, bool sendNotification)
        {
            var delivery = await context.ConsignmentDeliveries.Include(x => x.Consignment).FirstOrDefaultAsync(x => x.Id == deliveryId);
            if (delivery == null)
                throw new Exception("Consignment information is missing");

            if (delivery.Consignment.ConsignmentStateType >= ConsignmentDeliveryState.InTransit)
                throw new Exception("Consignment crew cannot be changed after consignment is picked up by crew");

            if (delivery.CrewId == crewId)
                throw new Exception("You cannot re-assign shipment to same crew.");

            if (delivery.CrewId.GetValueOrDefault() > 0)
            {
                await notificationService.CreateFirebaseNotification(delivery.Id, delivery.ConsignmentId, delivery.CrewId,
                    delivery.Consignment.ShipmentCode, NotificationType.Cancel, NotificationCategory.CIT);
            }

            delivery.DeliveryState = ConsignmentDeliveryState.CrewAssigned;
            delivery.CrewId = crewId;
            if (sendNotification)
            {
                await notificationService.CreateFirebaseNotification(delivery.Id, delivery.ConsignmentId, delivery.CrewId,
                  delivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);
            }
            await context.SaveChangesAsync();

        }


        public async Task<ConsignmentListViewModel> GetShipment(int shipmentId)
        {
            try
            {
                var query = GetConsignmentsQuery(0, 0, 0, null, null, ShipmentExecutionType.All, ShipmentType.Unknown, ConsignmentApprovalState.All);

                var x = query.FirstOrDefault(x => x.Id == shipmentId);

                if (x == null)
                    throw new KeyNotFoundException();

                x.FromPartyCode = await partiesCache.GetCode(x.FromPartyId);
                x.FromPartyName = await partiesCache.GetName(x.FromPartyId);

                x.ToPartyCode = await partiesCache.GetCode(x.ToPartyId);
                x.ToPartyName = await partiesCache.GetName(x.ToPartyId);

                x.FromPartyStationName = await partiesCache.GetName(x.CollectionStationId);
                x.ToPartyStationName = await partiesCache.GetName(x.DeliveryStationId);
                if (x.BillBranchId != null)
                {
                    var BillingBranchCode = await partiesCache.GetCode(x.BillBranchId ?? 0);
                    var BillingBranchName = await partiesCache.GetName(x.BillBranchId);
                    x.BillingRegion = BillingBranchCode + "-" + BillingBranchName;
                }

                x.FromPartyGeolocation = await partiesCache.GetGeoCoordinate(x.FromPartyId);
                x.ToPartyGeolocation = await partiesCache.GetGeoCoordinate(x.ToPartyId);

                x.FromPartyGeoStatus = await partiesCache.GetGeoStatus(x.FromPartyId);
                x.ToPartyGeoStatus = await partiesCache.GetGeoStatus(x.ToPartyId);
                x.Denomination ??= new CitDenominationViewModel();
                x.Denomination.Valuables = x.Valueables;
                x.Denomination.ExchangeRate = x.ExchangeRate;
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

                    d.CrewName = await partiesCache.GetName(d.CrewId);
                    d.OrganizationType = await partiesCache.GetOrganizationType(d.CrewId);

                    if (d.OrganizationType == OrganizationType.Unknown)
                    {
                        d.OrganizationType = context.Orgnizations.FirstOrDefault(x => x.Id == d.CrewId)?.OrganizationType;
                        await partiesCache.SetOrganizationType(d.CrewId, d.OrganizationType);
                    }
                }
                return x;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
