using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Guarding
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class GaurdsController : ControllerBase, IGaurdService
    {
        private readonly AppDbContext context;
        private readonly ILogger<GaurdsController> logger;
        private readonly SequenceService sequenceService;
        public GaurdsController(AppDbContext appDbContext,
            ILogger<GaurdsController> logger, SequenceService sequenceService)
        {
            context = appDbContext;
            this.logger = logger;
            this.sequenceService = sequenceService;
        }

        public async Task<GaurdFormViewModel> GetAsync(int id)
        {
            var gaurd = await (from r in context.PartyRelationships
                               join p in context.Parties on r.FromPartyId equals p.Id
                               where r.Id == id
                               select new GaurdFormViewModel()
                               {
                                   RelationshipId = r.Id,
                                   BranchId = r.ToPartyId,
                                   Name = p.FormalName,
                                   Id = p.Id,
                                   StartDate = r.StartDate,
                                   EndDate = r.ThruDate
                               }).FirstOrDefaultAsync();
            return gaurd;
        }

        public async Task<IEnumerable<SelectListItem>> GetGaurds()
        {
            var gaurds = await (from p in context.Parties
                                join r in context.PartyRelationships on p.Id equals r.FromPartyId
                                //    where r.FromPartyRole == RoleType.Gaurd
                                where p.PartyType == PartyType.Person
                                select new SelectListItem()
                                {
                                    IntValue = p.Id,
                                    Text = p.FormalName,
                                }).ToListAsync();

            return gaurds;
        }

        public async Task<IndexViewModel<GaurdListViewModel>> GetPageAsync([FromQuery] GaurdAdditionalValueViewModel vm)
        {
            var query = from p in context.Parties
                        join r in context.PartyRelationships on p.Id equals r.FromPartyId
                        where r.ToPartyId == vm.BranchId
                        && r.FromPartyRole == RoleType.Gaurd
                        select new GaurdListViewModel()
                        {
                            RelationshipId = r.Id,
                            Id = p.Id,
                            Name = p.FormalName,
                            BranchId = vm.BranchId,
                            StartDate = r.StartDate,
                            EndDate = r.ThruDate
                        };

            var totalRows = await query.CountAsync();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<GaurdListViewModel>(items, totalRows);
        }

        public async Task<int> PostAsync(GaurdFormViewModel selectedItem)
        {
            try
            {
                var gaurdsDeploymentRules = await context.GaurdingOrganizations.FirstOrDefaultAsync(x => x.BranchId == selectedItem.BranchId);
                if (gaurdsDeploymentRules == null)
                    throw new BadRequestException("please specify gaurds deployment rules on prvious page!");



                PartyRelationship relationship = null;

                // if end date less then start date
                if (selectedItem.EndDate != null)
                {
                    if (DateTime.Compare((DateTime)selectedItem.EndDate, selectedItem.StartDate.GetValueOrDefault()) < 0)
                    {
                        throw new BadRequestException("End Date should be greater then start date");
                    }
                }

                if (selectedItem.RelationshipId == 0)
                {
                    relationship = await context.PartyRelationships.FirstOrDefaultAsync(
                       x => x.ToPartyId == selectedItem.BranchId
                       && x.FromPartyId == selectedItem.Id
                       // && x.FromPartyRole == RoleType.Customer
                       && (x.ThruDate == null || x.ThruDate >= selectedItem.StartDate));

                    if (relationship != null)
                        throw new BadRequestException("Selected Gaurd is already Active in due date");

                    relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == selectedItem.Id
                    && x.FromPartyRole == RoleType.Gaurd
                    && x.ToPartyRole == RoleType.Customer
                    && (x.ThruDate == null || x.ThruDate >= selectedItem.StartDate));

                    if (relationship != null)
                        throw new BadRequestException("Selected Gaurd is already Active in due date in " +
                            $"{context.Parties.FirstOrDefault(x => x.Id == relationship.ToPartyId).FormalName} ");


                    var appointedGaurds = await context.PartyRelationships.Where(x => x.FromPartyRole == RoleType.Gaurd
                    && x.ToPartyId == selectedItem.BranchId
                    && (x.ThruDate == null || x.ThruDate >= selectedItem.StartDate)).ToListAsync();
                    //&& x.IsActive).ToListAsync();

                    if (appointedGaurds.Count == gaurdsDeploymentRules.TotalNoOfGaurdsRequired)//3)
                        throw new BadRequestException($"{appointedGaurds.Count} " +
                           $"out of {gaurdsDeploymentRules.TotalNoOfGaurdsRequired} gaurds appointed in due date " +
                           $"please terminate someone or enhance gaurd capacity to appoint more gaurds!");
                }

                relationship = context.PartyRelationships.FirstOrDefault(x => x.Id == selectedItem.RelationshipId);
                if (relationship == null)
                {

                    relationship = new PartyRelationship
                    {
                        Id = sequenceService.GetNextPartiesSequence(),
                        ToPartyRole = RoleType.Customer,
                        ToPartyId = selectedItem.BranchId,
                        FromPartyId = selectedItem.Id.GetValueOrDefault(),
                        FromPartyRole = RoleType.Gaurd,
                        StartDate = selectedItem.StartDate.GetValueOrDefault(),

                    };

                    context.PartyRelationships.Add(relationship);
                }
                relationship.ThruDate = selectedItem.EndDate;
                relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);

                await context.SaveChangesAsync();

                gaurdsDeploymentRules.NoOfGaurdsAppointed = await context.PartyRelationships.Where(x => x.FromPartyRole == RoleType.Gaurd
                    && x.ToPartyId == selectedItem.BranchId
                    && x.StartDate <= MyDateTime.Today
                    && (x.ThruDate == null || x.ThruDate >= MyDateTime.Today)).CountAsync();

                await context.SaveChangesAsync();
                return relationship.Id;

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
