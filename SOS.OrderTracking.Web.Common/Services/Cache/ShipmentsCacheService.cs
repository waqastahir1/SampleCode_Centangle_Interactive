using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Services.Cache
{
    public class ShipmentsCacheService : CacheServiceBase
    {
        private const string ShipmentCard = "SCARD";

        public ShipmentsCacheService(IDistributedCache cache, ILogger<ShipmentsCacheService> logger) : base(cache, logger)
        {
        }

        #region Code
        public async Task<ConsignmentListViewModel> GetShipment(int id)
        {
            var str = await GetString($"{ShipmentCard}{id}");
            if (!string.IsNullOrEmpty(str))
            {
                return JsonConvert.DeserializeObject<ConsignmentListViewModel>(str);
            }
            throw new KeyNotFoundException();
        }

        public async Task<ConsignmentListViewModel> GetShipmentOrDefault(int id)
        {
            var str = await GetString($"{ShipmentCard}{id}");
            if (!string.IsNullOrEmpty(str))
            {
                return JsonConvert.DeserializeObject<ConsignmentListViewModel>(str);
            }
            return null;
        }

        public async Task SetShipment(int id, ConsignmentListViewModel vm)
        {
            await SetString($"{ShipmentCard}{id}", vm == null ? string.Empty : JsonConvert.SerializeObject(vm));
        }

        #endregion
    }
}
