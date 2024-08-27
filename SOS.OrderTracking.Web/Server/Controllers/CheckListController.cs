using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CheckListController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ILogger<ManageGaurdsController> logger;
        private readonly SequenceService sequenceService;
        public CheckListController(AppDbContext appDbContext,
            ILogger<ManageGaurdsController> logger, SequenceService sequenceService)
        {
            context = appDbContext;
            this.logger = logger;
            this.sequenceService = sequenceService;

        }
        
        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] BaseIndexModel vm)
        {
            IQueryable<CheckListListViewModel> query = null;
            try
            { 
                    query = (from c in context.CheckListTypes
                                 select new CheckListListViewModel()
                                 {
                                     Id = c.Id,
                                     Feature = c.Name,
                                 });
              
                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

                return Ok(new IndexViewModel<CheckListListViewModel>(items, totalRows));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }

        }
        //get all banks to show in top dropdown
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var banks = new List<SelectListItem>();
                   banks.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "All"
                });
                 banks.AddRange(await (from o in context.Orgnizations
                                   join p in context.Parties on o.Id equals p.Id
                                   where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                                   select new SelectListItem(p.Id, p.FormalName)).ToListAsync());
             
                return Ok(banks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post(CheckListFormViewModel SelectedItem)
        {
            try
            {   
                var isRecordExist = await context.BankCheckLists.FirstOrDefaultAsync(x => x.Id == SelectedItem.checkListId);
                if (isRecordExist == null)
                {
                        CheckListType checkListType = new CheckListType();
                        checkListType.Id = sequenceService.GetNextCommonSequence();
                        checkListType.Name = SelectedItem.Feature;

                        context.CheckListTypes.Add(checkListType);
                        await context.SaveChangesAsync();
                }
                else
                {
                    isRecordExist.isActive = SelectedItem.isActive;
                    context.BankCheckLists.Update(isRecordExist);
                    await context.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCheckList()
        {
            try
            {
                var list = (from c in context.CheckListTypes
                            select
                            new SelectListItem(c.Id, c.Name));
                return Ok(list);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
    }
}
