using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Extensions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsignmentsController : ControllerBase
    {
        private readonly AppDbContext context; 
        private readonly ILogger<ConsignmentsController> logger; 
        private readonly NotificationService _notificationService;
        private readonly ShipmentsCacheService shipmentsCache;
        private readonly PartiesCacheService _partiesCache;

        public ConsignmentsController(AppDbContext context,
            ILogger<ConsignmentsController> logger, 
            PartiesCacheService partiesCache,
            NotificationService notificationService,
            ShipmentsCacheService shipmentsCache)
        {
            this.context = context;
            this.logger = logger;
            _partiesCache = partiesCache;
            _notificationService = notificationService;
            this.shipmentsCache = shipmentsCache;
        }

        [HttpPost]
        [Route("PostLocation")]
        public async Task<IActionResult> PostLocation([FromBody] System.Text.Json.JsonElement entity)
        {
            try
            {
                logger.LogInformation($"PostLocation {User.Identity.Name} -> {entity.GetRawText()}");
                LocationModel model = JsonConvert.DeserializeObject<LocationModel>(entity.GetRawText());
                var latestPoint = model.Points.OrderByDescending(x => x.TimeStamp).FirstOrDefault();

                if(latestPoint == null)
                {
                    logger.LogWarning($"PostLocation {User.Identity.Name} -> No Location data");
                    return Ok("No Locations points");
                }

                // get the latest location of crew
                var locationInfo = await (from u in context.Users
                                          //from l in context.PartyLocations.Where(x => x.PartyId == u.PartyId).DefaultIfEmpty()
                                          where u.UserName == User.Identity.Name
                                          //orderby l.TimeStamp descending
                                          select new
                                          {
                                              u.PartyId,
                                              //l.Geolocation,
                                              //l.TimeStamp
                                          })
                                          .FirstOrDefaultAsync();

                var result = await _partiesCache.SetTemporalGeoCoordinate(locationInfo.PartyId, new Shared.ViewModels.TemporatlPoint(latestPoint.Lat, latestPoint.Long_, latestPoint.TimeStamp.GetValueOrDefault()));

                //logger.LogInformation($"locationInfo {locationInfo?.PartyId} -> {locationInfo.Geolocation?.Y}, {locationInfo.Geolocation?.X} ts={locationInfo.TimeStamp}, u-> {result}");

            

                // just take the unique locations which came later than latest location
                //foreach (var location in model.Points
                //    .Where(x => x.TimeStamp > locationInfo.TimeStamp && x.Long_.HasValue && x.Lat.HasValue).Distinct())
                //{

                //    context.PartyLocations.Add(
                //            new PartyLocation()
                //            {
                //                PartyId = locationInfo.PartyId,
                //                TimeStamp = location.TimeStamp.Value,
                //                CreatdBy = User.Identity.Name,
                //                CreatedAt = MyDateTime.Now,
                //                Geolocation = new Point(location.Long_.Value, location.Lat.Value) { SRID = 4326 }
                //            });
                //}

              
                //await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //logger.LogError(ex.ToString());
                //logger.LogError(ex.InnerException?.ToString());
            }
            return Ok();
        }

        [HttpPost]
        [Route("PostDenomination")]
        public async Task<IActionResult> PostDenomination([FromBody] System.Text.Json.JsonElement entity)
        {
            logger.LogInformation($"PostDenomination {User.Identity.Name} -> {entity.GetRawText()}");
            var viewModel = JsonConvert.DeserializeObject<CitDenominationViewModel>(entity.GetRawText());

            if (viewModel.ConsignmentId == 0)
            {
                return BadRequest("Denomination cannot be tracked without Consignment...");
            }

            try
            {
              var  denom = await context.Denominations.FirstOrDefaultAsync(x => x.ConsignmentId == viewModel.ConsignmentId);
                if (denom == null)
                {
                    denom = new Denomination()
                    {
                        Id = context.Sequences.GetNextCommonSequence(),
                        ConsignmentId = viewModel.ConsignmentId
                    };
                    context.Denominations.Add(denom);
                }

                var f = 1;
                if (viewModel.Type == DenominationType.Bundles)
                {
                    f = 1000;
                }
                else if (viewModel.Type == DenominationType.Packets)
                {
                    f = 100;
                }

                denom.Currency1x = viewModel.Currency1x.GetValueOrDefault();
                denom.Currency2x = viewModel.Currency2x.GetValueOrDefault();
                denom.Currency5x = viewModel.Currency5x.GetValueOrDefault();
                denom.Currency10x = viewModel.Currency10x.GetValueOrDefault();
                denom.Currency20x = viewModel.Currency20x.GetValueOrDefault();
                denom.Currency50x = viewModel.Currency50x.GetValueOrDefault();

                //todo: un-comment when mobile app is updated for 75 denom.
                //denom.Currency75x = viewModel.Currency75x.GetValueOrDefault();

                denom.Currency100x = viewModel.Currency100x.GetValueOrDefault();
                denom.Currency500x = viewModel.Currency500x.GetValueOrDefault();
                denom.Currency1000x = viewModel.Currency1000x.GetValueOrDefault();
                denom.Currency5000x = viewModel.Currency5000x.GetValueOrDefault();

                Consignment consignment = context.Consignments.FirstOrDefault(x => x.Id == viewModel.ConsignmentId);

                consignment.Amount =
                    (denom.Currency1x * 1 * f) +
                    (denom.Currency2x * 2 * f) +
                    (denom.Currency5x * 5 * f) +
                    (denom.Currency10x * 10 * f) +
                    (denom.Currency20x * 20 * f) +
                    (denom.Currency50x * 50 * f) +
                    (denom.Currency75x * 75 * f) +
                    (denom.Currency100x * 100 * f) +
                    (denom.Currency500x * 500 * f) +
                    (denom.Currency1000x * 1000 * f) +
                    (denom.Currency5000x * 5000 * f);

                var resultCount = await context.SaveChangesAsync();
                var cachedShipment = await shipmentsCache.GetShipment(consignment.Id);
                if (cachedShipment != null)
                {
                    cachedShipment.Amount = consignment.Amount;
                    cachedShipment.AmountPKR = consignment.AmountPKR;
                    cachedShipment.Denomination = denom.ToViewModel(consignment.ShipmentCode);
                    await shipmentsCache.SetShipment(consignment.Id, cachedShipment);
                }
                logger.LogInformation($"Denomination updated {consignment.Id} -> {consignment.Amount} - rows-> {resultCount}");
                foreach (var d in await context.ConsignmentDeliveries.Include(x => x.Consignment).Where(x => x.ConsignmentId == viewModel.ConsignmentId).ToArrayAsync())
                {
                    if (d.Id != viewModel.DeliveryId)
                    {
                        await _notificationService.CreateShipmentNotificationForMobile(d.ConsignmentId,  NotificationType.UpdatedDenomination, NotificationCategory.CIT);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

 
}