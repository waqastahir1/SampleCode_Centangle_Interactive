using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.APIs.Models;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.ATMR;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ATMRController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ILogger logger;
        private readonly HttpClient http;

        public ATMRController(AppDbContext context,
            ILogger<ATMRController> logger,
            HttpClient http)
        {
            this.context = context;
            this.logger = logger;
            this.http = http;
        }
         
        [HttpGet]
        [Produces(typeof(ATMServiceModel))]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var query = (from c in context.ATMServices
                             from a in context.Parties.Where(x => x.Id == c.ATMId)
                             from b in context.Parties.Where(x => x.Id == c.CashBranchId).DefaultIfEmpty()
                             from s in context.Parties.Where(x => x.Id == c.CustomerId).DefaultIfEmpty()
                             from cit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.ShipmentId)
                             from rcit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.ReturnShipmentId)
                             from acrcit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.AccessCashReturnShipmentId)
                             from crcit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.CardReturnShipmentId)
                             where c.Id == id
                             select new ATMServiceModel()
                             {
                                 AtmServiceId = c.Id,
                                 AtmCode = a.ShortName,
                                 AtmName = a.FormalName,
                                 AtmAddress = a.Address,
                                 ATMRServiceType = c.ATMRServiceType,
                                 ATMRServiceTypeStr = c.ATMRServiceType.ToString(),
                                 Lat = a.Orgnization.Geolocation.X,
                                 Lng = a.Orgnization.Geolocation.Y,
                                 ConsignmentNo = c.ShipmentCode,
                                 CashBranchAddress = b.Address,
                                 CashBranchContact = b.Address,
                                 CashBranchCode = b.ShortName,
                                 CashBranchName = b.FormalName,
                                 Deadline = c.Deadline,
                                 Currency1000x = c.Currency1000x,
                                 Currency5000x = c.Currency5000x,
                                 Currency500x = c.Currency500x,
                                 CachierName = "Dummy Name 1",
                                 TechnitianName = "Dummy Name 2",

                                 ATMServiceState = c.ATMReplanishmentState,
                                 ATMServiceStateStr = c.ATMReplanishmentState.ToString(),
                                 CreatedAt = c.TimeStamp,
                                 DueAt = c.DueTime,
                                 CashPickupQrCode = $"Pickup-{c.Id}_{c.ATMId}",
                                 CitExchangeQrCode = cit.PickupCode,
                                 CitDropoffQrCode = cit.DropoffCode,
                                 CitConsignmentId = cit.ConsignmentId,
                                 CitDeliveryId = cit.Id,

                                 CitRemainingCashReturnCollectionQrCode = rcit.PickupCode,
                                 CitRemainingCashReturnConsignmentId = rcit.ConsignmentId,
                                 CitRemainingCashReturnDeliveryId = rcit.Id,
                                 
                                 CitAccessCashQrCode = acrcit.PickupCode,
                                 CitAccessCashReturnConsignmentId = acrcit.ConsignmentId,
                                 CitAccessCashReturnDeliveryId = acrcit.Id,

                                 CitCardsReturnQrCode = crcit.PickupCode,
                                 CitAtmCardsReturnConsignmentId = crcit.ConsignmentId,
                                 CitAtmCardsReturnDeliveryId = crcit.Id,
                                 SealCodes = new string[0],
                                 IsFinalized = true,
                                 //SealCodes = context.ATMRSealCodes.Where(x => x.AtmrServiceId == c.Id).Select(x => x.SealCode).ToList()
                             });


                var atmService = await query.FirstOrDefaultAsync();
                if (atmService == null)
                    return NoContent(); 

                atmService.Checklist = new ATMChecklist[]
                               {
                                    new ATMChecklist
                                    {
                                         Id = 1,
                                          Title = "Link Down"
                                    },
                                    new ATMChecklist
                                    {
                                         Id = 2,
                                          Title = "Dispenser Fault"
                                    },
                                    new ATMChecklist
                                    {
                                         Id = 3,
                                          Title = "Card Reader Fault"
                                    },
                                    new ATMChecklist
                                    {
                                         Id = 4,
                                          Title = "Journal Printer Replenishment"
                                    }
                               };

                return Ok(atmService);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString() + ex.InnerException?.ToString());
            }
        }

        [HttpGet]
        [Produces(typeof(ATMServiceModel[]))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = (from c in context.ATMServices
                             from a in context.Parties.Where(x => x.Id == c.ATMId)
                             from b in context.Parties.Where(x => x.Id == c.CashBranchId).DefaultIfEmpty()
                             from s in context.Parties.Where(x => x.Id == c.CustomerId).DefaultIfEmpty()
                             from cit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.ShipmentId)
                             from rcit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.ReturnShipmentId)
                             from acrcit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.AccessCashReturnShipmentId)
                             from crcit in context.ConsignmentDeliveries.Where(x => x.ConsignmentId == c.CardReturnShipmentId)
                             where c.ATMRServiceType >= ATMRServiceType.ATMR
                             select new ATMServiceModel()
                             {
                                 AtmServiceId = c.Id,
                                 AtmCode = a.ShortName,
                                 AtmName = a.FormalName,
                                 AtmAddress = a.Address,
                                 ATMRServiceType = c.ATMRServiceType,
                                 ATMRServiceTypeStr = c.ATMRServiceType.ToString(),
                                 ATMServiceState = c.ATMReplanishmentState,
                                 ATMServiceStateStr = c.ATMReplanishmentState.ToString(),
                                 Lat = a.Orgnization.Geolocation.X,
                                 Lng = a.Orgnization.Geolocation.Y,
                                 ConsignmentNo = c.ShipmentCode,
                                 CashBranchAddress = b.Address,
                                 CashBranchContact = b.Address,
                                 CashBranchCode = b.ShortName,
                                 CashBranchName = b.FormalName,
                                 Deadline = c.Deadline,
                                 Currency1000x = c.Currency1000x,
                                 Currency5000x = c.Currency5000x,
                                 Currency500x = c.Currency500x,
                                 CachierName = "Dummy Name 1",
                                 TechnitianName = "Dummy Name 2",
                                 CreatedAt = c.TimeStamp,
                                 DueAt = c.DueTime,
                                 CashPickupQrCode = $"Pickup-{c.Id}_{a.Id}",
                                 CitExchangeQrCode = cit.PickupCode,
                                 CitDropoffQrCode = cit.DropoffCode,
                                 CitConsignmentId = cit.ConsignmentId,
                                 CitDeliveryId = cit.Id,

                                 CitRemainingCashReturnCollectionQrCode = rcit.PickupCode,
                                 CitRemainingCashReturnConsignmentId = rcit.ConsignmentId,
                                 CitRemainingCashReturnDeliveryId = rcit.Id,

                                 CitAccessCashQrCode = acrcit.PickupCode,
                                 CitAccessCashReturnConsignmentId = acrcit.ConsignmentId,
                                 CitAccessCashReturnDeliveryId = acrcit.Id,

                                 CitCardsReturnQrCode = crcit.PickupCode,
                                 CitAtmCardsReturnConsignmentId = crcit.ConsignmentId,
                                 IsFinalized = true,
                                 SealCodes = Array.Empty<string>()
                                 //SealCodes = context.ATMRSealCodes.Where(x => x.AtmrServiceId == c.Id).Select(x => x.SealCode).ToList()
                             });
                 
                var atmService = await query.ToArrayAsync();
                if (atmService.Count() == 0)
                    return NoContent();
                 
                for (int i = 0; i < atmService.Length; i++)
                {
                    atmService[i].Checklist = new ATMChecklist[]
                              {
                                    new ATMChecklist
                                    {
                                         Id = 1,
                                          Title = "Link Down"
                                    },
                                    new ATMChecklist
                                    {
                                         Id = 2,
                                          Title = "Dispenser Fault"
                                    },
                                    new ATMChecklist
                                    {
                                         Id = 3,
                                          Title = "Card Reader Fault"
                                    },
                                    new ATMChecklist
                                    {
                                         Id = 4,
                                          Title = "Journal Printer Replenishment"
                                    }
                              };
                }
               

                return Ok(atmService);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString() + ex.InnerException?.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Acknowledge([FromBody] ATMRBaseModel model)
        {
            var atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == model.AtmrServiceId);
            if (atmService == null)
                return NotFound("consignment does not exist");

            atmService.ATMReplanishmentState = ATMServiceState.ReachedPickup;
            context.SaveChanges();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ScanCashPickupQrCode([FromBody] ATMRBaseModel model)
        {
            var atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == model.AtmrServiceId);
            if (atmService == null)
                return NotFound("consignment does not exist");

            atmService.ATMReplanishmentState = ATMServiceState.InTransit;
            context.SaveChanges();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ScanCashTakeOverQrCode([FromBody] ATMRBaseModel model)
        {
            var atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == model.AtmrServiceId);
            if (atmService == null)
                return NotFound("consignment does not exist");

            atmService.ATMReplanishmentState = ATMServiceState.InProgress;
            context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteAtmrConsignment([FromBody] ATMRBaseModel model)
        {
            var atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == model.AtmrServiceId);
            if (atmService == null)
                return NotFound("consignment does not exist");

            atmService.ATMReplanishmentState = ATMServiceState.Completed;
            context.SaveChanges();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> PostChecklist([FromBody] CheckListPostModel model)
        {
            var consignment = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == model.AtmrServiceId);
            if (consignment == null)
                return NotFound("consignment does not exist");

            return Ok();
        }

         
        [HttpPost]
        [Route("PostSealCodes")]
        public async Task<IActionResult> PostSealCodes([FromBody] AtmrSealCodesModel model)
        {
            if (model.SealCodes == null || model.SealCodes.Length == 0)
                return BadRequest("Sealcode list is empty");

            if (!context.ATMServices.Any(x => x.Id == model.AtmrServiceId))
                return NotFound();

            var rejected = new List<string>();

            foreach (var item in model.SealCodes)
            {
                var sealCodeExists = await context.ATMRSealCodes.AnyAsync(x => x.SealCode == item && x.AtmrServiceId == model.AtmrServiceId);
                if (sealCodeExists)
                {
                    rejected.Add(item);
                }
                else
                {
                    var sealCode = new ATMRSealCode()
                    {
                        AtmrServiceId = model.AtmrServiceId,
                        SealCode = item,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.Identity.Name
                    };
                    context.ATMRSealCodes.Add(sealCode);
                }
            }
            await context.SaveChangesAsync();

            return Ok(new AtmrSealCodesModel()
            {
                AtmrServiceId = model.AtmrServiceId, 
                SealCodes = rejected.ToArray()
            });
        }

        [HttpPost]
        [Route("PostCashCounter")]
        public async Task<IActionResult> PostCashCounter([FromBody] AtmrCashCounterModel model)
        {
             
            if (!(await context.ATMServices.AnyAsync(x => x.Id == model.AtmrServiceId)))
                return NotFound();

            return Ok();
        }

        [HttpPost]
        [Route("PostAtmCardImage")]
        public async Task<IActionResult> PostAtmCardImage([FromBody] AtmCarImage model)
        {

            if (!(await context.ATMServices.AnyAsync(x => x.Id == model.AtmrServiceId)))
                return NotFound();

            if (string.IsNullOrEmpty(model.Image) || model.Image.Length < 200)
                return BadRequest("Image Data not found");

            return Ok();
        }


        [HttpPost]
        [Route("PostAtmCardImages")]
        public async Task<IActionResult> PostAtmCardImages([FromBody] AtmCarImages model)
        {

            if (!(await context.ATMServices.AnyAsync(x => x.Id == model.AtmrServiceId)))
                return NotFound();

            if (model.Images == null || model.Images.Length == 0)
                return BadRequest("Image Data not found");

            return Ok();
        }
    }

}

