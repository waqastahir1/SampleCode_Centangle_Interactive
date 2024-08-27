using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    class RelationshipStatusCronService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        public RelationshipStatusCronService(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        /// <summary>
        ///  enables or disables IsActive Attribute based on Start/End Date of relationship
        /// </summary>
        public void Start()
        {
            DateTime? processedDate = null;
            _ = Task.Run(async () =>
              {
                  while (true)
                  {
                      try
                      {
                          if (processedDate != MyDateTime.Today)
                          {
                              using var scope = serviceScopeFactory.CreateScope();

                              var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                              SqlParameter date = new SqlParameter("@date", System.Data.SqlDbType.Date)
                              {
                                  Direction = System.Data.ParameterDirection.Input,
                                  Value = MyDateTime.Today
                              };

                              logger.Information($"{context.PartyRelationships.Count()} relationships in total");

                              var rowCount = context.Database.ExecuteSqlRaw($"UPDATE PartyRelationships  SET IsActive = 'false' WHERE StartDate > @date OR   ThruDate < @date ", date);
                              logger.Information($"{rowCount} relationships disabled");
                              rowCount = context.Database.ExecuteSqlRaw($"UPDATE PartyRelationships  SET IsActive = 'true' WHERE StartDate <= @date AND (ThruDate is null OR ThruDate >= @date) ", date);
                              logger.Information($"{rowCount} relationships enabled");
                              processedDate = MyDateTime.Today;

                          }

                      }
                      catch (Exception ex)
                      {
                          logger.Error(ex.ToString());
                      }

                      await Task.Delay(TimeSpan.FromMinutes(5));
                  }
              });

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using var scope = serviceScopeFactory.CreateScope();

                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var cache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();
                        var total = context.Consignments.Count();
                        var organizations = context.Parties.Include(x => x.Orgnization).ToList();
                        logger.Information("Start cache building");
                        foreach (var branchParty in organizations)
                        {
                            await cache.SetName(branchParty.Id, branchParty.FormalName);
                            await cache.SetCode(branchParty.Id, branchParty.ShortName);
                            await cache.SetAddress(branchParty.Id, branchParty.Address);
                            await cache.SetContactNo(branchParty.Id, $"{branchParty.PersonalContactNo} {branchParty.OfficialContactNo}");

                            await cache.SetStationName(branchParty.Id, organizations.FirstOrDefault(x => x.Id == branchParty.StationId)?.FormalName);
                            await cache.SetRegionName(branchParty.Id, organizations.FirstOrDefault(x => x.Id == branchParty.RegionId)?.FormalName);
                            await cache.SetRegionAbbr(branchParty.Id, organizations.FirstOrDefault(x => x.Id == branchParty.RegionId)?.Abbrevation);

                            if (branchParty?.Orgnization?.Geolocation != null)
                                await cache.SetGeoCoordinate(branchParty.Id, new Web.Shared.ViewModels.Point(branchParty.Orgnization.Geolocation.Y, branchParty.Orgnization.Geolocation.X));
                        }
                        logger.Information("Finished cache building");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5));
                }
            });

        }
    }
}
