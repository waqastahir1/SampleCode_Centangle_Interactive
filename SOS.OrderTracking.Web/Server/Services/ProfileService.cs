using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel; 
using SOS.OrderTracking.Web.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Services
{
    public class ProfileService : IProfileService
    {
        public ProfileService()
        {

        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var nameClaim = context.Subject.FindAll(JwtClaimTypes.Name);
            context.IssuedClaims.AddRange(nameClaim);

            var roleClaims = context.Subject.FindAll(JwtClaimTypes.Role);
            context.IssuedClaims.AddRange(roleClaims);

            var dobClaims = context.Subject.FindAll(ClaimTypes.DateOfBirth);
            context.IssuedClaims.AddRange(dobClaims);

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
