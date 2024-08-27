using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.BranchesDistance
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class DistanceHistoryController : ControllerBase, IDistanceHistoryService
    {
        private readonly AppDbContext context;
        public DistanceHistoryController(AppDbContext context)
        {
            this.context = context;
        }
        public Task<DistanceHistoryFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<IndexViewModel<DistanceHistoryListViewModel>> GetPageAsync([FromQuery]DistanceHistoryAdditionalValueViewModel vm)
        {
            var query = from i in context.IntraPartyDistancesHistory
                        where i.FromPartyId == vm.FromPartyId
                        && i.ToPartyId == vm.ToPartyId
                        orderby i.Id descending
                        select new DistanceHistoryListViewModel()
                        {
                            Id = i.Id,
                            Distance = i.Distance / 1000,
                            DistanceStatus = i.DistanceStatus,
                            DistanceSource = i.DistanceSource,
                            CreatedBy = i.CreatedBy,
                            CreatedAt = i.CreatedAt
                        };
            var totalRows = query.Count();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<DistanceHistoryListViewModel>(items, totalRows);
        }

        public Task<int> PostAsync(DistanceHistoryFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
