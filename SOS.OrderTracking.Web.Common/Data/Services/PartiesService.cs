using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class PartiesService
    {
        private readonly AppDbContext context;

        public PartiesService(AppDbContext context)
        {
            this.context = context;
        }
        public IQueryable<OrganizationModel> GetCustomers()
        {
            return (from o in context.Orgnizations
                    join p in context.Parties on o.Id equals p.Id
                    where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                    && p.IsActive
                    orderby p.FormalName
                    select new OrganizationModel()
                    {
                        Id = o.Id,
                        Name = p.FormalName,
                        Code = p.ShortName,
                        ContactNo = p.PersonalContactNo,
                        Address = p.Address
                    });//TODO: Check for unique Bank names only
        }

        public OrganizationModel GetMainCustomer(int customerBranchId)
        {
            return (from o in context.Orgnizations
                    join p in context.Parties on o.Id equals p.Id
                    join r in context.PartyRelationships on p.Id equals r.ToPartyId
                    where o.OrganizationType == OrganizationType.MainCustomer
                    && r.ToPartyRole == RoleType.ParentOrganization
                    && r.FromPartyId == customerBranchId
                    && p.IsActive
                    orderby p.FormalName
                    select new OrganizationModel()
                    {
                        Id = o.Id,
                        Name = p.FormalName,
                        Code = p.ShortName,
                        ContactNo = p.PersonalContactNo,
                        Address = p.Address
                    }).FirstOrDefault();
        }

        public async Task<IEnumerable<SelectListItem>> GetOrganizationsByTypeAsync(OrganizationType type, string q)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         where o.OrganizationType.HasFlag(type)
                         && (p.ShortName.ToLower().Contains(q) || p.FormalName.ToLower().Contains(q))
                         && p.IsActive
                         orderby p.FormalName
                         select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName, p.Abbrevation));

            return await query.Take(20).ToArrayAsync();
        }
        public async Task<IEnumerable<SelectListItem>> GetCustomerBranchOrCrewOrganizationsStationAsync(string q, int stationId)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id

                         where (o.OrganizationType.HasFlag(OrganizationType.CustomerBranch))
                         && (p.ShortName.ToLower().Contains(q) || p.FormalName.ToLower().Contains(q))
                         && p.IsActive
                         && p.StationId == stationId
                         orderby p.FormalName
                         select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName, p.Abbrevation));

            return await query.Take(20).ToArrayAsync();
        }
        public async Task<IEnumerable<SelectListItem>> GetCrewMembersAsync(string q)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         join r in context.PartyRelationships on p.Id equals r.ToPartyId
                         join rp in context.Parties on r.FromPartyId equals rp.Id

                         where o.OrganizationType.HasFlag(OrganizationType.Crew)
                         && (rp.ShortName.Contains(q) || rp.FormalName.ToLower().Contains(q))
                         && p.IsActive && r.IsActive && rp.IsActive
                         orderby p.FormalName
                         select new SelectListItem(rp.Id, rp.ShortName + "-" + rp.FormalName, rp.Abbrevation));

            return await query.Distinct().Take(20).ToArrayAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetPeopleAsync(string q)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();
            var query = from p in context.Parties
                        join r in context.People on p.Id equals r.Id
                        where (p.ShortName.Contains(q) || p.FormalName.ToLower().Contains(q))
                        && p.IsActive
                        orderby p.FormalName
                        select new SelectListItem(p.Id, p.ShortName + "-" + p.FormalName, p.Abbrevation);

            return await query.Distinct().Take(20).ToArrayAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetCrewMembersStationAsync(string q, int stationId)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         join r in context.PartyRelationships on p.Id equals r.ToPartyId
                         join rp in context.Parties on r.FromPartyId equals rp.Id

                         where o.OrganizationType.HasFlag(OrganizationType.Crew)
                         && (rp.ShortName.Contains(q) || rp.FormalName.ToLower().Contains(q))
                         && p.IsActive && r.IsActive && rp.IsActive
                         && p.StationId == stationId
                         orderby p.FormalName
                         select new SelectListItem(rp.Id, rp.ShortName + "-" + rp.FormalName, rp.Abbrevation));

            return await query.Distinct().Take(20).ToArrayAsync();
        }
        public async Task<IEnumerable<SelectListItem>> GetCrewsAsync(string q)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();

            var query = (from p in context.Parties
                         join o in context.Orgnizations on p.Id equals o.Id
                         join r in context.PartyRelationships on p.Id equals r.FromPartyId
                         where o.OrganizationType == OrganizationType.Crew
                         && r.FromPartyRole == RoleType.Crew
                         && r.IsActive
                         && (string.IsNullOrEmpty(q) || p.FormalName.Contains(q))
                         select new SelectListItem()
                         {
                             IntValue = p.Id,
                             Text = p.FormalName,
                             // CrewLocation = new Point(o.Geolocation.Y, o.Geolocation.X),
                         });

            return await query.Distinct().Take(20).ToArrayAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetOrganizationsByTypeAsync(string q, params OrganizationType[] type)
        {
            if (q == null)
                return Array.Empty<SelectListItem>();

            q = q.ToLower();
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         where type.Contains(o.OrganizationType)
                         && (p.ShortName.ToLower().Contains(q) || p.FormalName.ToLower().Contains(q))
                         && p.IsActive
                         orderby p.FormalName
                         select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName, p.Abbrevation));

            return await query.Take(20).ToArrayAsync();
        }

        /// <summary>
        /// Returns Formal Name of Organizations with specified type
        /// </summary>
        /// <param name="type"></param> 
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetOrganizationsByTypeAsync(OrganizationType type, int? id = null)
        {
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         where ((o.OrganizationType == type && (id == null || o.Id == id)) || o.OrganizationType == OrganizationType.PrimaryOrganization)
                         && p.IsActive
                         orderby p.FormalName
                         select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName));
            return await query.ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAllRegionsAsync()
        {
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         where o.OrganizationType == OrganizationType.RegionalControlCenter
                         orderby p.FormalName
                         select new SelectListItem(o.Id, p.FormalName, p.Abbrevation));

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<SelectListItem>> GetUserOrganizations(string userName, OrganizationType organizationType)
        {
            var query = (from o in context.Parties
                         join r in context.PartyRelationships on o.Id equals r.ToPartyId
                         join p in context.Parties on r.FromPartyId equals p.Id
                         join u in context.Users on p.Id equals u.PartyId
                         where u.UserName == userName
                         && r.StartDate <= MyDateTime.Today
                         && (r.ThruDate == null || r.ThruDate >= MyDateTime.Today)
                         && o.Orgnization.OrganizationType == organizationType
                         && o.IsActive
                         orderby o.FormalName
                         select new SelectListItem(o.Id, o.FormalName, o.Abbrevation));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetExternalUserRegion(string userName)
        {
            var query = (from o in context.Parties
                         join p in context.Parties on o.Id equals p.RegionId
                         join u in context.Users on p.Id equals u.PartyId
                         where u.UserName == userName
                         && p.IsActive
                         orderby o.FormalName
                         select new SelectListItem(o.Id, o.FormalName, o.Abbrevation));

            return await query.ToArrayAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetExternalUserSubRegion(string userName)
        {
            var query = (from o in context.Parties
                         join p in context.Parties on o.Id equals p.SubregionId
                         join u in context.Users on p.Id equals u.PartyId
                         where u.UserName == userName
                         && o.IsActive
                         orderby o.FormalName
                         select new SelectListItem(o.Id, o.FormalName, o.Abbrevation));

            return await query.ToArrayAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetExternalUserStation(string userName)
        {
            var query = (from o in context.Parties
                         join p in context.Orgnizations on o.Id equals p.Id
                         where p.OrganizationType == OrganizationType.Station
                         && o.IsActive
                         orderby o.FormalName
                         select new SelectListItem(o.Id, o.FormalName, o.Abbrevation));

            var stations = await query.ToListAsync();
            stations.Insert(0, new SelectListItem(0, "All Stations"));
            return stations;
        }


        public async Task<IEnumerable<SelectListItem>> GetOrganizationsByTypeFlagAsync(OrganizationType type)
        {
            var query = (from o in context.Orgnizations
                         join p in context.Parties on o.Id equals p.Id
                         where o.OrganizationType.HasFlag(type)
                         && p.IsActive
                         orderby p.FormalName
                         select new SelectListItem(o.Id, p.FormalName, p.Abbrevation));

            return await query.ToArrayAsync();
        }


        /// <summary>
        /// Final
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SelectListItem>> GetChildOrganizations(List<int?> parentIds, OrganizationType childType)
        {
            return await (from p in context.Parties
                          join r in context.PartyRelationships on p.Id equals r.FromPartyId
                          where parentIds.Contains(r.ToPartyId) &&
                              p.Orgnization.OrganizationType == childType
                              && p.IsActive
                          orderby p.FormalName
                          select new SelectListItem(p.Id, p.FormalName, p.Abbrevation))
                          .AsNoTracking()
                    .ToListAsync();
        }

        /// <summary>
        /// Final - Proxy
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="childType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SelectListItem>> GetChildOrganizations(int? parentId, OrganizationType childType)
        {
            return await GetChildOrganizations(new List<int?>() { parentId }, childType);
        }


        public async Task<SelectListItem> GetParentRegions(int childId, OrganizationType parentType)
        {
            return await (from p in context.Parties
                          join r in context.PartyRelationships on p.Id equals r.ToPartyId
                          where r.FromPartyId == childId &&
                               r.ToPartyRole == RoleType.RegionalOrg &&
                               p.Orgnization.OrganizationType.HasFlag(parentType)
                               && p.IsActive
                          select new SelectListItem(p.Id, p.ShortName + "-" + p.FormalName)).FirstOrDefaultAsync();
        }

        public async Task<SelectListItem> GetAssociatedCPCAsync(int childId)
        {
            return await (from p in context.Parties
                          join r in context.PartyRelationships on p.Id equals r.ToPartyId
                          where r.FromPartyId == childId &&
                               r.ToPartyRole == RoleType.BankCPC &&
                               r.FromPartyRole == RoleType.ChildOrganization
                               && p.IsActive
                          select new SelectListItem(p.Id, p.ShortName + "-" + p.FormalName)).FirstOrDefaultAsync();
        }

        public async Task<SelectListItem> GetAssociatedPartyByRole(int childId, RoleType fromPartyRole, RoleType toRoleType)
        {
            return await (from r in context.PartyRelationships
                          where r.FromPartyId == childId &&
                               r.ToPartyRole == toRoleType &&
                               r.FromPartyRole == fromPartyRole
                          select new SelectListItem(r.ToPartyId, r.ToParty.ShortName + "-" + r.ToParty.FormalName))
                          .FirstOrDefaultAsync();
        }

        public IQueryable<CrewListViewModel> GetCrews(int regionId, int subRegionId, int stationId, string sortColumn)
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
                         && (regionId == 0 || o.RegionId == regionId)
                         && (subRegionId == 0 || o.SubregionId == subRegionId)
                         && (stationId == 0 || o.StationId == stationId)
                         select new CrewListViewModel()
                         {
                             Id = o.Id,
                             Name = o.FormalName,
                             StartDate = r.StartDate,
                             ThruDate = r.ThruDate,
                             RegionName = rgn.FormalName,
                             SubRegionName = sr.FormalName,
                             StationName = station.FormalName,
                             Vehicle = v.Description,
                             RegionId = o.RegionId,
                             SubRegionId = o.SubregionId,
                             StationId = o.StationId,
                             isActive = r.IsActive,
                             ActiveMembersCount = (from r in context.PartyRelationships
                                                   join p in context.Parties on r.FromPartyId equals p.Id
                                                   join e in context.People on p.Id equals e.Id
                                                   where r.ToPartyId == o.Id && r.IsActive

                                                   select r).Count(),
                             //PresentMembersCount = (from a in context.EmployeeAttendance
                             //                       join r in context.PartyRelationships on a.RelationshipId equals r.Id
                             //                      where r.ToPartyId == o.Id && r.IsActive
                             //                       && (a.AttendanceDate.Date.Equals(MyDateTime.Today) && a.AttendanceState == Shared.Enums.AttendanceState.Present)
                             //                      select a).Count(),
                             UserId = context.Users.FirstOrDefault(x => x.PartyId == o.Id).Id
                         });

            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = query.OrderBy(sortColumn);
            }

            return query;
        }

        public async Task<IEnumerable<SelectListItem>> GetRegularEmployees()
        {
            var parties = await (from pr in context.Parties
                                 join p in context.People on pr.Id equals p.Id
                                 where p.EmploymentType == EmploymentType.Regular
                                 select new
                                 {
                                     pr.Id,
                                     pr.ShortName,
                                     pr.FormalName,
                                     p.NationalId
                                 }).ToArrayAsync();

            return parties.Select(x => new SelectListItem()
            {
                IntValue = x.Id,
                Text = $"{x.ShortName}-{x.FormalName} {x.NationalId}".Trim()
            });
        }

    }
}
