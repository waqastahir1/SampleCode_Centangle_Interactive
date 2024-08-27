using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System.Collections.Generic;
using System.Linq;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class VaultService
    {
        private readonly AppDbContext context;

        public VaultService(AppDbContext context)
        {
            this.context = context;
        }
        public IQueryable<VaultListViewModel> GetVaults(int regionId, int subRegionId, int stationId, string sortColumn)
        {
            var query = (from o in context.Parties
                         from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                         from sr in context.Parties.Where(x => x.Id == o.SubregionId).DefaultIfEmpty()
                         from rgn in context.Parties.Where(x => x.Id == o.RegionId).DefaultIfEmpty()

                         from va in context.AssetAllocations.Where(x => x.PartyId == o.Id).DefaultIfEmpty()
                         from v in context.Assets.Where(x => x.Id == va.AssetId).DefaultIfEmpty()
                         orderby o.FormalName

                         where o.RegionId == regionId
                         && (subRegionId == 0 || o.SubregionId == subRegionId)
                         && (stationId == 0 || o.StationId == stationId)
                         select new VaultListViewModel()
                         {
                             Id = o.Id,
                             Name = o.FormalName,
                             RegionName = rgn.FormalName,
                             SubRegionName = sr.FormalName,
                             StationName = station.FormalName,
                             Vehicle = v.Description,
                             RegionId = o.RegionId,
                             SubRegionId = o.SubregionId,
                             StationId = o.StationId,

                             RelationshipInfo = (from r in context.PartyRelationships
                                                 join p in context.Parties on r.FromPartyId equals p.Id
                                                 where r.ToPartyId == o.Id && r.IsActive
                                                 select new RelationshipInfo()
                                                 {
                                                     StartDate = r.StartDate,
                                                     ThruDate = r.ThruDate,
                                                     IsActive = r.IsActive
                                                 }).FirstOrDefault(),
                             ActiveMembersCount = (from r in context.PartyRelationships
                                                   join p in context.Parties on r.FromPartyId equals p.Id
                                                   where r.ToPartyId == o.Id && r.IsActive
                                                   select r).Count(),
                             PresentMembersCount = (from a in context.EmployeeAttendance
                                                    join r in context.PartyRelationships on a.RelationshipId equals r.Id
                                                    where r.ToPartyId == o.Id && r.IsActive
                                                     && (a.AttendanceDate.Date.Equals(MyDateTime.Today) && a.AttendanceState == Shared.Enums.AttendanceState.Present)
                                                    select a).Count(),
                             HasAccount = context.Users.Any(x => x.PartyId == o.Id)
                         });

            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = query.OrderBy(sortColumn);
            }

            return query;
        }

    }
}
