using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class GaurdsAttendanceController : ControllerBase
    {

        private readonly AppDbContext context;
        private readonly ILogger<GaurdsAttendanceController> logger;
        UserManager<ApplicationUser> userManager;
        public GaurdsAttendanceController(AppDbContext appDbContext,
             UserManager<ApplicationUser> userManager,
            ILogger<GaurdsAttendanceController> logger)
        {
            this.userManager = userManager;
            context = appDbContext;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int rowsPerPage,
            int currentIndex,
            int bankId,
            DateTime Date)
        {
            try
            {
                var query = await (from r in context.PartyRelationships
                                   join pr in context.Parties on r.FromPartyId equals pr.Id
                                   from a in context.EmployeeAttendance.Where(x => x.RelationshipId == r.Id).DefaultIfEmpty()
                                   where (r.FromPartyRole == RoleType.Gaurd
                                   && r.ToPartyId == bankId
                                   && r.StartDate <= Date
                                   && (!r.ThruDate.HasValue || r.ThruDate >= Date)
                                )
                                   select new GaurdsAttendaceListViewModel()
                                   {
                                       RelationshipId = r.Id,
                                       Name = pr.FormalName,
                                       ContactNo = pr.PersonalContactNo,
                                       Address = pr.Address,
                                       AttendanceState = a.AttendanceState
                                   }).ToArrayAsync();

                var totalRows = query.Count();

                var items =  query.Skip((currentIndex - 1) * rowsPerPage).Take(rowsPerPage);

                return Ok(new IndexViewModel<GaurdsAttendaceListViewModel>(items.ToList(), totalRows));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var banks = await (from o in context.Orgnizations
                                   join p in context.Parties on o.Id equals p.Id
                                   where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                                   select new SelectListItem(p.Id, p.ShortName+" - "+p.FormalName)).ToListAsync();

                return Ok(banks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var query = await (from r in context.EmployeeAttendance
                                   where r.RelationshipId == id
                                   select new GaurdsAttendanceViewModel()
                                   {
                                       RelationshipId = r.RelationshipId,
                                       AttendanceState = (AttendanceState)r.AttendanceState
                                   }).FirstOrDefaultAsync();

                return Ok(query);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post(GaurdsAttendanceViewModel SelectedItem)
        {

            var CurrentLoggedInUserId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value; //getting currently logged in user
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == CurrentLoggedInUserId);
            var gaurdsAttendance = await context.EmployeeAttendance.FirstOrDefaultAsync(x => x.RelationshipId == SelectedItem.RelationshipId); 
            if (gaurdsAttendance != null)
            {                                                                                                                             
                context.EmployeeAttendance.Remove(gaurdsAttendance);
            }
            gaurdsAttendance = new EmployeeAttendance();
            gaurdsAttendance.RelationshipId = SelectedItem.RelationshipId;
            gaurdsAttendance.AttendanceDate = MyDateTime.Now;
            gaurdsAttendance.MarkedAt = MyDateTime.Now;
            gaurdsAttendance.MarkedBy = user.UserName;
            gaurdsAttendance.ApprovedAt = MyDateTime.Now;
            gaurdsAttendance.AttendanceState = SelectedItem.AttendanceState;
            context.EmployeeAttendance.Add(gaurdsAttendance);
            await context.SaveChangesAsync();
            return Ok();

        }
    }
}
