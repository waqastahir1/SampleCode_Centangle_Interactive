using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using System.Linq;
using System.Threading.Tasks;
using static SOS.OrderTracking.Web.Shared.Constants.Roles;

namespace System.Security.Claims
{
    public static class UserExtensions
    {
        public static bool IsInitiator(this ClaimsPrincipal user)
        {
            return user.IsInRole(BANK_CPC) || user.IsInRole(BANK_BRANCH) || user.IsInRole(BANK_HYBRID);
        }
        public static bool IsSupervisor(this ClaimsPrincipal user)
        {
            return user.IsInRole(BANK_CPC_MANAGER) || user.IsInRole(BANK_BRANCH_MANAGER) || user.IsInRole(BANK_HYBRID);
        }

        public static bool IsInititorOrSupervisor(this ClaimsPrincipal user)
        {
            return (IsInitiator(user) || IsSupervisor(user) || user.IsInRole(BANK_HYBRID));
        }
        public static bool HasSOSRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(SUPER_ADMIN) || user.IsInRole(ADMIN) || user.IsInRole(REGIONAL_ADMIN) || user.IsInRole(SUBREGIONAL_ADMIN) || user.IsInRole(HEADOFFICE_BILLING);
        }
        public static bool HasSOSAdministrationRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(SUPER_ADMIN) || user.IsInRole(ADMIN);
        }
        public static async Task<string> GetUserRole(this ClaimsPrincipal user, IServiceScopeFactory scopeFactory)
        {
            if (!user.IsAuthenticated()) return "";
            if (user.IsInRole(SUPER_ADMIN)) return SUPER_ADMIN;
            else if (user.IsInRole(REGIONAL_ADMIN)) return REGIONAL_ADMIN;
            else if (user.IsInRole(SUBREGIONAL_ADMIN)) return SUBREGIONAL_ADMIN;
            else if (user.IsInRole(HEADOFFICE_BILLING)) return HEADOFFICE_BILLING;
            else if (user.IsInRole(ADMIN)) return ADMIN;
            else if (user.IsInRole(BANK_BRANCH)) return "Branch Initiator";
            else if (user.IsInRole(BANK_BRANCH_MANAGER)) return "Branch Supervisor";
            else if (user.IsInRole(BANK_CPC)) return "CPC Initiator";
            else if (user.IsInRole(BANK_CPC_MANAGER)) return "CPC Supervisor";
            else if (user.IsInRole(BANK_HYBRID)) return "Branch Initiator & Supervisor";
            else if (user.IsInRole(BANK)) return "Bank Headoffice";
            else if (user.IsInRole(VAULT_MANAGER)) return "Vault Manager";
            else if (user.IsInRole("BankGaurding")) return "Bank Gaurding";
            else if (user.IsInRole("CIT")) return "Crew";
            else
            {
                var userManager = scopeFactory.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var context = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
                var roles = await userManager.GetRolesAsync(context.Users.FirstOrDefault(x => x.UserName == user.Identity.Name));
                return string.Join(", ", roles);
            }
        }

        //public static string UserId(this ClaimsPrincipal user)
        //{
        //    return user.FindFirstValue(ClaimTypes.NameIdentifier);
        //}
    }
}
