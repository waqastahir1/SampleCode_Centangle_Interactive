using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Portal.GBMS.Models;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    public class GbmsSyncService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        public GbmsSyncService(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public void StartSyncing()
        {
            DateTime? lastTimeStamp = DateTime.Now.AddMonths(-1);


            Task.Run(async () =>
            {
                //await Task.Delay(TimeSpan.FromMinutes(100));
                while (true)
                {
                    try
                    {

                        using (var scope = serviceScopeFactory.CreateScope())
                        {

                            var assetService = scope.ServiceProvider.GetRequiredService<AssetsService>();
                            var gbms = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                            DateTime startTime = MyDateTime.Now;

                            var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

                            gbms.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
                            bool syncAll = lastTimeStamp == null;
                            logger.Information("--------- Starting sync cycle - {0} at {1}  ---------", $" Syncing All -> {syncAll}", startTime);

                            #region Locations

                            var regions = await gbms.RbRegions.ToArrayAsync();
                            logger.Information("RBREGION Started {0}", regions.Count());
                            await UpdateRegions(regions, scope, OrganizationType.RegionalControlCenter);
                            logger.Information("RBREGION");

                            var subRegions = await gbms.RbSubRegions.ToArrayAsync();
                            logger.Information("RBSUBREGIONS Started {0}", subRegions.Count());
                            await UpdateSubRegions(subRegions, scope, OrganizationType.SubRegionalControlStation);

                            var stations = await gbms.RbStations.ToArrayAsync();
                            logger.Information("RBSTATIONS Started {0}", stations.Count());
                            await UpdateStations(stations, scope, OrganizationType.Station);

                            var subRegionStationsData = await gbms.RbSubRegionsStations.ToArrayAsync();
                            logger.Information("RBSTATIONS Started {0}", subRegionStationsData.Count());
                            await BuildRelationships(subRegionStationsData, scope);
                            logger.Information("RBSUBREGIONSSTATIONS");


                            #endregion

                            #region Vehicles

                            var dedicatedVehicles = await gbms.RbMainCustomerManagementCitDedicatedVehicles.ToArrayAsync();
                            logger.Information("RBVEHICLES Started {0}", dedicatedVehicles.Count());

                            await UpdateDedicatedBranches(dedicatedVehicles, scope, true);

                            var vehicles = await gbms.RbVehicles.ToArrayAsync();
                            logger.Information("RBVEHICLES Started {0}", vehicles.Count());
                            await UpdateVehicles(vehicles, assetService);
                            logger.Information("RBVEHICLES");

                            #endregion

                            #region PDW Employee

                            var pdwEmployeeData = await gbms.PdwEmployeeMasters.ToArrayAsync();
                            logger.Information("PDWEMPLOYEEMASTER => {0}", pdwEmployeeData.Length);
                            await UpdatePdwEmployees(pdwEmployeeData, scope, EmploymentType.Gaurd, true);
                            logger.Information("PDWEMPLOYEEMASTER completed");

                            var pdwEmployeePicData = await gbms.PdwEmployeePics.ToArrayAsync();
                            await UpdatePdwEmployeePic(pdwEmployeePicData, scope);
                            logger.Information("PDWEMPLOYEEPIC");

                            #endregion

                            #region Pay Management Employees

                            var payEmployeeData = await gbms.PayEmployeeMasters.Where(x => x.XCode.StartsWith("1")).ToArrayAsync();
                            logger.Information("PAYEMPLOYEEMASTER -> {0}", payEmployeeData.Length);
                            await UpdatePayEmployees(payEmployeeData, scope, EmploymentType.Regular, true);
                            logger.Information("PAYEMPLOYEEMASTER completed");

                            #endregion

                            #region Pay Employee

                            //var payEmployeeData = await gbms.PayEmployeeMasters.Where(x => x.XDepartmentDescription == "Workforce CIT").ToArrayAsync();
                            //logger.Information("PAYEMPLOYEEMASTER -> {0}", payEmployeeData.Length);
                            //await UpdatePayEmployees(payEmployeeData, scope, EmploymentType.Regular, true);
                            //logger.Information("PAYEMPLOYEEMASTER completed");

                            var payEmployeePicData = await gbms.PayEmployeePics.ToArrayAsync();
                            await UpdatePayEmployeePic(payEmployeePicData, scope);
                            logger.Information("PAYEMPLOYEEPIC");

                            #endregion


                            #region Customer 
                            var mainCustomerData = await gbms.RbMainCustomerManagements.Where(x => lastTimeStamp == null || (x.AddDate >= lastTimeStamp || x.ModDate >= lastTimeStamp)).ToArrayAsync();

                            logger.Information("RBMAINCUSTOMERMANAGEMENT Started {0} records,", mainCustomerData.Count());
                            await UpdateMainCustomer(mainCustomerData, scope);
                            logger.Information("RBMAINCUSTOMERMANAGEMENT");

                            var customerData = await gbms.RbCustomerManagements.Where(x => lastTimeStamp == null || (x.AddDate >= lastTimeStamp || x.ModDate >= lastTimeStamp)).ToArrayAsync();
                            logger.Information("RBCUSTOMERMANAGEMENT Started {0} records,", customerData.Count());
                            await UpdateCustomer(customerData, scope);

                            var customerBranchData = await gbms.RbMainCustomerManagementBranches
                            .Where(x => lastTimeStamp == null || (x.AddDate >= lastTimeStamp || x.ModDate >= lastTimeStamp))
                            //.Where(x=>x.XCode == "JSBL" && x.XBranchCode == "9010")
                            .ToArrayAsync();

                            await UpdateCustomerBranch(customerBranchData, scope, syncAll);
                            logger.Information("RBMAINCUSTOMERMANAGEMENTBRANCHES Started {0} records,", customerBranchData.Count());

                            #endregion
                            lastTimeStamp = (startTime - lastTimeStamp).GetValueOrDefault().Days > 0 ? null : startTime;
                            cache.SetString("CacheTime", MyDateTime.Now.ToString("o"));
                        }

                    }
                    catch (Exception ex)
                    {
                        //  telemetry.TrackException(ex);
                        logger.Error(ex.ToString());
                    }
                    logger.Information("Waiting...");
                    await Task.Delay(TimeSpan.FromMinutes(10));
                    if (lastTimeStamp.Value.Hour >= 2 && lastTimeStamp.Value.Hour <= 5)
                    {
                        lastTimeStamp = null;
                    }
                }
            });
        }

        private async Task UpdatePdwEmployeePic(PdwEmployeePic[] records, IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (var item in records)
            {

                var asset = context.Parties.FirstOrDefault(x => x.ShortName == item.XEmployee);
                if (asset != null)
                {
                    asset.ImageLink = item.XPicLink.Replace("https://www.azmcloud.com:100", "http://104.215.144.226:100");
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task UpdateDedicatedBranches(IList<RbMainCustomerManagementCitDedicatedVehicle> records, IServiceScope scope, bool syncAll)
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var gbmscontext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
            if (syncAll)
            {
                //await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE DedicatedVehiclesCapacities SET SyncStatus = '0'");
            }
            foreach (var items in records.GroupBy(x => x.XBranch))
            {
                var item = items.FirstOrDefault();
                var parentCode = await gbmscontext.RbMainCustomerManagementBranches
                            .Where(x => x.XBranchName == item.XBranchDescription)
                            .Select(x => x.XCode)
                            .FirstOrDefaultAsync();

                var dedicatedBranchs = context.Parties.Include(x => x.Orgnization).Where(x => x.ShortName == item.XBranch && (x.ParentCode == parentCode
                        && x.Orgnization.OrganizationType.HasFlag(OrganizationType.CustomerBranch))).ToList();
                Party dedicatedBranch = dedicatedBranchs.FirstOrDefault();
                if (dedicatedBranch != null)
                {
                    var approvedCapacity = context.DedicatedVehiclesCapacities.FirstOrDefault(x => x.OrganizationId == dedicatedBranch.Id);
                    if (approvedCapacity == null)
                    {
                        approvedCapacity = new DedicatedVehiclesCapacity()
                        {
                            Id = context.Sequences.GetNextCommonSequence(),
                            CreatedAt = item.AddDate.GetValueOrDefault(),
                            CreatedBy = $"gbms-{MyDateTime.Now.ToString("o")}",
                            FromDate = MyDateTime.Now,
                            OrganizationId = dedicatedBranch.Id
                        };

                        context.DedicatedVehiclesCapacities.Add(approvedCapacity);
                    }
                    approvedCapacity.VehicleCapacity = Convert.ToByte(items.Count());
                    approvedCapacity.TripPerDay = Convert.ToInt32(item.XTrips);
                    approvedCapacity.RadiusInKm = item.XRadiusKm;
                    approvedCapacity.UpdatedAt = item.ModDate.GetValueOrDefault();
                    approvedCapacity.UpdatedBy = $"gbms-{MyDateTime.Now:o}";
                    approvedCapacity.IsActive = true;
                    //approvedCapacity.SyncStatus = 1;
                    await context.SaveChangesAsync();
                    // Console.WriteLine($"{dedicatedBranch.FormalName}");
                }
            }
            if (syncAll)
            {
                // await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE DedicatedVehiclesCapacities SET IsActive = 0 WHERE SyncStatus = 0");
            }
        }

        private async Task UpdatePayEmployeePic(IList<PayEmployeePic> records, IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (var item in records)
            {

                var asset = context.Parties.FirstOrDefault(x => x.ShortName == item.XEmployee);
                if (asset != null)
                {
                    asset.ImageLink = item.XPicLink;
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task UpdateVehicles(IList<RbVehicle> records,
            AssetsService assetsService)
        {
            foreach (var item in records)
            {
                await assetsService.AddOrUpdateAsset(item.XCode, item.XDescription, AssetType.Vehicle, item.XStation);
            }
        }

        #region Locations


        private async Task UpdateRegions(IEnumerable<RbRegion> locations, IServiceScope scope, OrganizationType organizationType)
        {
            foreach (var item in locations)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var region = context.Parties.FirstOrDefault(x => x.ShortName == item.XCode);
                if (region == null)
                {
                    var regionId = context.Sequences.GetNextPartiesSequence();
                    region = new Party()
                    {
                        Id = regionId,
                        PartyType = PartyType.Organization,
                        Orgnization = new Organization()
                        {
                            Id = regionId,
                            OrganizationType = organizationType
                        },
                        ShortName = item.XCode,
                        ExternalId = (int)item.XrowId.GetValueOrDefault()
                    };
                    context.Parties.Add(region);
                    context.Orgnizations.Add(region.Orgnization);
                }
                region.FormalName = item.XDescription;
                region.Abbrevation = item.XAbbrevation;
                region.LastSync = DateTime.UtcNow;

                await context.SaveChangesAsync();
            }
        }

        private async Task UpdateSubRegions(IEnumerable<RbSubRegion> subRegions, IServiceScope scope, OrganizationType organizationType)
        {
            foreach (var item in subRegions)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var subregion = context.Parties.FirstOrDefault(x => x.ShortName == item.XCode);
                if (subregion == null)
                {
                    var subRegionId = context.Sequences.GetNextPartiesSequence();
                    subregion = new Party()
                    {
                        Id = subRegionId,
                        PartyType = PartyType.Organization,
                        ShortName = item.XCode,
                        ExternalId = (int)item.XrowId.GetValueOrDefault(),
                        Orgnization = new Organization
                        {
                            Id = subRegionId,
                            OrganizationType = organizationType,
                        }
                    };
                    context.Parties.Add(subregion);
                    context.Orgnizations.Add(subregion.Orgnization);
                }
                subregion.FormalName = item.XDescription;
                subregion.Abbrevation = item.XAbbrevation;
                subregion.LastSync = DateTime.UtcNow;

                var region = context.Parties.First(x => x.ShortName == item.XRegion);
                var relationship = context.PartyRelationships.FirstOrDefault(
                    x => x.FromPartyId == subregion.Id && x.ToPartyRole == RoleType.RegionalOrg);

                if (relationship == null)
                {
                    relationship = new PartyRelationship()
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        FromPartyId = subregion.Id,
                        FromPartyRole = RoleType.SubRegionalOrg,
                        ToPartyRole = RoleType.RegionalOrg
                    };
                    context.PartyRelationships.Add(relationship);
                }
                relationship.ToPartyId = region.Id;
                subregion.RegionId = region.Id;
                await context.SaveChangesAsync();
            }
        }

        private async Task UpdateStations(IEnumerable<RbStation> locations, IServiceScope scope, OrganizationType organizationType)
        {
            var cache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();
            foreach (var item in locations)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var party = context.Parties.FirstOrDefault(x => x.ShortName == item.XCode);
                if (party == null)
                {
                    var regionId = context.Sequences.GetNextPartiesSequence();
                    party = new Party()
                    {
                        Id = regionId,
                        Orgnization = new Organization()
                        {
                            Id = regionId,
                            OrganizationType = organizationType
                        },
                        ShortName = item.XCode,
                        ExternalId = (int)item.XrowId.GetValueOrDefault()
                    };
                    context.Parties.Add(party);
                    context.Orgnizations.Add(party.Orgnization);
                }
                party.FormalName = item.XDescription;
                party.Abbrevation = item.XAbbrevation;
                party.LastSync = DateTime.UtcNow;

                await context.SaveChangesAsync();
                await cache.SetName(party.Id, party.FormalName);
            }
        }


        private async Task BuildRelationships(IEnumerable<RbSubRegionsStation> subRegionStationDtos, IServiceScope scope)
        {
            foreach (var item in subRegionStationDtos)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var subregion = context.Parties.FirstOrDefault(x => x.ShortName == item.XCode);
                var station = context.Parties.FirstOrDefault(x => x.ShortName == item.XStationCode);

                if (subregion == null || station == null)
                {
                    logger.Warning($"Missing information for {JsonConvert.SerializeObject(item)}");
                    continue;
                }

                var relationship = context.PartyRelationships.FirstOrDefault(
                    x => x.FromPartyId == station.Id && x.ToPartyRole == RoleType.SubRegionalOrg);
                if (relationship == null)
                {
                    relationship = new PartyRelationship()
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        FromPartyId = station.Id,
                        FromPartyRole = RoleType.StationOrg,
                        ToPartyRole = RoleType.SubRegionalOrg
                    };
                    context.PartyRelationships.Add(relationship);
                }
                relationship.ToPartyId = subregion.Id;
                station.SubregionId = subregion.Id;
                station.RegionId = subregion.RegionId;
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region Customer

        private async Task UpdateMainCustomer(IEnumerable<RbMainCustomerManagement> customers, IServiceScope scope)
        {
            foreach (var item in customers)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var party = context.Parties.Include(x => x.Orgnization).FirstOrDefault(x => x.ShortName == item.XCode);
                if (party == null)
                {
                    party = new Party()
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        ShortName = item.XCode,
                        PartyType = PartyType.Organization,
                        ExternalId = (int)item.XrowId.GetValueOrDefault()
                    };
                    context.Parties.Add(party);

                    var organization = new Organization()
                    {
                        Id = party.Id,
                        OrganizationType = OrganizationType.MainCustomer,

                    };
                    context.Orgnizations.Add(organization);
                }
                party.Abbrevation = item.XAbbrevation;
                party.FormalName = item.XName;
                party.UpdatedAtErp = item.ModDate;

                await context.SaveChangesAsync();
            }
        }


        private async Task UpdateCustomer(IEnumerable<RbCustomerManagement> customers, IServiceScope scope)
        {
            foreach (var item in customers)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var party = context.Parties.Include(x => x.Orgnization).FirstOrDefault(x => x.ShortName == item.XCode && x.Orgnization.OrganizationType == OrganizationType.Customer);
                if (party == null)
                {
                    var id = context.Sequences.GetNextPartiesSequence();
                    party = new Party()
                    {
                        Id = id,
                        ShortName = item.XCode,
                        ExternalId = (int)item.XrowId.GetValueOrDefault(),
                        PartyType = PartyType.Organization,
                        Orgnization = new Organization()
                        {
                            Id = id,
                            OrganizationType = OrganizationType.Customer
                        }
                    };
                    context.Parties.Add(party);
                    context.Orgnizations.Add(party.Orgnization);
                }
                party.Abbrevation = item.XAbbrevation;
                party.FormalName = item.XName;
                party.UpdatedAtErp = item.ModDate;
                party.Orgnization.ExternalCustomerType = item.XCustomerType;

                var mainCustomer = context.Parties.FirstOrDefault(x => x.ShortName == item.XMainCustomer);
                if (mainCustomer == null)
                {
                    logger.Warning($"Main CUstomer is missing for {JsonConvert.SerializeObject(item.XCode)}");
                    continue;
                }
                var partyRelationshdip = context.PartyRelationships.FirstOrDefault(x =>
                x.FromParty.ShortName == item.XCode && x.FromPartyRole == RoleType.ChildOrganization && x.ToPartyRole == RoleType.ParentOrganization);
                if (partyRelationshdip == null)
                {
                    partyRelationshdip = new PartyRelationship()
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        FromPartyId = party.Id,
                        FromPartyRole = RoleType.ChildOrganization,
                        ToPartyRole = RoleType.ParentOrganization,
                        StartDate = MyDateTime.Now
                    };
                    context.PartyRelationships.Add(partyRelationshdip);
                }
                partyRelationshdip.ToPartyId = mainCustomer.Id;


                await context.SaveChangesAsync();
            }
        }

        private async Task UpdateCustomerBranch(IEnumerable<RbMainCustomerManagementBranch> branches, IServiceScope scope, bool syncAll)
        {
            int i = 1;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var cache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();

            var context1 = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (syncAll)
            {
                //await context1.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET SycStatus = 0");
            }
            var maincutomers = context1.Parties.Where(x => x.Orgnization.OrganizationType == OrganizationType.MainCustomer).ToList();

            foreach (var item in branches)
            {

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    item.XBranchCode = $"{item.XCode}-{item.XBranchCode}";
                    if (item.XLatitude != null && item.XLatitude.IndexOf('.') < 1)
                    {
                        item.XLatitude = item.XLatitude.Substring(0, 2) + "." + item.XLatitude.Substring(2);
                    }

                    if (item.XLongitude != null && item.XLongitude.IndexOf('.') < 1)
                    {
                        item.XLongitude = item.XLongitude.Substring(0, 2) + "." + item.XLongitude.Substring(2);
                    }

                    logger.Debug($"{i++}/{branches.Count()} -- {item.XCode}-{item.XBranchCode} -- {item.XLatitude}|{item.XLongitude}");
                    var branchParty = context.Parties.Include(x => x.Orgnization)
                        .FirstOrDefault(x => x.ShortName == item.XBranchCode
                        && x.ParentCode == item.XCode
                        && x.Orgnization.OrganizationType.HasFlag(OrganizationType.CustomerBranch));

                    if (branchParty == null)
                    {
                        var id = context.Sequences.GetNextPartiesSequence();
                        branchParty = new Party()
                        {
                            Id = id,
                            ShortName = item.XBranchCode,
                            ParentCode = item.XCode,
                            ExternalId = item.DetailId,
                            PartyType = PartyType.Organization,
                            Orgnization = new Organization
                            {
                                Id = id
                            }
                        };
                        context.Parties.Add(branchParty);
                        context.Orgnizations.Add(branchParty.Orgnization);
                    }
                    if (branchParty.Orgnization == null)
                    {
                        branchParty.Orgnization = new Organization()
                        {
                            Id = branchParty.Id
                        };
                        context.Orgnizations.Add(branchParty.Orgnization);
                    }
                    //branchParty.Abbrevation = item.XABBREVATION;
                    branchParty.PartyType = PartyType.Organization;
                    branchParty.FormalName = item.XBranchName;
                    branchParty.Address = item.XAddress1;
                    branchParty.PersonalContactNo = item.XMobile;
                    branchParty.OfficialContactNo = item.XLandline;
                    branchParty.UpdatedAtErp = item.ModDate;
                    branchParty.Orgnization.ExternalBranchType = item.XBranchType;
                    branchParty.Orgnization.ExternalCustomerType = null;
                    branchParty.Orgnization.IsCPCBranch = item.XCpcBranch == "Y";
                    branchParty.IsActive = item.XBranchStatus != "110";
                    branchParty.SycStatus = 1;
                    branchParty.Orgnization.AtmCitBill = item.XAtmCitBill == "Y" ? (byte)1 : (byte)0;
                    branchParty.LastSync = MyDateTime.Now;

                    await cache.SetName(branchParty.Id, branchParty.FormalName);
                    await cache.SetCode(branchParty.Id, branchParty.ShortName);
                    await cache.SetAddress(branchParty.Id, branchParty.Address);
                    await cache.SetContactNo(branchParty.Id, $"{branchParty.PersonalContactNo} {branchParty.OfficialContactNo}");

                    if (item.XBranchType == "500")
                    {
                        branchParty.Orgnization.OrganizationType = OrganizationType.ATM;
                    }
                    else if (item.XBranchTypeDescription == "Cash Manager")
                    {
                        branchParty.Orgnization.OrganizationType = OrganizationType.CashManager;
                    }
                    else if (item.XBranchTypeDescription == "Dedicated")
                    {
                        branchParty.Orgnization.OrganizationType = OrganizationType.Dedicated;
                    }
                    else if (item.XBranchType == "400")
                    {
                        branchParty.Orgnization.OrganizationType = OrganizationType.HillyArea;
                    }
                    else if (item.XBranchType == "100")
                    {
                        branchParty.Orgnization.OrganizationType = OrganizationType.Normal;
                    }
                    else
                    {
                        throw new Exception($"{item.XBranchTypeDescription} is not mapped");
                    }

                    var mainCustomer = context.Parties.FirstOrDefault(x => x.ShortName == item.XCode && (x.Orgnization.OrganizationType == OrganizationType.MainCustomer || x.ShortName == "SOS"));
                    if (mainCustomer == null)
                    {
                        logger.Warning($"Main Customer of branch is missing for {JsonConvert.SerializeObject(item.XCode)}");
                        throw new Exception($"Main Customer of branch is missing for {JsonConvert.SerializeObject(item.XCode)}");
                    }
                    var mainCustomerRelationshdip = context.PartyRelationships.FirstOrDefault(x =>
                    x.FromPartyId == branchParty.Id && x.ToPartyId == mainCustomer.Id
                    && x.FromPartyRole == RoleType.ChildOrganization
                    && x.ToPartyRole == RoleType.ParentOrganization);

                    if (mainCustomerRelationshdip == null)
                    {
                        mainCustomerRelationshdip = new PartyRelationship()
                        {
                            Id = context.Sequences.GetNextPartiesSequence(),
                            FromPartyId = branchParty.Id,
                            FromPartyRole = RoleType.ChildOrganization,
                            ToPartyRole = RoleType.ParentOrganization,
                            StartDate = MyDateTime.Now
                        };
                        context.PartyRelationships.Add(mainCustomerRelationshdip);
                    }
                    mainCustomerRelationshdip.ToPartyId = mainCustomer.Id;

                    if (!string.IsNullOrEmpty(item.XCpc))
                    {

                        var bankCPC = (from b in context.Parties
                                       where b.ShortName == item.XCpc && b.Orgnization.IsCPCBranch
                                       select b).FirstOrDefault();

                        if (bankCPC == null)
                        {
                            logger.Warning($"Bank CPC of branch is missing for {JsonConvert.SerializeObject(item.XBranchCode)}");
                            continue;
                        }

                        var bankCPCRelationshdip = context.PartyRelationships.FirstOrDefault(x => x.FromPartyId == branchParty.Id
                                && x.FromPartyRole == RoleType.ChildOrganization
                                && x.ToPartyRole == RoleType.BankCPC);

                        if (bankCPCRelationshdip == null)
                        {
                            bankCPCRelationshdip = new PartyRelationship()
                            {
                                Id = context.Sequences.GetNextPartiesSequence(),
                                FromPartyId = branchParty.Id,
                                FromPartyRole = RoleType.ChildOrganization,
                                ToPartyRole = RoleType.BankCPC,
                                StartDate = MyDateTime.Now
                            };
                            context.PartyRelationships.Add(bankCPCRelationshdip);
                        }
                        bankCPCRelationshdip.ToPartyId = bankCPC.Id;
                    }

                    if (!string.IsNullOrEmpty(item.XAtmBranch))
                    {


                        var associatedAtmBranch = (from b in context.Parties
                                                   where b.ShortName == item.XAtmBranch
                                                   select b).FirstOrDefault();

                        if (associatedAtmBranch == null)
                        {
                            logger.Warning($"ATM branch is missing for {JsonConvert.SerializeObject(item.XCode)}");
                            continue;
                        }

                        var atmBranchRelationshdip = context.PartyRelationships.FirstOrDefault(x =>
                            x.FromPartyId == branchParty.Id &&
                            x.FromPartyRole == RoleType.ATM &&
                            x.ToPartyRole == RoleType.ATMBranch);

                        if (atmBranchRelationshdip == null)
                        {
                            atmBranchRelationshdip = new PartyRelationship()
                            {
                                Id = context.Sequences.GetNextPartiesSequence(),
                                FromPartyId = branchParty.Id,
                                FromPartyRole = RoleType.ATM,
                                ToPartyRole = RoleType.ATMBranch,
                                StartDate = MyDateTime.Now
                            };
                            context.PartyRelationships.Add(atmBranchRelationshdip);
                        }
                        atmBranchRelationshdip.ToPartyId = associatedAtmBranch.Id;
                    }

                    //---------------------------//
                    var station = string.IsNullOrEmpty(item.XStation) ? null : context.Parties.FirstOrDefault(x => x.ShortName == item.XStation);
                    var subRegion = string.IsNullOrEmpty(item.XSubRegion) ? null : context.Parties.FirstOrDefault(x => x.ShortName == item.XSubRegion);

                    branchParty.StationId = station?.Id;
                    branchParty.SubregionId = subRegion?.Id;
                    branchParty.RegionId = subRegion?.RegionId;

                    await cache.SetStationName(branchParty.Id, item.XStationDescription);

                    if (branchParty.RegionId.HasValue)
                    {
                        var region = context.Parties.FirstOrDefault(x => x.Id == branchParty.RegionId);
                        if (region != null)
                        {
                            await cache.SetRegionName(branchParty.Id, region.FormalName);
                            await cache.SetRegionAbbr(branchParty.Id, region.Abbrevation);
                        }
                    }

                    await context.SaveChangesAsync();
                    short geolocationVersion = 0;
                    branchParty.Orgnization.ExternalBranchType = $"{branchParty.Orgnization.ExternalBranchType}###" +
                             $"{branchParty.Orgnization.OrganizationType}";

                    if (!string.IsNullOrWhiteSpace(item.XLatitude) && !string.IsNullOrWhiteSpace(item.XLongitude))
                    {
                        if (double.TryParse(item.XLatitude, out double lat) && double.TryParse(item.XLongitude, out double lng))
                        {
                            if (lat > -90 && lat < 90 && lng > -180 && lng < 180)
                            {
                                if (lat != (branchParty.Orgnization.Geolocation?.Y).GetValueOrDefault() || lng != (branchParty.Orgnization.Geolocation?.X).GetValueOrDefault())
                                {
                                    var currentLocation = geometryFactory.CreatePoint(new Coordinate(lng, lat));
                                    branchParty.Orgnization.Geolocation = currentLocation;
                                    branchParty.Orgnization.GeolocationUpdateAt = DateTime.Now;
                                    branchParty.Orgnization.GeolocationVersion++;
                                    geolocationVersion = branchParty.Orgnization.GeolocationVersion;
                                    await cache.SetGeoCoordinate(branchParty.Id, new Web.Shared.ViewModels.Point(lat, lng));
                                }
                            }
                            else
                            {
                                logger.Warning($"Invalid lat/longs for branch {item.XCode}-{item.XBranchCode}  -> lat:{item.XLatitude}, lng:{item.XLongitude}");
                            }
                        }
                        branchParty.Orgnization.LocationStatus = item.XGeoStatus == "A" ? DataRecordStatus.Approved : DataRecordStatus.Draft;
                        context.SaveChanges();
                    }
                    //if (geolocationVersion > 0)
                    //{
                    //    await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE IntraPartyDistances SET DistanceStatus = '0' WHERE DistanceStatus = '1' AND (FromPartyId = {branchParty.Id} OR ToPartyId = {branchParty.Id})");
                    //    await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND (FromPartyId = {branchParty.Id} OR ToPartyId = {branchParty.Id})");
                    //}

                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString() + ex.InnerException?.ToString());
                }
            }

            if (syncAll)
            {
                //await context1.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET IsActive = 0 WHERE SycStatus = 0 AND Id in (SELECT Id from Orgnizations WHERE OrganizationType & {(int)OrganizationType.CustomerBranch} = {(int)OrganizationType.CustomerBranch})");
            }
        }
        #endregion

        #region Employee

        private async Task UpdatePdwEmployees(IEnumerable<PdwEmployeeMaster> employees, IServiceScope scope, EmploymentType employmentType, bool syncAll)
        {
            var context2 = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (syncAll)
            {
                await context2.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET SycStatus = 0 WHERE Id in (SELECT Id from People WHERE EmploymentType = 2)");
            }
            int total = employees.Count();
            try
            {
                foreach (var item in employees)
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var party = await context.Parties.Include(x => x.People).FirstOrDefaultAsync(x => x.ShortName == item.XCode);
                    var person = party?.People;

                    if (party == null || person == null)
                    {
                        party = new Party()
                        {
                            Id = context.Sequences.GetNextPartiesSequence(),
                            ShortName = item.XCode,
                            ExternalId = (int)item.XrowId.GetValueOrDefault()
                        };
                        context.Parties.Add(party);

                        person = new Person()
                        {
                            Id = party.Id
                        };

                        context.People.Add(person);
                    }

                    party.PartyType = PartyType.Person;
                    party.FormalName = item.XName;
                    party.Address = item.XAddress;
                    party.PersonalContactNo = item.XPersonalMobile;
                    party.ExternalId = (int)item.XrowId.GetValueOrDefault();
                    party.UpdatedAtErp = item.ModDate;
                    party.PersonalEmail = item.XPersonalEmail;
                    party.OfficialEmail = item.XOfficialEmail;
                    party.PersonalContactNo = item.XPersonalMobile;
                    party.OfficialContactNo = item.XOfficialMobile;
                    party.SycStatus = item.XStatus == "A" ? (byte)1 : (byte)0;
                    person.NationalId = item.XCnic;
                    person.Status = party.IsActive = item.XStatus == "A";

                    if (!string.IsNullOrEmpty(item.XDesignation))
                        person.DesignationId = Convert.ToInt32(item.XDesignation);

                    person.DesignationDesc = item.XDesignationDescription;
                    person.EmploymentType = employmentType;
                    if (!string.IsNullOrEmpty(item.XJoiningDate))
                    {
                        person.JoiningDate = DateTime.ParseExact(item.XJoiningDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(item.XDateOfBirth))
                    {
                        person.DateOfBirth = DateTime.ParseExact(item.XDateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }
                    person.Gender = item.XGender == "M" ? Gender.Male : Gender.Female;
                    var station = await context.Parties.FirstOrDefaultAsync(x => x.ShortName == item.XSection);
                    if (station != null)
                    {
                        party.StationId = station.Id;
                        party.SubregionId = station.SubregionId;
                        party.RegionId = station.RegionId;
                    }

                    var relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == person.Id && x.ToPartyId == 1);
                    if (relationship == null)
                    {
                        relationship = new PartyRelationship
                        {
                            Id = context.Sequences.GetNextPartiesSequence(),
                            FromPartyId = person.Id,
                            FromPartyRole = RoleType.Employee,
                            ToPartyId = 1,
                            ToPartyRole = RoleType.Employer,
                            StartDate = person.JoiningDate.GetValueOrDefault(),

                        };
                        context.PartyRelationships.Add(relationship);
                    }

                    logger.Information($"PDW {party.ShortName} {party.SycStatus} -> {item.XStatus}");
                    await context.SaveChangesAsync();
                    //await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET SycStatus = 1 WHERE Id  = {party.Id}");
                    //await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE People SET Status = {party.People.Status} WHERE Id = {party.Id}");
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            if (syncAll)
            {
                await context2.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET IsActive = 0 WHERE SycStatus = 0 AND Id in (SELECT Id from People WHERE EmploymentType = 2)");
            }
        }


        private async Task UpdatePayEmployees(IEnumerable<PayEmployeeMaster> employees, IServiceScope scope, EmploymentType employmentType, bool syncAll)
        {
            var context2 = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (syncAll)
            {
                await context2.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET SycStatus = 0 WHERE Id in (SELECT Id from People WHERE EmploymentType = 1)");
            }
            foreach (var item in employees)
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var party = await context.Parties.FirstOrDefaultAsync(x => x.ShortName == item.XCode);
                var person = await context.People.FirstOrDefaultAsync(x => x.Origin.ShortName == item.XCode);

                if (party == null || person == null)
                {
                    party = new Party()
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        PartyType = PartyType.Person,
                        ShortName = item.XCode,
                        ExternalId = (int)item.XrowId.GetValueOrDefault()
                    };
                    context.Parties.Add(party);

                    person = new Person()
                    {
                        Id = party.Id
                    };

                    context.People.Add(person);

                }
                party.FormalName = item.XName;
                party.Address = item.XAddress;
                party.PersonalContactNo = item.XPersonalMobile;
                party.ExternalId = (int)item.XrowId.GetValueOrDefault();
                party.UpdatedAtErp = item.ModDate;
                party.PersonalEmail = item.XPersonalEmail;
                party.OfficialEmail = item.XOfficialEmail;
                party.PersonalContactNo = item.XPersonalMobile;
                party.OfficialContactNo = item.XOfficialMobile;
                party.SycStatus = item.XStatus == "A" ? (byte)1 : (byte)0;

                person.NationalId = item.XCnic;
                person.Status = party.IsActive = item.XStatus == "A";

                if (!string.IsNullOrEmpty(item.XDesignation))
                    person.DesignationId = Convert.ToInt32(item.XDesignation);

                person.DesignationDesc = item.XDesignationDescription;
                person.EmploymentType = employmentType;
                if (!string.IsNullOrEmpty(item.XJoiningDate))
                {
                    person.JoiningDate = DateTime.ParseExact(item.XJoiningDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(item.XDateOfBirth))
                {
                    person.DateOfBirth = DateTime.ParseExact(item.XDateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                person.Gender = item.XGender == "M" ? Gender.Male : Gender.Female;

                var station = await context.Parties.FirstOrDefaultAsync(x => x.ShortName == item.XSection);
                if (station != null)
                {
                    party.StationId = station.Id;
                    party.SubregionId = station.SubregionId;
                    party.RegionId = station.RegionId;
                }
                var relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == person.Id && x.ToPartyId == 1);
                if (relationship == null)
                {
                    relationship = new PartyRelationship
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        FromPartyId = person.Id,
                        FromPartyRole = RoleType.Employee,
                        ToPartyId = 1,
                        ToPartyRole = RoleType.Employer,
                        StartDate = person.JoiningDate.GetValueOrDefault()
                    };
                    context.PartyRelationships.Add(relationship);
                }

                await context.SaveChangesAsync();
                logger.Information($"PAY {party.ShortName} {party.SycStatus} -> {item.XStatus}");
                //await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET SycStatus = 1 WHERE Id  = {party.Id}");
                //await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE People SET Status = {party.People.Status} WHERE Id = {party.Id}");
            }
            if (syncAll)
            {
                await context2.Database.ExecuteSqlInterpolatedAsync($"UPDATE Parties SET IsActive = 0 WHERE SycStatus = 0 AND Id in (SELECT Id from People WHERE EmploymentType = 1)");
            }

        }
        #endregion
    }

}
