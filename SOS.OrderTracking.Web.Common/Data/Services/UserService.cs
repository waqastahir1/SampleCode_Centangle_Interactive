using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Data.Services
{
    public class UserService
    {
        private readonly AppDbContext context;
        private readonly ILogger<UserService> logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<int?>> GetUserCrewId(string userName, DateTime date, RoleType? organizationRole = null)
        {
            var query = await (from u in context.Users
                               where u.UserName == userName
                               select (int?)u.PartyId)
                         .ToListAsync();
            return query;
        }

    }
}