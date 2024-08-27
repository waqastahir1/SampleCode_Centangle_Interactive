using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOS.OrderTracking.Web.Server.Services;

namespace SOS.OrderTracking.Web.Server.Controllers.Operations
{
    [AllowAnonymous]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class LiveShipmentsController : ControllerBase
    {
        private readonly NotificationAgent notificationAgent;

        public LiveShipmentsController(
          NotificationAgent notificationAgent)
        {
            this.notificationAgent = notificationAgent;
        }


        public IActionResult PublishIncrementalUpdates(int id)
        {

            //var shipment = await shipmentsCacheService.GetShipment(id);
            //await PubSub.Hub.Default.PublishAsync(shipment); 

            return Ok(notificationAgent.QueueShipmentIdToRefresh(id));
        }
    }
}
