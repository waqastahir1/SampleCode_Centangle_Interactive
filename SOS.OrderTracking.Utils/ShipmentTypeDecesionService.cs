using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    class ShipmentTypeDecesionService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        public ShipmentTypeDecesionService(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public void Start()
        {
            var minDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var gbms = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var shipmentsCache = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                        var partiesCache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();
                        Consignment consignment = null;
                        try
                        {
                            var shipmentQuery = (from c in context.Consignments
                                                 join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                                                 join crew in context.Parties on d.CrewId equals crew.Id
                                                 from v in context.AssetAllocations.Where(x => x.PartyId == crew.Id).DefaultIfEmpty()
                                                 join f in context.Orgnizations on c.FromPartyId equals f.Id
                                                 join t in context.Orgnizations on c.ToPartyId equals t.Id
                                                 join b in context.Orgnizations on c.BillBranchId equals b.Id
                                                 where
                                                 //c.Id == 1000422364 &&
                                                 d.CrewId > 0
                                                 && c.ConsignmentStateType >= ConsignmentDeliveryState.CrewAssigned
                                                 && c.ShipmentType == ShipmentType.Unknown
                                                 && c.PostingMessage == null
                                                 && c.ConsignmentStatus == ConsignmentStatus.TobePosted
                                                 && c.Distance > 0
                                                 && ((c.CurrencySymbol == CurrencySymbol.PKR && c.Amount > 0) || (c.AmountPKR > 0))
                                                 && c.DueTime > minDate
                                                 select new
                                                 {
                                                     c.Id,
                                                     d.CrewId,
                                                     c.BillingRegionId,
                                                     c.BillingSubRegionId,
                                                     c.BillingStationId,
                                                     c.BillBranchId,
                                                     c.BillBranch.ShortName,
                                                     c.BillBranch.FormalName,
                                                     c.BillBranch.ParentCode,
                                                     c.FromPartyId,
                                                     c.ToPartyId,
                                                     Distance = (decimal)c.Distance / 1000,
                                                     AssetId = v != null ? v.AssetId : 0,
                                                     c.DueTime,
                                                     Obj = c,
                                                     CollectionType = f.OrganizationType,
                                                     DeliveryType = t.OrganizationType,
                                                     BillBranchType = b.OrganizationType,
                                                     AtmCitBillFrom = f.AtmCitBill,
                                                     AtmCitBillTo = t.AtmCitBill
                                                 });

                            var shipment = await shipmentQuery.FirstOrDefaultAsync();

                            if (shipment == null)
                            {
                                Console.WriteLine("Sleeping");
                                await Task.Delay(TimeSpan.FromMinutes(2));
                                continue;
                            }

                            consignment = shipment.Obj;

                            var billingRegion = await partiesCache.GetCode(shipment.BillingRegionId);
                            var billingSubRegion = await partiesCache.GetCode(shipment.BillingSubRegionId);
                            var billingStation = await partiesCache.GetCode(shipment.BillingStationId);


                            var contracts = await (from cm in gbms.RbMainCustomerManagementCitRatesMappings
                                                   join sc in gbms.RbServiceCharges on cm.XServiceId equals sc.XCode
                                                   where cm.XStatus == "100" // Active
                                                   && (cm.XRegionalOffice == billingSubRegion || cm.XRegionalOffice == null) && (cm.XStation == billingStation || cm.XStation == null)
                                                   && cm.XCode == shipment.ParentCode
                                                   && (cm.XBranch == shipment.ShortName || cm.XBranch == null)
                                                   //&& sc.XShipmentType == "10"
                                                   //&& shipment.Distance >= sc.XFromKms && shipment.Distance <= sc.XToKms
                                                   select new
                                                   {
                                                       cm.XBranch,
                                                       cm.XStation,
                                                       cm.XRegionalOffice,
                                                       sc.XShipmentType,
                                                       sc.XShipmentTypeDescription,
                                                       sc.XFromKms,
                                                       sc.XToKms
                                                   }).ToListAsync();

                            #region Narrow down branch

                            string[] shipmentTypes = new[] { "10", "20", "30", "40", "50", "80", "85", "90", "95" };
                            foreach (var shipmentType in shipmentTypes)
                            {
                                if (contracts.Any(x => x.XBranch == shipment.FormalName && x.XShipmentType == shipmentType))
                                {
                                    contracts = contracts.Where(x => (x.XBranch == shipment.FormalName && x.XShipmentType == shipmentType) || x.XShipmentType != shipmentType).ToList();
                                }

                                if (contracts.Any(x => x.XStation == billingStation && x.XShipmentType == shipmentType))
                                {
                                    contracts = contracts.Where(x => (x.XStation == billingStation && x.XShipmentType == shipmentType) || x.XShipmentType != shipmentType).ToList();
                                }
                                if (contracts.Any(x => x.XRegionalOffice == billingSubRegion && x.XShipmentType == shipmentType))
                                {
                                    contracts = contracts.Where(x => (x.XRegionalOffice == billingSubRegion && x.XShipmentType == shipmentType) || x.XShipmentType != shipmentType).ToList();
                                }

                            }

                            #endregion

                            #region Dedicated

                            // if any of related branches have dedicated vehicle attached

                            var isDedicated = (from aa in context.AssetAllocations
                                               where (aa.PartyId == shipment.FromPartyId || aa.PartyId == shipment.ToPartyId || aa.PartyId == shipment.BillBranchId)
                                               && aa.AssetId == shipment.AssetId
                                               && aa.AllocatedFrom <= shipment.DueTime
                                               && (aa.AllocatedThru >= shipment.DueTime || aa.AllocatedThru == null)
                                               select aa).Any();

                            if (isDedicated)
                            {
                                // pull details of dedicated vehicle to check redius and total trips
                                var dedicatedInfo = (from d in context.DedicatedVehiclesCapacities
                                                     where d.OrganizationId == shipment.BillBranchId
                                                     || d.OrganizationId == shipment.FromPartyId
                                                     || d.OrganizationId == shipment.ToPartyId
                                                     select d).FirstOrDefault();

                                if (dedicatedInfo != null && Convert.ToDecimal(shipment.Distance) <= dedicatedInfo.RadiusInKm)
                                {
                                    var todaysShipments = (from c1 in context.Consignments
                                                           join d in context.ConsignmentDeliveries on c1.Id equals d.ConsignmentId
                                                           where c1.BillBranchId == shipment.BillBranchId
                                                           && d.CrewId == shipment.CrewId
                                                           && c1.DueTime.Date == DateTime.Today
                                                           && c1.ShipmentType == ShipmentType.Dedicated
                                                           select c1.Id).Count();

                                    if (todaysShipments < dedicatedInfo.TripPerDay)
                                    {
                                        shipment.Obj.ShipmentType = ShipmentType.Dedicated;
                                        context.Consignments.Update(shipment.Obj);
                                        logger.Information("{0}", $"{shipment.Id} is marked decicated {context.SaveChanges()}");
                                        await shipmentsCache.SetShipment(shipment.Id, null);
                                        continue;
                                    }
                                }
                            }

                            #endregion

                            #region ATM CIIT

                            if (shipment.AtmCitBillFrom == 1 || shipment.AtmCitBillTo == 1)
                            {
                                var atmCitContract = contracts.FirstOrDefault(x => Math.Round(shipment.Distance) >= x.XFromKms && Math.Round(shipment.Distance) <= x.XToKms && (x.XShipmentType == "80" || x.XShipmentType == "85"));
                                if (atmCitContract == null)
                                    throw new Exception($"No AtmCIT contract found for shipment {shipment.Id}");

                                shipment.Obj.ShipmentType = (ShipmentType)Convert.ToByte(atmCitContract.XShipmentType);
                                logger.Information($"{shipment.Id} is marked {atmCitContract.XShipmentTypeDescription} {context.SaveChanges()}");
                                await shipmentsCache.SetShipment(shipment.Id, null);
                                continue;
                            }

                            #endregion

                            #region Cash Manager

                            if (shipment.CollectionType == OrganizationType.CashManager || shipment.DeliveryType == OrganizationType.CashManager)
                            {
                                var cashManagerContract = contracts.FirstOrDefault(x => Math.Round(shipment.Distance) >= x.XFromKms && Math.Round(shipment.Distance) <= x.XToKms
                                 && (x.XShipmentType == "90" || x.XShipmentType == "95"));
                                if (cashManagerContract == null)
                                    throw new Exception($"No Cash Manager contract found for shipment {shipment.Id}");

                                shipment.Obj.ShipmentType = (ShipmentType)Convert.ToByte(cashManagerContract.XShipmentType);
                                logger.Information($"{shipment.Id} is marked {cashManagerContract.XShipmentTypeDescription} {context.SaveChanges()}");
                                await shipmentsCache.SetShipment(shipment.Id, null);
                                continue;
                            }
                            #endregion

                            #region Hilly

                            if (shipment.Distance > 30 && shipment.BillBranchType == OrganizationType.HillyArea)
                            {
                                shipment.Obj.ShipmentType = ShipmentType.HillyAreas;
                                logger.Information($"{shipment.Id} is marked {ShipmentType.HillyAreas} {context.SaveChanges()}");
                                await shipmentsCache.SetShipment(shipment.Id, null);
                                continue;
                            }

                            #endregion

                            var simpleContract = contracts.FirstOrDefault(x => Math.Round(shipment.Distance) >= x.XFromKms && Math.Round(shipment.Distance) <= x.XToKms && (x.XShipmentType == "10" || x.XShipmentType == "20"));

                            if (simpleContract == null)
                                throw new Exception($"No {simpleContract} contract found for shipment {shipment.Id}");

                            shipment.Obj.ShipmentType = (ShipmentType)Convert.ToByte(simpleContract.XShipmentType);
                            logger.Information("{0}", $"{shipment.Id} is marked {simpleContract.XShipmentTypeDescription} {context.SaveChanges()}");
                            await shipmentsCache.SetShipment(shipment.Id, null);

                        }
                        catch (Exception ex)
                        {
                            if (consignment != null)
                            {
                                consignment.PostingMessage = ex.Message;
                                context.SaveChanges();
                            }
                            logger.Error(ex.Message);
                        }
                        if (consignment != null)
                        {
                            await shipmentsCache.SetShipment(consignment.Id, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }

                }
            });
        }

    }
}