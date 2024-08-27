using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Extenstions;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Shared.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    public class MissingShipments
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;
        public MissingShipments(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public void StartFindingMissingShipments()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var time = DateTime.Now.TimeOfDay;
#if !DEBUG
                    if (!(time.Hours > 20 && time.Hours < 24))
                    {
                        logger.Information($"Missing Shipments: Time is not in between 8PM to 1AM, time now is {DateTime.Now.ToString("dd-MM-yy hh:mm tt")}, Sleeping for 1 hours...");
                        await Task.Delay(TimeSpan.FromHours(1));
                    }
#endif
                    bool IsTableHaveRecords = true;
                    int SkipRecords = 0;
                    while (IsTableHaveRecords)
                    {
                        try
                        {
                            var startDate = new DateTime(2023, 8, 1);
                            logger.Information($"Missing Shipments: Starting at {DateTime.Now.ToString("dd-MM-yy hh:mm tt")}...");

                            using (var scope = serviceScopeFactory.CreateScope())
                            {
                                var sosViewsContext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();

                                int Take = 0;

#if DEBUG
                                Take = 15000;
#else
                                Take = 1500000;
#endif
                                var query = (from c in sosViewsContext.RbCitShipments
                                             where c.DDate.Date >= startDate.Date
                                             select new MissingShipmentsReportViewModel()
                                             {
                                                 Id = c.MasterId,
                                                 MissingShipmentNumberFrom = c.XShipmentNo.TransformIntFromNumerics(),
                                                 PreviousShipmentNumber = c.XShipmentNo,
                                                 PreviousShipmentCity = c.LocationName.Replace("Cash in Transit (CIT) - ", ""),
                                                 PreviousShipmentDate = c.DDate,
                                                 PreviousShipmentPickup = $"{c.XCollection} {c.XCollectionDescription}",
                                                 PreviousShipmentDropOff = $"{c.XDelivery} {c.XDeliveryDescription}"
                                             });

                                logger.Information($"Missing Shipments: Total Records are {query.Count()}");

                                query = query.Skip(SkipRecords).Take(Take);

                                if (query == null || query.Count() < Take)
                                    IsTableHaveRecords = false;

                                SkipRecords = SkipRecords + Take;

                                var tempList = query.ToList();
                                logger.Information("Missing Shipments: Converted to list...");

                                var list = tempList.Where(x => x.MissingShipmentNumberFrom.ToString().Length > 5 && x.MissingShipmentNumberFrom.ToString().Length < 11).OrderBy(x => x.MissingShipmentNumberFrom);
                                logger.Information("Missing Shipments: list ordered ...");

                                double previousShipmentId = 0;
                                MissingShipmentsReportViewModel previousItem = new MissingShipmentsReportViewModel();
                                logger.Information($"Missing Shipments: Foreach started from {list.First().MissingShipmentNumberFrom} to {list.Last().MissingShipmentNumberFrom}...");

                                foreach (var shipment in list)
                                {
                                    DeleteShipmentIfMarkedMissing(shipment.MissingShipmentNumberFrom);

                                    if (shipment == list.First())
                                    {
                                        previousItem = shipment;
                                        continue;
                                    }
                                    logger.Information($"Missing Shipments: working on shipment {shipment.MissingShipmentNumberFrom}");

                                    previousShipmentId = previousItem.MissingShipmentNumberFrom;
                                    var currentItem = shipment;
                                    var currentShipmentId = currentItem.MissingShipmentNumberFrom;

                                    if (currentShipmentId != previousShipmentId + 1)
                                    {
                                        var From = previousShipmentId + 1;
                                        var To = currentShipmentId - 2 == previousShipmentId ? previousShipmentId + 1 : currentShipmentId - 1;

                                        logger.Information($"Missing Shipments: Applying updation for missing shipment with From: {From} and To: {To}");

                                        if (From == To)
                                            CreateUpdateMissingShipmentEntryIfNotCancelled(From, previousItem, currentItem);

                                        else if (From < To)
                                        {
                                            await PerofrmLooping(From, To, previousItem, currentItem);
                                            //for (var i = From; i <= To; i++)
                                            //{
                                            //    CreateUpdateMissingShipmentEntryIfNotCancelled(i, previousItem, currentItem);
                                            //}
                                        }
                                    }
                                    previousItem = shipment;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Missing Shipments: Error in main: " + ex.ToString());
                        }
                        logger.Information($"Missing Shipments: {SkipRecords} Records Completed");
                    }
                    logger.Information($"Missing Shipments: Completed at {DateTime.Now.ToString("dd-MM-yy hh:mm tt")} and Sleeping Now...");
                    await Task.Delay(TimeSpan.FromHours(2));

                }
            });
        }

        public void StartFindingMissingShipmentsV2()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var startDate = DateTime.Now.AddMonths(-8);
                        logger.Information($"Missing Shipments: Starting at {DateTime.Now.ToString("dd-MM-yy hh:mm tt")}...");

                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var sosViewsContext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                            int Take = 30000;

                            var query = (from c in sosViewsContext.RbCitShipments
                                         where c.DDate.Date >= startDate.Date
                                         select new MissingShipmentsReportViewModel()
                                         {
                                             Id = c.MasterId,
                                             MissingShipmentNumberFrom = c.XShipmentNo.TransformIntFromNumerics(),
                                             PreviousShipmentNumber = c.XShipmentNo,
                                             PreviousShipmentCity = c.LocationName.Replace("Cash in Transit (CIT) - ", ""),
                                             PreviousShipmentDate = c.DDate,
                                             PreviousShipmentPickup = $"{c.XCollection} {c.XCollectionDescription}",
                                             PreviousShipmentDropOff = $"{c.XDelivery} {c.XDeliveryDescription}"
                                         });
                            var count = query.Count();
                            logger.Information($"Missing Shipments: Total Records are {count}");
                            await sosViewsContext.DisposeAsync();

                            await PerformParallelForeachForAllDataWithPaginationV2(count, Take, startDate);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Missing Shipments: Error in main: " + ex.ToString());
                    }
                    logger.Information($"Missing Shipments: Completed at {DateTime.Now.ToString("dd-MM-yy hh:mm tt")} and Sleeping Now...");
                    await Task.Delay(TimeSpan.FromHours(2));

                }
            });
        }


        private async Task PerformParallelForeachForAllDataWithPagination(int records, int Take, DateTime startDate)
        {

            try
            {
                int SkipRecords = 0;

                var totalReuiredLoops = records / Take;
                totalReuiredLoops++;

                var range = Enumerable.Range(0, totalReuiredLoops);
                var options = new ParallelOptions { MaxDegreeOfParallelism = totalReuiredLoops };
                await Parallel.ForEachAsync(range, options, async (item, token) =>
                {

                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var sosViewsContext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();

                        var query = (from c in sosViewsContext.RbCitShipments
                                     where c.DDate.Date >= startDate.Date
                                     select new MissingShipmentsReportViewModel()
                                     {
                                         Id = c.MasterId,
                                         MissingShipmentNumberFrom = c.XShipmentNo.TransformIntFromNumerics(),
                                         PreviousShipmentNumber = c.XShipmentNo,
                                         PreviousShipmentCity = c.LocationName.Replace("Cash in Transit (CIT) - ", ""),
                                         PreviousShipmentDate = c.DDate,
                                         PreviousShipmentPickup = $"{c.XCollection} {c.XCollectionDescription}",
                                         PreviousShipmentDropOff = $"{c.XDelivery} {c.XDeliveryDescription}"
                                     });
                        query = query.Skip(SkipRecords).Take(Take);

                        SkipRecords = SkipRecords + Take;
                        var tempList = await query.ToListAsync();
                        logger.Information("Missing Shipments: Converted to list...");

                        var list = tempList.Where(x => x.MissingShipmentNumberFrom.ToString().Length > 5 && x.MissingShipmentNumberFrom.ToString().Length < 11).OrderBy(x => x.MissingShipmentNumberFrom);
                        logger.Information("Missing Shipments: list ordered ...");

                        await PerformFromAndToMissings(list);
                    }
                });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private async Task PerformParallelForeachForAllDataWithPaginationV2(int records, int Take, DateTime startDate)
        {
            try
            {
                List<MissingShipmentsReportViewModel> FullList = new List<MissingShipmentsReportViewModel>();
                var totalReuiredLoops = records / Take;
                totalReuiredLoops++;

                var range = Enumerable.Range(0, totalReuiredLoops);
                var options = new ParallelOptions { MaxDegreeOfParallelism = totalReuiredLoops };

                await Parallel.ForEachAsync(range, options, async (item, token) =>
                {
                    try
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var sosViewsContext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                            sosViewsContext.Database.SetCommandTimeout(300);

                            var query = (from c in sosViewsContext.RbCitShipments
                                         where c.DDate.Date >= startDate.Date
                                         select new MissingShipmentsReportViewModel()
                                         {
                                             Id = c.MasterId,
                                             MissingShipmentNumberFrom = c.XShipmentNo.TransformIntFromNumerics(),
                                             PreviousShipmentNumber = c.XShipmentNo,
                                             PreviousShipmentCity = c.LocationName.Replace("Cash in Transit (CIT) - ", ""),
                                             PreviousShipmentDate = c.DDate,
                                             PreviousShipmentPickup = $"{c.XCollection} {c.XCollectionDescription}",
                                             PreviousShipmentDropOff = $"{c.XDelivery} {c.XDeliveryDescription}"
                                         });
                            query = query.Skip(Take * item).Take(Take);

                            //SkipRecords = SkipRecords + Take;
                            if (query.Count() > 0)
                            {
                                var tempList = await query.ToListAsync();
                                var list = tempList.Where(x => x.MissingShipmentNumberFrom.ToString().Length > 5 && x.MissingShipmentNumberFrom.ToString().Length < 8);
                                logger.Information("Missing Shipments: converted to list ...");

                                if (list.Count() > 0)
                                {
                                    lock (FullList)
                                        FullList.AddRange(list);
                                    //FullList.AddRange(list);
                                    logger.Information($"Missing Shipments: list added with {list.Count()} shipments to parent list...");
                                }
                            }
                            await sosViewsContext.DisposeAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                });
                if (FullList.Count() > 0)
                    await PerformFromAndToMissings(FullList.OrderBy(x => x.MissingShipmentNumberFrom));
                //await PerformFromAndToMissingsWithForParallelV2(FullList);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task PerformFromAndToMissings(IOrderedEnumerable<MissingShipmentsReportViewModel> list)
        {

            double previousShipmentId = 0;
            MissingShipmentsReportViewModel previousItem = new MissingShipmentsReportViewModel();
            logger.Information($"Missing Shipments: Foreach started from {list.First().MissingShipmentNumberFrom} to {list.Last().MissingShipmentNumberFrom}...");

            foreach (var shipment in list)
            {
                DeleteShipmentIfMarkedMissing(shipment.MissingShipmentNumberFrom);

                if (shipment == list.First())
                {
                    previousItem = shipment;
                    continue;
                }
                //logger.Information($"Missing Shipments: working on shipment {shipment.MissingShipmentNumberFrom}");

                previousShipmentId = previousItem.MissingShipmentNumberFrom;
                var currentItem = shipment;
                var currentShipmentId = currentItem.MissingShipmentNumberFrom;

                if (currentShipmentId != previousShipmentId + 1)
                {
                    var From = previousShipmentId + 1;
                    var To = currentShipmentId - 2 == previousShipmentId ? previousShipmentId + 1 : currentShipmentId - 1;


                    if (From == To)
                        CreateUpdateMissingShipmentEntryIfNotCancelled(From, previousItem, currentItem);

                    else if (From < To)
                    {
                        logger.Information($"Missing Shipments: Applying updation for missing shipment with From: {From} and To: {To}");
                        await PerofrmLooping(From, To, previousItem, currentItem);
                    }
                    else
                    {
                        logger.Information($"Missing Shipments: Applying Reverse updation for missing shipment with From: {From} and To: {To}");
                        await PerofrmLooping(To, From, currentItem, previousItem);
                    }
                }
                previousItem = shipment;
            }
        }

        public async Task PerformFromAndToMissingsWithForParallelV2(List<MissingShipmentsReportViewModel> list)
        {
            try
            {
                list = list.Distinct().OrderBy(x => x.MissingShipmentNumberFrom).ToList();
                logger.Information($"Missing Shipments: Foreach started from {list.First().MissingShipmentNumberFrom} to {list.Last().MissingShipmentNumberFrom} with total {list.Count()} records...");
                Parallel.For(1, list.Count(), async i =>
                {
                    try
                    {
                        var shipment = list[i];
                        DeleteShipmentIfMarkedMissing(shipment.MissingShipmentNumberFrom);

                        var previousItem = list[i - 1];
                        double previousShipmentId = previousItem.MissingShipmentNumberFrom;

                        //logger.Information($"Missing Shipments: working on shipment {shipment.MissingShipmentNumberFrom}");

                        var currentItem = shipment;
                        var currentShipmentId = currentItem.MissingShipmentNumberFrom;

                        if (currentShipmentId != previousShipmentId + 1)
                        {
                            var From = previousShipmentId + 1;
                            var To = currentShipmentId - 2 == previousShipmentId ? previousShipmentId + 1 : currentShipmentId - 1;

                            if (From == To)
                                await CreateUpdateMissingShipmentEntryIfNotCancelledV2(From, previousItem, currentItem);

                            else if (From < To)
                            {
                                logger.Information($"Missing Shipments: Applying updation for missing shipment with From: {From} and To: {To}");
                                await PerofrmLooping(From, To, previousItem, currentItem);
                            }
                            else
                            {
                                logger.Information($"Missing Shipments: Applying Reverse updation for missing shipment with From: {From} and To: {To}");
                                await PerofrmLooping(To, From, previousItem, currentItem);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PerofrmLooping(double from, double to, MissingShipmentsReportViewModel previousItem, MissingShipmentsReportViewModel currentItem)
        {
            try
            {
                var range = Enumerable.Range(Convert.ToInt32(from), (Convert.ToInt32(to - from)) + 1);
                var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                await Parallel.ForEachAsync(range, options, async (item, token) =>
                {
                    await CreateUpdateMissingShipmentEntryIfNotCancelledV2(item, previousItem, currentItem);
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void DeleteShipmentIfMarkedMissing(double Number)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var shipment = context.MissingShipments.Where(x => x.ShipmentNumberProcessed == Number).FirstOrDefault();
                    if (shipment != null)
                    {
                        context.MissingShipments.Remove(shipment);
                        context.SaveChanges();
                    }
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task CreateUpdateMissingShipmentEntryIfNotCancelledV2(double number, MissingShipmentsReportViewModel previousItem, MissingShipmentsReportViewModel currentItem)
        {
            try
            {
                //logger.Information($"Missing Shipments: working on {number}");

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var sosViewsContext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (!(context.CancelledShipments.Where(x => x.ShipmentNumber == number).Any() || sosViewsContext.RbCitShipments.Where(x => x.XShipmentNo == number.ToString()).Any()))
                    {
                        await sosViewsContext.DisposeAsync();

                        var alreadyMarkedMissingShipment = await context.MissingShipments.Where(x => x.ShipmentNumberProcessed == number).FirstOrDefaultAsync();
                        try
                        {
                            //string json = "";
                            //var recordsAlike = sosViewsContext.RbCitShipments.Where(x => x.XShipmentNo.Contains(number.ToString())).Select(x => x.XShipmentNo);
                            //if (recordsAlike != null)
                            //    json = JsonConvert.SerializeObject(recordsAlike.ToList());

                            if (alreadyMarkedMissingShipment == null)
                            {
                                var missingShipment = new MissingShipment()
                                {
                                    ShipmentNumberProcessed = number,
                                    PreviousShipmentDate = previousItem.PreviousShipmentDate,
                                    PreviousShipmentPickup = previousItem.PreviousShipmentPickup,
                                    PreviousShipmentDropOff = previousItem.PreviousShipmentDropOff,
                                    PreviousShipmentCity = previousItem.PreviousShipmentCity,
                                    PreviousShipmentNumber = previousItem.PreviousShipmentNumber,
                                    PreviousShipmentNumberProcessed = previousItem.MissingShipmentNumberFrom,
                                    NextShipmentDate = currentItem.PreviousShipmentDate,
                                    NextShipmentPickup = currentItem.PreviousShipmentPickup,
                                    NextShipmentDropOff = currentItem.PreviousShipmentDropOff,
                                    NextShipmentCity = currentItem.PreviousShipmentCity,
                                    NextShipmentNumber = currentItem.PreviousShipmentNumber,
                                    NextShipmentNumberProcessed = currentItem.MissingShipmentNumberFrom,
                                    //SimilarRecordsJson = json
                                };
                                await context.MissingShipments.AddAsync(missingShipment);
                                logger.Information($"Missing Shipments: marking {number} as missing shipment number");

                            }
                            else
                            {
                                if (previousItem.MissingShipmentNumberFrom > alreadyMarkedMissingShipment.PreviousShipmentNumberProcessed && previousItem.MissingShipmentNumberFrom < number)
                                {
                                    alreadyMarkedMissingShipment.PreviousShipmentDate = previousItem.PreviousShipmentDate;
                                    alreadyMarkedMissingShipment.PreviousShipmentPickup = previousItem.PreviousShipmentPickup;
                                    alreadyMarkedMissingShipment.PreviousShipmentDropOff = previousItem.PreviousShipmentDropOff;
                                    alreadyMarkedMissingShipment.PreviousShipmentCity = previousItem.PreviousShipmentCity;
                                    alreadyMarkedMissingShipment.PreviousShipmentNumber = previousItem.PreviousShipmentNumber;
                                    alreadyMarkedMissingShipment.PreviousShipmentNumberProcessed = previousItem.MissingShipmentNumberFrom;
                                }
                                if (currentItem.MissingShipmentNumberFrom < alreadyMarkedMissingShipment.NextShipmentNumberProcessed && currentItem.MissingShipmentNumberFrom > number)
                                {
                                    alreadyMarkedMissingShipment.NextShipmentDate = currentItem.PreviousShipmentDate;
                                    alreadyMarkedMissingShipment.NextShipmentPickup = currentItem.PreviousShipmentPickup;
                                    alreadyMarkedMissingShipment.NextShipmentDropOff = currentItem.PreviousShipmentDropOff;
                                    alreadyMarkedMissingShipment.NextShipmentCity = currentItem.PreviousShipmentCity;
                                    alreadyMarkedMissingShipment.NextShipmentNumber = currentItem.PreviousShipmentNumber;
                                    alreadyMarkedMissingShipment.NextShipmentNumberProcessed = currentItem.MissingShipmentNumberFrom;
                                }
                                //alreadyMarkedMissingShipment.SimilarRecordsJson = json;
                            }
                            await context.SaveChangesAsync();

                        }
                        catch (Exception ex)
                        {
                            logger.Error("Missing Shipments: Updating Single Entry Error: " + ex.ToString());
                        }
                    }
                    await context.DisposeAsync();
                }
                //logger.Information($"Missing Shipments: working completed on {number}");

            }
            catch (Exception ex)
            {
                logger.Error("Missing Shipments: Finding from CancelledShipments or RbCitShipments Error: " + ex.ToString());
            }
        }

        private void CreateUpdateMissingShipmentEntryIfNotCancelled(double number, MissingShipmentsReportViewModel previousItem, MissingShipmentsReportViewModel currentItem)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var sosViewsContext = scope.ServiceProvider.GetRequiredService<SOS_VIEWSContext>();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (!(context.CancelledShipments.Where(x => x.ShipmentNumber == number).Any() || sosViewsContext.RbCitShipments.Where(x => x.XShipmentNo == number.ToString()).Any()))
                    {
                        var alreadyMarkedMissingShipment = context.MissingShipments.Where(x => x.ShipmentNumberProcessed == number).FirstOrDefault();
                        try
                        {
                            string json = "";
                            var recordsAlike = sosViewsContext.RbCitShipments.Where(x => x.XShipmentNo.Contains(number.ToString())).Select(x => x.XShipmentNo);
                            if (recordsAlike != null)
                                json = JsonConvert.SerializeObject(recordsAlike.ToList());

                            if (alreadyMarkedMissingShipment == null)
                            {
                                var missingShipment = new MissingShipment()
                                {
                                    ShipmentNumberProcessed = number,
                                    PreviousShipmentDate = previousItem.PreviousShipmentDate,
                                    PreviousShipmentPickup = previousItem.PreviousShipmentPickup,
                                    PreviousShipmentDropOff = previousItem.PreviousShipmentDropOff,
                                    PreviousShipmentCity = previousItem.PreviousShipmentCity,
                                    PreviousShipmentNumber = previousItem.PreviousShipmentNumber,
                                    PreviousShipmentNumberProcessed = previousItem.MissingShipmentNumberFrom,
                                    NextShipmentDate = currentItem.PreviousShipmentDate,
                                    NextShipmentPickup = currentItem.PreviousShipmentPickup,
                                    NextShipmentDropOff = currentItem.PreviousShipmentDropOff,
                                    NextShipmentCity = currentItem.PreviousShipmentCity,
                                    NextShipmentNumber = currentItem.PreviousShipmentNumber,
                                    NextShipmentNumberProcessed = currentItem.MissingShipmentNumberFrom,
                                    SimilarRecordsJson = json
                                };
                                context.MissingShipments.Add(missingShipment);
                                logger.Information($"Missing Shipments: marking {number} as missing shipment number");

                            }
                            else
                            {
                                if (previousItem.MissingShipmentNumberFrom > alreadyMarkedMissingShipment.PreviousShipmentNumberProcessed)
                                {
                                    alreadyMarkedMissingShipment.PreviousShipmentDate = previousItem.PreviousShipmentDate;
                                    alreadyMarkedMissingShipment.PreviousShipmentPickup = previousItem.PreviousShipmentPickup;
                                    alreadyMarkedMissingShipment.PreviousShipmentDropOff = previousItem.PreviousShipmentDropOff;
                                    alreadyMarkedMissingShipment.PreviousShipmentCity = previousItem.PreviousShipmentCity;
                                    alreadyMarkedMissingShipment.PreviousShipmentNumber = previousItem.PreviousShipmentNumber;
                                    alreadyMarkedMissingShipment.PreviousShipmentNumberProcessed = previousItem.MissingShipmentNumberFrom;
                                }
                                if (currentItem.MissingShipmentNumberFrom < alreadyMarkedMissingShipment.NextShipmentNumberProcessed)
                                {
                                    alreadyMarkedMissingShipment.NextShipmentDate = currentItem.PreviousShipmentDate;
                                    alreadyMarkedMissingShipment.NextShipmentPickup = currentItem.PreviousShipmentPickup;
                                    alreadyMarkedMissingShipment.NextShipmentDropOff = currentItem.PreviousShipmentDropOff;
                                    alreadyMarkedMissingShipment.NextShipmentCity = currentItem.PreviousShipmentCity;
                                    alreadyMarkedMissingShipment.NextShipmentNumber = currentItem.PreviousShipmentNumber;
                                    alreadyMarkedMissingShipment.NextShipmentNumberProcessed = currentItem.MissingShipmentNumberFrom;
                                }
                                alreadyMarkedMissingShipment.SimilarRecordsJson = json;
                            }
                            context.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            logger.Error("Missing Shipments: Updating Single Entry Error: " + ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Missing Shipments: Finding from CancelledShipments or RbCitShipments Error: " + ex.ToString());
            }
        }

    }
}
