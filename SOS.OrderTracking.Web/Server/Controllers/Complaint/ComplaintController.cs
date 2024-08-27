using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Index.HPRtree;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Complaint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Complaint
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ComplaintController : ControllerBase, IComplaintService
    {
        private readonly AppDbContext context;
        private ILogger<ComplaintController> logger;
        public ComplaintController(AppDbContext context, ILogger<ComplaintController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Task<ComplaintFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<IndexViewModel<ComplaintListViewModel>> GetPageAsync([FromQuery] ComplaintAdditionalValueViewModel vm)
        {
            var query = (from c in context.Complaints
                         from con in context.Consignments.Where(x => x.Id == c.ConsignmentId).DefaultIfEmpty()
                         join s in context.ComplaintStatuses on c.Id equals s.ComplaintId
                         where (vm.RatingValue == 0 || vm.RatingValue == con.Rating)
                         select new ComplaintListViewModel()
                         {
                             ComplaintId = c.Id,
                             ComplaintStatus = s.Status,
                             CreatedAt = c.CreatedAt,
                             CreatedBy = c.CreatedBy,
                             Rating = con.Rating,
                             Description = c.Description,
                             ConsignmentCode = con.ShipmentCode,
                             Categories = (from cat in context.Categories
                                          join comp in context.ComplaintCategories on cat.Id equals comp.CategoryId
                                          where comp.ComplaintId == c.Id
                                          select cat.Name).ToList(),
                         });
            if(vm.Category != default)
            {
                query = query.Where(x => x.Categories.Contains(vm.Category));
            }
            //foreach(var item in query)
            //{
            //    item.Categories = new();
            //    var categories = await (from c in context.Categories
            //                            join comp in context.ComplaintCategories on c.Id equals comp.CategoryId
            //                            where comp.ComplaintId == item.ComplaintId
            //                            select c).ToListAsync();
            //    foreach(var cat in categories)
            //    {
            //        item.Categories.Add(cat.Name);
            //    }
            //}
            var totalRows = await query.CountAsync();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<ComplaintListViewModel>(items, totalRows);
        }

        public Task<int> PostAsync(ComplaintFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
