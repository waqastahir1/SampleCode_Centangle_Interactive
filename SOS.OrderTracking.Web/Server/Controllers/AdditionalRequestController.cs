using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Parties;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class AdditionalRequestController : ControllerBase
    {
        protected AppDbContext context { get; set; } 

        [Inject]
        public SequenceService sequenceService { get; set; }
        public AdditionalRequestController(AppDbContext context)
        {
            this.context = context; 
        }
        [HttpGet]
        public async Task<IActionResult> GetPage(int rowsPerPage, int currentIndex)
        {

            var query = (from r in context.ResourceRequests
                         select new AdditionRequestListViewModel()
                         {
                             Id = r.Id,
                             FromDate = r.FromDate,
                             ThruDate = r.ThruDate,
                             RequestedAt = r.RequestedAt,
                             RequestStatus = r.RequestStatus
                         });
            var totalRows = query.Count();

            var items = await query.Skip((currentIndex - 1) * rowsPerPage).Take(rowsPerPage).ToArrayAsync();

            return Ok(new IndexViewModel<AdditionRequestListViewModel>(items, totalRows));
        }
        [HttpPost]
        public async Task<IActionResult> Post(AdditionalRequestFormViewModel SelectedItem)
        {
            ResourceRequest resourceRequest = new ResourceRequest()
            {
                //Allocation type  all new addition
                Id = context.Sequences.GetNextCommonSequence(),
                RequestType = SelectedItem.RequestType,
                Quantity = SelectedItem.Quantity,
                FromDate = (DateTime)SelectedItem.FromDate,
                ThruDate = SelectedItem.ThruDate,
                RequestStatus = RequestStatus.Pending,
                Remarks1 = SelectedItem.Remarks,
                RequestedById = User.Identity.Name,
                RequestedAt = MyDateTime.Now
            };
            context.ResourceRequests.Add(resourceRequest);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
