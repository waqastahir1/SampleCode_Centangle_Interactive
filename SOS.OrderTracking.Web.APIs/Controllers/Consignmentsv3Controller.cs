using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Serilog.Core;
using SOS.OrderTracking.Web.APIs.Models;
using SOS.OrderTracking.Web.APIs.Models.WorkOrder;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.StaticClasses;
using SOS.OrderTracking.Web.Shared.ViewModels.BankSetting;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConsignmentService = SOS.OrderTracking.Web.APIs.Services.ConsignmentService;
using Constants = SOS.OrderTracking.Web.Shared.Constants;

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConsignmentsV3Controller : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ConsignmentService consignmentService;
        private readonly UserService userService;
        private readonly ILogger logger;
        private readonly HttpClient http;
        private readonly NotificationService notificationService;
        private readonly ShipmentsCacheService shipmentsCache;
        static GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        private readonly SmtpEmailManager emailManager;
        private readonly IWebHostEnvironment env;

        public ConsignmentsV3Controller(AppDbContext context,
            ConsignmentService consignmentService,
            UserService userService,
            ILogger<ConsignmentsV3Controller> logger,
            HttpClient http,
            NotificationService notificationService,
            ShipmentsCacheService shipmentsCache,
            ShipmentDeliveryLogService shipmentDeliveryLogService,
            SmtpEmailManager emailManager,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.consignmentService = consignmentService;
            this.userService = userService;
            this.logger = logger;
            this.http = http;
            this.notificationService = notificationService;
            this.shipmentsCache = shipmentsCache;
            this.emailManager = emailManager;
            this.env = env;
        }

        #region Get
        [HttpGet]
        [Produces(typeof(CitConsignmentV3Model[]))]
        public async Task<IActionResult> GetAll(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var crewId = await userService.GetUserCrewId(User.Identity.Name, MyDateTime.Today);

            var query = consignmentService.GetQueryV3(crewId);
            var list = (await query.ToListAsync());
            foreach (var next in list)
            {
                next.FromPartyLat = next.FromPartyLat.GetValueOrDefault();
                next.FromPartyLong = next.FromPartyLong.GetValueOrDefault();

                next.ToPartyLat = next.ToPartyLat.GetValueOrDefault();
                next.ToPartyLong = next.ToPartyLong.GetValueOrDefault();

                var fromParty = await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == next.FromPartyId);
                var toParty = await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == next.ToPartyId);
                next.SealsCodes = await context.ShipmentSealCodes
                    .Where(x => x.ConsignmentId == next.ConsignmentId)
                    .Select(x => x.SealCode).ToArrayAsync();

                if ((fromParty?.OrganizationType).GetValueOrDefault().HasFlag(OrganizationType.PrimaryOrganization))
                {
                    next.DeliveryType = (toParty?.OrganizationType).GetValueOrDefault().HasFlag(OrganizationType.PrimaryOrganization) ?
                        DeliveryType.CrewToCrew : DeliveryType.CrewToCustomer;
                }
                else
                {
                    next.DeliveryType = (toParty?.OrganizationType).GetValueOrDefault().HasFlag(OrganizationType.PrimaryOrganization) ?
                       DeliveryType.CustomerToCrew : DeliveryType.NormalDelivery;
                }


                next.ConsignmentDeliveryStatus = (next.Description == "Vault" || next.Description == "VaultOnWheels")
                            && next.ConsignmentDeliveryStatus == "InTransit" ? "InVault" : next.ConsignmentDeliveryStatus;

                next.ManualShipmentCode = next.ManualShipmentCode ?? string.Empty;
                next.IsCds = next.ConsignmentNo.Contains("CDS");

                next.SealsCodes = context.ShipmentSealCodes
                                   .Where(x => x.ConsignmentId == next.ConsignmentId).Select(x => x.SealCode).ToList();


                BankSettingFormViewModel bankSettingCollectionFormViewModel = null;
                BankSettingFormViewModel bankSettingDeliveryFormViewModel = null;

                if (!string.IsNullOrEmpty(next.CollectiionMainCustomerJsonData))
                {
                    try
                    {
                        bankSettingCollectionFormViewModel = JsonConvert.DeserializeObject<BankSettingFormViewModel>(next.CollectiionMainCustomerJsonData);
                        next.SkipQRCodeOnCollection = bankSettingCollectionFormViewModel.SkipQRCodeOnCollection && (!next.IsVault);
                        next.EnableManualShipmentNo = bankSettingCollectionFormViewModel.EnableManualShipmentNo;
                    }
                    catch { }
                }


                if (!string.IsNullOrEmpty(next.DeliveryMainCustomerJsonData))
                {
                    try
                    {
                        bankSettingDeliveryFormViewModel = JsonConvert.DeserializeObject<BankSettingFormViewModel>(next.DeliveryMainCustomerJsonData);
                        next.SkipQRCodeOnDelivery = bankSettingDeliveryFormViewModel.SkipQRCodeOnDelivery && (!next.IsVault);
                    }
                    catch { }
                }

                if (next.FromPartyName.Contains("SBP") && bankSettingDeliveryFormViewModel != null)
                    if (bankSettingDeliveryFormViewModel.SkipQRForSBP)
                        next.SkipQRCodeOnCollection = true;

                if (next.ToPartyName.Contains("SBP") && bankSettingCollectionFormViewModel != null)
                    if (bankSettingCollectionFormViewModel.SkipQRForSBP)
                        next.SkipQRCodeOnDelivery = true;


            }

            return Ok(list);
        }

        [HttpGet]
        [Produces(typeof(CitConsignmentV3Model))]
        public async Task<IActionResult> Get(int id)
        {
            var crewId = await userService.GetUserCrewId(User.Identity.Name, MyDateTime.Today);
            if (crewId.Count == 0)
                return BadRequest("This user is not included in any crew");

            var next = consignmentService.GetQueryV3(crewId: crewId, deliveryId: id).FirstOrDefault();

            if (next == null)
            {
                logger.LogInformation("Enither shipment is not found or this shipment is not assigned to this crew.");
                return BadRequest("Enither shipment is not found or this shipment is not assigned to this crew.");

            }

            var fromParty = context.Orgnizations.Find(next.FromPartyId);
            var toParty = context.Orgnizations.Find(next.ToPartyId);

            if ((fromParty?.OrganizationType).GetValueOrDefault().HasFlag(OrganizationType.PrimaryOrganization))
            {
                next.DeliveryType = (toParty?.OrganizationType).GetValueOrDefault().HasFlag(OrganizationType.PrimaryOrganization) ?
                    DeliveryType.CrewToCrew : DeliveryType.CrewToCustomer;
            }
            else
            {
                next.DeliveryType = (toParty?.OrganizationType).GetValueOrDefault().HasFlag(OrganizationType.PrimaryOrganization) ?
                   DeliveryType.CustomerToCrew : DeliveryType.NormalDelivery;
            }

            next.ConsignmentDeliveryStatus = (next.Description == "Vault" || next.Description == "VaultOnWheels")
                          && next.ConsignmentDeliveryStatus == "InTransit" ? "InVault" : next.ConsignmentDeliveryStatus;

            next.ManualShipmentCode = next.ManualShipmentCode ?? string.Empty;

            next.SealsCodes = context.ShipmentSealCodes
                               .Where(x => x.ConsignmentId == next.ConsignmentId).Select(x => x.SealCode).ToList();

            next.FromPartyLat = next.FromPartyLat.GetValueOrDefault();
            next.FromPartyLong = next.FromPartyLong.GetValueOrDefault();

            next.ToPartyLat = next.ToPartyLat.GetValueOrDefault();
            next.ToPartyLong = next.ToPartyLong.GetValueOrDefault();
            next.IsCds = next.ConsignmentNo.Contains("CDS");

            next.PickupPinCode = next.FromPartyId;
            next.DropoffPinCode = next.ToPartyId;

            BankSettingFormViewModel bankSettingCollectionFormViewModel = null;
            BankSettingFormViewModel bankSettingDeliveryFormViewModel = null;

            if (!string.IsNullOrEmpty(next.CollectiionMainCustomerJsonData))
            {
                try
                {
                    bankSettingCollectionFormViewModel = JsonConvert.DeserializeObject<BankSettingFormViewModel>(next.CollectiionMainCustomerJsonData);
                    next.SkipQRCodeOnCollection = bankSettingCollectionFormViewModel.SkipQRCodeOnCollection && (!next.IsVault);
                    next.EnableManualShipmentNo = bankSettingCollectionFormViewModel.EnableManualShipmentNo;
                }
                catch { }
            }


            if (!string.IsNullOrEmpty(next.DeliveryMainCustomerJsonData))
            {
                try
                {
                    bankSettingDeliveryFormViewModel = JsonConvert.DeserializeObject<BankSettingFormViewModel>(next.DeliveryMainCustomerJsonData);
                    next.SkipQRCodeOnDelivery = bankSettingDeliveryFormViewModel.SkipQRCodeOnDelivery && (!next.IsVault);
                }
                catch { }
            }

            if (next.FromPartyName.Contains("SBP") && bankSettingDeliveryFormViewModel != null)
                if (bankSettingDeliveryFormViewModel.SkipQRForSBP)
                    next.SkipQRCodeOnCollection = true;

            if (next.ToPartyName.Contains("SBP") && bankSettingCollectionFormViewModel != null)
                if (bankSettingCollectionFormViewModel.SkipQRForSBP)
                    next.SkipQRCodeOnDelivery = true;


            return Ok(next);
        }

        #endregion

        #region Checkin v3
        /// <summary>
        /// V6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Acknowledge([FromBody] CitConsignmentBaseModel model)
        {
            try
            {
                LogSignature(model);
                var previousState = ConsignmentDeliveryState.CrewAssigned;
                var currentState = ConsignmentDeliveryState.Acknowldged;
                var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == model.ConsignmentId);
                if (consignment == null)
                    throw new Exception("consignment does not exist");

                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Parent)
                    .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

                if (delivery == null)
                    throw new Exception("consignment delivery does not exist");

                await UpdateDeliveryState(delivery, previousState, currentState, model);

                if (delivery.Parent == null)
                    await UpdateConsignmentAndStatusAsync(consignment, delivery.Id, currentState, model.TimeStamp.DateTime);

                return Ok();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return Ok();
        }

        /// <summary>
        /// V6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckinPickup([FromBody] System.Text.Json.JsonElement entity)
        {
            try
            {
                LogSignature(entity);
                var model = JsonConvert.DeserializeObject<CheckinPickupModel>(entity.GetRawText());
                LogSignature(model);
                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Parent)
                    .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);


                if (delivery == null)
                    throw new Exception($"Delivery not found");

                await UpdateDeliveryState(delivery, model.PreviousState, model.CurrentState, model);

                if (delivery.Parent == null)
                    await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id, model.CurrentState, model.TimeStamp.DateTime);

                return Ok();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message);
            }
        }

        private async Task UpdateDeliveryState<T>(ConsignmentDelivery delivery, Shared.Enums.ConsignmentDeliveryState previousState, Shared.Enums.ConsignmentDeliveryState currentState, T model)
            where T : CitConsignmentBaseModel
        {

            if (delivery.DeliveryState < currentState)
            {
                delivery.DeliveryState = currentState;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// V6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckinDropoff([FromBody] CheckinDropOffModel model)
        {
            LogSignature(model);
            try
            {
                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Childern)
                    .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

                if (delivery == null)
                    return NotFound($"Delivery not found");


                await UpdateDeliveryState(delivery, model.PreviousState, model.CurrentState, model);

                if (delivery.Childern.Count == 0)
                {
                    await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id, model.CurrentState, model.TimeStamp.DateTime);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message);
            }
        }

        #endregion


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SendEmail(int id)
        {
            var consignment = await context.Consignments.Where(x => x.Id == id).FirstOrDefaultAsync();
            var report = await GetConsignmentReport2(consignment);
            if (report != null)
            {
                var receipt = GetReceipt(report);

                var emailBody = HTMLEmailFormats.InTransitEmailFormat(report, "Delivered");
#if DEBUG
                await emailManager.SendEmail("waqastaahir@gmail.com", emailBody, $"Test, Pickup {report.PickupBranchSupervisorEmail}, Deliver {report.DeliveryBranchSupervisorEmail} ", receipt);
#else
				await emailManager.SendEmail("yousaf.rao@sospakistan.net", emailBody, $"Test, Pickup {report.PickupBranchSupervisorEmail}, Deliver {report.DeliveryBranchSupervisorEmail} ", receipt);
#endif
                return Ok();
            }
            else
            {
                return BadRequest("no report");
            }
        }

        #region Scan QR Code
        /// <summary>
        /// V8
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostQRCodeScanPickup([FromBody] QRCodeModel model)
        {
            try
            {
                LogSignature(model);

                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Parent.Consignment)
                    .FirstOrDefaultAsync(x => x.PickupCode == model.QrCode && x.Id == model.DeliveryId);

                if (delivery == null)
                    throw new Exception($"Invalid QRCode {model.QrCode} Or Invalid Delivery Id");

                if (delivery.DeliveryState < ConsignmentDeliveryState.InTransit)
                {
                    delivery.DeliveryState = ConsignmentDeliveryState.InTransit;
                    delivery.CollectionPoint = model.GetPoint();
                    delivery.CollectionMode = 1;
                    delivery.ActualPickupTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();
                }

                if (delivery.Parent != null)
                {
                    delivery.Parent.DeliveryState = ConsignmentDeliveryState.Delivered;
                    delivery.ActualDropTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();
                    await notificationService.CreateFirebaseNotification(delivery.Parent.Id,
                        delivery.Parent.ConsignmentId, delivery.Parent.CrewId, delivery.Parent.Consignment.ShipmentCode,
                        NotificationType.Delivered, NotificationCategory.CIT);
                }
                else
                {
                    delivery.Consignment.ActualCollectionTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();

                }

                await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id, ConsignmentDeliveryState.InTransit, model.TimeStamp.DateTime);
                if (delivery.DeliveryState == ConsignmentDeliveryState.InTransit)
                {
                    var report = await GetConsignmentReport(delivery);

                    if (report != null)
                    {
                        var receipt = GetReceipt(report);

                        var emailBody = HTMLEmailFormats.InTransitEmailFormat(report, "Collected");

                        if (!string.IsNullOrEmpty(report.PickupBranchSupervisorEmail))
                            await emailManager.SendEmail(report.PickupBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Picked Up", receipt);

                        if (!string.IsNullOrEmpty(report.DeliveryBranchSupervisorEmail))
                            await emailManager.SendEmail(report.DeliveryBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Picked Up", receipt);
                    }
                }
            }

            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        /// <summary>
        /// V6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostPinCodePickup([FromBody] PinCodeModel model)
        {
            try
            {
                LogSignature(model);

                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Parent.Consignment)
                    .FirstOrDefaultAsync(x => x.FromPartyId == model.PinCode && x.Id == model.DeliveryId);

                if (delivery == null)
                    return NotFound($"Invalid PinCode {model.PinCode} Or Invalid Delivery Id");



                await UpdateDeliveryState(delivery, ConsignmentDeliveryState.ReachedPickup, ConsignmentDeliveryState.InTransit, model);

                delivery.Parent.DeliveryState = ConsignmentDeliveryState.Delivered;

                if (delivery.Parent != null)

                    await notificationService.CreateFirebaseNotification(delivery.Parent.Id,
                        delivery.Parent.ConsignmentId, delivery.Parent.CrewId, delivery.Parent.Consignment.ShipmentCode,
                        NotificationType.Delivered, NotificationCategory.CIT);


                delivery.DeliveryState = ConsignmentDeliveryState.InTransit;
                await context.SaveChangesAsync();
                await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id, ConsignmentDeliveryState.InTransit, model.TimeStamp.DateTime);
                return Ok();

            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// V7
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostQRCodeScanDropoff([FromBody] QRCodeModel model)
        {
            try
            {
                LogSignature(model);
                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                       .Include(x => x.Childern)
                    .FirstOrDefaultAsync(x => x.DropoffCode == model.QrCode && x.Id == model.DeliveryId);

                if (delivery == null)
                    throw new Exception($"Invalid QRCode {model.QrCode}");

                if (delivery.DeliveryState < ConsignmentDeliveryState.Delivered)
                {
                    delivery.DeliveryState = ConsignmentDeliveryState.Delivered;
                    delivery.DeliveryPoint = model.GetPoint();
                    delivery.DeliveryMode = 1;
                    delivery.ActualDropTime = model.TimeStamp.DateTime.AddHours(5);

                    await context.SaveChangesAsync();
                }

                if (delivery.Childern.Count > 0)
                {
                    var child = delivery.Childern.First();
                    child.DeliveryState = ConsignmentDeliveryState.InTransit;
                    child.ActualPickupTime = model.TimeStamp.DateTime.AddHours(5);
                    child.DeliveryPoint = model.GetPoint();
                    child.DeliveryMode = 1;

                    await context.SaveChangesAsync();

                    await notificationService.CreateFirebaseNotification(child.Id,
                    child.ConsignmentId, child.CrewId, delivery.Consignment.ShipmentCode, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                }
                else
                {
                    delivery.Consignment.ActualDeliveryTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();

                }

                await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id,
                    delivery.Childern.Count == 0 ? ConsignmentDeliveryState.Delivered : ConsignmentDeliveryState.InTransit, model.TimeStamp.DateTime);
                if (delivery.DeliveryState == ConsignmentDeliveryState.Delivered)
                {
                    var report = await GetConsignmentReport(delivery);
                    if (report != null)
                    {
                        var receipt = GetReceipt(report);

                        var emailBody = HTMLEmailFormats.InTransitEmailFormat(report, "Delivered");

                        if (!string.IsNullOrEmpty(report.PickupBranchSupervisorEmail))
                            await emailManager.SendEmail(report.PickupBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Delivered", receipt);

                        if (!string.IsNullOrEmpty(report.DeliveryBranchSupervisorEmail))
                            await emailManager.SendEmail(report.DeliveryBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Delivered", receipt);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// V6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostPinCodeDropoff([FromBody] PinCodeModel model)
        {
            LogSignature(model);
            try
            {
                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Childern)
                    .FirstOrDefaultAsync(x => x.ToPartyId == model.PinCode && x.Id == model.DeliveryId);

                if (delivery == null)
                    return NotFound($"Invalid PinCode {model.PinCode}");

                if (delivery.DeliveryState == ConsignmentDeliveryState.Delivered)
                    return Ok();

                await UpdateDeliveryState(delivery, ConsignmentDeliveryState.ReachedDestination, ConsignmentDeliveryState.Delivered, model);

                delivery.DeliveryPoint = model.GetPoint();
                delivery.DeliveryMode = 2;
                await context.SaveChangesAsync();

                await UpdateConsignmentAndStatusAsync(delivery.Consignment,
                    delivery.Id,
                    delivery.Childern.Count == 0 ? ConsignmentDeliveryState.Delivered : ConsignmentDeliveryState.InTransit, model.TimeStamp.DateTime);
                return Ok();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Skip QR Code

        /// <summary>
        /// V8
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SkipQRCodeScanPickup([FromBody] CitConsignmentBaseModel model)
        {
            LogSignature(model);

            try
            {
                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Parent.Consignment)
                    .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

                if (delivery == null)
                    return NotFound($"Invalid Delivery Id");

                if (delivery.DeliveryState < ConsignmentDeliveryState.InTransit)
                {
                    delivery.DeliveryState = ConsignmentDeliveryState.InTransit;
                    delivery.CollectionMode = 0;
                    delivery.CollectionPoint = model.GetPoint();
                    delivery.ActualPickupTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();
                }

                if (delivery.Parent != null)
                {
                    delivery.Parent.DeliveryState = ConsignmentDeliveryState.Delivered;
                    delivery.Parent.ActualDropTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();

                    await notificationService.CreateFirebaseNotification(delivery.Parent.Id,
                    delivery.Parent.ConsignmentId, delivery.Parent.CrewId, delivery.Parent.Consignment.ShipmentCode,
                    NotificationType.Delivered, NotificationCategory.CIT);
                }
                else
                {
                    delivery.Consignment.ActualCollectionTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();
                }

                await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id, ConsignmentDeliveryState.InTransit, model.TimeStamp.DateTime);

                if (delivery.DeliveryState == ConsignmentDeliveryState.InTransit)
                {
                    var report = await GetConsignmentReport(delivery);
                    if (report != null)
                    {
                        var receipt = GetReceipt(report);

                        var emailBody = HTMLEmailFormats.InTransitEmailFormat(report, "Collected");

                        if (!string.IsNullOrEmpty(report.PickupBranchSupervisorEmail))
                            await emailManager.SendEmail(report.PickupBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Picked Up", receipt);

                        if (!string.IsNullOrEmpty(report.DeliveryBranchSupervisorEmail))
                            await emailManager.SendEmail(report.DeliveryBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Picked Up", receipt);
                    }
                }
                return Ok();

            }
            catch (Exception ex)
            {
                LogException(ex);
                return BadRequest(ex.Message + (ex.InnerException == null ? "" : " Inner Exception= '" + ex.InnerException.ToString() + "'"));
            }
        }

        /// <summary>
        /// V8
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SkipQRCodeScanDropoff([FromBody] CitConsignmentBaseModel model)
        {

            LogSignature(model);

            try
            {
                var delivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .Include(x => x.Childern)
                    .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

                if (delivery == null)
                    return NotFound($"Delivery not found");


                if (delivery.DeliveryState < ConsignmentDeliveryState.Delivered)
                {
                    delivery.ActualDropTime = model.TimeStamp.DateTime.AddHours(5);
                    delivery.DeliveryState = ConsignmentDeliveryState.Delivered;
                }

                delivery.DeliveryPoint = model.GetPoint();
                delivery.DeliveryMode = 0;
                await context.SaveChangesAsync();

                if (delivery.Childern.Count == 0 && delivery.Consignment.ConsignmentStateType < ConsignmentDeliveryState.Delivered)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments set ConsignmentStateType = 64 WHERE Id = {delivery.ConsignmentId};");
                }

                if (delivery.Childern.Count > 0)
                {
                    var child = delivery.Childern.First();
                    child.DeliveryState = ConsignmentDeliveryState.InTransit;
                    child.ActualPickupTime = model.TimeStamp.DateTime.AddHours(5);
                    child.DeliveryPoint = model.GetPoint();
                    child.DeliveryMode = 0;

                    await context.SaveChangesAsync();

                    await notificationService.CreateFirebaseNotification(child.Id,
                    child.ConsignmentId, child.CrewId, delivery.Consignment.ShipmentCode, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                }
                else
                {
                    delivery.Consignment.ActualDeliveryTime = model.TimeStamp.DateTime.AddHours(5);
                    await context.SaveChangesAsync();
                }

                await UpdateConsignmentAndStatusAsync(delivery.Consignment, delivery.Id,
               delivery.Childern.Count == 0 ? ConsignmentDeliveryState.Delivered : ConsignmentDeliveryState.InTransit, model.TimeStamp.DateTime);

                if (delivery.DeliveryState == ConsignmentDeliveryState.Delivered)
                {
                    var report = await GetConsignmentReport(delivery);

                    if (report != null)
                    {
                        var receipt = GetReceipt(report);

                        var emailBody = HTMLEmailFormats.InTransitEmailFormat(report, "Delivered");

                        if (!string.IsNullOrEmpty(report.PickupBranchSupervisorEmail))
                            await emailManager.SendEmail(report.PickupBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Delivered", receipt);

                        if (!string.IsNullOrEmpty(report.DeliveryBranchSupervisorEmail))
                            await emailManager.SendEmail(report.DeliveryBranchSupervisorEmail, emailBody, $"Consignment: {report.ShipmentRecieptNo} Delivered", receipt);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<ConsignmentReportViewModel> GetConsignmentReport(ConsignmentDelivery delivery)
        {

            var pickBranchSupervisor = (from u in context.Users
                                        join ur in context.UserRoles on u.Id equals ur.UserId
                                        join r in context.Roles on ur.RoleId equals r.Id
                                        where u.PhoneNumberConfirmed && u.EmailConfirmed && u.PartyId == delivery.Consignment.FromPartyId
                                        && (r.Id == Constants.Roles.BANK_BRANCH_MANAGER || r.Id == Constants.Roles.BANK_HYBRID || r.Id == Constants.Roles.BANK_CPC_MANAGER)
                                        orderby u.CreatedAt descending
                                        select u.Email).FirstOrDefault();

            var destinationBranchSupervisor = (from u in context.Users
                                               join ur in context.UserRoles on u.Id equals ur.UserId
                                               join r in context.Roles on ur.RoleId equals r.Id
                                               where u.PhoneNumberConfirmed && u.EmailConfirmed && u.PartyId == delivery.Consignment.ToPartyId
                                               && (r.Id == Constants.Roles.BANK_BRANCH_MANAGER || r.Id == Constants.Roles.BANK_HYBRID || r.Id == Constants.Roles.BANK_CPC_MANAGER)
                                               orderby u.CreatedAt descending
                                               select u.Email).FirstOrDefault();

            await Task.Delay(10000);

            var model = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join b in context.Parties on c.BillBranchId equals b.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         from n in context.Denominations.Where(x => x.ConsignmentId == c.Id).DefaultIfEmpty()
                         from cr in context.Parties.Where(x => x.Id == d.CrewId).DefaultIfEmpty()
                         where c.Id == delivery.ConsignmentId
                         select new ConsignmentReportViewModel()
                         {
                             AC_Code = c.Id.ToString(),
                             Amount = c.CurrencySymbol == CurrencySymbol.PrizeBond ? c.AmountPKR.ToString("N0") : c.Amount.ToString("N0"),
                             AmountInWords = c.CurrencySymbol.ToString() + " " + ((c.CurrencySymbol == CurrencySymbol.MixCurrency || c.CurrencySymbol == CurrencySymbol.PrizeBond || c.CurrencySymbol == CurrencySymbol.Other) ? c.AmountPKR.ToString() : CurrencyHelper.AmountInWords(c.Amount)),
                             AmountInWordsPKR = ((c.CurrencySymbol == CurrencySymbol.MixCurrency || c.CurrencySymbol == CurrencySymbol.PrizeBond || c.CurrencySymbol == CurrencySymbol.Other) ? "" : "PKR ") + CurrencyHelper.AmountInWords(c.AmountPKR),
                             AmountType = n.DenominationType.ToString(),
                             Date = c.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                             PickupBranch = f.ShortName + " - " + f.FormalName,
                             DeliveryBranch = t.ShortName + " - " + t.FormalName,
                             FirstCopyName = f.ShortName,
                             ThirdCopyName = t.ShortName,
                             CurrencySymbol = c.CurrencySymbol.ToString(),
                             CurrencySymbol_ = c.CurrencySymbol,
                             ConsignedByName1 = context.Users.Where(x => x.UserName == delivery.Consignment.CreatedBy).Select(x => x.Name).FirstOrDefault(),
                             ConsignedByName2 = context.Users.Where(x => x.UserName == delivery.Consignment.ApprovedBy).Select(x => x.Name).FirstOrDefault(),
                             AcceptedByName1 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId
                                                && r.FromPartyRole == RoleType.CheifCrewAgent && r.IsActive
                                                select p).FirstOrDefault().FormalName,
                             AcceptedByName2 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId && r.IsActive
                                                && r.FromPartyRole == RoleType.AssistantCrewAgent
                                                select p).FirstOrDefault().FormalName,
                             PickupTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                             DeliveryTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"),
                             SealNos = context.ShipmentSealCodes.Where(x => x.ConsignmentId == c.Id).Select(x => x.SealCode).ToList(),
                             VehicleNo = context.AssetAllocations.FirstOrDefault(x => x.PartyId == d.CrewId).Asset.Description,
                             CustomerToBeBilledName = b.ShortName + " - " + b.FormalName,
                             CustomerToBeBilled = b.ShortName + " - " + b.FormalName,
                             ShipmentRecieptNo = c.ShipmentCode,
                             Currency5000x = n.Currency5000x,
                             Currency1000x = n.Currency1000x,
                             Currency500x = n.Currency500x,
                             Currency100x = n.Currency100x,
                             Currency75x = n.Currency75x,
                             Currency50x = n.Currency50x,
                             Currency20x = n.Currency20x,
                             Currency10x = n.Currency10x,
                             Currency5x = n.Currency5x,
                             Currency2x = n.Currency2x,
                             Currency1x = n.Currency1x,

                             Currency40000x = n.Currency40000x,
                             Currency15000x = n.Currency15000x,
                             Currency25000x = n.Currency25000x,
                             Currency7500x = n.Currency7500x,
                             Currency1500x = n.Currency1500x,
                             Currency750x = n.Currency750x,
                             Currency200x = n.Currency200x,

                             PrizeMoney40000x = n.PrizeMoney40000x,
                             PrizeMoney15000x = n.PrizeMoney15000x,
                             PrizeMoney25000x = n.PrizeMoney25000x,
                             PrizeMoney7500x = n.PrizeMoney7500x,
                             PrizeMoney1500x = n.PrizeMoney1500x,
                             PrizeMoney750x = n.PrizeMoney750x,
                             PrizeMoney200x = n.PrizeMoney200x,
                             PrizeMoney100x = n.PrizeMoney100x,

                             Currency5000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5000x, 5000, n.DenominationType),
                             Currency1000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1000x, 1000, n.DenominationType),
                             Currency500xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency500x, 500, n.DenominationType),
                             Currency100xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency100x, 100, n.DenominationType),
                             Currency75xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency75x, 75, n.DenominationType),
                             Currency50xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency50x, 50, n.DenominationType),
                             Currency20xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency20x, 20, n.DenominationType),
                             Currency10xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency10x, 10, n.DenominationType),
                             Currency5xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5x, 5, n.DenominationType),
                             Currency2xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency2x, 2, n.DenominationType),
                             Currency1xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1x, 1, n.DenominationType),

                             Currency40000xAmount = CurrencyHelper.PrizeBondFormula(40000, n.Currency40000x, n.PrizeMoney40000x, n.DenominationType),
                             Currency25000xAmount = CurrencyHelper.PrizeBondFormula(25000, n.Currency25000x, n.PrizeMoney25000x, n.DenominationType),
                             Currency15000xAmount = CurrencyHelper.PrizeBondFormula(15000, n.Currency15000x, n.PrizeMoney15000x, n.DenominationType),
                             Currency7500xAmount = CurrencyHelper.PrizeBondFormula(7500, n.Currency7500x, n.PrizeMoney7500x, n.DenominationType),
                             Currency1500xAmount = CurrencyHelper.PrizeBondFormula(1500, n.Currency1500x, n.PrizeMoney1500x, n.DenominationType),
                             Currency750xAmount = CurrencyHelper.PrizeBondFormula(750, n.Currency750x, n.PrizeMoney750x, n.DenominationType),
                             Currency200xAmount = CurrencyHelper.PrizeBondFormula(200, n.Currency200x, n.PrizeMoney200x, n.DenominationType),

                             Comments = c.Comments,
                             Valueables = c.Valueables,
                             NoOfBags = c.NoOfBags,
                             SealedBags = c.SealedBags,
                             PickupBranchSupervisorEmail = pickBranchSupervisor,
                             DeliveryBranchSupervisorEmail = destinationBranchSupervisor,
                             ExchangeRate = c.ExchangeRate,
                         }).FirstOrDefault();

            model.NoOfSeals = model.SealNos.Count;
            model.SealNo = string.Join(", ", model.SealNos);
            switch (model.CurrencySymbol_)
            {
                case CurrencySymbol.USD:
                case CurrencySymbol.EURO:
                    model.AmountInWords += $"\n{model.AmountInWordsPKR} \n\rExchange Rate:{model.ExchangeRate} PKR/{model.CurrencySymbol}";
                    break;

                case CurrencySymbol.MixCurrency:
                case CurrencySymbol.Other:
                    model.AmountInWords += $"\n{model.AmountInWordsPKR}";
                    break;
                case CurrencySymbol.PrizeBond:
                    model.AmountInWords = model.AmountInWordsPKR;
                    break;
            }

            return model;
        }
        private async Task<ConsignmentReportViewModel> GetConsignmentReport2(Consignment Consignment)
        {


            var pickBranchSupervisor = (from u in context.Users
                                        join ur in context.UserRoles on u.Id equals ur.UserId
                                        join r in context.Roles on ur.RoleId equals r.Id
                                        where u.PhoneNumberConfirmed && u.EmailConfirmed && u.PartyId == Consignment.FromPartyId
                                        && (r.Id == Constants.Roles.BANK_BRANCH_MANAGER || r.Id == Constants.Roles.BANK_HYBRID || r.Id == Constants.Roles.BANK_CPC_MANAGER)
                                        orderby u.CreatedAt descending
                                        select u.Email).FirstOrDefault();

            var destinationBranchSupervisor = (from u in context.Users
                                               join ur in context.UserRoles on u.Id equals ur.UserId
                                               join r in context.Roles on ur.RoleId equals r.Id
                                               where u.PhoneNumberConfirmed && u.EmailConfirmed && u.PartyId == Consignment.ToPartyId
                                               && (r.Id == Constants.Roles.BANK_BRANCH_MANAGER || r.Id == Constants.Roles.BANK_HYBRID || r.Id == Constants.Roles.BANK_CPC_MANAGER)
                                               orderby u.CreatedAt descending
                                               select u.Email).FirstOrDefault();

            await Task.Delay(10000);


            var model = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join b in context.Parties on c.BillBranchId equals b.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         from n in context.Denominations.Where(x => x.ConsignmentId == c.Id).DefaultIfEmpty()
                         from cr in context.Parties.Where(x => x.Id == d.CrewId).DefaultIfEmpty()
                         where c.Id == Consignment.Id
                         select new ConsignmentReportViewModel()
                         {
                             AC_Code = c.Id.ToString(),
                             Amount = c.CurrencySymbol == CurrencySymbol.PrizeBond ? c.AmountPKR.ToString("N0") : c.Amount.ToString("N0"),
                             AmountInWords = c.CurrencySymbol.ToString() + " " + ((c.CurrencySymbol == CurrencySymbol.MixCurrency || c.CurrencySymbol == CurrencySymbol.PrizeBond || c.CurrencySymbol == CurrencySymbol.Other) ? c.AmountPKR.ToString() : CurrencyHelper.AmountInWords(c.Amount)),
                             AmountInWordsPKR = ((c.CurrencySymbol == CurrencySymbol.MixCurrency || c.CurrencySymbol == CurrencySymbol.PrizeBond || c.CurrencySymbol == CurrencySymbol.Other) ? "" : "PKR ") + CurrencyHelper.AmountInWords(c.AmountPKR),
                             AmountType = n.DenominationType.ToString(),
                             Date = c.CreatedAt.ToString("dd/MM/yyyy hh:mm"),
                             PickupBranch = f.ShortName + " - " + f.FormalName,
                             DeliveryBranch = t.ShortName + " - " + t.FormalName,
                             FirstCopyName = f.ShortName,
                             ThirdCopyName = t.ShortName,
                             CurrencySymbol = c.CurrencySymbol.ToString(),
                             CurrencySymbol_ = c.CurrencySymbol,
                             ConsignedByName1 = context.Users.Where(x => x.UserName == Consignment.CreatedBy).Select(x => x.Name).FirstOrDefault(),
                             ConsignedByName2 = context.Users.Where(x => x.UserName == Consignment.ApprovedBy).Select(x => x.Name).FirstOrDefault(),
                             AcceptedByName1 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId
                                                && r.FromPartyRole == RoleType.CheifCrewAgent && r.IsActive
                                                select p).FirstOrDefault().FormalName,
                             AcceptedByName2 = (from r in context.PartyRelationships
                                                join p in context.Parties on r.FromPartyId equals p.Id
                                                where r.ToPartyId == d.CrewId && r.IsActive
                                                && r.FromPartyRole == RoleType.AssistantCrewAgent
                                                select p).FirstOrDefault().FormalName,
                             PickupTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.InTransit).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm"),
                             DeliveryTime = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm") == "01/01/0001 00:00"
                             ? "" : context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == c.Id && x.ConsignmentStateType == ConsignmentDeliveryState.Delivered).TimeStamp.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm"),
                             SealNos = context.ShipmentSealCodes.Where(x => x.ConsignmentId == c.Id).Select(x => x.SealCode).ToList(),
                             VehicleNo = context.AssetAllocations.FirstOrDefault(x => x.PartyId == d.CrewId).Asset.Description,
                             CustomerToBeBilledName = b.ShortName + " - " + b.FormalName,
                             CustomerToBeBilled = b.ShortName + " - " + b.FormalName,
                             ShipmentRecieptNo = c.ShipmentCode,
                             Currency5000x = n.Currency5000x,
                             Currency1000x = n.Currency1000x,
                             Currency500x = n.Currency500x,
                             Currency100x = n.Currency100x,
                             Currency75x = n.Currency75x,
                             Currency50x = n.Currency50x,
                             Currency20x = n.Currency20x,
                             Currency10x = n.Currency10x,
                             Currency5x = n.Currency5x,
                             Currency2x = n.Currency2x,
                             Currency1x = n.Currency1x,

                             Currency40000x = n.Currency40000x,
                             Currency15000x = n.Currency15000x,
                             Currency25000x = n.Currency25000x,
                             Currency7500x = n.Currency7500x,
                             Currency1500x = n.Currency1500x,
                             Currency750x = n.Currency750x,
                             Currency200x = n.Currency200x,

                             PrizeMoney40000x = n.PrizeMoney40000x,
                             PrizeMoney15000x = n.PrizeMoney15000x,
                             PrizeMoney25000x = n.PrizeMoney25000x,
                             PrizeMoney7500x = n.PrizeMoney7500x,
                             PrizeMoney1500x = n.PrizeMoney1500x,
                             PrizeMoney750x = n.PrizeMoney750x,
                             PrizeMoney200x = n.PrizeMoney200x,
                             PrizeMoney100x = n.PrizeMoney100x,

                             Currency5000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5000x, 5000, n.DenominationType),
                             Currency1000xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1000x, 1000, n.DenominationType),
                             Currency500xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency500x, 500, n.DenominationType),
                             Currency100xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency100x, 100, n.DenominationType),
                             Currency75xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency75x, 75, n.DenominationType),
                             Currency50xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency50x, 50, n.DenominationType),
                             Currency20xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency20x, 20, n.DenominationType),
                             Currency10xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency10x, 10, n.DenominationType),
                             Currency5xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency5x, 5, n.DenominationType),
                             Currency2xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency2x, 2, n.DenominationType),
                             Currency1xAmount = CurrencyHelper.CalculateAmountOneByOne(n.Currency1x, 1, n.DenominationType),

                             Currency40000xAmount = CurrencyHelper.PrizeBondFormula(40000, n.Currency40000x, n.PrizeMoney40000x, n.DenominationType),
                             Currency25000xAmount = CurrencyHelper.PrizeBondFormula(25000, n.Currency25000x, n.PrizeMoney25000x, n.DenominationType),
                             Currency15000xAmount = CurrencyHelper.PrizeBondFormula(15000, n.Currency15000x, n.PrizeMoney15000x, n.DenominationType),
                             Currency7500xAmount = CurrencyHelper.PrizeBondFormula(7500, n.Currency7500x, n.PrizeMoney7500x, n.DenominationType),
                             Currency1500xAmount = CurrencyHelper.PrizeBondFormula(1500, n.Currency1500x, n.PrizeMoney1500x, n.DenominationType),
                             Currency750xAmount = CurrencyHelper.PrizeBondFormula(750, n.Currency750x, n.PrizeMoney750x, n.DenominationType),
                             Currency200xAmount = CurrencyHelper.PrizeBondFormula(200, n.Currency200x, n.PrizeMoney200x, n.DenominationType),

                             Comments = c.Comments,
                             Valueables = c.Valueables,
                             NoOfBags = c.NoOfBags,
                             SealedBags = c.SealedBags,
                             PickupBranchSupervisorEmail = pickBranchSupervisor,
                             DeliveryBranchSupervisorEmail = destinationBranchSupervisor,
                             ExchangeRate = c.ExchangeRate,
                         }).FirstOrDefault();

            model.NoOfSeals = model.SealNos.Count;
            model.SealNo = string.Join(", ", model.SealNos);
            switch (model.CurrencySymbol_)
            {
                case CurrencySymbol.USD:
                case CurrencySymbol.EURO:
                    model.AmountInWords += $"\n{model.AmountInWordsPKR} \n\rExchange Rate:{model.ExchangeRate} PKR/{model.CurrencySymbol}";
                    break;

                case CurrencySymbol.MixCurrency:
                case CurrencySymbol.Other:
                    model.AmountInWords += $"\n{model.AmountInWordsPKR}";
                    break;
                case CurrencySymbol.PrizeBond:
                    model.AmountInWords = model.AmountInWordsPKR;
                    break;
            }

            return model;
        }

        private MemoryStream GetReceipt(ConsignmentReportViewModel model)
        {

            #region Actual Data

            var comments = model.Comments != null ? JsonConvert.DeserializeObject<List<ShipmentComment>>(model.Comments).ToList() : new List<ShipmentComment>();
            StringBuilder concatedComment = new();
            foreach (var comment in comments)
            {
                concatedComment.Append(comment.Description + ",");
            }
            model.Comments = concatedComment.ToString();
            var reportName = @"\Reports\ConsignmentReceipt.rdlc";
            switch (model.CurrencySymbol_)
            {
                case CurrencySymbol.MixCurrency:
                case CurrencySymbol.Other:
                    reportName = @"\Reports\ConsignmentReceiptMulti.rdlc";
                    break;
                case CurrencySymbol.PrizeBond:
                    reportName = @"\Reports\ConsignmentReceiptPrizebond.rdlc";
                    break;
            }

            string basePath = env.ContentRootPath;
            // Here, we have loaded the Product List.rdlc sample report from application the Resources folder.
            FileStream reportStream = new FileStream(basePath + reportName, FileMode.Open, FileAccess.Read);
            BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter(reportStream);
            writer.ReportProcessingMode = (BoldReports.Writer.ProcessingMode)BoldReports.ReportViewerEnums.ProcessingMode.Local;


            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var sealNo in model.SealNos)
            {
                index++;
                if (index == model.SealNos.Count())
                    sb.Append(sealNo);
                else
                    sb.Append(sealNo + ",");
            }
            model.SealNo = sb.ToString();
            model.NoOfSeals = model.SealNos.Count();
            #endregion

            List<ConsignmentReportViewModel> datasource = new List<ConsignmentReportViewModel>();
            datasource.Add(model);

            // Pass the dataset collection for report
            writer.DataSources.Clear();
            writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "ConsignmentDataSet", Value = datasource });

            string fileName = null;
            WriterFormat format;
            string type = null;

            fileName = $"CR-{model.ShipmentRecieptNo}.pdf";
            type = "pdf";
            format = WriterFormat.PDF;

            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);
            memoryStream.Position = 0;
            return memoryStream;

        }

        #endregion


        #region Vault 


        [HttpPost]
        public async Task<IActionResult> SelfVaultIn([FromBody] CitConsignmentBaseModel model)
        {
            logger.LogInformation($"SelfVaultIn -> {model.ConsignmentId}, {model.DeliveryId}");

            var delivery = await context.ConsignmentDeliveries.Include(x => x.Consignment)
                .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

            if (delivery == null)
                return NotFound($"Delivery not found");

            var id = await consignmentService.VaultIn(delivery.Id, model.TimeStamp.DateTime);

            return Ok(id);
        }

        [HttpPost]
        public async Task<IActionResult> SelfVaultOut([FromBody] CitConsignmentBaseModel model)
        {
            logger.LogInformation($"SelfVaultOut -> {model.ConsignmentId}, {model.DeliveryId}");

            var delivery = await context.ConsignmentDeliveries.Include(x => x.Consignment)
                .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

            if (delivery == null)
                return NotFound($"Delivery not found");
            var crewId = await userService.GetUserCrewId(User.Identity.Name, MyDateTime.Today, RoleType.Crew);

            if (crewId.Count == 0)
                return BadRequest("You must be part of a Crew to Vault out this consignment OR You can handover this consignment to some other designated Crew.");

            var id = await consignmentService.VaultOut(delivery.Id, crewId.First(), model.TimeStamp.DateTime);

            return Ok(id);
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> CancelShipment([FromBody] CitConsignmentBaseModel model)
        {
            LogSignature(model);

            var delivery = await context.ConsignmentDeliveries
                .Include(x => x.Consignment)
                .Include(x => x.Parent)
                .FirstOrDefaultAsync(x => x.Id == model.DeliveryId);

            if (delivery == null)
                return NotFound($"Delivery not found");

            delivery.Consignment.ConsignmentStatus = ConsignmentStatus.Cancelled;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PostBags([FromBody] BagsModel model)
        {
            LogSignature(model);

            //if (model.NoOfBags <= 0)
            //    return BadRequest("Bags should be more than zero");

            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == model.ConsignmentId);
            if (consignment == null)
                return NotFound();

            consignment.NoOfBags = model.NoOfBags;
            consignment.SealedBags = model.IsSealed;
            await context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> PostSealCodes([FromBody] System.Text.Json.JsonElement entity)
        {
            try
            {
                logger.LogInformation($"PostSealCodes {User.Identity.Name} -> {entity.GetRawText()}");
                var model = JsonConvert.DeserializeObject<SealCodesModel>(entity.GetRawText());

                if (model.SealCodes == null || model.SealCodes.Length == 0)
                    throw new Exception("Sealcode list is empty");

                if (!context.Consignments.Any(x => x.Id == model.ConsignmentId))
                    throw new Exception($"Shipment not fount{model.ConsignmentId}");

                var rejected = new List<string>();

                foreach (var item in model.SealCodes)
                {
                    //db required to enter seal code, Once seal number used its must not used
                    var sealCodeExists = await context.ShipmentSealCodes.AnyAsync(x => x.SealCode == item && x.ConsignmentId == model.ConsignmentId);// && x.ConsignmentId == model.ConsignmentId);
                    if (sealCodeExists)
                    {
                        rejected.Add(item);
                        logger.LogInformation($"Rejecting sealcode {model.ConsignmentId} -> {item}");
                    }
                    else
                    {
                        var sealCode = new ShipmentSealCode()
                        {
                            ConsignmentId = model.ConsignmentId,
                            SealCode = item,
                            CreatedAt = model.TimeStamp.DateTime,
                            CreatedBy = User.Identity.Name
                        };
                        context.ShipmentSealCodes.Add(sealCode);
                        logger.LogInformation($"Adding sealcode {model.ConsignmentId} -> {item}");
                    }
                }
                var resultCount = await context.SaveChangesAsync();
                logger.LogInformation($"Sealcodes Added {model.ConsignmentId} -> {resultCount}");

                var cachedShipment = await shipmentsCache.GetShipment(model.ConsignmentId);
                if (cachedShipment != null)
                {
                    cachedShipment.SealCodes = context.ShipmentSealCodes.Where(x => x.ConsignmentId == cachedShipment.Id).Select(x => x.SealCode).ToList();
                    await shipmentsCache.SetShipment(cachedShipment.Id, cachedShipment);
                }
                foreach (var d in await context.ConsignmentDeliveries.Include(x => x.Consignment).Where(x => x.ConsignmentId == model.ConsignmentId).ToArrayAsync())
                {
                    if (d.Id != model.DeliveryId)
                    {
                        await notificationService.CreateFirebaseNotification(d.Id, d.ConsignmentId, d.CrewId, d.Consignment.ShipmentCode, NotificationType.UpdatedSealCode, NotificationCategory.CIT);
                    }
                }

                return Ok(new SealCodesModel()
                {
                    ConsignmentId = model.ConsignmentId,
                    DeliveryId = model.DeliveryId,
                    SealCodes = rejected.ToArray()
                });
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return Ok(new SealCodesModel()
            {
                SealCodes = Array.Empty<string>()
            });
        }


        private void LogException(Exception ex, [CallerMemberName] string membername = "")
        {
            logger.LogWarning($"{membername} => " + ex.InnerException?.InnerException?.Message + ex.InnerException?.Message + ex.Message);
            logger.LogDebug($"{membername} => " + ex.InnerException?.InnerException?.ToString() + ex.InnerException?.ToString() + ex.ToString());
        }

        //Updates status of delivery state to confirmted from  
        [HttpPost]
        public async Task<IActionResult> PostShipmentCode([FromBody] ShipmentCodeModel model)
        {

            try
            {
                LogSignature(model);

                var consignment = await context.Consignments
                    .FirstOrDefaultAsync(x => x.Id == model.ConsignmentId);

                if (consignment == null)
                    throw new Exception($"Invalid consignment Id");

                consignment.ManualShipmentCode = model.ManualShipmentCode;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return Ok();
        }

        private async Task UpdateConsignmentAndStatusAsync(Consignment consignment,
            int deliveryId,
            Shared.Enums.ConsignmentDeliveryState consignmentState,
            DateTime timeStamp)
        {

            logger.LogDebug($"Updating status -> {consignment.Id}, {consignmentState}, {timeStamp.AddHours(5)}");

            if (consignment.ConsignmentStateType < consignmentState)
            {
                consignment.ConsignmentStateType = consignmentState;
                await context.SaveChangesAsync();
            }

            var status = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == consignment.Id
                    && x.DeliveryId == deliveryId
                    && x.ConsignmentStateType == consignmentState);

            if (status == null)
            {
                status = new ConsignmentState()
                {
                    ConsignmentId = consignment.Id,
                    DeliveryId = deliveryId,
                    CreatedBy = User.Identity.Name,
                    ConsignmentStateType = consignmentState,
                };
                context.ConsignmentStates.Add(status);
            }

            if (status.Status == StateTypes.Waiting)
            {
                status.TimeStamp = timeStamp.AddHours(5);
                status.Status = StateTypes.Confirmed;
                status.CreatedBy = User.Identity.Name;

            }
            await context.SaveChangesAsync();

            //var shipmentViewModel = await shipmentsCache.GetShipmentOrDefault(consignment.Id);
            //if (shipmentViewModel != null)
            //{
            //    shipmentViewModel.ConsignmentStateType = consignmentState;
            //    var deliveryState = shipmentViewModel.DeliveryStates.FirstOrDefault(x => x.ConsignmentStateType.Equals(consignmentState));
            //    if(deliveryState == null)
            //    {
            //        deliveryState = new Shared.ViewModels.ConsignmentStateViewModel()
            //        {
            //            ConsignmentId = status.ConsignmentId,
            //            DeliveryId = status.DeliveryId
            //        };
            //        shipmentViewModel.DeliveryStates.Add(deliveryState);
            //    }
            //    deliveryState.TimeStamp = status.TimeStamp;
            //    deliveryState.Status = status.Status;
            //    deliveryState.Tag = status.Tag;

            //    await shipmentsCache.SetShipment(shipmentViewModel.Id, shipmentViewModel);
            //}

            await PublishUpdate(consignment.Id);
        }

        private void LogSignature<T>(T model, [CallerMemberName] string membername = "")
        {
            logger.LogInformation($"{membername} -> {User.Identity.Name} model-> {JsonConvert.SerializeObject(model)}");
        }

        private void LogSignature(System.Text.Json.JsonElement json, [CallerMemberName] string membername = "")
        {
            logger.LogInformation($"{membername} -> {User.Identity.Name} json-> {json.GetRawText()}");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task PublishUpdate(int id)
        {
            try
            {
                var response = await http.GetAsync($"v1/LiveShipments/PublishIncrementalUpdates?id={id}");

                logger.LogDebug($"request -> {response.RequestMessage?.RequestUri.AbsoluteUri}");
                var content = await response.Content.ReadAsStringAsync();
                content = content.Substring(0, Math.Min(50, content.Length));
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"{response.StatusCode}-{content}");
                }

                logger.LogDebug($"request -> {response.RequestMessage?.RequestUri.AbsoluteUri} -- {response.StatusCode}-{content}");

                response = await http.GetAsync(new Uri($"http://sosgroup.com.pk/v1/LiveShipments/PublishIncrementalUpdates?id={id}", UriKind.Absolute));
                logger.LogInformation("Publish update response {0}, queue {1}", response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

    }


    public class ConsignmentBaseModel
    {
        public int ConsignmentId { get; set; }
    }

    public class ShipmentCodeModel : ConsignmentBaseModel
    {
        public string ManualShipmentCode { get; set; }
    }


    public class CitConsignmentBaseModel : ConsignmentBaseModel
    {
        public DateTimeOffset TimeStamp { get; set; }

        public int DeliveryId { get; set; }

        public double? Lat { get; set; }

        public double? Long_ { get; set; }

        public double? Lng { get; set; }

        [JsonIgnore]
        public Shared.Enums.ConsignmentDeliveryState PreviousState { get; set; }

        [JsonIgnore]
        public Shared.Enums.ConsignmentDeliveryState CurrentState { get; set; }

        private static GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        public Point GetPoint()
        {
            if (Lat.HasValue && (Long_.HasValue))
                return geometryFactory.CreatePoint(new Coordinate(Long_.Value, Lat.Value));

            if (Lat.HasValue && (Lng.HasValue))
                return geometryFactory.CreatePoint(new Coordinate(Lng.Value, Lat.Value));

            return null;
        }
    }

    public class CheckinPickupModel : CitConsignmentBaseModel
    {
        public CheckinPickupModel()
        {
            PreviousState = ConsignmentDeliveryState.Acknowldged;
            CurrentState = ConsignmentDeliveryState.ReachedPickup;
        }
    }

    public class CheckinDropOffModel : CitConsignmentBaseModel
    {
        public CheckinDropOffModel()
        {
            PreviousState = ConsignmentDeliveryState.InTransit;
            CurrentState = ConsignmentDeliveryState.ReachedDestination;
        }
    }

    public class QRCodeModel : CitConsignmentBaseModel
    {
        public string QrCode { get; set; }

        public bool Result { get; set; }
    }

    public class PinCodeModel : CitConsignmentBaseModel
    {
        public int PinCode { get; set; }

        public bool Result { get; set; }
    }

    public class SealCodesModel : CitConsignmentBaseModel
    {
        public string[] SealCodes { get; set; }
    }

    public class BagsModel : CitConsignmentBaseModel
    {
        public int NoOfBags { get; set; }

        public bool IsSealed { get; set; }
    }

    public class CitDenominationViewModel : CitConsignmentBaseModel
    {


        public DenominationType Type
        {
            get; set;
        }

        public int? Currency1x { get; set; }
        public int? Currency2x { get; set; }
        public int? Currency5x { get; set; }

        public int? Currency10x
        {
            get; set;
        }

        public int? Currency20x
        {
            get; set;
        }

        public int? Currency50x
        {
            get; set;
        }

        public int? Currency100x
        {
            get; set;
        }
        public int? Currency75x
        {
            get; set;
        }

        public int? Currency500x
        {
            get; set;
        }

        public int? Currency1000x
        {
            get; set;
        }

        public int? Currency5000x
        {
            get; set;
        }
    }

    public class LocationModel
    {
        public GeoCoordinate[] Points { get; set; }

    }
}