
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.Services
{
    public class PartiesCacheService : CacheServiceBase
    {
        private const string Address = "PADD";
        private const string Code = "PC";
        private const string ContactNo = "PCN";
        private const string GeoCoordinates = "PGC";
        private const string TemporalGeoCoordinates = "PTPGC_";
        private const string GeoStatus = "PGS";
        private const string OrganizationType_ = "ORGT";
        private const string Name = "PN";
        private const string RegionAbbr = "PRA";
        private const string RegionName = "PRN";
        private const string StationName = "PSN";

        public PartiesCacheService(IDistributedCache cache, ILogger<PartiesCacheService> logger) : base(cache, logger)
        {
        }

        #region Code
        public async Task<string> GetCode(int id)
        {
            return await GetString($"{Code}{id}");
        }

        public async Task SetCode(int id, string code)
        {
            await SetString($"{Code}{id}", code);
        }

        #endregion

        #region Address
        public async Task<string> GetAddress(int id)
        {
            return await GetString($"{Address}{id}");
        }

        public async Task SetAddress(int id, string address)
        {
            await SetString($"{Address}{id}", address);
        }

        #endregion

        #region Contact
        public async Task<string> GetContactNo(int id)
        {
            return await GetString($"{ContactNo}{id}");
        }

        public async Task SetContactNo(int id, string contactNo)
        {
            await SetString($"{ContactNo}{id}", contactNo);
        }

        #endregion

        #region Name
        public async Task<string> GetName(int? id)
        {
            return await GetString($"{Name}{id}");
        }

        public async Task SetName(int id, string name)
        {
            await SetString($"{Name}{id}", name);
        }

        #endregion

        #region RegionName

        public async Task<string> GetRegionName(int id)
        {
            return await GetString($"{RegionName}{id}");
        }

        public async Task SetRegionName(int id, string name)
        {
            await SetString($"{RegionName}{id}", name);
        }

        #endregion

        #region RegionName

        public async Task<string> GetRegionAbbr(int id)
        {
            return await GetString($"{RegionAbbr}{id}");
        }

        public async Task SetRegionAbbr(int id, string abbr)
        {
            await SetString($"{RegionAbbr}{id}", abbr);
        }

        #endregion

        #region RegionName

        public async Task<string> GetStationName(int id)
        {
            return await GetString($"{StationName}{id}");
        }

        public async Task SetStationName(int id, string name)
        {
            await SetString($"{StationName}{id}", name);
        }

        #endregion

        #region GeoCoordinates

        static Point p = new Point();
        public async Task<Point> GetGeoCoordinate(int id)
        {
            var partyCoordinates = await GetString($"{GeoCoordinates}{id}");
            if (!string.IsNullOrEmpty(partyCoordinates) && partyCoordinates.ToLower().Contains("lat"))
            {
                return JsonConvert.DeserializeObject<Point>(partyCoordinates);
            }
            return p;
        }

        public async Task SetGeoCoordinate(int id, Point geoCoordinates)
        {
            if (geoCoordinates != null)
                await SetString($"{GeoCoordinates}{id}", JsonConvert.SerializeObject(geoCoordinates));
        }
        #endregion



        #region GeoCoordinates

        static TemporatlPoint defaultTemporalPoint = new();
        public async Task<TemporatlPoint> GetTemporatlGeoCoordinate(int id)
        {
            var val = await GetString($"{TemporalGeoCoordinates}{id}");
            if (!string.IsNullOrEmpty(val) && val.ToLower().Contains("lat"))
            {
                return JsonConvert.DeserializeObject<TemporatlPoint>(val);
            }
            return defaultTemporalPoint;
        }

        /// <summary>
        /// Sets the value of supplied geocoordinates is latest from an existing point in cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="geoCoordinates"></param>
        /// <returns></returns>
        public async Task<bool> SetTemporalGeoCoordinate(int id, TemporatlPoint geoCoordinates)
        {

            if (geoCoordinates != null)
            {
                var last = await GetTemporatlGeoCoordinate(id);
                if (geoCoordinates.TimeStamp > last.TimeStamp)
                {
                    await SetString($"{TemporalGeoCoordinates}{id}", JsonConvert.SerializeObject(geoCoordinates));
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region GeoStatus

        public async Task<DataRecordStatus> GetGeoStatus(int id)
        {
            var geoStatusString = await GetString($"{GeoStatus}{id}");
            if (!string.IsNullOrEmpty(geoStatusString))
            {
                if (int.TryParse(geoStatusString, out int geoStatus))
                {
                    return (DataRecordStatus)geoStatus;
                }
            }
            return DataRecordStatus.None;
        }

        public async Task SetGeoStatus(int id, DataRecordStatus geoStatus)
        {
            await SetString($"{GeoStatus}{id}", ((int)geoStatus).ToString());
        }
        #endregion

        public async Task<OrganizationType> GetOrganizationType(int? id)
        {
            var organizationTypeString = await GetString($"{OrganizationType_}{id}");
            if (!string.IsNullOrEmpty(organizationTypeString))
            {
                if (int.TryParse(organizationTypeString, out int organizationType))
                {
                    return (OrganizationType)organizationType;
                }
            }
            return OrganizationType.Unknown;
        }

        public async Task SetOrganizationType(int? id, OrganizationType? organizationType)
        {
            await SetString($"{OrganizationType_}{id}", ((int)organizationType.GetValueOrDefault()).ToString());
        }


    }


    public class CacheServiceBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;


        public CacheServiceBase(IDistributedCache cache, ILogger logger)
        {
            _cache = cache;
            _logger = logger;
        }



        protected async Task<int> GetInt(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return 0;
            }
        }

        protected async Task SetInt(string key, int value)
        {
            try
            {
                await _cache.SetStringAsync(key, value.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }

        protected async Task<string> GetString(string key, string defaultValue = null)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                return value ?? defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return defaultValue;
            }
        }

        protected async Task<DateTime?> GetDateTime(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                return DateTime.ParseExact(value, "dd-MMM-yyyy HH:mm:ss", null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return null;
            }
        }

        protected async Task SetString(string key, string value)
        {
            try
            {
                if (value != null)
                    await _cache.SetStringAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }

        protected async Task SetDateTime(string key, DateTime value)
        {
            try
            {
                await _cache.SetStringAsync(key, value.ToString("dd-MMM-yyyy HH:mm:ss"));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}
