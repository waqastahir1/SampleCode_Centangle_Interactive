using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class ATMRServicesController : ControllerBase
    { 
        private readonly AppDbContext context; 
        private readonly SequenceService sequenceService; 
        private readonly EmployeeService peopleService;
        private readonly PartiesService partiesService;
        private readonly NotificationService notificationService;
        private readonly AtmrService atmrService;

        public ATMRServicesController(AppDbContext appDbContext, 
           SequenceService sequenceService,
           EmployeeService peopleService,
           PartiesService partiesService,
           NotificationService notificationService,
           AtmrService atmrService)
        {
            context = appDbContext; 
            this.sequenceService = sequenceService;
            this.peopleService = peopleService;
            this.partiesService = partiesService;
            this.notificationService = notificationService;
            this.atmrService = atmrService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(int rowsPerPage, int currentIndex)
        {
            try
            {
                var query = (from c in context.ATMServices
                             from a in context.Parties.Where(x => x.Id == c.ATMId)
                             from b in context.Parties.Where(x => x.Id == c.CashBranchId).DefaultIfEmpty()
                             from s in context.Parties.Where(x => x.Id == c.CustomerId).DefaultIfEmpty()
                             where c.ATMRServiceType >= ATMRServiceType.ATMR
                             orderby c.Id descending
                             select new ATMServiceListViewModel()
                             {
                                 Id = c.Id,
                                 AtmCode = a.ShortName,
                                 AtmName = a.FormalName,
                                 AtmAddress = a.Address,
                                 ATMRServiceType = c.ATMRServiceType,
                                 ConsignmentNo = c.ShipmentCode,
                                 CashBranchAddress = b.Address,
                                 CashBranchContact = b.PersonalContactNo,
                                 CashBranchCode = b.ShortName,
                                 CashBranchName = b.FormalName,
                                 CustomerId = s.Id,
                                 CustomerCode = s.ShortName,
                                 CustomerName = s.FormalName,
                                 Currency500x = c.Currency500x,
                                 Currency5000x = c.Currency5000x,
                                 Currency1000x = c.Currency1000x,
                                 QrCode = $"Pickup-{c.Id}_{c.ATMId}",
                                 ATMServiceState = c.ATMReplanishmentState,
                                 AtmServiceLogs = (from l in context.ATMServiceLogs
                                                   where l.ATMServiceId == c.Id
                                                   select new AtmServiceLogListViewModel
                                                   {
                                                       ATMReplanishmentState = (ATMServiceState)l.ATMServiceState,
                                                       ATMServiceId = c.Id,
                                                       Status = l.StateType,
                                                       TimeStamp = l.TimeStamp

                                                   }).ToList()
                             });
                var totalRows = query.Count();
                var items = await query.Skip((currentIndex - 1) * rowsPerPage).Take(rowsPerPage).ToListAsync();


                return Ok(new IndexViewModel<ATMServiceListViewModel>(items, totalRows));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {

            var order = await (from o in context.ATMServices
                               where o.Id == id
                               select new ATMServiceFormViewModel()
                               {
                                   Id = o.Id,
                                   ConsignmentNo = o.ShipmentCode,
                                   ATMId = o.ATMId,
                                   ATMRServiceType = o.ATMRServiceType,
                                   Currency500x = o.Currency500x,
                                   Currency1000x = o.Currency1000x,
                                   Currency5000x = o.Currency5000x,
                                   CashSourceBranchId = o.CashBranchId,
                                   Deadline = o.Deadline
                               }).FirstOrDefaultAsync();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ATMServiceFormViewModel selectedItem)
        {
            try
            {
                var atmrConsignment = await CreateOrder(selectedItem);

                if (atmrConsignment.Id > 0)
                { 
                    var citShipment = await atmrService.RequestCIT(new Shared.ATMR.ATMRBaseModel(atmrConsignment.Id), User.Identity.Name);
                    atmrConsignment.ShipmentId = citShipment.ShipmentId;

                    atmrConsignment.ReturnShipmentId = await atmrService.RequestReturnCIT(atmrConsignment.Id, User.Identity.Name, "RC", "Return Cash");
                    atmrConsignment.AccessCashReturnShipmentId = await atmrService.RequestReturnCIT(atmrConsignment.Id, User.Identity.Name, "RAC", "Return Access Cash");
                    atmrConsignment.CardReturnShipmentId = await atmrService.RequestReturnCIT(atmrConsignment.Id, User.Identity.Name, "CDS", "Return Cards");

                    await context.SaveChangesAsync();
                    await notificationService.CreateFirebaseNotification(atmrConsignment.Id, atmrConsignment.Id, atmrConsignment.ATMId, atmrConsignment.ShipmentCode,
                      selectedItem.Id == 0 ? NotificationType.New : NotificationType.UpdatedConsignment, NotificationCategory.ATMR);
                }
                return Ok(atmrConsignment.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<ATMService> CreateOrder(ATMServiceFormViewModel selectedItem)
        {
            ATMService atmService = null;

            var customer = partiesService.GetMainCustomer(selectedItem.ATMId);
            if (customer == null)
                throw new InvalidOperationException($"Main customer is not defined for {selectedItem.ConsignmentNo}");

            if (selectedItem.Id > 0)
            {
                atmService = await context.ATMServices.FirstOrDefaultAsync(x => x.Id == selectedItem.Id);
            }
            if (atmService == null)
            {
                var nextId = sequenceService.GetNextCitOrdersSequence();
                atmService = new ATMService()
                {
                    Id = nextId,
                    ShipmentCode = $"ATMR/{MyDateTime.Now.Year}/" + nextId.ToString("D4"),
                  
                    ATMReplanishmentState = ATMServiceState.Created,
                    ATMRServiceType = selectedItem.ATMRServiceType,
                    TimeStamp = DateTime.UtcNow,
                    CreatedBy = User.Identity.Name,
                    CachierId = peopleService.GetAtmManager(selectedItem.ATMId)?.IntValue,
                    TechnitianId = peopleService.GetAtmManager(selectedItem.ATMId)?.IntValue,
                };

                context.ATMServices.Add(atmService);
            }
            atmService.ATMId = selectedItem.ATMId;
            atmService.CashBranchId = selectedItem.CashSourceBranchId;
            atmService.CustomerId = customer.Id;
            atmService.Deadline = selectedItem.Deadline;
            atmService.Currency1000x = selectedItem.Currency1000x;
            atmService.Currency5000x = selectedItem.Currency5000x;
            atmService.Currency500x = selectedItem.Currency500x;
            atmService.PickupQrCode = $"{atmService.ShipmentCode}{selectedItem.ATMId}-Pickup";
            atmService.CITPickupQrCode = $"{atmService.ShipmentCode}{selectedItem.ATMId}-CIT-Pickup";
            atmService.CITDropoffQrCode = $"{atmService.ShipmentCode}{selectedItem.ATMId}-CIT-Dropoff";
            atmService.ReturnCITPickupQrCode = $"{atmService.ShipmentCode}{selectedItem.ATMId}-Return-CIT-Pickup";

            var serviceLog = await context.ATMServiceLogs.FirstOrDefaultAsync(x => x.ATMServiceId == atmService.Id);
            if(serviceLog == null)
            {
                serviceLog = new ATMServiceLog()
                {
                    ATMServiceId = atmService.Id,
                    Id = sequenceService.GetNextCommonSequence(),
                    ATMServiceState = 1,
                    StateType = StateTypes.Confirmed,
                    UserId = User.Identity.Name,
                    TimeStamp = MyDateTime.Now,
                };
                context.ATMServiceLogs.Add(serviceLog);
            }
            await context.SaveChangesAsync();
            return atmService;
        }
    }
}

