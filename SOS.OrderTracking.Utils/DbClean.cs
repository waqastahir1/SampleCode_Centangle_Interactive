using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Portal.GBMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    public class DbClean
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        public DbClean(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public void StartCleaningConsignmentsBySOSTeam()
        {
            Task.Run(async () =>
            {
                logger.Error("to use the service, comment debugger and return line");
                return;
                bool doAction = true;
                while (doAction)
                {
                    try
                    {

                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var context1 = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                            var query = (from c in context1.Consignments
                                                      .Include(x => x.ConsignmentDeliveries).ThenInclude(x => x.Childern)
                                                      .Include(x => x.Denominations)
                                                      .Include(x => x.ShipmentSealCodes)
                                                      .Include(x => x.DeliveryCharges)
                                         join u in context1.Users on c.CreatedBy equals u.UserName
                                         join ur in context1.UserRoles on u.Id equals ur.UserId
                                         where ur.RoleId == "SOS-Regional-Admin" ||
                                         ur.RoleId == "SOS-SubRegional-Admin" ||
                                         ur.RoleId == "SOS-Admin" ||
                                         ur.RoleId == "Super-Admin"
                                         select c);

                            var count = query.Count();
                            logger.Information($"{count} shipments are remaining ");

                            var consignments = await query.Take(10000).ToListAsync();
                            await context1.DisposeAsync();
                            if (consignments != null && consignments.Count > 0)
                            {
                                var options = new ParallelOptions { MaxDegreeOfParallelism = 3 };
                                Parallel.ForEach(consignments, options, (item, token) =>
                                {
                                    try
                                    {
                                        using (var scope = serviceScopeFactory.CreateScope())
                                        {
                                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                                            var consignmentstates = (from c in context.ConsignmentStates
                                                                     where item.Id == c.ConsignmentId
                                                                     select c);
                                            if (consignmentstates.Any())
                                            {
                                                context.ConsignmentStates.RemoveRange(consignmentstates);

                                            }
                                            var deliveryChilds = item.ConsignmentDeliveries.Select(x => x.Childern).SelectMany(y => y);

                                            if (deliveryChilds.Any())
                                                context.ConsignmentDeliveries.RemoveRange(deliveryChilds);

                                            if (item.ConsignmentDeliveries.Any())
                                                context.ConsignmentDeliveries.RemoveRange(item.ConsignmentDeliveries);

                                            if (item.Denominations.Any())
                                                context.Denominations.RemoveRange(item.Denominations);

                                            if (item.ShipmentSealCodes.Any())
                                                context.ShipmentSealCodes.RemoveRange(item.ShipmentSealCodes);

                                            if (item.DeliveryCharges.Any())
                                                context.ShipmentCharges.RemoveRange(item.DeliveryCharges);

                                            context.Consignments.Remove(item);
                                            context.DeletedConsignments.Add(new Web.Common.Data.Models.CitShipment.DeletedConsignment() { ConsignmentId = item.Id, CreatedBy = item.CreatedBy });
                                            context.SaveChanges();
                                            context.Dispose();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error($"shipments deletion foreach error: {ex.Message}");
                                        throw;
                                    }
                                });
                                logger.Information($"{consignments.Count} shipments are deleted in this cycle ");

                            }
                            else
                            {
                                logger.Information($"All the shipments created by SOS team are deleted");
                                doAction = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"shipments deletion error: {ex.Message}");

                        throw;
                    }
                }
            });
        }

        public void StartCleaningDeliveredNotifications()
        {
            Task.Run(async () =>
            {
                logger.Error("to use the service, comment debugger and return line");
                return;
                bool doAction = true;
                while (doAction)
                {
                    try
                    {

                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var notificationsCount = (from n in context.Notifications
                                                      where n.NotificationStatus >= Web.Shared.Enums.NotificationStatus.Sent
                                                      select n).Count();
                            logger.Information($"{notificationsCount} notifications remaining ");

                            var notifications = (from n in context.Notifications
                                                 where n.NotificationStatus >= Web.Shared.Enums.NotificationStatus.Sent
                                                 select n).Take(100000);
                            if (notifications != null && notifications.Count() > 0)
                            {
                                var count = notifications.Count();
                                context.Notifications.RemoveRange(notifications);
                                await context.SaveChangesAsync();
                                logger.Information($"{count} notifications deleted ");
                            }
                            else
                            {
                                logger.Information($"All notificaitons are deleted");
                                doAction = false;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error($"notificaitons deletion error: {ex.Message}");
                        throw;
                    }
                }
            });
        }
    }
}
