using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Admin
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class SOSDashboardController : ControllerBase, ISOSDashboardService
    {
        private readonly SequenceService sequenceService;
        private readonly AppDbContext context;
        private readonly ILogger<SOSDashboardController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        public SOSDashboardController(AppDbContext context,
           ConsignmentService workOrderService,
          ILogger<SOSDashboardController> logger,
          SequenceService sequenceService,
          UserManager<ApplicationUser> userManager
            )
        {
            this.logger = logger;
            this.sequenceService = sequenceService;
            this.context = context;
            this.userManager = userManager;
        }
        public Task<SOSDashboardFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IndexViewModel<SOSDashboardListViewModel>> GetPageAsync([FromQuery] BaseIndexModel vm)
        {
            try
            {


                var shipmentTypeWise = from c in context.Consignments select c;

                List<SOSDashboardListViewModel> dashboardListViewModels = new();
                SOSDashboardListViewModel viewModel = new();
                viewModel.DomensticShipmentCount = await shipmentTypeWise.Where(x => x.ShipmentType == ShipmentType.Domestic).CountAsync();
                viewModel.DedicatedShipmentCount = await shipmentTypeWise.Where(x => x.ShipmentType == ShipmentType.Dedicated).CountAsync();
                viewModel.LocalShipmentCount = await shipmentTypeWise.Where(x => x.ShipmentType == ShipmentType.Local).CountAsync();
                viewModel.AtmShipmentCount = await shipmentTypeWise.Where(x => x.ShipmentType == ShipmentType.ATMCITDomestic
                || x.ShipmentType == ShipmentType.ATMCITLocal).CountAsync();

                //////--------- Region Wise shipments ------------/////////
                

                var regionWiseGroup = (from c in context.Consignments
                                 join p in context.Parties on c.FromPartyId equals p.Id 
                                 join r in context.Parties on p.RegionId equals r.Id
                                 group r.FormalName by r.FormalName).ToList();
                 
                List<Shared.ViewModels.WorkOrder.Dashboard.Shipments> regionWiseShipments = new();
                foreach (var region in regionWiseGroup)
                {
                    regionWiseShipments.Add(new Shared.ViewModels.WorkOrder.Dashboard.Shipments() { FormalName = region.FirstOrDefault(), ShipmentsCount = region.Count() });

                }
                viewModel.RegionWiseShipmentsList = regionWiseShipments.OrderByDescending(x => x.ShipmentsCount).ToList();



                /////---------- Customer Wise ---------///////

                var customerWiseGroup = (from c in context.Consignments
                                           join p in context.Parties on c.BillBranchId equals p.Id
                                           join pr in context.PartyRelationships on p.Id equals pr.FromPartyId
                                           join mcp in context.Parties on pr.ToPartyId equals mcp.Id //main customer
                                           group mcp.FormalName
                                           by mcp.FormalName ).ToList();//.GroupBy(x => x.pr.ToPartyId);
                 
                List<Shared.ViewModels.WorkOrder.Dashboard.Shipments> mainCustomers = new();
                foreach (var mainCust in customerWiseGroup)
                {
                    mainCustomers.Add(new Shared.ViewModels.WorkOrder.Dashboard.Shipments() { FormalName = mainCust.FirstOrDefault(), ShipmentsCount = mainCust.Count() });

                }

                viewModel.MainCustomerShipmentsList = mainCustomers.OrderByDescending(x => x.ShipmentsCount).Take(10).ToList();
             

                dashboardListViewModels.Add(viewModel);

                // var totalRows = query.Count();

                var items = dashboardListViewModels; //await query.ToListAsync();//.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToListAsync();
                logger.LogInformation($"totalRows-> {1}, currentRows-> {items.Count}");
                return new IndexViewModel<SOSDashboardListViewModel>(items, 1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<int> PostAsync(SOSDashboardFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
