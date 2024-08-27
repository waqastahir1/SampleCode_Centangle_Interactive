using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Common.Extenstions;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using SOS.OrderTracking.Web.Common.Services.Cache;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class DeliveryChargesController : ControllerBase
    {
        

        private readonly AppDbContext context;
        private readonly ShipmentsCacheService shipmentsCache;
        private readonly SequenceService sequenceService;

        public DeliveryChargesController(AppDbContext appDbContext,
           ShipmentsCacheService shipmentsCache,
           SequenceService sequenceService)
        {
            context = appDbContext;
            this.shipmentsCache = shipmentsCache;
            this.sequenceService = sequenceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            //Return denominations identified by the given Consignment FK value
            var charges = await (from d in context.ShipmentCharges
                                 join c in context.Consignments on d.ConsignmentId equals c.Id
                                 where d.ConsignmentId == id
                                 select d).ToArrayAsync();

            DeliveryChargesViewModel chargesVm = charges.ToViewModel(id);
            return Ok(chargesVm);
        } 

        [HttpPost]
        public async Task<IActionResult> Post(DeliveryChargesViewModel viewModel)
        {
            try
            {
                var charges = context.ShipmentCharges
                    .Where(c => c.ConsignmentId == viewModel.ConsignmentId );

                foreach (var c in charges)
                {
                    c.Amount = (c.ChargeTypeId == 1) ? viewModel.WaitingCharges : c.Amount;
                    c.Amount = (c.ChargeTypeId == 2) ? viewModel.TollCharges : c.Amount;
                }
                await context.SaveChangesAsync();
                await shipmentsCache.SetShipment(viewModel.ConsignmentId, null);
                //return Delivery ID for which charges are updated
                return Ok(viewModel.ConsignmentId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
