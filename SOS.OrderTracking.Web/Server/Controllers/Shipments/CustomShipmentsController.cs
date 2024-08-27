using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.CustomShipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Shipments
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CustomShipmentsController : ControllerBase, ICustomShipmentService
    {
        private readonly AppDbContext context;
        private readonly PartiesCacheService cache;

        public CustomShipmentsController(AppDbContext context, PartiesCacheService cache)
        {
            this.context = context;
            this.cache = cache;
        }
        public Task<CustomShipmentFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<IndexViewModel<CustomShipmentListViewModel>> GetPageAsync([FromQuery] CustomShipmentAdditionalValueViewModel vm)
        {
            try
            {

                var totalRows = 0;

                var items = new List<CustomShipmentListViewModel>();


                return new IndexViewModel<CustomShipmentListViewModel>(items, totalRows);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IEnumerable<CustomShipmentListViewModel>> SearchCustomShipments([FromQuery]CustomShipmentAdditionalValueViewModel vm)
        {
            try
            {
                IQueryable<CustomShipmentListViewModel> query = null;

                query = (from c in context.Consignments
                         join f in context.Parties on c.FromPartyId equals f.Id
                         join t in context.Parties on c.ToPartyId equals t.Id
                         join s in context.Parties on c.MainCustomerId equals s.Id
                         join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                         where (vm.RegionId == null || t.RegionId == vm.RegionId || f.RegionId == vm.RegionId)
                         && (vm.SubRegionId == null || f.SubregionId == vm.SubRegionId || t.SubregionId == vm.SubRegionId)
                         && (vm.StationId == null || f.StationId == vm.StationId || t.StationId == vm.StationId)
                         && (vm.StartDate.Date <= c.CreatedAt.Date && vm.EndDate.Date >= c.CreatedAt.Date)
                         && (vm.ConsignmentStateTypeInt == null || (ConsignmentDeliveryState)vm.ConsignmentStateTypeInt.GetValueOrDefault() == d.DeliveryState)
                         && (vm.CrewId == 0 || vm.CrewId == d.CrewId)
                         && (vm.ConsignmentStatus == ConsignmentStatus.All || c.ConsignmentStatus == vm.ConsignmentStatus)
                         && (vm.PostingMessage == null || c.PostingMessage.Contains(vm.PostingMessage))
                         && c.Type ==  ShipmentExecutionType.Live
                         && c.ApprovalState.HasFlag(ConsignmentApprovalState.Approved)
                         orderby c.Id, d.ParentId
                         select new CustomShipmentListViewModel()
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
                             Amount = c.Amount,
                             CreatedAt = c.CreatedAt,
                             ConsignmentStateType = d.DeliveryState,
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
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Task<int> PostAsync(CustomShipmentFormViewModel selectedItem)
        {
            throw new NotImplementedException();
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
