using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class CPCConsignmentsController : ControllerBase
    {



        private readonly AppDbContext context;

        private readonly ConsignmentService workOrderService;

        private readonly PartiesService partiesService;

        private readonly SequenceService sequenceService;
        private readonly IHubContext<ConsignmentHub> consignmentHub;

        public CPCConsignmentsController(AppDbContext appDbContext,

           ConsignmentService workOrderService,
           PartiesService partiesService,
           SequenceService sequenceService,
           IHubContext<ConsignmentHub> consignmentHub)
        {
            context = appDbContext;

            this.workOrderService = workOrderService;
            this.partiesService = partiesService;
            this.sequenceService = sequenceService;
            this.consignmentHub = consignmentHub;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PublishUpdates()
        {
            await consignmentHub.Clients.All.SendAsync("RefreshCITConsignments", "asad@sos.com", "Hello");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int rowsPerPage, int currentIndex)
        {
            //var query = workOrderService.GetConsignmentsQuery(ConsignmentType.Approved, 0, 0, 0, null, null);
            //var totalRows = query.Count();

            //var items = await query.Skip((currentIndex - 1) * rowsPerPage).Take(rowsPerPage).ToListAsync();

            //return Ok(new IndexViewModel<ConsignmentListViewModel>(items, totalRows));
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var order = await (from o in context.Consignments
                               where o.Id == id
                               select new ShipmentFormViewModel()
                               {
                                   Id = o.Id,
                                   ConsignmentNo = o.ShipmentCode, 
                                   FromPartyId = o.FromPartyId,
                                   ToPartyId = o.ToPartyId,
                                   Type = o.Type,
                                   CurrencySymbol = o.CurrencySymbol,
                                   Amount = o.Amount
                               }).FirstAsync();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CPCFormViewModel SelectedItem)
        {
            try
            {
                Consignment consignment = null;
                Denomination denomination = null;
                if (SelectedItem.Id == 0)
                {
                    var nextId = sequenceService.GetNextCitOrdersSequence();
                    consignment = new Consignment()
                    {
                        Id = nextId,
                        ShipmentCode = $"CIT/{MyDateTime.Now.Year}/" + nextId.ToString("D4"),
                        Type = ShipmentExecutionType.Live,
                        CreatedAt = MyDateTime.Now,
                        ConsignmentStateType = Shared.Enums.ConsignmentDeliveryState.Created,
                        CreatedBy = "",
                    };
                    denomination = new Denomination()
                    {
                        ConsignmentId = consignment.Id,
                        Id = sequenceService.GetNextCommonSequence(),
                        Currency10x = SelectedItem.Currency10x.GetValueOrDefault(),
                        Currency20x = SelectedItem.Currency20x.GetValueOrDefault(),
                        Currency50x = SelectedItem.Currency50x.GetValueOrDefault(),
                        Currency75x = SelectedItem.Currency75x.GetValueOrDefault(),
                        Currency100x = SelectedItem.Currency100x.GetValueOrDefault(),
                        Currency500x = SelectedItem.Currency500x.GetValueOrDefault(),
                        Currency1000x = SelectedItem.Currency1000x.GetValueOrDefault(),
                        Currency5000x = SelectedItem.Currency5000x.GetValueOrDefault()
                    };
                    consignment.Amount = denomination.Currency10x
                        + denomination.Currency20x
                        + denomination.Currency50x
                        + denomination.Currency75x
                        + denomination.Currency100x
                        + denomination.Currency500x
                        + denomination.Currency1000x
                        + denomination.Currency5000x;

                    consignment.Denominations.Add(denomination);
                    context.Consignments.Add(consignment);
                }


                consignment.CustomerId = SelectedItem.CustomerId;
                consignment.FromPartyId = SelectedItem.FromPartyId;
                consignment.ToPartyId = 1; // todo: nearest 
                consignment.CurrencySymbol = CurrencySymbol.PKR;
                consignment.Amount = SelectedItem.Amount;

                consignment.ServiceType = ServiceType.ByRoad;
                consignment.ShipmentType = ShipmentType.Local;

                //linking CitDenominationViewModel with CITViewModel for preserving later
                /*CitDenominationViewModel = new CitDenominationViewModel
                    {
                    WorkOrderId = workOrder.Id,
                    ShipmentCode = workOrder.ShipmentCode
                    };*/
                await context.SaveChangesAsync();
                await PublishUpdates();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
