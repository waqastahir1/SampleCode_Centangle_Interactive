using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext context;
        private UserManager<ApplicationUser> userManager { get; set; }

        public EmployeeService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public SelectListItem GetAtmManager(int atmId)
        {
            try
            {
                var v = (from p in context.Parties
                         join r in context.PartyRelationships on p.Id equals r.ToPartyId
                         where r.FromPartyId == atmId && (r.ToPartyRole == RoleType.ATMCashier || r.ToPartyRole == RoleType.ATMTechnician)
                         select new SelectListItem(p.Id, p.FormalName))
                        .FirstOrDefault();
                return v;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public async Task<RelationshipDetailViewModel> RelationshipDetail(int employeeId, DateTime? startDate)
        {
            RelationshipDetailViewModel relationshipDetailViewModel = null;
            PartyRelationship relationship = null;
            //terminating employee from another organization
            relationship = context.PartyRelationships.Where(x => x.FromPartyId == employeeId
                && x.ToPartyId != 1
                && (!x.ThruDate.HasValue || x.ThruDate.Value >= startDate)).FirstOrDefault();
            if (relationship != null)
            {
                var detail = context.Parties.FirstOrDefault(x => x.Id == relationship.ToPartyId);
                OrganizationType organizationType = (await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == relationship.ToPartyId)).OrganizationType;
                relationshipDetailViewModel = new RelationshipDetailViewModel();
                relationshipDetailViewModel.EmployeeId = detail.Id;
                relationshipDetailViewModel.OrganizationName = detail.ShortName + " - " + detail.FormalName;
                relationshipDetailViewModel.OrganizationTypeAsString = Enum.GetName(typeof(OrganizationType), organizationType);
                relationshipDetailViewModel.StartDate = relationship.StartDate;
                relationshipDetailViewModel.EndDate = relationship.ThruDate.HasValue == true ? relationship.ThruDate : null;
                return relationshipDetailViewModel;
            }
            return relationshipDetailViewModel;
        }
        public async Task<IEnumerable<SelectListItem>> GetEmployeesByTypeAsync(EmploymentType type)
        {

            //if (q == null)
            //  return Array.Empty<SelectListItem>();
            var parties = await (from pr in context.Parties
                                 join p in context.People on pr.Id equals p.Id
                                 where p.EmploymentType == type
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

        public async Task ChangeEmployeeRole(int employeeId, int organizationId,
            DateTime? startDate, DateTime? endDate, bool addToRole)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.PartyId == employeeId);
                if (user == null && addToRole)
                {
                    throw new Exception("Please make login Acount of selected employee to allocsate this role!");
                }
                // if end date less then start date
                if (endDate.HasValue)
                {
                    if (endDate < startDate)
                    {
                        throw new Exception("End Date should be greater then start date");
                    }
                }

                var organization = await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == organizationId);


                var relationship = await context.PartyRelationships.Where(x => x.FromPartyId == employeeId
                        && x.ToPartyId != 1
                        && (!x.ThruDate.HasValue || x.ThruDate >= startDate)).FirstOrDefaultAsync();
                if (relationship != null)
                {
                    var organizationName = context.Parties.FirstOrDefault(x => x.Id == relationship.ToPartyId).FormalName;
                    //if start date of already assigned relation is greater then termination date
                    if (relationship.StartDate > startDate.Value.AddDays(-1))
                        throw new Exception("Selected employee is already exist in Organization Named:" + organizationName + " from " + relationship.StartDate.ToString("o") +
                            " you cannot appoint him here before his startdate in another organization!");
                    relationship.ThruDate = startDate.Value.AddDays(-1);
                    await context.SaveChangesAsync();
                }



                string roleName = null;
                switch (organization.OrganizationType)
                {
                    case OrganizationType.RegionalControlCenter:
                        roleName = "SOS-Regional-Admin";
                        break;

                    case OrganizationType.SubRegionalControlStation:
                        roleName = "SOS-SubRegional-Admin";
                        break;

                    case OrganizationType.Crew:
                        roleName = "CIT";
                        break;

                    case OrganizationType.Vault:
                        roleName = "CIT";
                        break;

                    case OrganizationType.ATM:
                        roleName = "ATMR";
                        break;
                }
                if (addToRole)
                {
                    var allAssignedRoles = await userManager.GetRolesAsync(user);
                    var removeRoles = await userManager.RemoveFromRolesAsync(user, allAssignedRoles.ToArray());
                    var result = await userManager.AddToRoleAsync(user, roleName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
