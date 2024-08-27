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
    public class BankCheckListController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ILogger<BankCheckListController> logger;
        private readonly SequenceService sequenceService;
        public BankCheckListController(AppDbContext appDbContext,
            ILogger<BankCheckListController> logger, SequenceService sequenceService)
        {
            context = appDbContext;
            this.logger = logger;
            this.sequenceService = sequenceService;

        }

        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] CheckListAdditionalValueViewModel vm)
        {
            IQueryable<CheckListListViewModel> query = null;
            try
            {

                query = (from c in context.CheckListTypes
                         join b in context.BankCheckLists on c.Id equals b.CheckListTypeId
                         where b.OrganizationId == vm.bankId
                         select new CheckListListViewModel()
                         {
                             Id = b.Id,
                             Feature = c.Name,
                             isActive = b.isActive
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
                    Value = "all",
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
        [HttpGet]
        public async Task<IActionResult> GetFewCustomers(string search)
        {
            try
            {
                var banks = new List<SelectListItem>();
                banks.Add(new SelectListItem("all", "All"));
                banks.AddRange(await (from o in context.Orgnizations
                                      join p in context.Parties on o.Id equals p.Id
                                      where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization) && p.FormalName.Contains(search)
                                      select new SelectListItem(p.Id.ToString(), p.FormalName)).Take(3).ToListAsync());

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
                BankCheckList bankCheckList = null;

                var banks = (await (from o in context.Orgnizations
                                    join p in context.Parties on o.Id equals p.Id
                                    where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                                    select new SelectListItem(p.Id, p.FormalName)).ToListAsync());
            
                
                    var isRecordExist = await context.BankCheckLists.FirstOrDefaultAsync(x => x.Id == SelectedItem.checkListId);
                    if (isRecordExist == null)
                    {
                        if (SelectedItem.BankId.Equals("all"))
                        {

                            foreach (var bank in banks)
                            {
                            var isParticularExist = await context.BankCheckLists.FirstOrDefaultAsync(x => x.CheckListTypeId == SelectedItem.featureId && x.OrganizationId == Convert.ToInt32(bank.Value));
                            //check if selected particular already associated with selected bank
                            if (isParticularExist == null)
                            {
                                bankCheckList = new BankCheckList();
                                bankCheckList.Id = sequenceService.GetNextCommonSequence();
                                bankCheckList.CheckListTypeId = SelectedItem.featureId;
                                bankCheckList.OrganizationId = Convert.ToInt32(bank.Value);
                                bankCheckList.isActive = false;
                                context.BankCheckLists.Add(bankCheckList);
                            }
                            }
                        }

                        else
                        {
                        var isParticularExist = await context.BankCheckLists.FirstOrDefaultAsync(x => x.CheckListTypeId == SelectedItem.featureId && x.OrganizationId == Convert.ToInt32(SelectedItem.BankId));
                        //check if selected particular already associated with selected bank
                        if (isParticularExist == null)
                        {
                            bankCheckList = new BankCheckList();
                            bankCheckList.Id = sequenceService.GetNextCommonSequence();
                            bankCheckList.CheckListTypeId = SelectedItem.featureId;
                            bankCheckList.OrganizationId = Convert.ToInt32(SelectedItem.BankId);
                            bankCheckList.isActive = false;
                            context.BankCheckLists.Add(bankCheckList);
                        }
                        }
                        
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
