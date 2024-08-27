using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SOS.OrderTracking.Utils
{
    public class CacheService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public CacheService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public void StartShipmentCacheUpdateService()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var shipments = await context.Consignments.Where(x => x.DueTime > DateTime.Now.AddDays(-3)).Select(x=>x.Id).ToListAsync();
                        foreach (var id in shipments)
                        {
                            var workOrderService = scope.ServiceProvider.GetRequiredService<ConsignmentService>();
                            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                            var shipment = await workOrderService.GetShipment(id);
                            await shipmentsCacheService.SetShipment(shipment.Id, shipment);

                        }

                        
                    }
                    catch { }
                    await Task.Delay(TimeSpan.FromHours(2));

                }
            });

           
        }

    }
}
