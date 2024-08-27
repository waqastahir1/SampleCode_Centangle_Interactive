using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Services
{
    public class UserCacheService : CacheServiceBase
    {

        private const string SessionSecret = "UTKN";
        private const string SessionTime = "UTK_EXP";
        private const string SignalRHubConnnectionId = "UCTNID";

        private const string OrganizationId = "OrgId";

        private const string UserIpKey = "UserIp";

        public UserCacheService(IDistributedCache cache, ILogger<PartiesCacheService> logger) : base(cache, logger)
        {
        }

        #region Code
        public async Task<string> GetUserSessionSecret(string id)
        {
            return await GetString($"{SessionSecret}{id}");
        }

        public async Task SetUserSessioinSecret(string id, string code)
        {
            await SetString($"{SessionSecret}{id}", code);
        }

        #endregion

        #region Code
        public async Task<string> GetHubConnectionId(string id)
        {
            return await GetString($"{SignalRHubConnnectionId}{id}");
        }

        public async Task SetHubConnectionId(string id, string connectionId)
        {
            await SetString($"{SignalRHubConnnectionId}{id}", connectionId);
        }

        #endregion

        #region SessionTime
        public async Task<DateTime?> GetSessionTime(string id)
        {
            return await GetDateTime($"{SessionTime}{id}");
        }

        public async Task SetSessionTime(string id, DateTime dateTime)
        {
            await SetDateTime($"{SessionTime}{id}", dateTime);
        }

        #endregion

        #region UserOrgId
        public async Task<int> GetOrgId(string id)
        {
            return await GetInt($"{OrganizationId}{id}");
        }

        public async Task SetOrgId(string id, int orgId)
        {
            await SetInt($"{OrganizationId}{id}", orgId);
        }

        #endregion


        #region UserIp
        public async Task<string> GetUserIp(string userName)
        {
            return await GetString($"{UserIpKey}{userName}");
        }

        public async Task SetUserIp(string id, string userIp)
        {
            await SetString($"{UserIpKey}{id}", userIp);
        }

        #endregion
    }
}
