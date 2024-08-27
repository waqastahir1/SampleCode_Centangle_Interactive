using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class DenominationController : ControllerBase
    {



        private readonly AppDbContext context;

        private readonly SequenceService sequenceService;
        private readonly NotificationService notificationService;
        private readonly ShipmentsCacheService shipmentsCacheService;
        private readonly IHubContext<ConsignmentHub> consignmentHub;
        private readonly ConsignmentService workOrderService;

        public DenominationController(AppDbContext appDbContext,

           SequenceService sequenceService, NotificationService notificationService, ShipmentsCacheService shipmentsCacheService, 
           IHubContext<ConsignmentHub> consignmentHub, ConsignmentService workOrderService)
        {
            context = appDbContext;

            this.sequenceService = sequenceService;
            this.notificationService = notificationService;
            this.shipmentsCacheService = shipmentsCacheService;
            this.consignmentHub = consignmentHub;
            this.workOrderService = workOrderService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> PublishIncrementalUpdates(int shipmentId)
        {
            var shipment = await workOrderService.GetShipment(shipmentId);
            await shipmentsCacheService.SetShipment(shipment.Id, shipment);

            await consignmentHub.Clients.All.SendAsync("OnShipmentUpdated", shipmentId.ToString());
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            //Return denominations identified by the given PK value
            var denom = await (from d in context.Denominations
                               join c in context.Consignments on d.ConsignmentId equals c.Id
                               where d.Id == id
                               select new CitDenominationViewModel()
                               {
                                   Id = d.Id,
                                   Type = d.DenominationType,
                                   ConsignmentId = d.ConsignmentId,
                                   ShipmentCode = c.ShipmentCode,
                                   Currency1x = d.Currency1x,
                                   Currency2x = d.Currency2x,
                                   Currency5x = d.Currency5x,
                                   Currency10x = d.Currency10x,
                                   Currency20x = d.Currency20x,
                                   Currency50x = d.Currency50x,
                                   Currency75x = d.Currency75x,
                                   Currency100x = d.Currency100x,
                                   Currency500x = d.Currency500x,
                                   Currency1000x = d.Currency1000x,
                                   Currency5000x = d.Currency5000x,
                                   TotalAmount = c.AmountPKR,
                                   CurrencySymbol = c.CurrencySymbol

                               }).FirstOrDefaultAsync();

            return Ok(denom ?? new CitDenominationViewModel() { ConsignmentId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Post(CitDenominationViewModel viewModel)
        {
            if (viewModel.ConsignmentId == 0)
            {
                return BadRequest("Denomination cannot be tracked without Consignment...");
            }

            Consignment consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == viewModel.ConsignmentId);
            if (consignment.IsFinalized)
            {
                return BadRequest("Cannot change demonination once shipment is finalized");
            }

            Denomination denom = null;
            try
            {
                denom = await context.Denominations.FirstOrDefaultAsync(x => x.Id == viewModel.Id);
                if (denom == null)
                {
                    denom = new Denomination()
                    {
                        Id = context.Denominations.Max(x => (int?)x.Id).GetValueOrDefault() + 1,
                        ConsignmentId = viewModel.ConsignmentId
                    };
                }

                denom.DenominationType = viewModel.Type;
                denom.Currency1x = viewModel.Currency1x.GetValueOrDefault();
                denom.Currency2x = viewModel.Currency2x.GetValueOrDefault();
                denom.Currency5x = viewModel.Currency5x.GetValueOrDefault();
                denom.Currency10x = viewModel.Currency10x.GetValueOrDefault();
                denom.Currency20x = viewModel.Currency20x.GetValueOrDefault();
                denom.Currency50x = viewModel.Currency50x.GetValueOrDefault();
                denom.Currency75x = viewModel.Currency75x.GetValueOrDefault();
                denom.Currency100x = viewModel.Currency100x.GetValueOrDefault();
                denom.Currency500x = viewModel.Currency500x.GetValueOrDefault();
                denom.Currency1000x = viewModel.Currency1000x.GetValueOrDefault();
                denom.Currency5000x = viewModel.Currency5000x.GetValueOrDefault();

                
                consignment.CurrencySymbol = viewModel.CurrencySymbol;
                consignment.ExchangeRate = viewModel.ExchangeRate;
                consignment.IsFinalized = viewModel.FinalizeShipment;

                if (consignment.Amount != viewModel.TotalAmount)
                {
                    if (consignment.ApprovalState == ConsignmentApprovalState.Approved)
                    {
                        consignment.ApprovalState = User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_CPC) ? ConsignmentApprovalState.ReApprove : ConsignmentApprovalState.Approved;
                    }
                    else
                    {
                        consignment.ApprovalState = User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;
                    }
                }

                consignment.Amount = viewModel.TotalAmount;
                consignment.AmountPKR = viewModel.AmountPKR;

                if (consignment.IsFinalized)
                {
                    consignment.FinalizedAt = MyDateTime.Now;
                    consignment.FinalizedBy = User.Identity.Name;
                }
                context.Consignments.Update(consignment);
                await context.SaveChangesAsync();
                if (consignment.ApprovalState.HasFlag(ConsignmentApprovalState.Draft))
                {
                    var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                    notificationIds.ForEach(x=>  NotificationAgent.WebPushNotificationsQueue.Add(x));
                }

                if (consignment.IsFinalized)
                {
                    await notificationService.CreateShipmentNotificationForMobile(viewModel.ConsignmentId, Shared.Enums.NotificationType.ShipmentFinalized, Shared.Enums.NotificationCategory.CIT);
                }
                else
                {
                    await notificationService.CreateShipmentNotificationForMobile(viewModel.ConsignmentId, Shared.Enums.NotificationType.UpdatedConsignment, Shared.Enums.NotificationCategory.CIT);
                }
                await shipmentsCacheService.SetShipment(consignment.Id, null);
                await PublishIncrementalUpdates(consignment.Id);
                return Ok(denom.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
