using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;

namespace SOS.OrderTracking.Web.Server.Controllers.Operations
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CitFinalizeConsignmentsController : ControllerBase, ICitFinalizeConsignmentsService
    {
        private readonly AppDbContext context;
        private readonly PartiesCacheService cache;
        private readonly ShipmentsCacheService shipmentsCache;

        public CitFinalizeConsignmentsController(AppDbContext context, PartiesCacheService cache, ShipmentsCacheService shipmentsCache)
        {
            this.context = context;
            this.cache = cache;
            this.shipmentsCache = shipmentsCache;
        }
       
        public Task<ShipmentFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

     
        [HttpGet]
        public async Task<IndexViewModel<CitFinalizeConsignmentsListViewModel>> GetPageAsync([FromQuery] CitFinalizeConsignmentsAdditionalValueModel vm)
        {
            try
            {
                IQueryable<CitFinalizeConsignmentsListViewModel> query = null;

                query = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         where (vm.RegionId == null || t.RegionId == vm.RegionId || f.RegionId == vm.RegionId)
                         && (vm.SubRegionId == null || f.SubregionId == vm.SubRegionId || t.SubregionId == vm.SubRegionId)
                         && (vm.StationId == null || f.StationId == vm.StationId || t.StationId == vm.StationId)
                         && (vm.StartDate.Date <= c.CreatedAt.Date && vm.EndDate.Date >= c.CreatedAt.Date)
                         && (vm.ConsignmentStateType == null ||  vm.ConsignmentStateType == c.ConsignmentStateType)
                         && (vm.CrewId == 0 || vm.CrewId == d.CrewId)
                         && (vm.ConsignmentStatus == ConsignmentStatus.All || c.ConsignmentStatus == vm.ConsignmentStatus)
                         && c.ApprovalState.HasFlag( ConsignmentApprovalState.Approved)
                         && c.Type == ShipmentExecutionType.Live
                         orderby c.Id, d.ParentId
                         select new CitFinalizeConsignmentsListViewModel()
                         {
                             ManualShipmentCode = c.ManualShipmentCode,
                             ConsignmentId = c.Id,
                             ShipmentCode = c.ShipmentCode,
                             ShipmentType = c.ShipmentType,
                             FromPartyId = f.Id,
                             ToPartyId = t.Id,
                             FromPartyCode = f.ShortName,
                             ToPartyCode = t.ShortName,
                             FromPartyName = f.FormalName,
                             ToPartyName = t.FormalName,
                             Amount = c.CurrencySymbol == CurrencySymbol.PKR? c.Amount : c.AmountPKR,
                             CreatedAt = c.CreatedAt,
                             ConsignmentStateType = c.ConsignmentStateType,
                             CrewName = context.Parties.FirstOrDefault(x => x.Id == d.CrewId).FormalName,
                             DeliveryId = d.Id,
                             CrewId = d.CrewId,
                             PreviousId = d.ParentId,
                             BillBranchId = c.BillBranchId,
                             BillBranchName = context.Parties.FirstOrDefault(x => x.Id == c.BillBranchId).ShortName,
                             ConsignmentStatus = c.ConsignmentStatus,
                             CreatedBy = c.CreatedBy,
                             Distance = c.Distance,
                             DistanceStatus = c.DistanceStatus
                         });
 
                var totalRows = query.Count();

                var items = await query.ToListAsync();

                foreach (var x in items)
                {
                    x.FromPartyGeoStatus = await cache.GetGeoStatus(x.FromPartyId);
                    x.ToPartyGeoStatus = await cache.GetGeoStatus(x.ToPartyId);
                }

                return new IndexViewModel<CitFinalizeConsignmentsListViewModel>(items, totalRows);
            }
            catch (Exception)
            { 
                throw;
            }
        }

        public Task<int> PostAsync(ShipmentFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<int> FinalizeShipment(List<CitFinalizeConsignmentsListViewModel> citFinalizeConsignmentsListViewModels)
        {
            try
            {
                foreach (var item in citFinalizeConsignmentsListViewModels)
                {
                    var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == item.ConsignmentId);
                     
                    consignment.ConsignmentStatus = ConsignmentStatus.Pushing;
                    consignment.ShipmentType = item.ShipmentType;

                    //if (consignment.Comments?.Split(",").LastOrDefault() != item.ConsignmentStatus.ToString())
                    //    consignment.Comments = item.Comments + "," + item.ConsignmentStatus;
                    List<ShipmentComment> listOfComments = new();
                    if (consignment.Comments != null)
                        listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                    if (item.Comments != null)
                    {
                        if (listOfComments.LastOrDefault().Description?.Split(",").LastOrDefault() != item.ConsignmentStatus.ToString())
                        {
                            listOfComments.Add(new ShipmentComment()
                            {
                                Description = item.Comments + "," + item.ConsignmentStatus,
                                CreatedAt = MyDateTime.Now,
                                CreatedBy = User.Identity.Name,
                                ViewedAt = MyDateTime.Now,
                                ViewedBy = User.Identity.Name
                            });
                        }
                        consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                    }
                    await shipmentsCache.SetShipment(consignment.Id, null);
                    context.Consignments.Update(consignment);
                    await context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return 1;
        }


        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetCrews(int RegionId, int? SubRegionId, int? StationId)
        {
            if (RegionId == 0)
                return Array.Empty<SelectListItem>();

            var crews = from o in context.Parties
                        from station in context.Parties.Where(x => x.Id == o.StationId).DefaultIfEmpty()
                        where o.Orgnization.OrganizationType == OrganizationType.Crew
                        && o.RegionId == RegionId
                        && (SubRegionId == 0 || o.SubregionId == SubRegionId)
                        && (StationId == 0 || o.StationId == StationId)
                        select new SelectListItem(o.Id, o.FormalName);

            return await crews.ToArrayAsync();
        }

    }
}
