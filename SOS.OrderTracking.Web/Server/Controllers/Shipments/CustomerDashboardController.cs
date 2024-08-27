using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CustomerDashboardController : ControllerBase, ICustomerDashboardService
    {
        private readonly SequenceService sequenceService;
        private readonly AppDbContext context;
        private readonly ILogger<CustomerDashboardController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        public CustomerDashboardController(AppDbContext context,
           ConsignmentService workOrderService,
          ILogger<CustomerDashboardController> logger,
          SequenceService sequenceService,
          UserManager<ApplicationUser> userManager
            )
        {
            this.logger = logger;
            this.sequenceService = sequenceService;
            this.context = context;
            this.userManager = userManager;
        }

        public Task<CustomerDashboardFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IndexViewModel<CustomerDashboardListViewModel>> GetPageAsync([FromQuery] BaseIndexModel vm)
        {
            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var consignmentsOutgoing = from c in context.Consignments
                                           join p in context.Parties on c.FromPartyId equals p.Id
                                           where (p.Id == user.PartyId || (c.MainCustomerId == user.PartyId && p.Orgnization.OrganizationType == Shared.Enums.OrganizationType.MainCustomer))
                                           && c.CreatedAt.Date == MyDateTime.Now.Date
                                           select c;

                var consignmentsIncoming = from c in context.Consignments
                                           join p in context.Parties on c.ToPartyId equals p.Id
                                           where (p.Id == user.PartyId || (c.MainCustomerId == user.PartyId && p.Orgnization.OrganizationType == Shared.Enums.OrganizationType.MainCustomer))
                                          && c.CreatedAt.Date == MyDateTime.Now.Date
                                           select c;


                List<CustomerDashboardListViewModel> dashboardListViewModels = new();
                CustomerDashboardListViewModel viewModel = new();
                viewModel.ApprovalPendingOutgoing = await consignmentsOutgoing.Where(x => x.ApprovalState.HasFlag( Shared.Enums.ConsignmentApprovalState.Draft)).CountAsync();
                viewModel.ApprovalPendingIncoming = await consignmentsIncoming.Where(x => x.ApprovalState.HasFlag(Shared.Enums.ConsignmentApprovalState.Draft)).CountAsync();

                viewModel.WaitingForCrewOutgoing = await consignmentsOutgoing.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.CrewAssigned
                || x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.Created
                || x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.ReachedPickup).CountAsync();

                viewModel.WaitingForCrewIncoming = await consignmentsIncoming.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.CrewAssigned
               || x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.Created
               || x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.ReachedPickup).CountAsync();

                viewModel.InTransitOutgoing = await consignmentsOutgoing.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.ReachedDestination
                || x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.InTransit).CountAsync();

                viewModel.InTransitIncoming = await consignmentsIncoming.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.ReachedDestination
                || x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.InTransit).CountAsync();

                viewModel.DeliveredOutgoing = await consignmentsOutgoing.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.Delivered)
                    .CountAsync();

                viewModel.DeliveredIncoming = await consignmentsIncoming.Where(x => x.ConsignmentStateType == Shared.Enums.ConsignmentDeliveryState.Delivered)
                    .CountAsync();

                dashboardListViewModels.Add(viewModel);

               // var totalRows = query.Count();

                var items = dashboardListViewModels; //await query.ToListAsync();//.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToListAsync();
                logger.LogInformation($"totalRows-> {1}, currentRows-> {items.Count}");
                return new IndexViewModel<CustomerDashboardListViewModel>(items, 1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<int> PostAsync(CustomerDashboardFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
