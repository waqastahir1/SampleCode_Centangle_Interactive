using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class ShipmentDeliveryLogService
    {
        private readonly AppDbContext context;
        private readonly ILogger<ShipmentDeliveryLogService> logger;

        public ShipmentDeliveryLogService(AppDbContext context, ILogger<ShipmentDeliveryLogService> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<bool> HasAnySuccessCase(int deliveryId, ConsignmentDeliveryState shipmentState)
        {
            return (await context.ShipmentDeliveryApiLogs.AnyAsync(x => x.ShipmentDeliveryId == deliveryId && x.ShipmentState == shipmentState
             && x.Status));
        }

        public async Task LogSuccess(int deliveryId, ConsignmentDeliveryState shipmentState, string content)
        {
            try
            {
                var successLog = new Models.ShipmentDeliveryApiLog()
                {
                    ShipmentDeliveryId = deliveryId,
                    JsonContent = content,
                    RretryCount = 0,
                    ShipmentState = shipmentState,
                    Status = true,
                    TimeStamp = MyDateTime.Now
                };
                context.ShipmentDeliveryApiLogs.Add(successLog);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.ToString());
                // to avooid duplicate entry exception in case of multip concurrent calls
            }
        }

        public async Task<int> LogError(int deliveryId, ConsignmentDeliveryState shipmentState, string content)
        {
            try
            {
                var failedLog = await context.ShipmentDeliveryApiLogs.FirstOrDefaultAsync(x => x.ShipmentDeliveryId == deliveryId && x.ShipmentState == shipmentState
                  && !x.Status);

                if (failedLog == null)
                {
                    failedLog = new Models.ShipmentDeliveryApiLog()
                    {
                        ShipmentDeliveryId = deliveryId,
                        JsonContent = content,
                        RretryCount = 0,
                        ShipmentState = shipmentState,
                        Status = false,
                        TimeStamp = MyDateTime.Now
                    };
                    context.ShipmentDeliveryApiLogs.Add(failedLog);
                }

                failedLog.RretryCount++;

                await context.SaveChangesAsync();
                return failedLog.RretryCount;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.ToString());
                return -1;
                // to avooid duplicate entry exception in case of multip concurrent calls
            }
        }
    }
}
