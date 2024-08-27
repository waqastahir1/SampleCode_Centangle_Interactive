using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.ATMR;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class AtmrService
    {
        private readonly AppDbContext context;

        public AtmrService(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<RequestCITViewModel> RequestCIT(ATMRBaseModel model, string createdBy, bool addDenomination = true)
        {

            var atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == model.AtmrServiceId);
            if (atmService == null)
                throw new NotFoundException("consignment does not exist");

            var dummyLocation = context.Locations.First();

            var nextId = context.Sequences.GetNextCitOrdersSequence();
            var citShipment = new Consignment()
            {
                Id = nextId,
                ShipmentCode = $"CIT/{MyDateTime.Now.Year}/" + nextId.ToString("D4"),
                Type = ShipmentExecutionType.Live,
                CreatedAt = MyDateTime.Now,
                ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                CreatedBy = createdBy,
                ApprovalState = ConsignmentApprovalState.Approved,
                IsFinalized = true
            };

            context.Consignments.Add(citShipment);

            var parties = await context.Parties.Where(x => x.Id == atmService.CashBranchId || x.Id == atmService.ATMId)
                  .ToListAsync();

            var collectionBranch = parties.FirstOrDefault(x => x.Id == atmService.CashBranchId);
            var deliveryBranch = parties.FirstOrDefault(x => x.Id == atmService.ATMId);
            var billingBranch = parties.FirstOrDefault(x => x.Id == atmService.CashBranchId);

            citShipment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
            citShipment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
            citShipment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

            citShipment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
            citShipment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
            citShipment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


            citShipment.BillingRegionId = billingBranch.RegionId.GetValueOrDefault();
            citShipment.BillingSubRegionId = billingBranch.SubregionId.GetValueOrDefault();
            citShipment.BillingStationId = billingBranch.StationId.GetValueOrDefault();


            citShipment.CustomerId = atmService.CustomerId;
            citShipment.FromPartyId = atmService.CashBranchId.GetValueOrDefault();
            citShipment.BillBranchId = atmService.CashBranchId.GetValueOrDefault();
            citShipment.MainCustomerId = atmService.CashBranchId.GetValueOrDefault();
            citShipment.ToPartyId = atmService.ATMId;
            citShipment.CurrencySymbol = CurrencySymbol.PKR;
            citShipment.ServiceType = ServiceType.ByRoad;
            citShipment.ShipmentType = ShipmentType.ATMCITLocal;
            citShipment.Type = ShipmentExecutionType.Live;
            citShipment.DueTime = citShipment.CreatedAt;
            if (addDenomination)
            {
                citShipment.Amount = (atmService.Currency500x * 50000) +
                    (atmService.Currency1000x * 100000) +
                    (atmService.Currency5000x * 500000);

                //linking Denomination with Consignment for preserving later
                Denomination denom = new Denomination
                {
                    //A.B: Implement Sequence
                    Id = context.Sequences.GetNextDenominationSequence(),
                    ConsignmentId = citShipment.Id,
                    DenominationType = DenominationType.Packets,
                    Currency500x = atmService.Currency500x,
                    Currency1000x = atmService.Currency1000x,
                    Currency5000x = atmService.Currency5000x
                };

                context.Denominations.Add(denom);
            }
            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var delivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                CrewId = null,
                ConsignmentId = citShipment.Id,
                DestinationLocationId = dummyLocation.Id,
                PickupLocationId = dummyLocation.Id,
                FromPartyId = citShipment.FromPartyId,
                ToPartyId = citShipment.ToPartyId,
                PlanedPickupTime = MyDateTime.Now,
                PlanedDropTime = MyDateTime.Now.AddHours(1),
                PickupCode = atmService.CITPickupQrCode,
                DropoffCode = atmService.CITDropoffQrCode
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

            context.ConsignmentStates.Add(new Models.ConsignmentState()
            {
                ConsignmentId = delivery.ConsignmentId,
                DeliveryId = deliveryId,
                CreatedBy = createdBy,
                ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                TimeStamp = MyDateTime.Now,
                Status = StateTypes.Confirmed
            });

            var states = Enum.GetValues(typeof(Shared.Enums.ConsignmentDeliveryState));
            for (int i = 1; i < states.Length; i++)
            {
                context.ConsignmentStates.Add(new Models.ConsignmentState()
                {
                    ConsignmentId = delivery.ConsignmentId,
                    DeliveryId = deliveryId,
                    CreatedBy = createdBy,
                    ConsignmentStateType = (Shared.Enums.ConsignmentDeliveryState)states.GetValue(i),
                });
            }

            var atmSealCodes = await context.ATMRSealCodes.Where(x => x.AtmrServiceId == atmService.Id)
                .Select(x => new ShipmentSealCode()
                {
                    SealCode = x.SealCode,
                    ConsignmentId = citShipment.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                }).ToListAsync();

            context.ShipmentSealCodes.AddRange(atmSealCodes);
            await context.SaveChangesAsync();
            return new RequestCITViewModel()
            {
                AtmrServiceId = atmService.Id,
                CITDropoffQrCode = atmService.CITDropoffQrCode,
                CITPickupQrCode = atmService.CITPickupQrCode,
                ShipmentId = citShipment.Id
            };
        }

        public async Task<int> RequestReturnCIT(int atmrServiceId, string createdBy, string label, string comments)
        {

            var atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == atmrServiceId);
            if (atmService == null)
                throw new NotFoundException("ATMR consignment does not exist");

            var dummyLocation = context.Locations.First();

            var nextId = context.Sequences.GetNextCitOrdersSequence();
            var consignment = new Consignment()
            {
                Id = nextId,
                ShipmentCode = $"CIT/{label}/{MyDateTime.Now.Year}/" + nextId.ToString("D4"),
                Type = ShipmentExecutionType.Live,
                CreatedAt = MyDateTime.Now,
                ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                CreatedBy = createdBy,
                IsFinalized = true,
                ApprovalState = ConsignmentApprovalState.Approved
            };

            context.Consignments.Add(consignment);


            var parties = await context.Parties.Where(x => x.Id == atmService.CashBranchId || x.Id == atmService.ATMId)
                  .ToListAsync();

            var collectionBranch = parties.FirstOrDefault(x => x.Id == atmService.CashBranchId);
            var deliveryBranch = parties.FirstOrDefault(x => x.Id == atmService.ATMId);
            var billingBranch = parties.FirstOrDefault(x => x.Id == atmService.CashBranchId);

            consignment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
            consignment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
            consignment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

            consignment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
            consignment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
            consignment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


            consignment.BillingRegionId = billingBranch.RegionId.GetValueOrDefault();
            consignment.BillingSubRegionId = billingBranch.SubregionId.GetValueOrDefault();
            consignment.BillingStationId = billingBranch.StationId.GetValueOrDefault();

            consignment.CustomerId = atmService.CustomerId;
            consignment.FromPartyId = atmService.ATMId;
            consignment.ToPartyId = atmService.CashBranchId.GetValueOrDefault();
            consignment.BillBranchId = atmService.CashBranchId.GetValueOrDefault();
            consignment.MainCustomerId = atmService.CashBranchId.GetValueOrDefault();
            consignment.CurrencySymbol = CurrencySymbol.PKR;
            consignment.Amount = 0;
            List<ShipmentComment> listOfComments = new();
            if (consignment.Comments != null)
                listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
            if (comments != null)
            {
                listOfComments.Add(new ShipmentComment()
                {
                    Description = comments,
                    CreatedAt = MyDateTime.Now,
                    CreatedBy = createdBy,
                    ViewedAt = MyDateTime.Now,
                    ViewedBy = createdBy
                });

                consignment.Comments = JsonConvert.SerializeObject(listOfComments);
            }
            consignment.ServiceType = ServiceType.ByRoad;
            consignment.ShipmentType = ShipmentType.ATMCITLocal;
            consignment.Type = ShipmentExecutionType.Live;
            consignment.DueTime = consignment.CreatedAt;

            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var delivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                CrewId = null,
                ConsignmentId = consignment.Id,
                DestinationLocationId = dummyLocation.Id,
                PickupLocationId = dummyLocation.Id,
                FromPartyId = consignment.FromPartyId,
                ToPartyId = consignment.ToPartyId,
                PlanedPickupTime = MyDateTime.Now,
                PlanedDropTime = MyDateTime.Now.AddHours(1),
                PickupCode = atmService.ReturnCITPickupQrCode,
                DropoffCode = $"{consignment.ShipmentCode}{deliveryId}-Dropoff"
            };
            context.ConsignmentDeliveries.Add(delivery);

            context.ConsignmentStates.Add(new Models.ConsignmentState()
            {
                ConsignmentId = delivery.ConsignmentId,
                DeliveryId = deliveryId,
                CreatedBy = createdBy,
                ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                TimeStamp = MyDateTime.Now,
                Status = StateTypes.Confirmed
            });

            var states = Enum.GetValues(typeof(Shared.Enums.ConsignmentDeliveryState));
            for (int i = 1; i < states.Length; i++)
            {
                context.ConsignmentStates.Add(new Models.ConsignmentState()
                {
                    ConsignmentId = delivery.ConsignmentId,
                    DeliveryId = deliveryId,
                    CreatedBy = createdBy,
                    ConsignmentStateType = (Shared.Enums.ConsignmentDeliveryState)states.GetValue(i),
                });
            }

            Denomination denom = new Denomination
            {
                //A.B: Implement Sequence
                Id = context.Sequences.GetNextDenominationSequence(),
                ConsignmentId = consignment.Id,
                DenominationType = DenominationType.Packets,
                Currency500x = 0,
                Currency1000x = 0,
                Currency5000x = 0
            };

            context.Denominations.Add(denom);

            await context.SaveChangesAsync();
            return consignment.Id;
        }
    }
}
