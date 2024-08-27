using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CrewCheckoutController : ControllerBase ,ICrewCheckoutService
    {
        private readonly AppDbContext context;
        private readonly ILogger<CrewCheckoutController> logger;
        private UserManager<ApplicationUser> userManager;
        private readonly SequenceService sequenceService;
        public CrewCheckoutController(AppDbContext appDbContext,
             UserManager<ApplicationUser> userManager,
            ILogger<CrewCheckoutController> logger, SequenceService sequenceService)
        {
            this.userManager = userManager;
            context = appDbContext;
            this.logger = logger;
            this.sequenceService = sequenceService;
        }

        [HttpGet]
        public async Task<IndexViewModel<CrewAttendanceListViewModel>> GetPageAsync([FromQuery]CrewAttendanceAdditionalValueViewModel vm)
        {
            try
            {
                var query = await(from e in context.People
                                  join p in context.Parties on e.Id equals p.Id
                                  join r in context.PartyRelationships on p.Id equals r.FromPartyId
                                  from a in context.EmployeeAttendance.Where(x => x.RelationshipId == r.Id
                                  //  && x.AttendanceState == AttendanceState.Absent 
                                  && x.AttendanceDate == vm.AttendanceDate.GetValueOrDefault()).DefaultIfEmpty()
                                  where r.ToPartyId == 1
                                  && e.EmploymentType == EmploymentType.Gaurd
                                  && vm.StationId.GetValueOrDefault() == p.StationId.Value
                                  select new CrewAttendanceListViewModel()
                                  {
                                      RelationshipId = r.Id,
                                      Name = p.FormalName + " ( " + e.DesignationDesc + " ) ",
                                      ContactNo = p.PersonalContactNo,
                                      Address = p.Address,
                                      AttendanceState = a.AttendanceState,
                                      isChecked = a.AttendanceState == AttendanceState.Present ? true : false,
                                      CheckoutTime = a.CheckoutTime,
                                      CNIC = e.NationalId,
                                      EmployeeCode = p.ShortName
                                  }).ToArrayAsync();

                query = query.Where(x => x.AttendanceState == AttendanceState.Present).ToArray();
                var totalRows = query.Count();

                var items = query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToList();

                return new IndexViewModel<CrewAttendanceListViewModel>(items, totalRows);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new BadRequestException(ex.Message);
            }
        }
        [HttpPost]
        public async Task<int> MarkAttendence(List<CrewAttendanceFormViewModel> SelectedItem)
        {
            var CurrentLoggedInUserId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value; //getting currently logged in user
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == CurrentLoggedInUserId);
            foreach (var selectedItem in SelectedItem)
            {
                var employeeAttendance = await context.EmployeeAttendance.FirstOrDefaultAsync(x => x.RelationshipId == selectedItem.RelationshipId && x.AttendanceDate == selectedItem.AttendanceDate);
                //if (employeeAttendance == null)
                //{
                //    employeeAttendance = new EmployeeAttendance();
                //    employeeAttendance.RelationshipId = selectedItem.RelationshipId;
                //    employeeAttendance.CheckinTime = selectedItem.CheckinTime;
                //    employeeAttendance.AttendanceDate = selectedItem.AttendanceDate;
                //    employeeAttendance.MarkedAt = DateTime.Now;
                //    employeeAttendance.MarkedBy = user.Name;
                //    employeeAttendance.ApprovedAt = DateTime.Now;
                //    employeeAttendance.AttendanceState = selectedItem.AttendanceState;
                //    context.EmployeeAttendance.Add(employeeAttendance);
                //}

                employeeAttendance.CheckoutTime = selectedItem.CheckoutTime;

                await context.SaveChangesAsync();
            }
            return 1;

        }

        public Task<CrewAttendanceFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(CrewAttendanceFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}