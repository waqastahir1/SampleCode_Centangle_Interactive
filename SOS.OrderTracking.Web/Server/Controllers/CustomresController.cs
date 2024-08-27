using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class CustomersController : ControllerBase
    {
        

        private readonly AppDbContext context;

        private readonly PartiesService partiesService;

        private readonly SequenceService sequenceService;

        private readonly ILogger<CustomersController> logger;

        public CustomersController(AppDbContext appDbContext, 
            
            PartiesService partiesService,
            SequenceService sequenceService,
            ILogger<CustomersController> logger)
        {
            context = appDbContext;
            
            this.partiesService = partiesService;
            this.sequenceService = sequenceService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int rowsPerPage, int currentIndex)
        {
            var query = partiesService.GetCustomers();
            var totalRows = query.Count();
        
            var items = await query.Skip((currentIndex - 1) * rowsPerPage).Take(rowsPerPage).ToArrayAsync();

            return Ok(new IndexViewModel<OrganizationModel>(items, totalRows));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await (from o in context.Orgnizations
                                  join p in context.Parties on o.Id equals p.Id
                          where o.Id == id
                          select new OrganizationViewModel()
                          {
                              Id = o.Id,
                              Code = p.ShortName,
                              Name = p.FormalName,
                              Address = p.Address,
                              ContactNo = p.PersonalContactNo,
                              Lat = o.Geolocation.Y,
                              Long = o.Geolocation.X
                          }).FirstAsync();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OrganizationViewModel SelectedItem)
        {
            Organization organization = null;
            PartyRelationship relationship = null;
            Party party = null;
            try
            {
                if (SelectedItem.Id == 0)
                {
                    party = new Party
                    {
                        Id = sequenceService.GetNextPartiesSequence(),
                        PartyType = PartyType.Organization
                    };
                    context.Parties.Add(party);

                    // Create organization with same key
                    organization = new Organization
                    {
                        Id = party.Id,
                        OrganizationType = OrganizationType.Customer
                    };

                    context.Orgnizations.Add(organization);

                    relationship = new PartyRelationship
                    {
                        Id = context.PartyRelationships.Max(x => (int?)x.Id).GetValueOrDefault() + 1,
                        ToPartyId = 1, // Parent Customer Id
                        FromPartyId = party.Id,
                        FromPartyRole = RoleType.Customer,
                        ToPartyRole = RoleType.ServiceProvider
                    };

                    context.PartyRelationships.Add(relationship);

                }
                else
                {
                    organization = await context.Orgnizations.FirstAsync(x => x.Id == SelectedItem.Id);
                    party = await context.Parties.FirstAsync(x => x.Id == SelectedItem.Id);
                    relationship = await context.PartyRelationships.AsTracking()
                        .FirstAsync(x => x.Id == SelectedItem.RelationshipId);
                }
                party.FormalName = SelectedItem.Name;
                party.ShortName = SelectedItem.Code;
                party.Address = SelectedItem.Address;
                party.PersonalContactNo = SelectedItem.ContactNo;
                organization.Geolocation = new NetTopologySuite.Geometries.Point(SelectedItem.Long, SelectedItem.Lat) { SRID = 3857 };

                relationship.StartDate = MyDateTime.Today;
               int result = await context.SaveChangesAsync();
                logger.LogInformation($"{result} Rows Effected");
                if (SelectedItem.Uploaded)
                {
                  
                }

                return Ok();
            }
            catch(Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
    }
}
