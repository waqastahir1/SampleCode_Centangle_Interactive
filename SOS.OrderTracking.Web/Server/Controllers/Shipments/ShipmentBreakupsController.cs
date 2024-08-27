using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.GBMS;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    public class ShipmentBreakupsController : BaseController , ICitGridService
    {
        private readonly ConsignmentService workOrderService;

        private readonly IHubContext<ConsignmentHub> consignmentHub;
        private readonly PartiesService partiesService; 

        public ShipmentBreakupsController(AppDbContext context,
           ConsignmentService workOrderService,
           IHubContext<ConsignmentHub> consignmentHub,
           PartiesService partiesService
            ) : base(context)
        {
            this.workOrderService = workOrderService;
            this.consignmentHub = consignmentHub;
            this.partiesService = partiesService; 
        }

        [HttpGet]
        public async Task<IndexViewModel<CitGridListViewModel>> GetPageAsync([FromQuery] CitGridAdditionalValueViewModel vm)
        {
            try
            {
                IQueryable<CitGridListViewModel> query = null; 
                List<int> childBranchIds = null;
                if (vm.MainCustomerId > 0)
                    childBranchIds = await context.PartyRelationships
                        .Where(x => x.ToPartyId == vm.MainCustomerId && x.FromPartyRole == RoleType.ChildOrganization)
                        .Select(x => x.FromPartyId).ToListAsync();

                query = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         from cr in context.Parties.Where(x => x.Id == d.CrewId).DefaultIfEmpty()
                         where (vm.CrewId == 0 || d.CrewId == vm.CrewId)
                         && (vm.BillBranchId == 0 || c.BillBranchId == vm.BillBranchId)
                         && (vm.ConsignmentStateType == null || c.ConsignmentStateType == vm.ConsignmentStateType)
                         && (vm.MainCustomerId == 0 || childBranchIds.Contains(c.BillBranchId))
                         && (vm.RegionId == null || vm.RegionId == 0 || t.RegionId == vm.RegionId || f.RegionId == vm.RegionId)
                         && (vm.SubRegionId == null || f.SubregionId == vm.SubRegionId || t.SubregionId == vm.SubRegionId)
                         && (vm.StationId == null || f.StationId == vm.StationId || t.StationId == vm.StationId)
                         && (vm.StartDate.Date <= c.CreatedAt.Date && vm.EndDate.Date >= c.CreatedAt.Date)
                         && c.Type == ShipmentExecutionType.Live
                         && c.ApprovalState.HasFlag( ConsignmentApprovalState.Approved)
                         orderby c.Id, d.ParentId
                         select new CitGridListViewModel()
                         { 
                             Id = c.Id,
                             ShipmentCode = c.ShipmentCode,
                             CrewName = cr.FormalName,
                             FromPartyCode = f.ShortName,
                             ToPartyCode = t.ShortName,
                             Amount = c.Amount,
                             CreatedAt = c.CreatedAt,
                             ConsignmentStateType = c.ConsignmentStateType,
                             CrewId = d.CrewId,
                             DeliveryId = d.Id,
                             PreviousId = d.ParentId,
                             BillBranchId = c.BillBranchId,
                             ShipmentType = c.ShipmentType
                         });
                if (User.IsInRole("SOS-Admin") || User.IsInRole("SOS-Regional-Admin") || User.IsInRole("SOS-SubRegional-Admin"))
                {

                }
                else if (User.IsInRole("BankBranch") || User.IsInRole("BankCPC"))
                {
                    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    // customers = new List<int>() { user.PartyId };
                    query = query.Where(x => x.BillBranchId == user.PartyId);
                }

                // var query = workOrderService.GetConsignmentsQuery(ConsignmentType.CIT,
                //vm.RegionId, vm.SubRegionId, vm.StationId, customers);


                var items = await query.Skip((vm.CurrentIndex - 1 )  * vm.RowsPerPage)
                    .Take(vm.RowsPerPage).ToListAsync();


                var totalRows = query.Count();
                return new IndexViewModel<CitGridListViewModel>(items, totalRows);
            }

            catch (Exception)
            {
                
                throw;
            }
        }


        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetCrews(int RegionId,int? SubRegionId,int? StationId)
        {
            if (RegionId == 0)
                return Array.Empty<SelectListItem>();

            var crews = from o in context.Parties
                        from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                        where o.Orgnization.OrganizationType == OrganizationType.Crew
                        && o.RegionId == RegionId
                        && (SubRegionId == 0 || o.SubregionId == SubRegionId)
                        && (StationId == 0 || o.StationId == StationId)
                        select new SelectListItem(o.Id, o.FormalName);


            return await crews.ToArrayAsync();
        }
      
        public Task<ShipmentFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(ShipmentFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
