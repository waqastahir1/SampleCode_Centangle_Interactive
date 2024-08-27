using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    class ShipmentsPushService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        public ShipmentsPushService(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public void Start()
        {
            var firstOfMonth = new DateTime(MyDateTime.Today.Year, MyDateTime.Today.Month, 1);
            // Pushes shipment partially
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    Consignment consignment = null;
                    try
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var gbms = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var shipmentsCache = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                        try
                        {

                            consignment = (from x in context.Consignments
                                              .Include(x => x.Customer.Orgnization)
                                              .Include(x => x.FromParty.Orgnization)
                                              .Include(x => x.ToParty.Orgnization)
                                              .Include(x => x.Customer)
                                              .Include(x => x.MainCustomer)
                                              .Include(x => x.BillBranch.Orgnization)
                                              .Include(x => x.ShipmentSealCodes)
                                           join t in context.ConsignmentStates on x.Id equals t.ConsignmentId
                                           orderby x.ActualDeliveryTime
                                           where
                                             //x.Id == 1000380914 &&
                                             (x.ConsignmentStatus == ConsignmentStatus.Pushing
                                             || x.ConsignmentStatus == ConsignmentStatus.RePush
                                             || (x.ConsignmentStatus == ConsignmentStatus.PushingFailed && x.PostingMessage.ToLower().StartsWith("violation of primary key")))
                                             && x.ShipmentType > ShipmentType.Unknown
                                             && t.ConsignmentStateType == ConsignmentDeliveryState.Delivered
                                             && t.TimeStamp > firstOfMonth
                                             && ((x.CurrencySymbol == CurrencySymbol.PKR && x.Amount > 0) || (x.AmountPKR > 0))
                                           select x).FirstOrDefault();

                            //.FirstOrDefaultAsync(x => x.Id == 378323);

                            context.Database.ExecuteSqlRaw("update consignments set ConsignmentStatus = 3 where ConsignmentStatus = 0");

                            context.Database.ExecuteSqlRaw($"update consignments set ConsignmentStatus = 5  where ConsignmentStatus = 3 AND ShipmentType > 0 AND DistanceStatus = 4");


                            if (consignment == null)
                            {
                                Console.WriteLine("Sleeping");
                                await Task.Delay(TimeSpan.FromMinutes(2));
                                continue;
                            }

                            //Temp Fix for Currencies other than PKR, because of mobile app issue (feature Unavailibity in mobile app)
                            if (consignment.CurrencySymbol != CurrencySymbol.PKR && consignment.Amount == 0) consignment.Amount = consignment.AmountPKR;
                            
                            
                            consignment.ManualShipmentCode = string.IsNullOrWhiteSpace(consignment.ManualShipmentCode) ? $"P{consignment.Id}" : consignment.ManualShipmentCode;

                            await DeleteShipmentIfExists(consignment.Id.ToString(), consignment.ManualShipmentCode, gbms);

                            string sealCodeErrors = string.Empty;

                            var sealcodes = consignment.ShipmentSealCodes.ToList();
                            if (sealcodes.Count() == 0)
                            {
                                throw new InvalidShipmentDataException($"Seal Codes are missing for shipment #{consignment.ShipmentCode}", ConsignmentStatus.PushingFailed);
                            }

                            foreach (var sealcode in sealcodes)
                            {
                                var duplicateShipments = (from seal in gbms.RbCitShipmentSealsDetails
                                                          join spt in gbms.RbCitShipments on seal.MasterId equals spt.MasterId
                                                          where seal.XSealCode == sealcode.SealCode
                                                          && (spt.XShipmentNo != consignment.ManualShipmentCode || spt.XPortalReference != consignment.ShipmentCode)

                                                          select $"{spt.XShipmentNo}, {spt.XPortalReference}").ToList();
                                if (duplicateShipments.Count > 0)
                                {
                                    sealCodeErrors += $"{sealcode.SealCode} is used in {string.Join(" + ", duplicateShipments)}";
                                }
                            }

                            if (sealCodeErrors != string.Empty)
                            {
                                throw new InvalidShipmentDataException(sealCodeErrors, ConsignmentStatus.DuplicateSeals);
                            }
                            var delivery = await (from d in context.ConsignmentDeliveries
                                                  from pa in context.AssetAllocations.Where(x => x.PartyId == d.CrewId).DefaultIfEmpty()
                                                  from a in context.Assets.Where(x => x.Id == pa.AssetId).DefaultIfEmpty()
                                                  where d.ConsignmentId == consignment.Id
                                                  select new
                                                  {
                                                      AssetCode = a.Code,
                                                      d.ActualPickupTime,
                                                      d.ActualDropTime
                                                  }).FirstOrDefaultAsync();


                            if (delivery == null)
                            {
                                consignment.ConsignmentStatus = ConsignmentStatus.PushingFailed;
                                consignment.PostingMessage = "This consignment is not yet assigned to any Crew";
                                consignment.PostedAt = MyDateTime.Now;
                                context.SaveChanges();
                                throw new InvalidShipmentDataException("This consignment is not yet assigned to any Crew", ConsignmentStatus.PushingFailed);

                            }

                            var intraPartyDistance = await context.IntraPartyDistances.OrderByDescending(x => x.DistanceStatus)
                            .FirstOrDefaultAsync(x => (x.FromPartyId == consignment.FromPartyId && x.ToPartyId == consignment.ToPartyId)
                            || (x.FromPartyId == consignment.ToPartyId && x.ToPartyId == consignment.FromPartyId));
                            var distance = (intraPartyDistance?.Distance).GetValueOrDefault();

                            //only for when dropoff is changed
                            IntraPartyDistance changedToPartyDistance = new IntraPartyDistance();
                            if (consignment.ToChangedPartyId != null && consignment.IsToChangedPartyVerified)
                            {
                                changedToPartyDistance = await context.IntraPartyDistances.OrderByDescending(x => x.DistanceStatus)
                            .FirstOrDefaultAsync(x => (x.FromPartyId == consignment.FromPartyId && x.ToPartyId == consignment.ToChangedPartyId)
                            || (x.FromPartyId == consignment.ToChangedPartyId && x.ToPartyId == consignment.FromPartyId));
                            }
                            var changedToDistance = (changedToPartyDistance?.Distance).GetValueOrDefault();

                            if (distance > 0 && ((consignment.ToChangedPartyId != null && consignment.IsToChangedPartyVerified) ? changedToDistance > 0 : true))
                            {
                                consignment.Distance = distance + changedToDistance;
                                consignment.DistanceStatus = (consignment.ToChangedPartyId != null && consignment.IsToChangedPartyVerified) ? (changedToPartyDistance?.DistanceStatus).GetValueOrDefault() : (intraPartyDistance?.DistanceStatus).GetValueOrDefault();
                            }
                            if (consignment.Distance == 0 || ((consignment.ToChangedPartyId != null && consignment.IsToChangedPartyVerified) ? changedToDistance == 0 : false))
                            {
                                consignment.ConsignmentStatus = ConsignmentStatus.PushingFailed;
                                consignment.PostingMessage = "Distance between sender and receiver branch is not calculated";
                                consignment.PostedAt = MyDateTime.Now;
                                context.SaveChanges();
                                throw new InvalidShipmentDataException($"Distance Issue {consignment.ShipmentCode}", ConsignmentStatus.DistanceIssue);
                            }
                            if (consignment.DistanceStatus <= DataRecordStatus.Draft)
                            {
                                consignment.ConsignmentStatus = ConsignmentStatus.PushingFailed;
                                consignment.PostingMessage = "Distance is un-approved";
                                consignment.PostedAt = MyDateTime.Now;
                                context.SaveChanges();
                                throw new InvalidShipmentDataException($"Distance is un-approved {consignment.ShipmentCode}", ConsignmentStatus.DistanceIssue);

                            }

                            var states = context.ConsignmentStates.Where(x => x.ConsignmentId == consignment.Id)
                            .ToList();

                            var pickupDate = states.OrderBy(x => x.TimeStamp).First(x => x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp;

                            var DeliveryDate = states.OrderBy(x => x.TimeStamp).Last(x => x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp;

                            var shipmentCharges = context.ShipmentCharges.Where(x => x.ConsignmentId == consignment.Id).ToList();
                            var tollTax = shipmentCharges.FirstOrDefault(x => x.ChargeTypeId == 2)?.Amount;
                            var waitingMinutes = shipmentCharges.FirstOrDefault(x => x.ChargeTypeId == 1)?.Amount;

                            var km = consignment.Distance / 1000d;
                            var mainCustomerView = await gbms.RbMainCustomerManagements.FirstOrDefaultAsync(x => x.XCode == consignment.MainCustomer.ShortName);

                            if (consignment.FromParty.SubregionId == null)
                                throw new InvalidShipmentDataException($"Subregion is not defined for pickup branch ({consignment.FromParty.ShortName})", ConsignmentStatus.PushingFailed);

                            if (consignment.FromParty.StationId == null)
                                throw new InvalidShipmentDataException($"Station is not defined for pickup branch ({consignment.FromParty.ShortName})", ConsignmentStatus.PushingFailed);

                            var regionCode = context.Parties
                            .FirstOrDefault(x => x.Id == consignment.FromParty.SubregionId).ShortName;

                            var stationCode = context.Parties
                           .FirstOrDefault(x => x.Id == consignment.FromParty.StationId).ShortName;


                            // Vault shipment must properly pushed to GBMS with count of nights.
                            var vaultNights = 0;
                            if (pickupDate.HasValue && DeliveryDate.HasValue)
                            {
                                vaultNights = Convert.ToInt32((DeliveryDate.Value.Date - pickupDate.Value.Date).TotalDays);
                            }
                            try
                            {
                                File.AppendAllLines("c:\\temp\\logs.txt", new string[] { $"EXEC SP_CIT_SHIPMENT_DELETE '{consignment.ManualShipmentCode}','{consignment.ShipmentCode}';" });
                            }
                            catch
                            { }

                            var parameters = new string[] {

                            consignment.ManualShipmentCode,
                            pickupDate?.ToString("dd-MMM-yyyy"),
                            $"{Convert.ToInt32(consignment.ShipmentType)}",
                            consignment.MainCustomer.ShortName,
                            consignment.FromParty.ShortName, // from party
                            consignment.ToParty.ShortName, // to party
                            consignment.BillBranch.ShortName, // bill branch
                            consignment.Customer.ShortName,  // customer
                            delivery.AssetCode,
                            pickupDate?.ToString("dd-MMM-yyyy"),
                            pickupDate?.ToString("1900-01-01 HH:mm"),
                            DeliveryDate?.ToString("dd-MMM-yyyy"),
                            DeliveryDate?.ToString("1900-01-01 HH:mm"),
                            $"{consignment.NoOfBags}", // No of Bags
                            $"{context.ShipmentSealCodes.Count(x=>x.ConsignmentId == consignment.Id)}", // No of Seals
                            consignment.Amount.ToString(),  // Amount carried
                            Math.Round(km, 1).ToString(),
                            $"{tollTax.GetValueOrDefault()}", // Toll Tax
                            $"{waitingMinutes.GetValueOrDefault()}", // Waiting Minutes
                            "0", // Additional Cost
                            $"{vaultNights}", // Vault Nights
                            "Y", // Autobill
                            "0",  // Fixed Amount
                            $"{regionCode}",  // Region
                            $"{stationCode}", //Station
                            $"{consignment.ShipmentCode}"//Portal Reference code 
                            };

                            var result = gbms.SpResults.FromSqlRaw("SP_CIT_SHIPMENT_INSERT " +
                                    " @p0,  @p1,  @p2,  @p3,  @p4,  @p5,  @p6, " +
                                    " @p7,  @p8,  @p9,  @p10, @p11, @p12, @p13, @p14, " +
                                    " @p15, @p16, @p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24, @p25", parameters: parameters).ToList().FirstOrDefault();

                            if (result.X_Mess.ToLower().Trim().Contains("violation of primary key"))
                                continue;

                            consignment.PostingMessage = result.X_Mess;
                            consignment.ConsignmentStatus = result?.X_Error == "F" ?
                            ConsignmentStatus.PartiallyPushed : ConsignmentStatus.PushingFailed;
                            consignment.PostedAt = MyDateTime.Now;
                            context.SaveChanges();
                            logger.Debug("{0} {1}", $"Processed {consignment.ShipmentCode} {regionCode} {stationCode} {delivery.AssetCode}, pickup time", pickupDate.GetValueOrDefault());

                        }
                        catch (InvalidShipmentDataException ex)
                        {
                            if (consignment != null && context != null)
                            {
                                consignment.ConsignmentStatus = ex.ConsignmentStatus;
                                consignment.PostingMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 250));
                                consignment.PostedAt = MyDateTime.Now;
                                var effectedRows = context.SaveChanges();
                                logger.Debug($"effectedRows {effectedRows}");
                            }
                            logger.Error(ex.ConsignmentStatus + "-----" + ex.Message);
                        }

                        catch (Exception ex)
                        {
                            if (consignment != null && context != null)
                            {
                                consignment.ConsignmentStatus = ConsignmentStatus.PushingFailed;
                                consignment.PostingMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 250));
                                consignment.PostedAt = MyDateTime.Now;
                                var effectedRows = context.SaveChanges();
                                logger.Debug($"effectedRows {effectedRows}");
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
                        logger.Error(ex.ToString());
                    }


                }
            });


            // Pushes Sealcodes
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
                        var consignment = await context.Consignments
                          .Include(x => x.ShipmentSealCodes)
                          .FirstOrDefaultAsync(x => x.ConsignmentStatus == ConsignmentStatus.PartiallyPushed && x.ShipmentSealCodes.Count > 0);

                        if (consignment == null)
                        {
                            await Task.Delay(TimeSpan.FromMinutes(2));
                            continue;
                        }

                        try
                        {
                            consignment.ManualShipmentCode = string.IsNullOrWhiteSpace(consignment.ManualShipmentCode) ? $"P{consignment.Id}" : consignment.ManualShipmentCode;
                            var pickupDate = context.ConsignmentStates.Where(x => x.ConsignmentId == consignment.Id).OrderBy(x => x.TimeStamp).First(x => x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp;

                            //var pickupDate = context.ConsignmentStates.Where(x => x.ConsignmentId == consignment.Id &&
                            //x.ConsignmentStateType <= ConsignmentDeliveryState.InTransit)
                            //.Max(x => x.TimeStamp);

                            var sealcodes = consignment.ShipmentSealCodes.ToList();
                            if (sealcodes.Count == 0)
                            {
                                throw new InvalidShipmentDataException($"Seal Codes are missing for shipment #{consignment.ShipmentCode}", ConsignmentStatus.PushingFailed);
                            }

                            string sealCodeErrors = string.Empty;
                            foreach (var sealcode in sealcodes)
                            {
                                var duplicateShipments = (from seal in gbms.RbCitShipmentSealsDetails
                                                          join spt in gbms.RbCitShipments on seal.MasterId equals spt.MasterId
                                                          where seal.XSealCode == sealcode.SealCode
                                                          select $"{spt.XShipmentNo}, {spt.XPortalReference}").ToList();
                                if (duplicateShipments.Count > 0)
                                {

                                    sealCodeErrors += $"{sealcode.SealCode} is used in {string.Join(" + ", duplicateShipments)}";
                                }
                            }
                            if (sealCodeErrors != string.Empty)
                            {
                                await DeleteShipmentIfExists(consignment.ShipmentCode, consignment.ManualShipmentCode, gbms);
                                throw new InvalidShipmentDataException(sealCodeErrors, ConsignmentStatus.DuplicateSeals);
                            }
                            foreach (var sealcode in sealcodes)
                            {
                                var result = gbms.SpResults.FromSqlRaw("SP_CIT_SHIPMENT_SEAL_INSERT @p0,  @p1,  @p2",

                                parameters: new string[] {

                                    consignment.ManualShipmentCode,
                                    pickupDate?.ToString("dd-MMM-yyyy"),
                                    sealcode.SealCode
                                }).AsEnumerable().FirstOrDefault();

                                if (result?.X_Error == "T")
                                {
                                    throw new InvalidShipmentDataException($"Unable to post seal code {sealcode.SealCode} to gbms, error: {result.X_Mess}", ConsignmentStatus.PushingFailed);
                                }
                                logger.Debug("{0}", $"Posted SealCode {consignment.ShipmentCode} {sealcode.SealCode}");
                            }

                            consignment.ConsignmentStatus = ConsignmentStatus.Pushed;
                            consignment.PostedAt = MyDateTime.Now;
                            context.SaveChanges();
                        }
                        catch (InvalidShipmentDataException ex)
                        {
                            consignment.ConsignmentStatus = ex.ConsignmentStatus;
                            consignment.PostingMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 250));
                            consignment.PostedAt = MyDateTime.Now;
                            context.SaveChanges();
                            if (ex.ConsignmentStatus == ConsignmentStatus.Calculated)
                            {
                                logger.Warning(ex.Message);
                            }
                            else
                            {
                                logger.Error(ex.Message);
                            }
                        }
                        if (consignment != null)
                        {
                            await shipmentsCache.SetShipment(consignment.Id, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("{0}", ex.Message);
                    }

                }
            });


        }

        public void RecalculateDistances()
        {
            _ = Task.Run(() =>
            {
                logger.Information("Started");
                using var scope = serviceScopeFactory.CreateScope();
                var gbms = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                gbms.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
                context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

                //var drfatShipments = await gbms.RbCitShipments.Where(x => x.DocumentStatus == "U"
                //&& x.WorkflowStatus == "1" && x.PeriodName == "Nov 2020" && x.XPortalReference != null)
                //.ToListAsync();

                //logger.Information($"Fetched {drfatShipments.Count}");

                //var draftShipmentIds = drfatShipments.Select(x => x.XPortalReference);

                //File.WriteAllText($"c:\\temp\\draft_shipments{DateTime.Now.Minute}.txt", JsonConvert.SerializeObject(drfatShipments));

                //var citShipments = context.Consignments.Where(x => draftShipmentIds.Contains(x.ShipmentCode));
                //await citShipments.ForEachAsync(x => x.ConsignmentStatus = ConsignmentStatus.RecalculateDistance);
                //var rows = await context.SaveChangesAsync();
                //logger.Information($"Updated {rows}"); 
                //while (true)
                //{
                //    try
                //    {
                //        var consignment = await context.Consignments
                //                          .Include(x => x.FromParty.Orgnization)
                //                          .Include(x => x.ToParty.Orgnization)
                //                          .OrderBy(x => x.Id)

                //                            .FirstOrDefaultAsync(x => x.ConsignmentStatus == ConsignmentStatus.RecalculateDistance && x.Id > 78750);

                //        if (consignment == null)
                //        {
                //            logger.Information($"Finished");
                //            await Task.Delay(TimeSpan.FromMinutes(2));
                //            continue;
                //        }

                //        try
                //        {
                //            var gbmsShipment = gbms.RbCitShipments 
                //            .FirstOrDefault(x => x.XPortalReference == consignment.ShipmentCode);
                //            if (gbmsShipment == null)
                //            {
                //                throw new InvalidShipmentDataException("Portal Reference missmatch", ConsignmentStatus.DistanceRecalculationFailed);
                //            }

                //            var consignmentDistance = await context.IntraPartyDistances.FirstOrDefaultAsync(x => x.FromPartyId == consignment.FromPartyId
                //            && x.ToPartyId == consignment.ToPartyId);

                //            if ((consignment.FromParty.Orgnization.LocationStatus <= DataRecordStatus.Draft))
                //            {
                //                throw new InvalidShipmentDataException($"Lat/Long for collection branch  {consignment.FromParty.ShortName} is unapproved", ConsignmentStatus.DistanceRecalculationFailed);
                //            }
                //            if ((consignment.ToParty.Orgnization.LocationStatus <= DataRecordStatus.Draft))
                //            {
                //                throw new InvalidShipmentDataException($"Lat/Long for Dropoff branch  {consignment.ToParty.ShortName} is unapproved", ConsignmentStatus.DistanceRecalculationFailed);
                //            }
                //            if((consignment.FromParty.Orgnization.Geolocation?.X).GetValueOrDefault() == 0 || (consignment.FromParty.Orgnization.Geolocation?.Y).GetValueOrDefault() == 0)
                //            {
                //                throw new InvalidShipmentDataException($"Lat/Long is not added collection branch  {consignment.FromParty.ShortName}", ConsignmentStatus.DistanceRecalculationFailed);
                //            }
                //            if ((consignment.ToParty.Orgnization.Geolocation?.X).GetValueOrDefault() == 0 || (consignment.ToParty.Orgnization.Geolocation?.Y).GetValueOrDefault() == 0)
                //            {
                //                throw new InvalidShipmentDataException($"Lat/Long is not added collection branch  {consignment.ToParty.ShortName}", ConsignmentStatus.DistanceRecalculationFailed);
                //            }


                //            if (consignmentDistance == null)
                //            {
                //                consignmentDistance = new IntraPartyDistance()
                //                {
                //                    FromPartyId = consignment.FromPartyId,
                //                    ToPartyId = consignment.ToPartyId
                //                };
                //                context.IntraPartyDistances.Add(consignmentDistance);
                //            }
                //            if (consignmentDistance.DistanceStatus != DataRecordStatus.AutoApproved)
                //            {
                //                var r = GMapsService.ClaculateDistanceUsinGoogle(new Point(consignment.FromParty.Orgnization.Geolocation.Y, consignment.FromParty.Orgnization.Geolocation.X),
                //                    new Point(consignment.ToParty.Orgnization.Geolocation.Y, consignment.ToParty.Orgnization.Geolocation.X), consignment.ShipmentCode);
                //                consignmentDistance.AverageTravelTime = (int)r.duration.Value;
                //                consignmentDistance.Distance = (int)r.distance.Value;
                //                logger.Information("Calculated from distance");
                //            }
                //            else
                //            {
                //                logger.Information("Resuing Auto Approved distance");
                //            }


                //            consignmentDistance.UpdatedBy = consignmentDistance.UpdatedBy == null ? "System" : $"System at {DateTime.Now}_{consignmentDistance.UpdatedBy}";
                //            consignmentDistance.UpdateAt = DateTime.Now;

                //            consignmentDistance.DistanceStatus = DataRecordStatus.AutoApproved; 
                //            consignment.Distance = consignmentDistance.Distance;
                //            consignment.DistanceStatus = DataRecordStatus.AutoApproved;
                //            context.SaveChanges();

                //            if (consignment.Distance == 0)
                //            { 
                //                throw new InvalidShipmentDataException("Distance between sender and receiver branch is not calculated", ConsignmentStatus.DistanceIssue);
                //            }

                //            var result = gbms.SpResults.FromSqlRaw("SP_CIT_SHIPMENT_KMS_UPDATE @p0,  @p1,  @p2",

                //               parameters: new string[] {

                //                    gbmsShipment.XShipmentNo,
                //                    DateTime.ParseExact(gbmsShipment.XDate, "dd-MM-yyyy", null).ToString("dd-MMM-yyyy"),
                //                   Math.Round( consignment.Distance/1000, 1).ToString()

                //               }).AsEnumerable().FirstOrDefault();

                //            if (result?.X_Error == "T")
                //            {
                //                throw new InvalidShipmentDataException($"gbms error: {result.X_Mess}", ConsignmentStatus.DistanceRecalculationFailed);
                //            }
                //            logger.Information($"Recalculated {consignment.ShipmentCode}");

                //            consignment.ConsignmentStatus = ConsignmentStatus.DistanceRecalculated; 
                //            context.SaveChanges();
                //        }
                //        catch (InvalidShipmentDataException ex)
                //        {
                //            consignment.ConsignmentStatus = ex.ConsignmentStatus;
                //            consignment.PostingMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 250));
                //            consignment.PostedAt = MyDateTime.Now;
                //            context.SaveChanges();
                //            if (ex.ConsignmentStatus == ConsignmentStatus.Calculated)
                //            {
                //                logger.Warning(ex.Message);
                //            }
                //            else
                //            {
                //                logger.Error(ex.Message);
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            consignment.ConsignmentStatus = ConsignmentStatus.DistanceRecalculationFailed;
                //            consignment.PostingMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 250));
                //            consignment.PostedAt = MyDateTime.Now;
                //            context.SaveChanges();
                //            logger.Error(ex.Message);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Error(ex.ToString());
                //    }
                //}

            });
        }

        private async Task DeleteShipmentIfExists(string shipmentCode, string manualShipmentCode, SOS_VIEWSContext gbms)
        {
            var shipment = await gbms.RbCitShipments.FirstOrDefaultAsync(x => x.XPortalReference == shipmentCode || x.XShipmentNo == manualShipmentCode);

            if (shipment != null)
            {
                if (shipment.WorkflowStatus == "1" && shipment.DocumentStatus == "U")
                {
                    var result = gbms.SpResults.FromSqlRaw("SP_CIT_SHIPMENT_DELETE  @p0,  @p1 ",
                   parameters: new string[] {
                                   "",
                                   shipmentCode
                   })
                   .AsEnumerable().FirstOrDefault();

                    logger.Warning($"Deleted {shipmentCode} {result.X_Mess}");
                }
                else
                {
                    throw new InvalidShipmentDataException("Shipment cannot be deleted from gbms", ConsignmentStatus.Calculated);
                }
            }
        }

        public void DeliverStuckShipments()
        {
            // Pushes shipment partially
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

                        var shipment = await (from c in context.Consignments
                                              join s in context.ConsignmentStates on c.Id equals s.ConsignmentId
                                              where s.ConsignmentStateType == ConsignmentDeliveryState.Delivered && s.Status == StateTypes.Confirmed
                                              && c.ConsignmentStateType < ConsignmentDeliveryState.Delivered
                                              && c.CreatedAt > DateTime.Today
                                              select new { c, s }).FirstOrDefaultAsync();

                        if (shipment == null)
                        {
                            await Task.Delay(5000);
                            continue;
                        }

                        shipment.c.ConsignmentStateType = ConsignmentDeliveryState.Delivered;
                        shipment.c.ActualDeliveryTime = shipment.s.TimeStamp;
                        await context.SaveChangesAsync();
                        var chacheShipment = await shipmentsCache.GetShipment(shipment.c.Id);
                        chacheShipment.ConsignmentStateType = ConsignmentDeliveryState.Delivered;
                        await shipmentsCache.SetShipment(chacheShipment.Id, chacheShipment);

                        logger.Information("{0}", $"Delivered {chacheShipment.Id}");
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