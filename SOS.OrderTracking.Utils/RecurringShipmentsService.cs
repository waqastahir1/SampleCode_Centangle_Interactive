using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SOS.OrderTracking.Utils
{
    public class RecurringShipmentsService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;
        private Timer timer;
        public RecurringShipmentsService(IServiceScopeFactory serviceScopeFactory, Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }


        public void Start()
        {
            TimerCallback timerCallback = new(async (o) =>
            {
                logger.Debug("schedule service running {0}", DateTime.Now.ToLongTimeString());

                try
                {

                    using (var scope = serviceScopeFactory.CreateScope())
                    {

                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var d = MyDateTime.Now.AddHours(1);

                        var query = context.ScheduledConsignments
                             .Where(x => d.DayOfWeek == x.DayOfWeek && d.Hour == x.Hour && d.Minute == x.Minute && x.ScheduleStatus == ScheduleStatus.Enable && x.FkConsignment.Type == ShipmentExecutionType.Recurring);

                        var schedules = query.ToArray();

                        foreach (var schedule in schedules)
                        {
                            int consignmentId = context.Sequences.GetNextCitOrdersSequence();
                            var dummyLocation = context.Locations.First();
                            var templateConsignment = await context.Consignments.Where(x => x.ConsignmentStatus != ConsignmentStatus.Cancelled || x.ConsignmentStatus != ConsignmentStatus.Declined)
                                .Include(x => x.Denominations)
                                .FirstAsync(x => x.Id == schedule.ConsignmentId);



                            if (templateConsignment != null)
                            {
                                logger.Information("creating consingment...");
                                List<ShipmentComment> listOfComments = new();
                                listOfComments.Add(new ShipmentComment()
                                {
                                    Description = "Auto created (Recurring)",
                                    CreatedAt = MyDateTime.Now,
                                    CreatedBy = templateConsignment.CreatedBy,
                                    ViewedAt = MyDateTime.Now,
                                    ViewedBy = templateConsignment.CreatedBy
                                });

                                var consignmentNew = new Consignment
                                {
                                    Id = consignmentId,
                                    CreatedAt = MyDateTime.Now,
                                    ShipmentCode = $"CIT/{MyDateTime.Now.Year}/" + consignmentId.ToString("D4"),
                                    Comments = JsonConvert.SerializeObject(listOfComments),
                                    Amount = templateConsignment.Amount,
                                    AmountPKR = templateConsignment.AmountPKR,
                                    BillBranchId = templateConsignment.BillBranchId,
                                    ConsignmentStateType = templateConsignment.ConsignmentStateType,
                                    CreatedBy = templateConsignment.CreatedBy,
                                    ConsignmentStatus = templateConsignment.ConsignmentStatus,
                                    CustomerId = templateConsignment.CustomerId,
                                    CurrencySymbol = templateConsignment.CurrencySymbol,
                                    Distance = templateConsignment.Distance,
                                    FromPartyId = templateConsignment.FromPartyId,
                                    DistanceStatus = templateConsignment.DistanceStatus,
                                    ExchangeRate = templateConsignment.ExchangeRate,
                                    MainCustomerId = templateConsignment.MainCustomerId,
                                    Type = ShipmentExecutionType.Live,
                                    ServiceType = templateConsignment.ServiceType,
                                    ToPartyId = templateConsignment.ToPartyId,
                                    ShipmentType = templateConsignment.ShipmentType,
                                    Valueables = templateConsignment.Valueables,

                                    BillingRegionId = templateConsignment.BillingRegionId,
                                    BillingSubRegionId = templateConsignment.BillingSubRegionId,
                                    BillingStationId = templateConsignment.BillingStationId,

                                    CollectionRegionId = templateConsignment.CollectionRegionId,
                                    CollectionSubRegionId = templateConsignment.CollectionSubRegionId,
                                    CollectionStationId = templateConsignment.CollectionStationId,

                                    DeliveryRegionId = templateConsignment.DeliveryRegionId,
                                    DeliverySubRegionId = templateConsignment.DeliverySubRegionId,
                                    DeliveryStationId = templateConsignment.DeliveryStationId,

                                    CounterPartyId = templateConsignment.CounterPartyId,
                                    OriginPartyId = templateConsignment.OriginPartyId,

                                    ApprovalState = templateConsignment.ApprovalState,
                                    ShipmentApprovalMode = templateConsignment.ShipmentApprovalMode,
                                    ApprovedAt = templateConsignment.ApprovedAt,
                                    ApprovedBy = templateConsignment.ApprovedBy,
                                    EscalationStatus = templateConsignment.EscalationStatus,
                                    SealedBags = templateConsignment.SealedBags,
                                    DueTime = MyDateTime.Now,
                                    //PlanedCollectionTime = d
                                };

                                context.Consignments.Add(consignmentNew);

                                var dinomination = await context.Denominations.FirstAsync(x => x.ConsignmentId == templateConsignment.Id);
                                var denominationNew = new Denomination
                                {
                                    ConsignmentId = consignmentId,
                                    Id = context.Sequences.GetNextDenominationSequence(),
                                    Currency1000x = dinomination.Currency1000x,
                                    Currency100x = dinomination.Currency100x,
                                    Currency10x = dinomination.Currency10x,
                                    Currency1x = dinomination.Currency1x,
                                    Currency20x = dinomination.Currency20x,
                                    Currency2x = dinomination.Currency2x,
                                    Currency5000x = dinomination.Currency5000x,
                                    Currency500x = dinomination.Currency500x,
                                    Currency50x = dinomination.Currency50x,
                                    Currency75x = dinomination.Currency75x,
                                    Currency5x = dinomination.Currency5x,
                                    DenominationType = dinomination.DenominationType
                                };
                                context.Denominations.Add(denominationNew);

                                var Templatedeliveries = await context.ConsignmentDeliveries.Where(x => x.ConsignmentId == templateConsignment.Id)
                                .ToListAsync();
                                List<int> deliveryIds = new List<int>();
                                foreach (var delivery in Templatedeliveries)
                                {
                                    int deliveryId = context.Sequences.GetNextDeliverySequence();
                                    deliveryIds.Add(deliveryId);
                                    var consignmentDelivery = new ConsignmentDelivery
                                    {
                                        FromPartyId = delivery.FromPartyId,
                                        ToPartyId = delivery.ToPartyId,
                                        DestinationLocationId = delivery.DestinationLocationId,
                                        PickupLocationId = delivery.PickupLocationId,
                                        ConsignmentId = consignmentId,
                                        Id = deliveryId,
                                        //PlanedPickupTime = MyDateTime.Now.AddHours(1),
                                        //PlanedDropTime = MyDateTime.Now.AddHours(2),
                                        PickupCode = $"{consignmentNew.ShipmentCode}{deliveryId}-Pickup",
                                        DropoffCode = $"{consignmentNew.ShipmentCode}{deliveryId}-Dropoff",
                                        CrewId = delivery.CrewId,
                                        DeliveryState = delivery.DeliveryState,

                                    };
                                    context.ConsignmentDeliveries.Add(consignmentDelivery);
                                }

                                context.ConsignmentStates.Add(new ConsignmentState
                                {
                                    ConsignmentId = consignmentId,
                                    DeliveryId = deliveryIds.FirstOrDefault(),
                                    ConsignmentStateType = ConsignmentDeliveryState.Created,
                                    TimeStamp = MyDateTime.Now,
                                    Status = StateTypes.Confirmed
                                });
                                if (Templatedeliveries.Any(x => x.DeliveryState == ConsignmentDeliveryState.CrewAssigned))
                                    //var states = Enum.GetValues(typeof(ConsignmentDeliveryState));

                                    context.ConsignmentStates.Add(new ConsignmentState
                                    {
                                        ConsignmentId = consignmentId,
                                        DeliveryId = deliveryIds.FirstOrDefault(),
                                        ConsignmentStateType = ConsignmentDeliveryState.CrewAssigned,
                                        TimeStamp = MyDateTime.Now,
                                        Status = StateTypes.Confirmed
                                    });
                                foreach (var item in context.ShipmentCharges.Where(x => x.ConsignmentId == templateConsignment.Id).ToList())
                                {
                                    context.ShipmentCharges.Add(new ShipmentCharge()
                                    {
                                        ChargeTypeId = item.ChargeTypeId,
                                        ConsignmentId = consignmentId,
                                        Amount = item.Amount,
                                        Status = item.Status
                                    });
                                }
                                await context.SaveChangesAsync();
                                logger.Information("Recurring Consignment {0} Created Successfully for {1}", consignmentNew.Id, templateConsignment.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            });
            timer = new Timer(timerCallback, null, 0, 60_000);
        }
    }
}
