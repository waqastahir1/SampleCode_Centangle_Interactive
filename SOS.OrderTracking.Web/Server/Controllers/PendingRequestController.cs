using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.AdditionRequests;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class PendingRequestController : ControllerBase
    {
        protected AppDbContext context { get; set; }
        public UserManager<ApplicationUser> userManager { get; set; }
        public PendingRequestController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetPage(int rowsPerPage, int currentIndex, int regionId)
        {
            var query = (from r in context.ResourceRequests
                         join u in context.Users on r.RequestedById equals u.Id
                         join p in context.Parties on u.PartyId equals p.Id
                         join l in context.Parties on p.RegionId equals l.Id
                         where p.RegionId == regionId
                         select new PendingRequestsListViewModel()
                         {
                             Id = r.Id,
                             FromDate = r.FromDate,
                             ThruDate = r.ThruDate,
                             RequestedAt = r.RequestedAt,
                             RequestStatus = r.RequestStatus
                         });
            var totalRows = query.Count();

            var items = await query.Skip((currentIndex - 1) * rowsPerPage).Take(rowsPerPage).ToArrayAsync();

            return Ok(new IndexViewModel<PendingRequestsListViewModel>(items, totalRows));
        }
        [HttpGet]
        public async Task<IActionResult> GetRegions()
        {
            var regions = await (from r in context.Orgnizations.Include(x=>x.Party)
                                 where r.OrganizationType ==  Shared.Enums.OrganizationType.RegionalControlCenter
                                 select new SelectListItem()
                                 {
                                     IntValue = r.Id,
                                     Text = r.Party.FormalName

                                 }).ToListAsync();
            return Ok(regions);
        }
    }
}
