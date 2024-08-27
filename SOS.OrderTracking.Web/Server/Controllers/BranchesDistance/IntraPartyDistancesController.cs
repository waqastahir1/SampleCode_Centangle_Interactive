using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.BranchesDistance
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class IntraPartyDistancesController : ControllerBase, IIntraPartyDistance
    {
        private readonly AppDbContext context;
        private readonly SequenceService sequenceService;
        public IntraPartyDistancesController(AppDbContext context, SequenceService sequenceService)
        {
            this.context = context;
            this.sequenceService = sequenceService;
        }
        [HttpGet]
        public async Task<IntraPartyDistanceFormViewModel> GetAsync([FromQuery] Tuple<int, int> id)
        {
            var query = await (from i in context.IntraPartyDistances
                               where i.FromPartyId == id.Item1 && i.ToPartyId == id.Item2
                               select new IntraPartyDistanceFormViewModel()
                               {
                                   FromPartyId = i.FromPartyId,
                                   ToPartyId = i.ToPartyId,
                                   FromPartyName = context.Parties.FirstOrDefault(x => x.Id == i.FromPartyId).FormalName,
                                   ToPartyName = context.Parties.FirstOrDefault(x => x.Id == i.ToPartyId).FormalName,
                                   Distance = i.Distance / 1000,
                                   AverageTravelTime = i.AverageTravelTime,
                                   DistanceStatus = i.DistanceStatus,
                                   DistanceSource = i.DistanceSource
                               }).FirstOrDefaultAsync();

            return query;
        }
        [HttpGet]
        public async Task<IndexViewModel<IntraPartyDistanceListViewModel>> GetPageAsync([FromQuery] IntraPartyDistanceAdditionalValueViewModel vm)
        {
            var query = from i in context.IntraPartyDistances
                        join f in context.Parties on i.FromPartyId equals f.Id
                        join t in context.Parties on i.ToPartyId equals t.Id
                        where (vm.FromPartyId == 0 || vm.FromPartyId == i.FromPartyId)
                        && (vm.ToPartyId == 0 || vm.ToPartyId == i.ToPartyId)
                        where i.FromPartyId != 0 && i.ToPartyId != 0
                        select new IntraPartyDistanceListViewModel()
                        {
                            FromPartyId = i.FromPartyId,
                            ToPartyId = i.ToPartyId,
                            FromPartyName = $"({f.ShortName}) {f.FormalName}",
                            ToPartyName = $"({t.ShortName}) {t.FormalName}",
                            Distance = i.Distance / 1000,
                            AverageTravelTime = i.AverageTravelTime,
                            DistanceStatus = i.DistanceStatus,
                            DistanceSource = i.DistanceSource,
                            CreatedAt = i.CreatedAt,
                            CreatedBy = i.CreatedBy
                        };

            var totalRows = query.Count();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<IntraPartyDistanceListViewModel>(items, totalRows);
        }
        [HttpPost]
        public async Task<Tuple<int, int>> PostAsync(IntraPartyDistanceFormViewModel selectedItem)
        {
              
            try
            {
              var  intraPartyDistance = await context.IntraPartyDistances.FirstOrDefaultAsync(x => x.FromPartyId == selectedItem.FromPartyId && x.ToPartyId == selectedItem.ToPartyId);

                if (intraPartyDistance == null)
                {
                    intraPartyDistance = new()
                    {
                        FromPartyId = selectedItem.FromPartyId.GetValueOrDefault(),
                        ToPartyId = selectedItem.ToPartyId.GetValueOrDefault(),
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name,
                    };
                    context.IntraPartyDistances.Add(intraPartyDistance);
                }
                else
                { 
                  var  distanceHistory = new IntraPartyDistanceHistory()
                    {
                        Id = sequenceService.GetNextIntraPartyDistanceSequence(),
                        FromPartyId = intraPartyDistance.FromPartyId,
                        ToPartyId = intraPartyDistance.ToPartyId,
                        CreatedAt = intraPartyDistance.CreatedAt,
                        CreatedBy = intraPartyDistance.CreatedBy,
                        UpdatedBy = User.Identity.Name,
                        UpdateAt = intraPartyDistance.UpdateAt,
                        DistanceSource = intraPartyDistance.DistanceSource,
                        DistanceStatus = intraPartyDistance.DistanceStatus,
                        Distance = intraPartyDistance.Distance,
                    };
                    context.IntraPartyDistancesHistory.Add(distanceHistory);

                }

                intraPartyDistance.UpdateAt = MyDateTime.Now;
                intraPartyDistance.UpdatedBy = User.Identity.Name;
                intraPartyDistance.DistanceSource = selectedItem.DistanceSource;
                intraPartyDistance.DistanceStatus = selectedItem.DistanceStatus;
                intraPartyDistance.AverageTravelTime = selectedItem.AverageTravelTime.GetValueOrDefault();
                intraPartyDistance.Distance = selectedItem.Distance.GetValueOrDefault() * 1000;

                await context.SaveChangesAsync();
                return new Tuple<int, int>(intraPartyDistance.FromPartyId, intraPartyDistance.ToPartyId);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            } 
        }
         
    }

}
