using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FmsController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly SOS_VIEWSContext GBMScontext;

        public FmsController(AppDbContext context, SOS_VIEWSContext GBMScontext)
        {
            this.context = context;
            this.GBMScontext = GBMScontext;
        }

        [HttpGet]
        [Route("GetActiveCrews")]
        public async Task<IActionResult> GetActiveCrews()
        {
            var query = (from o in context.Parties
                         join r in context.PartyRelationships on o.Id equals r.FromPartyId
                         from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                         from sr in context.Parties.Where(x => x.Id == o.SubregionId).DefaultIfEmpty()
                         from rgn in context.Parties.Where(x => x.Id == o.RegionId).DefaultIfEmpty()

                         from va in context.AssetAllocations.Where(x => x.PartyId == o.Id).DefaultIfEmpty()
                         from v in context.Assets.Where(x => x.Id == va.AssetId).DefaultIfEmpty()
                         orderby o.FormalName

                         where r.FromPartyRole == RoleType.Crew
                         && r.StartDate <= DateTime.Today && (r.ThruDate == null || r.ThruDate >= DateTime.Today)
                         select new
                         {
                             // Id = o.Id,
                             Crew = o.FormalName,
                             Vehicle = v.Description,
                             Members = (from activeMembers in context.PartyRelationships
                                        join p in context.Parties on activeMembers.FromPartyId equals p.Id
                                        where o.Id == activeMembers.ToPartyId && activeMembers.IsActive
                                        select new
                                        {
                                            Designation = activeMembers.FromPartyRole.ToString(),
                                            Name = p.FormalName,
                                            Code = p.ShortName,
                                        }).ToList(),
                         });

            return Ok(await query.ToListAsync());
        }

        [HttpPost]
        [Route("GetActiveCrews")]
        public async Task<IActionResult> PostVehicleStatus(VehicleInfo vehicleInfo)
        {


            return Ok();
        }

        [HttpGet]
        [Route("GetActiveVehicles")]
        public async Task<IActionResult> GetActiveVehicles()
        {
            try
            {
                var query = await GBMScontext.RbVehicles.ToArrayAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }

        }

    }

    public class VehicleInfo
    {
        public string VehicleCode { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public VehicleIncident VehicleIncident { get; set; }

        public string Reason { get; set; }

    }

    public enum VehicleIncident
    {

        OffRoad = 1,

        OnRoad = 32

    }
}
