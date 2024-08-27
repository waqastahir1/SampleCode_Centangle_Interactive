using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class OrganizationController : ControllerBase
    {


        private readonly AppDbContext context;

        private readonly PartiesService partiesService;

        public OrganizationController(AppDbContext appDbContext,
           PartiesService partiesService)
        {
            context = appDbContext;
            this.partiesService = partiesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            return Ok(await partiesService.GetOrganizationsByTypeAsync(OrganizationType.MainCustomer));

        }

        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await partiesService.GetOrganizationsByTypeFlagAsync(OrganizationType.CustomerBranch);
            return Ok(branches);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchBranches(string search, string type)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            var branches = await partiesService.GetOrganizationsByTypeAsync(OrganizationType.CustomerBranch, search);
            return Ok(branches.Select(x => new
            {
                Id = x.IntValue,
                x.Text
            }));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchBranchesByStation(string search, string type, int stationId)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            var branches = await partiesService.GetCustomerBranchOrCrewOrganizationsStationAsync(search, stationId);
            return Ok(branches.Select(x => new
            {
                Id = x.IntValue,
                x.Text
            }));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchMembers(string search, string type)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            var result = await partiesService.GetCrewMembersAsync(search);
            return Ok(result.Select(x => new
            {
                Id = x.IntValue,
                x.Text
            }));
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchPeople(string search, string type)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            var result = await partiesService.GetPeopleAsync(search);
            return Ok(result.Select(x => new
            {
                Id = x.IntValue,
                x.Text
            }));
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchMembersByStation(string search, string type, int stationId)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            var result = await partiesService.GetCrewMembersStationAsync(search, stationId);
            return Ok(result.Select(x => new
            {
                Id = x.IntValue,
                x.Text
            }));
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchCrew(string search, string type)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            var result = await partiesService.GetCrewsAsync(search);
            return Ok(result.Select(x => new
            {
                Id = x.IntValue,
                x.Text
            }));
        }



        [HttpGet]
        public async Task<IActionResult> GetChildBranches(int id)
        {
            //Get All child for given bank, i.e. all branches of a bank
            var results = await partiesService.GetChildOrganizations(new List<int?>() { id }, OrganizationType.CustomerBranch);
            return Ok(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetAtms()
        {
            int? parentId = null;
            if (User.IsInRole("BANK"))
            {
                parentId = (await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name)).PartyId;
            }
            return Ok(await partiesService.GetOrganizationsByTypeAsync(OrganizationType.ATM, parentId));
        }

        [HttpGet]
        public async Task<IActionResult> GetAtmBranch(int id)
        {
            var atmCashSources = new List<SelectListItem>();

            var parent = await partiesService.GetAssociatedPartyByRole(childId: id, RoleType.ATM, RoleType.ATMBranch);
            if (parent != null)
            {
                atmCashSources.Add(parent);
                var associatedCPC = await partiesService.GetAssociatedCPCAsync(parent.IntValue.Value);
                if (associatedCPC != null)
                {
                    atmCashSources.Add(associatedCPC);
                }
            }

            //Get All parents for given ATM, where parents of ATM must be branches
            //var results = await partiesService.GetParentOrganizations(id, OrganizationType.BankBranch).ToListAsync();
            return Ok(atmCashSources);
        }
        /// <summary>
        /// Getting braches which are under CPC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCPCBranches(int id)
        {
            var query = await (from p in context.Parties
                               join r in context.PartyRelationships on p.Id equals r.FromPartyId
                               where r.ToPartyId == id &&
                                   r.FromPartyRole == RoleType.ChildOrganization
                                   && r.ToPartyRole == RoleType.BankCPC
                               select new SelectListItem(p.Id, p.ShortName + "-" + p.FormalName)).ToListAsync();
            return Ok(query);
        }

        [HttpGet]
        public async Task<IActionResult> GetSiblingBranches(int id1, int id2)
        {
            try
            {
                var results = await (
                               from c in context.PartyRelationships
                               join r in context.PartyRelationships on c.ToPartyId equals r.ToPartyId
                               join p in context.Parties on r.FromPartyId equals p.Id
                               join o in context.Orgnizations on p.Id equals o.Id

                               where o.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
                                 && (id1 == c.FromPartyId || id2 == c.FromPartyId)
                                 && r.ToPartyRole == RoleType.ParentOrganization
                                 && c.ToPartyRole == RoleType.ParentOrganization
                               select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName)).Distinct()
             .ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString() + ex.InnerException?.ToString());
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchSiblingBranches(string search, string type)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 3)
            {
                return Ok();
            }
            try
            {
                _ = int.TryParse(type?.Split(',').FirstOrDefault(), out int fromPartyId);
                _ = int.TryParse(type?.Split(',').LastOrDefault(), out int toPartyId);
                search = search.ToLower();
                var results = await (
                               from c in context.PartyRelationships
                               join r in context.PartyRelationships on c.ToPartyId equals r.ToPartyId
                               join p in context.Parties on r.FromPartyId equals p.Id
                               join o in context.Orgnizations on p.Id equals o.Id

                               where o.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
                                 && (fromPartyId == c.FromPartyId || toPartyId == c.FromPartyId)
                                 && r.ToPartyRole == RoleType.ParentOrganization
                                 && c.ToPartyRole == RoleType.ParentOrganization
                                 && (p.ShortName.ToLower().Contains(search) || p.FormalName.ToLower().Contains(search))
                                 && p.IsActive
                               select new { o.Id, Text = p.ShortName + "-" + p.FormalName }).Distinct()
             .ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString() + ex.InnerException?.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMainCustomers()
        {
            int? parentId = null;
            if (User.IsInRole("BANK"))
            {
                parentId = (await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name)).PartyId;
            }
            return Ok(await partiesService.GetOrganizationsByTypeAsync(OrganizationType.MainCustomer, parentId));
        }
    }
}
