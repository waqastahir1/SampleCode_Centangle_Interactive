using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.BankSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Admin
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class BankSettingController : ControllerBase, IBankSettingService
    {
        private readonly AppDbContext context;
        private ILogger<BankSettingController> logger;
        public BankSettingController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<BankSettingFormViewModel> GetAsync(int id)
        {
            var bank = await context.Parties.FirstOrDefaultAsync(x => x.Id == id);

            return bank.JsonData == null ?
                new BankSettingFormViewModel() { Id = id} :
                JsonConvert.DeserializeObject<BankSettingFormViewModel>(bank.JsonData); 
        }

        [HttpGet]
        public async Task<IndexViewModel<BankSettingListViewModel>> GetPageAsync([FromQuery] BaseIndexModel vm)
        {

            //if (vm.RegionId.GetValueOrDefault() == 0)
            //    throw new NotFoundException("No Content Found");


            var query = (from p in context.Parties
                         where p.Orgnization.OrganizationType.HasFlag(OrganizationType.MainCustomer)
                         && (vm.RegionId.GetValueOrDefault() == 0 || vm.RegionId == p.RegionId)
                         && (vm.SubRegionId.GetValueOrDefault() == 0 || vm.SubRegionId == p.SubregionId)
                         && (vm.StationId.GetValueOrDefault() == 0 || vm.StationId == p.StationId)
                         select new BankSettingListViewModel()
                         {
                             Id = p.Id,
                             BankCode = p.ShortName,
                             FormalName = p.FormalName,
                             JsonData = p.JsonData
                         });

            if (!string.IsNullOrEmpty(vm.SortColumn))
            {
                query = query.OrderBy(vm.SortColumn);
            }
            var totalRows = await query.CountAsync();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<BankSettingListViewModel>(items, totalRows);
        }

        [HttpPost]
        public async Task<int> PostAsync(BankSettingFormViewModel selectedItem)
        {
            var bank = await context.Parties.FirstOrDefaultAsync(x => x.Id == selectedItem.Id);
            bank.JsonData = JsonConvert.SerializeObject(selectedItem);
            return await context.SaveChangesAsync();
        }
    }
}
