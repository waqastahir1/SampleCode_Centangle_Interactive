using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.APIs.Models.WorkOrder;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.BankSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Services
{
    public class ConsignmentService
    {
        private readonly AppDbContext context;
        private readonly NotificationService notificationService;

        public ConsignmentService(AppDbContext context, NotificationService notificationService)
        {
            this.context = context;
            this.notificationService = notificationService;
        }


        public IQueryable<CitConsignmentV3Model> GetQueryV3(List<int?> crewId, int deliveryId = 0)
        {
            var skipQRModel = new BankSettingFormViewModel
            {
                Id = 931228,
                SkipQRCodeOnCollection = true,
                SkipQRCodeOnDelivery = true,
                EnableManualShipmentNo = true
            };
            var skipQRJson = JsonConvert.SerializeObject(skipQRModel);
            var query = (from o in context.Consignments
                         from i in context.Denominations.Where(x => x.ConsignmentId == o.Id).DefaultIfEmpty()
                         from d in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == o.Id)
                         from f in context.Parties.Where(x => x.Id == d.FromPartyId).DefaultIfEmpty()
                         from t in context.Parties.Where(x => x.Id == d.ToPartyId).DefaultIfEmpty()
                         from mdr in context.PartyRelationships.Where(x => x.FromPartyId == o.ToPartyId && x.ToPartyRole == RoleType.ParentOrganization)
                         from md in context.Parties.Where(x => x.Id == mdr.ToPartyId)
                         from mdc in context.PartyRelationships.Where(x => x.FromPartyId == o.FromPartyId && x.ToPartyRole == RoleType.ParentOrganization)
                         from mc in context.Parties.Where(x => x.Id == mdc.ToPartyId)

                         from crew in context.Orgnizations.Where(x => x.Id == d.CrewId)
                         where crewId.Contains(d.CrewId) &&
                         (deliveryId > 0 || o.CreatedAt >= MyDateTime.Today || d.DeliveryState < Shared.Enums.ConsignmentDeliveryState.Delivered)
                         && o.Type == ShipmentExecutionType.Live
                         select new CitConsignmentV3Model()
                         {
                             ConsignmentId = o.Id,
                             DeliveryId = d.Id,
                             ConsignmentStateType = o.ConsignmentStateType,
                             DeliveryState = (int)d.DeliveryState,
                             ManualShipmentCode = o.ManualShipmentCode,
                             ConsignmentDeliveryStatus = d.DeliveryState.ToString(),
                             PickupQRCode = d.PickupCode,
                             DropoffQRCode = d.DropoffCode,
                             ConsignmentNo = o.ShipmentCode,
                             FromPartyId = f.Id,
                             FromPartyName = f.FormalName,
                             FromPartyMobileNo = f.PersonalContactNo,
                             FromPartyLandlineNo = f.OfficialContactNo,
                             ToPartyId = t.Id,
                             ToPartyName = t.FormalName,
                             ToPartyMobileNo = t.PersonalContactNo,
                             ToPartyLandlineNo = t.OfficialContactNo,
                             ConsignmentType = o.Type.ToString(),
                             CurrencySymbol = o.CurrencySymbol.ToString(),
                             Amount = o.Amount,
                             Currency1000x = i.Currency1000x,
                             Currency100x = i.Currency100x,
                             Currency10x = i.Currency10x,
                             Currency1x = i.Currency1x,
                             Currency20x = i.Currency20x,
                             Currency2x = i.Currency2x,
                             Currency5000x = i.Currency5000x,
                             Currency500x = i.Currency500x,
                             Currency50x = i.Currency50x,
                             Currency75x = i.Currency75x,
                             Currency5x = i.Currency5x,
                             DenominationType = i.DenominationType,
                             DenominationTypeStr = i.DenominationType.ToString(),
                             FromPartyLat = f.Orgnization.Geolocation.Y,
                             FromPartyLong = f.Orgnization.Geolocation.X,
                             ToPartyLat = t.Orgnization.Geolocation.Y,
                             ToPartyLong = t.Orgnization.Geolocation.X,
                             CrewId = d.CrewId,
                             IsVault = crew.OrganizationType == OrganizationType.Vault || crew.OrganizationType == OrganizationType.VaultOnWheels,
                             Type = "Crew",
                             Description = crew.OrganizationType.ToString(),
                             CollectiionMainCustomerJsonData = mc.JsonData,
                             DeliveryMainCustomerJsonData = md.JsonData,
                             CreatedAt = o.CreatedAt,
                             FromPartyAddress = f.Address,
                             ToPartyAddress = t.Address,
                             IsFinalized = o.IsFinalized,
                             NoOfBags = o.NoOfBags,
                             SealedBags = o.SealedBags
                         });

            if (deliveryId > 0)
            {
                query = query.Where(x => x.DeliveryId == deliveryId);
            }

            return query;
        }

        public async Task<int> VaultIn(int id, DateTime timeStamp)
        {
            var lastDelivery = await context.ConsignmentDeliveries
                 .Include(x => x.Consignment).FirstOrDefaultAsync(x => x.Id == id);

            if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                throw new Exception("You need to assign this consignment to one of crews before vault");

            lastDelivery.DeliveryState = Shared.Enums.ConsignmentDeliveryState.Delivered;

            var ccRelationship = await context.PartyRelationships.Include(x => x.ToParty).FirstOrDefaultAsync(x => x.ToPartyId == lastDelivery.CrewId
                  && x.FromPartyRole == RoleType.CheifCrewAgent
                  && x.StartDate <= timeStamp.Date && (x.ThruDate == null || x.ThruDate >= timeStamp.Date));

            var vault = (await context.PartyRelationships.Include(x => x.ToParty).FirstOrDefaultAsync(x => x.FromPartyId == ccRelationship.FromPartyId
                    && x.FromPartyRole == RoleType.VaultIncharge
                    && x.StartDate <= timeStamp.Date && (x.ThruDate == null || x.ThruDate >= timeStamp.Date)))?.ToParty;

            if (vault == null)
            {
                var vaultId = context.Sequences.GetNextPartiesSequence();
                vault = new Party()
                {
                    Id = vaultId,
                    FormalName = $"Vault-W/{ccRelationship.ToParty.FormalName}",
                    PartyType = PartyType.Organization,
                    RegionId = ccRelationship.ToParty.RegionId,
                    SubregionId = ccRelationship.ToParty.SubregionId,
                    StationId = ccRelationship.ToParty.StationId,

                    Orgnization = new Organization()
                    {
                        Id = vaultId,
                        OrganizationType = OrganizationType.VaultOnWheels,
                        IsCPCBranch = false
                    }
                };

                context.Orgnizations.Add(vault.Orgnization);
                context.Parties.Add(vault);

                var vaultRelationship = new PartyRelationship
                {
                    Id = context.Sequences.GetNextPartiesSequence(),
                    ToPartyId = 1, // Id of SOS
                    FromPartyId = vault.Id,
                    FromPartyRole = RoleType.Vault,
                    ToPartyRole = RoleType.ParentOrganization,
                    StartDate = timeStamp.Date
                };

                context.PartyRelationships.Add(vaultRelationship);


                var vaultManagerRelationship = new PartyRelationship
                {
                    Id = context.Sequences.GetNextPartiesSequence(),
                    FromPartyId = ccRelationship.FromPartyId,
                    ToPartyId = vault.Id,
                    FromPartyRole = RoleType.VaultIncharge,
                    ToPartyRole = RoleType.Vault,
                    StartDate = timeStamp.Date
                };

                context.PartyRelationships.Add(vaultManagerRelationship);
                await context.SaveChangesAsync();
            }

            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var newDelivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                ParentId = lastDelivery.Id,
                ConsignmentId = lastDelivery.ConsignmentId,
                CrewId = vault.Id,
                DestinationLocationId = lastDelivery.DestinationLocationId,
                PickupLocationId = lastDelivery.PickupLocationId,
                FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                ToPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                PlanedPickupTime = lastDelivery.PlanedPickupTime,
                PlanedDropTime = lastDelivery.PlanedDropTime,
                PickupCode = lastDelivery.PickupCode,
                DropoffCode = lastDelivery.DropoffCode,
                DeliveryState = Shared.Enums.ConsignmentDeliveryState.InTransit
            };

            context.ConsignmentDeliveries.Add(newDelivery);

            lastDelivery.ToPartyId = lastDelivery.CrewId.GetValueOrDefault();

            await context.SaveChangesAsync();

            return newDelivery.Id;
        }

        public async Task<int> VaultOut(int id, int? crewId, DateTime timeStamp)
        {
            var lastDelivery = await context.ConsignmentDeliveries
                 .Include(x => x.Consignment).FirstOrDefaultAsync(x => x.Id == id);

            if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                throw new Exception("You need to assign this consignment to one of crews before vault");

            lastDelivery.DeliveryState = Shared.Enums.ConsignmentDeliveryState.Delivered;
            lastDelivery.ActualDropTime = timeStamp;

            var deliveryId = context.Sequences.GetNextDeliverySequence();

            var newDelivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                ParentId = lastDelivery.Id,
                ConsignmentId = lastDelivery.ConsignmentId,
                CrewId = crewId,
                DestinationLocationId = lastDelivery.DestinationLocationId,
                PickupLocationId = lastDelivery.PickupLocationId,
                FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                ToPartyId = lastDelivery.Consignment.ToPartyId,
                PlanedPickupTime = lastDelivery.PlanedPickupTime,
                PlanedDropTime = lastDelivery.PlanedDropTime,
                PickupCode = lastDelivery.PickupCode,
                DropoffCode = lastDelivery.DropoffCode,
                DeliveryState = Shared.Enums.ConsignmentDeliveryState.InTransit,
                ActualPickupTime = timeStamp
            };

            context.ConsignmentDeliveries.Add(newDelivery);

            lastDelivery.ToPartyId = lastDelivery.CrewId.GetValueOrDefault();

            await context.SaveChangesAsync();

            return newDelivery.Id;

        }

    }
}
