using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SOS.OrderTracking.Web.Common.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Hubs
{
    [Authorize(AuthenticationSchemes = "IdentityServerJwtBearer")]
    public class ConsignmentHub : Hub
    {
        private readonly UserCacheService userCache;

        //static public Dictionary<string, string> Users { get; set; } = new Dictionary<string, string>();
        public ConsignmentHub(UserCacheService userCache)
        {
            this.userCache = userCache;
        } 
     
        public async override Task OnConnectedAsync()
        {
            var user = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await userCache.SetHubConnectionId(user, Context.ConnectionId); 
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

    }
}
