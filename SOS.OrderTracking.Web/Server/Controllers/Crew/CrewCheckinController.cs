using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class CrewCheckinController : ControllerBase ,ICrewCheckinService
    {
        private readonly AppDbContext context;
        private readonly ILogger<CrewCheckinController> logger;
        private UserManager<ApplicationUser> userManager;
        private readonly SequenceService sequenceService;
        public CrewCheckinController(AppDbContext appDbContext,
             UserManager<ApplicationUser> userManager,
            ILogger<CrewCheckinController> logger, SequenceService sequenceService)
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
                                  && x.AttendanceDate.Date == vm.AttendanceDate.GetValueOrDefault().Date).DefaultIfEmpty()
                                  //where //r.ToPartyId == 1
                                  //&& e.EmploymentType == EmploymentType.Gaurd
                                  //&& vm.StationId.GetValueOrDefault() == p.StationId.Value
                                  select new CrewAttendanceListViewModel()
                                  {
                                      RelationshipId = r.Id,
                                      EmployeeCode = p.ShortName,
                                      Name = p.FormalName + " ( " + e.DesignationDesc + " ) ",
                                      ContactNo = p.PersonalContactNo,
                                      Address = p.Address,
                                      AttendanceState = a.AttendanceState,
                                      isChecked = a.AttendanceState == AttendanceState.Present ? true : false,
                                      CheckinTime = a.CheckinTime,
                                      CNIC = e.NationalId,
                                  }).ToArrayAsync();

                query = query.Where(x => x.AttendanceState == AttendanceState.Absent || x.AttendanceState == AttendanceState.Unknown).ToArray();
                var totalRows = query.Count();

                var items = query.Skip((vm.CurrentIndex - 1 ) * vm.RowsPerPage).Take(vm.RowsPerPage).ToList();

                return new IndexViewModel<CrewAttendanceListViewModel>(items, totalRows);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new BadRequestException(ex.Message);
            }
        }

        public Task<CrewAttendanceFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(CrewAttendanceFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }

        public async Task<int> MarkAttendence(List<CrewAttendanceFormViewModel> SelectedItem)
        {
            var CurrentLoggedInUserId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value; //getting currently logged in user
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == CurrentLoggedInUserId);
            foreach (var selectedItem in SelectedItem)
            {
                var employeeAttendance = await context.EmployeeAttendance.FirstOrDefaultAsync(x => x.RelationshipId == selectedItem.RelationshipId && x.AttendanceDate == selectedItem.AttendanceDate);
                if (employeeAttendance == null)
                {
                    employeeAttendance = new EmployeeAttendance();
                    employeeAttendance.RelationshipId = selectedItem.RelationshipId;
                    employeeAttendance.AttendanceDate = selectedItem.AttendanceDate;
                    context.EmployeeAttendance.Add(employeeAttendance);
                }
                employeeAttendance.MarkedAt = DateTime.Now;
                employeeAttendance.MarkedBy = user.Name;
                employeeAttendance.ApprovedAt = DateTime.Now;
                employeeAttendance.AttendanceState = selectedItem.AttendanceState.GetValueOrDefault();

                if (selectedItem.AttendanceState == AttendanceState.Present)
                    employeeAttendance.CheckinTime = selectedItem.CheckinTime.GetValueOrDefault();

                await context.SaveChangesAsync();
            }
            return 1;
        }
    }
}
