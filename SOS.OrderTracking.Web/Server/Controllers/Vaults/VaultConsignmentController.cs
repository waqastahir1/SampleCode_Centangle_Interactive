using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Admin
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class VaultConsignmentController : ControllerBase, IVaultConsignmentService
    {
        private readonly AppDbContext context;
        public VaultConsignmentController(AppDbContext context)
        {
            this.context = context;
        }
        public Task<VaultConsignmentViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IndexViewModel<VaultConsignmentListViewModel>> GetPageAsync([FromQuery] VaultConsignmentAdditionalValueModel vm)
        {
            try
            {
                IQueryable<VaultConsignmentListViewModel> query = null;
                List<int> customers = null;
                List<int> childBranchIds = null;
                //if (vm.MainCustomerId > 0)
                //    childBranchIds = await context.PartyRelationships
                //        .Where(x => x.ToPartyId == vm.MainCustomerId && x.FromPartyRole == RoleType.ChildOrganization)
                //        .Select(x => x.FromPartyId).ToListAsync();

                query = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                        // join s in context.Parties on c.MainCustomerId equals s.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         from cr in context.Parties.Where(x => x.Id == d.CrewId).DefaultIfEmpty()
                         where (vm.VaultId == 0 || d.CrewId == vm.VaultId)
                       //  && (vm.BillBranchId == 0 || c.BillBranchId == vm.BillBranchId)
                         //&& (vm.ConsignmentStateType == null || c.ConsignmentStateType == vm.ConsignmentStateType)
                         //&& (vm.MainCustomerId == 0 || childBranchIds.Contains(c.BillBranchId))
                         && (t.RegionId == vm.RegionId || f.RegionId == vm.RegionId)
                         && (vm.SubRegionId == null || f.SubregionId == vm.SubRegionId || t.SubregionId == vm.SubRegionId)
                         && (vm.StationId == null || f.StationId == vm.StationId || t.StationId == vm.StationId)
                       //  && (vm.StartDate.Date <= c.CreatedAt.Date && vm.EndDate.Date >= c.CreatedAt.Date)
                       //  && c.Type == ConsignmentType.Approved
                         orderby c.Id, d.ParentId
                         select new VaultConsignmentListViewModel()
                         {
                             ShipmentCode = c.ShipmentCode,
                             CrewName = cr.FormalName,
                             FromPartyCode = f.ShortName,
                             ToPartyCode = t.ShortName,
                             Amount = c.Amount,
                             CreatedAt = c.CreatedAt,
                             ConsignmentStateType = d.DeliveryState,
                             VaultId = d.CrewId,
                             DeliveryId = d.Id,
                             PreviousId = d.ParentId,
                             BillBranchId = c.BillBranchId
                         });
                //if (User.IsInRole("SOS-Admin") || User.IsInRole("SOS-Regional-Admin") || User.IsInRole("SOS-SubRegional-Admin"))
                //{

                //}
                //else if (User.IsInRole("BankBranch") || User.IsInRole("BankCPC"))
                //{
                //    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                //    // customers = new List<int>() { user.PartyId };
                //    query = query.Where(x => x.BillBranchId == user.PartyId);
                //}

                // var query = workOrderService.GetConsignmentsQuery(ConsignmentType.CIT,
                //vm.RegionId, vm.SubRegionId, vm.StationId, customers);


                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage)
                    .Take(vm.RowsPerPage).ToListAsync();

                return new IndexViewModel<VaultConsignmentListViewModel> (items, totalRows);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<int> PostAsync(VaultConsignmentViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
