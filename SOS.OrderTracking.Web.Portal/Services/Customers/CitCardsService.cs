using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.Extenstions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Enums.CPC;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.BankSetting;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using static SOS.OrderTracking.Web.Shared.Constants.Roles;
using ConsignmentState = SOS.OrderTracking.Web.Common.Data.Models.ConsignmentState;
using Location = SOS.OrderTracking.Web.Common.Data.Models.Location;
using Point = SOS.OrderTracking.Web.Shared.ViewModels.Point;

namespace SOS.OrderTracking.Web.Portal.Services.Customers
{
    public class CitCardsService : ICitCardsService
    {
        private readonly SequenceService sequenceService;
        private readonly UserCacheService userCache;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<CitCardsService> logger;

        public ClaimsPrincipal? User { get; set; }

        public CitCardsService(AppDbContext context,
          ILogger<CitCardsService> logger,
          SequenceService sequenceService,
          UserCacheService userCache,
          IServiceScopeFactory scopeFactory,
          AuthenticationStateProvider authenticationStateAsync)
        {
            this.logger = logger;
            this.sequenceService = sequenceService;
            this.userCache = userCache;
            this.scopeFactory = scopeFactory;


            try
            {
                var authstate = authenticationStateAsync.GetAuthenticationStateAsync();
                User = authstate.Result.User;
            }
            catch { }

        }


        #region CRUD Methods
        public async Task<IndexViewModel<ConsignmentListViewModel>> GetPageAsync([FromQuery] CitCardsAdditionalValueViewModel vm)
        {
            try
            {
                if (User == null)
                    throw new ArgumentNullException("User is null");

                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                var workOrderService = scope.ServiceProvider.GetRequiredService<ConsignmentService>();
                List<int> customers = null;
                if (User.HasSOSRole() && vm.MainCustomerId > 0)
                {
                    customers = await context.PartyRelationships.Where(x => x.ToPartyId == vm.MainCustomerId && x.FromPartyRole == RoleType.ChildOrganization)
                   .Select(x => x.FromPartyId).ToListAsync();
                }
                else if (User.IsInititorOrSupervisor())
                {
                    vm.ConsignmentStatus = ConsignmentStatus.All;
                    vm.ConsignmentStateSummarized = ConsignmentDeliveryState.All;
                    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    customers = new List<int>() { user.PartyId };
                }
                else if (User.IsInRole(BANK))
                {
                    vm.ConsignmentStatus = ConsignmentStatus.All;
                    var user = await context.Users.Include(x => x.AllocatedBranches.Where(y => y.IsEnabled)).FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                    customers = user.AllocatedBranches.Select(x => x.PartyId).ToList();
                }

                var consignmentType = User.IsInititorOrSupervisor() ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;

                var query = workOrderService.GetConsignmentsIds(vm.RegionId.GetValueOrDefault(),
                    vm.SubRegionId.GetValueOrDefault(), vm.StationId.GetValueOrDefault(), customers, User.Identity.Name, vm.ConsignmentStateSummarized, vm.ShipmentType, ConsignmentApprovalState.Approved, ConsignmentApprovalState.ReApprove, consignmentType);

                if (vm.Rating > 0)
                {
                    query = query.Where(x => x.Rating == vm.Rating);
                }
                if (User.HasSOSRole() || User.IsInRole(BANK) || vm.ConsignmentType == ShipmentExecutionType.Scheduled)
                {
                    // for control rooms, show live shiipments + scheduled due in next hour
                    var almostNow = DateTime.Now.AddHours(1);
                    if (vm.ConsignmentType == ShipmentExecutionType.Live)
                    {
                        query = query.Where(x => x.PlanedCollectionTime == null || x.PlanedCollectionTime <= almostNow);
                    }
                    else
                    {
                        query = query.Where(x => x.PlanedCollectionTime != null);
                    }
                }

                if (vm.Id > 0)
                {
                    query = query.Where(x => x.Id == vm.Id);
                }
                else if (!string.IsNullOrEmpty(vm.SearchKey))
                {
                    query = query.Where(x => x.ShipmentCode.ToLower().Contains(vm.SearchKey.ToLower()) || x.ManualShipmentCode.ToLower().Contains(vm.SearchKey.ToLower()));
                }
                else
                {
                    query = query.Where(x => (vm.StartDate.Date <= x.DueTime.Date && vm.EndDate.Date >= x.DueTime.Date));

                    if (vm.ConsignmentStatus != ConsignmentStatus.All)
                    {
                        query = query.Where(x => x.ConsignmentStatus == vm.ConsignmentStatus);
                    }
                    if (vm.ConsignmentStateSummarized != ConsignmentDeliveryState.All)
                    {
                        if (vm.ConsignmentStateSummarized == ConsignmentDeliveryState.Clubbed)
                            query = query.Where(x => x.IsClubbed);

                        else
                            query = query.Where(x => x.ConsignmentStateType == vm.ConsignmentStateSummarized);
                    }
                }

                switch (vm.Sorting)
                {
                    case SortBy.CreationDateAsc:
                        query = query.OrderBy(x => x.CreatedAt);
                        break;
                    case SortBy.CreationDateDesc:
                        query = query.OrderByDescending(x => x.CreatedAt);
                        break;
                    case SortBy.DueDateAsc:
                        query = query.OrderBy(x => x.DueTime);
                        break;
                    case SortBy.DueDateDesc:
                        query = query.OrderByDescending(x => x.DueTime);
                        break;
                    case SortBy.DeliveryDateAsc:
                        query = query.OrderBy(x => x.ActualDeliveryTime);
                        break;
                    case SortBy.DeliveryDateDesc:
                        query = query.OrderByDescending(x => x.ActualDeliveryTime);
                        break;
                    case SortBy.ApprovalAsc:
                        query = query.OrderBy(x => x.ApprovalState);
                        break;
                    case SortBy.ApprovalDesc:
                        query = query.OrderByDescending(x => x.ApprovalState);
                        break;
                    default:
                        break;
                }
                var totalRows = query.Count();

                var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage)
                    .Take(vm.RowsPerPage).ToListAsync();


                var shipments = new List<ConsignmentListViewModel>(items.Count);
                var partiesCache = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<PartiesCacheService>();
                foreach (var item in items)
                {
                    try
                    {
                        //var ImageUrl = await context.ShipmentAttachments.Where(x => x.ConsignmentId == item.Id).Select(x => x.Url).FirstOrDefaultAsync();
                        var shipment = await shipmentsCacheService.GetShipment(item.Id);
                        shipment.Denomination.Valuables = item.Valueables;
                        shipment.ChangedDropOff = item.ToChangedPartyId == null ? null : workOrderService.GetChangedDropoffIfExists(item.Id);
                        if (shipment.CurrencySymbol == CurrencySymbol.USD || shipment.CurrencySymbol == CurrencySymbol.EURO)
                            shipment.Denomination.ExchangeRate = workOrderService.GetExchangeRate(item.Id);
                        if (shipment.BillBranchId != null)
                        {
                            var BillingBranchCode = await partiesCache.GetCode(shipment.BillBranchId ?? 0);
                            var BillingBranchName = await partiesCache.GetName(shipment.BillBranchId);
                            shipment.BillingRegion = BillingBranchCode + "-" + BillingBranchName;
                        }
                        shipment.ImageUrl = await context.ShipmentAttachments.Where(x => x.ConsignmentId == item.Id).Select(x => x.Url).FirstOrDefaultAsync();
                        shipments.Add(shipment);
                    }
                    catch
                    {
                        var shipment = await workOrderService.GetShipment(item.Id);
                        shipment.Denomination.Valuables = item.Valueables;
                        shipment.ChangedDropOff = item.ToChangedPartyId == null ? null : workOrderService.GetChangedDropoffIfExists(item.Id);

                        await shipmentsCacheService.SetShipment(shipment.Id, shipment);
                        //shipment.ImageUrl = ImageUrl;
                        shipments.Add(shipment);
                    }

                }

                foreach (var item in shipments.Where(x => x.ApprovalState == ConsignmentApprovalState.Draft || x.ApprovalState == ConsignmentApprovalState.ReApprove)
                    .ToArray())
                {
                    var initatorOrgId = await userCache.GetOrgId(item.CreatedBy);
                    var supervisorOrgId = await userCache.GetOrgId(User.Identity.Name);


                    if ((User.IsSupervisor()) && initatorOrgId != supervisorOrgId)
                    {
                        shipments.Remove(item);
                    }

                    if ((User.IsInitiator()) && item.CreatedBy != User.Identity.Name)
                    {
                        var creatorRole = await (from u in context.Users
                                                 join ur in context.UserRoles on u.Id equals ur.UserId
                                                 join r in context.Roles on ur.RoleId equals r.Id
                                                 where u.UserName == item.CreatedBy
                                                 select r.Name).FirstOrDefaultAsync();

                        if (creatorRole != BANK_CPC && creatorRole != BANK_CPC_MANAGER)
                            shipments.Remove(item);
                    }

                }


                logger.LogInformation($"totalRows-> {totalRows}, currentRows-> {items.Count}");
                return new IndexViewModel<ConsignmentListViewModel>(shipments, totalRows);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //[HttpGet]
        public async Task<ShipmentFormViewModel> GetAsync(int id)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var order = await (from c in context.Consignments
                               join f in context.Parties on c.FromPartyId equals f.Id
                               join t in context.Parties on c.ToPartyId equals t.Id
                               join b in context.Parties on c.BillBranchId equals b.Id
                               where c.Id == id
                               select new ShipmentFormViewModel()
                               {
                                   Id = c.Id,
                                   ConsignmentNo = c.ShipmentCode,
                                   ManualShipmentCode = c.ManualShipmentCode,
                                   BillBranchId = c.BillBranchId,
                                   FromPartyId = c.FromPartyId,
                                   ToPartyId = c.ToPartyId,
                                   ExchangeRate = c.ExchangeRate,
                                   Valueables = c.Valueables,
                                   AmountPKR = c.AmountPKR,
                                   BillBrannchName = b.ShortName + "-" + b.FormalName,
                                   FromPartyName = f.ShortName + "-" + f.FormalName,
                                   ToPartyName = t.ShortName + "-" + t.FormalName,
                                   Type = c.Type,
                                   CurrencySymbol = c.CurrencySymbol,
                                   Amount = c.Amount,
                                   ServiceType = c.ServiceType,
                                   Comments = c.Comments,
                                   ConsignmentStatus = c.ConsignmentStatus
                               }).FirstAsync();
            order.Comments = order.Comments != null ? JsonConvert.DeserializeObject<List<ShipmentComment>>(order.Comments).FirstOrDefault().Description : null;
            return order;
        }
        //[HttpPost]
        public async Task<int> PostAsync(ShipmentFormViewModel selectedItem)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var newConsignmentSequence = context.Sequences.GetNextCitOrdersSequence();
            var now = MyDateTime.Now;


            var parties = await context.Parties.Where(x => x.Id == selectedItem.BillBranchId || x.Id == selectedItem.FromPartyId || x.Id == selectedItem.ToPartyId)
                .ToListAsync();

            var collectionBranch = parties.FirstOrDefault(x => x.Id == selectedItem.FromPartyId);
            var deliveryBranch = parties.FirstOrDefault(x => x.Id == selectedItem.ToPartyId);

            var IsFinalized = false;
            if (selectedItem.TransactionMode == 2)
            {
                var ReceiverPermissionJson = (from mdr in context.PartyRelationships.Where(x => x.FromPartyId == selectedItem.FromPartyId && x.ToPartyRole == RoleType.ParentOrganization)
                                              join md in context.Parties on mdr.ToPartyId equals md.Id
                                              select md.JsonData).FirstOrDefault();

                if (!string.IsNullOrEmpty(ReceiverPermissionJson))
                {
                    try
                    {
                        var bankSettingFormViewModel = JsonConvert.DeserializeObject<BankSettingFormViewModel>(ReceiverPermissionJson);
                        IsFinalized = bankSettingFormViewModel.SkipQRCodeOnCollection;
                    }
                    catch { }
                }
            }


            var consignment = new Consignment()
            {
                Id = newConsignmentSequence,
                ShipmentCode = $"CIT/{now.Year}/" + newConsignmentSequence.ToString("D4"),
                ManualShipmentCode = selectedItem.ManualShipmentCode,
                ApprovalState = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved,
                CreatedAt = now,
                ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.Created,
                CreatedBy = User.Identity.Name,
                ServiceType = ServiceType.Unknown,
                ShipmentType = ShipmentType.Unknown,
                ConsignmentStatus = ConsignmentStatus.TobePosted,
                Type = selectedItem.Type,
                IsFinalized = IsFinalized || User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN),
                DueTime = now,
                SealedBags = selectedItem.SealedBags
            };

            context.Consignments.Add(consignment);

            //linking Denomination with Consignment for preserving later
            Denomination denom = new()
            {
                Id = context.Sequences.GetNextDenominationSequence(),
                ConsignmentId = consignment.Id,
                DenominationType = DenominationType.Leafs
            };
            context.Denominations.Add(denom);

            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var delivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                CrewId = null,
                ConsignmentId = consignment.Id,
                FromPartyId = selectedItem.FromPartyId,
                ToPartyId = selectedItem.ToPartyId,
                DestinationLocationId = 1,
                PickupLocationId = 1,
                PickupCode = $"{consignment.ShipmentCode}{deliveryId}-Pickup",
                DropoffCode = $"{consignment.ShipmentCode}{deliveryId}-Dropoff"
            };
            context.ConsignmentDeliveries.Add(delivery);

            var consignmentState = new ConsignmentState()
            {
                ConsignmentId = delivery.ConsignmentId,
                DeliveryId = delivery.Id,
                ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.Created,
                TimeStamp = MyDateTime.Now,
                Status = StateTypes.Confirmed,
            };
            context.ConsignmentStates.Add(consignmentState);


            var billingBranch = parties.FirstOrDefault(x => x.Id == selectedItem.BillBranchId);

            consignment.CustomerId = selectedItem.BillBranchId.GetValueOrDefault();

            consignment.FromPartyId = selectedItem.FromPartyId;
            consignment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
            consignment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
            consignment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

            consignment.ToPartyId = selectedItem.ToPartyId;
            consignment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
            consignment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
            consignment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


            consignment.BillBranchId = selectedItem.BillBranchId.GetValueOrDefault();
            consignment.BillingRegionId = billingBranch.RegionId.GetValueOrDefault();
            consignment.BillingSubRegionId = billingBranch.SubregionId.GetValueOrDefault();
            consignment.BillingStationId = billingBranch.StationId.GetValueOrDefault();

            consignment.MainCustomerId = context.PartyRelationships
                .First(x => x.FromPartyId == consignment.CustomerId && x.ToPartyRole == RoleType.ParentOrganization)
                .ToPartyId;

            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            consignment.OriginPartyId = user.PartyId;
            consignment.CounterPartyId = consignment.OriginPartyId == consignment.FromPartyId ? consignment.ToPartyId : consignment.OriginPartyId;

            consignment.CurrencySymbol = selectedItem.CurrencySymbol;
            consignment.Amount = selectedItem.Amount;
            consignment.AmountPKR = selectedItem.AmountPKR;
            consignment.ExchangeRate = selectedItem.ExchangeRate;
            consignment.Valueables = selectedItem.Valueables;
            consignment.ServiceType = selectedItem.ServiceType;
            consignment.ConsignmentStatus = selectedItem.ConsignmentStatus;

            List<ShipmentComment> listOfComments = new List<ShipmentComment>();
            if (consignment.Comments != null)
                listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);

            listOfComments.Add(new ShipmentComment()
            {
                Description = $"Shipment created with "
                + (string.IsNullOrEmpty(selectedItem.Comments) ? "No Comments" : $"Comments {selectedItem.Comments}"),
                CreatedAt = MyDateTime.Now,
                CreatedBy = string.IsNullOrEmpty(user.Name) ? User.Identity.Name : user.Name + " (" + User.Identity.Name + ") ",
                ViewedAt = MyDateTime.Now,
                ViewedBy = User.Identity.Name
            });
            consignment.Comments = JsonConvert.SerializeObject(listOfComments);

            try
            {
                var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                consignment.Distance = distanceInfo.Item1;
                consignment.DistanceStatus = distanceInfo.Item2;

            }
            catch { }
            await context.SaveChangesAsync();

            await PublishUpdates(consignment);
            var notificationService = scope.ServiceProvider.GetService<NotificationService>();
            if (consignment.ApprovalState == ConsignmentApprovalState.Approved && consignment.Type != ShipmentExecutionType.Live)
            {
                var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);
            }
            else if (User.IsInRole(BANK_CPC) || User.IsInRole(BANK_BRANCH))
            {
                var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                notificationIds.ForEach(x => NotificationAgent.WebPushNotificationsQueue.Add(x));
            }

            if (selectedItem.Id > 0)
            {
                await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
            }
            return consignment.Id;
        }

        public async Task<int> CreateCpcShipment(ShipmentFormViewModel selectedItem, int citShipmentId)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                //todo: replace with actual location
                var dummyLocation = context.Locations.First();
                int newConsignmentSequence = 0;

                newConsignmentSequence = context.Sequences.GetNextCitOrdersSequence();
                var now = MyDateTime.Now;
                var consignment = new CPCService()
                {
                    Id = newConsignmentSequence,
                    CitShipmentId = citShipmentId,
                    ShipmentCode = $"CIP/{now.Year}/" + newConsignmentSequence.ToString("D4"),
                    ApprovalState = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved,
                    CreatedAt = now,
                    CPCConsignmentDisposalState = CPCConsignmentDisposalState.NotStarted,
                    CPCConsignmentProcessingState = CPCConsignmentProcessingState.CashAwaited,
                    CreatedBy = User.Identity.Name,
                    IsFinalized = User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN),
                    DueTime = now,
                };

                context.CPCServices.Add(consignment);

                //linking Denomination with Consignment for preserving later
                Denomination denom = new()
                {
                    Id = context.Sequences.GetNextDenominationSequence(),
                    ConsignmentId = consignment.Id,
                    DenominationType = DenominationType.Leafs
                };
                context.Denominations.Add(denom);

                foreach (var item in context.ShipmentChargeType.ToList())
                {
                    context.ShipmentCharges.Add(new ShipmentCharge()
                    {
                        ChargeTypeId = item.Id,
                        ConsignmentId = consignment.Id,
                        Amount = 0,
                        Status = 1
                    });
                }


                var parties = await context.Parties.Where(x => x.Id == selectedItem.FromPartyId || x.Id == selectedItem.ToPartyId)
                    .ToListAsync();

                var collectionBranch = parties.FirstOrDefault(x => x.Id == selectedItem.FromPartyId);
                var deliveryBranch = parties.FirstOrDefault(x => x.Id == selectedItem.ToPartyId);

                consignment.CustomerId = selectedItem.BillBranchId.GetValueOrDefault();

                consignment.FromPartyId = selectedItem.FromPartyId;
                consignment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
                consignment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
                consignment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

                consignment.ToPartyId = selectedItem.ToPartyId;
                consignment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
                consignment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
                consignment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


                consignment.BillBranchId = selectedItem.FromPartyId;
                consignment.BillingRegionId = collectionBranch.RegionId.GetValueOrDefault();
                consignment.BillingSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
                consignment.BillingStationId = collectionBranch.StationId.GetValueOrDefault();

                consignment.MainCustomerId = context.PartyRelationships
                    .First(x => x.FromPartyId == consignment.CustomerId && x.ToPartyRole == RoleType.ParentOrganization)
                    .ToPartyId;

                consignment.CurrencySymbol = selectedItem.CurrencySymbol;
                consignment.AmountByCustomer = selectedItem.Amount;
                consignment.ExchangeRate = selectedItem.ExchangeRate;

                consignment.ConsignmentStatus = Web.Shared.Enums.ConsignmentStatus.TobePosted;
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);


                List<ShipmentComment> listOfComments = new List<ShipmentComment>();
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                if (selectedItem.Comments != null)
                {
                    listOfComments.Add(new ShipmentComment()
                    {
                        Description = selectedItem.Comments,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = string.IsNullOrEmpty(user.Name) ? User.Identity.Name : user.Name + " (" + User.Identity.Name + ") ",
                        ViewedAt = MyDateTime.Now,
                        ViewedBy = User.Identity.Name
                    });

                    consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                }

                await context.SaveChangesAsync();
                var notificationService = scope.ServiceProvider.GetService<NotificationService>();
                if (consignment.ApprovalState == ConsignmentApprovalState.Approved)
                {
                    var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);
                }
                else if (User.IsInRole(BANK_CPC) || User.IsInRole(BANK_BRANCH))
                {
                    var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                    notificationIds.ForEach(x => NotificationAgent.WebPushNotificationsQueue.Add(x));
                }

                if (selectedItem.Id > 0)
                {
                    await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                }

                return consignment.Id;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.Message);
            }
        }
        #endregion

        #region Extra Methods

        //[HttpGet]
        public async Task<ConsignmentListViewModel> GetShipmentFromCache(int id)
        {
            var scope = scopeFactory.CreateScope();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
            return await shipmentsCacheService.GetShipment(id);
        }

        public async Task<int> PostConsignmentDelivery([FromBody] DeliveryFormViewModel deliveryFormViewModel)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Location location = null;

            if (deliveryFormViewModel.LocationId == null)
            {
                if (!string.IsNullOrEmpty(deliveryFormViewModel.LocationName))
                {
                    location = await context.Locations.FirstOrDefaultAsync(x => x.Name.Equals(deliveryFormViewModel.LocationName));
                    if (location != null)
                        throw new BadRequestException("This location is already exist!");

                    location = new Location()
                    {
                        Id = sequenceService.GetNextPartiesSequence(),
                        Name = deliveryFormViewModel.LocationName,
                        Description = deliveryFormViewModel.LocationName,
                        Geolocation = (deliveryFormViewModel.Latitude > 0 || deliveryFormViewModel.Longitude > 0) ?
                        geometryFactory.CreatePoint(new Coordinate(deliveryFormViewModel.Longitude, deliveryFormViewModel.Latitude)) : null,
                        UpdatedAt = DateTime.Now,
                        Type = LocationType.Address,
                        Code = deliveryFormViewModel.LocationName.Substring(0, 3),
                    };
                    context.Locations.Add(location);
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new BadRequestException("Please select location!");
                }
            }
            else
            {
                location = await context.Locations.FirstOrDefaultAsync(x => x.Id == deliveryFormViewModel.LocationId);
            }

            int deliveryId = await AddDelivery(deliveryFormViewModel.ConsignmentId,
                  deliveryFormViewModel.CrewId.GetValueOrDefault(),
                  location.Id,
                  deliveryFormViewModel.PlanedPickupTime.GetValueOrDefault(),
                  deliveryFormViewModel.PlanedPickupTime.GetValueOrDefault());
            await shipmentsCacheService.SetShipment(deliveryFormViewModel.ConsignmentId, null);
            return deliveryId;
        }
        private async Task<int> AddDelivery(int consignmentId, int crewId, int locationId, DateTime? pickupTime, DateTime? dropoffTime)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

            var lastDelivery = await context.ConsignmentDeliveries
                .Include(x => x.Consignment)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId);

            if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                throw new BadRequestException("You need to assign this consignment to one of crews before including additional crews or vault");

            if (lastDelivery.CrewId.GetValueOrDefault() == crewId)
                throw new BadRequestException("You cannot assign two Consective deliveries to same crew or vault");

            //   var dummyLocation = context.Locations.First();
            var deliveryId = context.Sequences.GetNextDeliverySequence();
            var newDelivery = new ConsignmentDelivery()
            {
                Id = deliveryId,
                ParentId = lastDelivery.Id,
                ConsignmentId = consignmentId,
                CrewId = crewId,
                DestinationLocationId = locationId,
                PickupLocationId = locationId,
                FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                ToPartyId = lastDelivery.ToPartyId,
                PlanedPickupTime = pickupTime,
                PickupCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Handover",
                DropoffCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Dropoff",
                DeliveryState = Web.Shared.Enums.ConsignmentDeliveryState.CrewAssigned
            };

            context.ConsignmentDeliveries.Add(newDelivery);

            lastDelivery.ToPartyId = crewId;
            lastDelivery.DestinationLocationId = locationId;
            lastDelivery.PlanedDropTime = dropoffTime;
            lastDelivery.DropoffCode = newDelivery.PickupCode;

            await context.SaveChangesAsync();
            await ResetShipmentCacheAndPublishUpdate(lastDelivery.ConsignmentId);
            var notificationService = scope.ServiceProvider.GetService<NotificationService>();

            await notificationService.CreateFirebaseNotification(lastDelivery.Id, lastDelivery.ConsignmentId, lastDelivery.CrewId,
                lastDelivery.Consignment.ShipmentCode, NotificationType.UpdatedDropoff, NotificationCategory.CIT);

            await notificationService.CreateFirebaseNotification(newDelivery.Id, newDelivery.ConsignmentId, newDelivery.CrewId,
                newDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);
            return deliveryId;
        }

        //[HttpPost]
        public async Task<int> AssignCrew([FromBody] DeliveryCrewFormViewModel selectedItem)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var workOrderService = scope.ServiceProvider.GetRequiredService<ConsignmentService>();
            var notificationService = scope.ServiceProvider.GetService<NotificationService>();
            using (var trx = await context.Database.BeginTransactionAsync())
            {
                try
                {

                    await workOrderService.AssignCrewAsync(selectedItem.DeliveryId, selectedItem.CrewId, true);
                    await workOrderService.LogConsignmentState(selectedItem.ConsignmenId, selectedItem.DeliveryId, Web.Shared.Enums.ConsignmentDeliveryState.CrewAssigned, User.Identity.Name);
                    await trx.CommitAsync();
                    await ResetShipmentCacheAndPublishUpdate(selectedItem.ConsignmenId);

                    var notificationId = await notificationService.CreateCrewAssignedNotification(selectedItem.ConsignmenId, selectedItem.ConsignmenId.ToString());
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId);
                    return 1;
                }
                catch (Exception ex)
                {
                    await trx.RollbackAsync();
                    throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
                }
            }
        }

        private async Task PublishUpdates(Consignment shipment, bool IsUpdate = false)
        {
            var webNotification = new WebNotification()
            {
                Id = shipment.Id,
                FromPartyId = shipment.FromPartyId,
                ToPartyId = shipment.ToPartyId,
                CollectionRegionId = shipment.CollectionRegionId,
                CollectionSubRegionId = shipment.CollectionSubRegionId,
                CollectionStationId = shipment.CollectionStationId,
                DeliveryRegionId = shipment.DeliveryRegionId,
                DeliverySubRegionId = shipment.DeliverySubRegionId,
                DeliveryStationId = shipment.DeliveryStationId,
                Source = IsUpdate ? WebNotification.NotificationSource.OnShipmentUpdated : WebNotification.NotificationSource.OnNewShipment
            };

            PubSub.Hub.Default.PublishAsync(webNotification);
        }


        public async Task GetShipmentFromCachePublishUpdate(int id)
        {
            var scope = scopeFactory.CreateScope();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
            var shipment = await shipmentsCacheService.GetShipment(id);

            await PubSub.Hub.Default.PublishAsync(shipment);
        }

        public async Task ResetShipmentCacheAndPublishUpdate(int id)
        {
            var scope = scopeFactory.CreateScope();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
            var workOrderService = scope.ServiceProvider.GetRequiredService<ConsignmentService>();
            var shipment = await workOrderService.GetShipment(id);
            await shipmentsCacheService.SetShipment(shipment.Id, shipment);

            await PubSub.Hub.Default.PublishAsync(shipment);
        }

        //[HttpGet]
        [AllowAnonymous]
        public IActionResult QueueWebNotification(int id)
        {
            NotificationAgent.WebPushNotificationsQueue.Add(id);
            return null;
        }

        //[HttpGet]
        public async Task<IEnumerable<ShowConsignmentsViewModel>> GetConsignments(int crewId)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var consignments = await (from c in context.Consignments
                                      join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                                      join f in context.Parties on c.FromPartyId equals f.Id
                                      join t in context.Parties on c.ToPartyId equals t.Id
                                      where d.CrewId == crewId
                                      && MyDateTime.Today.Date == c.CreatedAt.Date
                                      && c.ConsignmentStateType < Web.Shared.Enums.ConsignmentDeliveryState.Delivered
                                      select new ShowConsignmentsViewModel()
                                      {
                                          PickupBranch = f.FormalName,
                                          DropoffBranch = t.FormalName,
                                          ConsignmentStatus = c.ConsignmentStateType.ToString(),
                                          Amount = c.Amount,
                                          CreatedAt = c.CreatedAt,
                                          ShipmentCode = c.ShipmentCode
                                      }).ToListAsync();
            return consignments;
        }

        //[HttpGet]
        public async Task<BranchFormViewModel> GetBranchData(int branchId)
        {
            BranchFormViewModel branch = new BranchFormViewModel();
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                branch = await (from p in context.Parties
                                join o in context.Orgnizations on p.Id equals o.Id
                                where p.Id == branchId
                                select new BranchFormViewModel()
                                {
                                    BranchId = p.Id,
                                    ShortName = p.ShortName,
                                    FormalName = p.FormalName,
                                    Address = p.Address,
                                    PersonalContactNo = p.PersonalContactNo,
                                    OfficialContactNo = p.OfficialContactNo,
                                    Latitude = o.Geolocation.Y,
                                    Longitude = o.Geolocation.X,
                                    LocationStatus = o.LocationStatus
                                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation("Exception : " + ex.ToString());
            }
            return branch;
        }

        //[HttpPost]
        public async Task<int> ChangeBranchData(BranchFormViewModel vm)
        {
            try
            {
                if (vm.Latitude.GetValueOrDefault() == 0 || vm.Longitude.GetValueOrDefault() == 0)
                    throw new BadRequestException("Please provide Latitude and Longitude values");
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var partiesCache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();
                var party = await context.Parties.Include(x => x.Orgnization).FirstOrDefaultAsync(x => x.Id == vm.BranchId);
                if (party != null)
                {
                    party.ShortName = vm.ShortName;
                    party.FormalName = vm.FormalName;
                    party.Address = vm.Address;
                    party.PersonalContactNo = vm.PersonalContactNo;
                    party.OfficialContactNo = vm.OfficialContactNo;
                    party.Orgnization.LocationStatus = vm.LocationStatus;

                    if (vm.Latitude > 0 && vm.Longitude > 0)
                    {
                        if (party.Orgnization.Geolocation?.X != vm.Longitude.Value || party.Orgnization.Geolocation?.Y != vm.Latitude.Value)
                        {
                            party.Orgnization.Geolocation = geometryFactory.CreatePoint(new Coordinate(vm.Longitude.Value, vm.Latitude.Value));
                            party.Orgnization.GeolocationUpdateAt = DateTime.Now;
                            party.Orgnization.GeolocationVersion++;

                            await partiesCache.SetGeoCoordinate(party.Id, new Point(party.Orgnization.Geolocation.Y, party.Orgnization.Geolocation.X));

                            // reset the distance status so that it can be recalculated by distance calculation service.
                            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE IntraPartyDistances SET DistanceStatus = '0' WHERE DistanceStatus = '1' AND (FromPartyId = {party.Id} OR ToPartyId = {party.Id})");

                            // update relevent shipments accordingly.
                            await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND (FromPartyId = {party.Id} OR ToPartyId = {party.Id})");

                        }
                    }
                    await context.SaveChangesAsync();
                    await partiesCache.SetGeoStatus(party.Orgnization.Id, vm.LocationStatus);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }
        public async Task<string> ChangeDropoffBranch(ChangeDropoffFormViewModel vm)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var consignment = await context.Consignments.Include(x => x.ConsignmentDeliveries).Where(x => x.Id == vm.ConsignmentId).FirstOrDefaultAsync();
                if (consignment.ToChangedPartyId != null)
                    return "Dropoff can only be changed once";
                if (consignment.FromPartyId == vm.DropoffBranchId)
                    return "Pickup branch and dropoff branch are same";

                consignment.ToChangedPartyId = vm.DropoffBranchId;
                consignment.IsToChangedPartyVerified = false;
                await context.SaveChangesAsync();

                if (User.IsSupervisor())
                {
                    if (consignment.ToChangedPartyId != null)
                    {
                        var newDropoff = await context.Parties.Where(x => x.Id == consignment.ToChangedPartyId).FirstOrDefaultAsync();
                        consignment.ApprovedAt = MyDateTime.Now;

                        consignment.IsToChangedPartyVerified = true;
                        //changing dropoff, making new dropoff and setting it to main dropoff
                        consignment.ToChangedPartyId = consignment.ToPartyId;

                        consignment.ToPartyId = newDropoff.Id;
                        consignment.DeliveryRegionId = newDropoff.RegionId.GetValueOrDefault();
                        consignment.DeliverySubRegionId = newDropoff.SubregionId.GetValueOrDefault();
                        consignment.DeliveryStationId = newDropoff.StationId.GetValueOrDefault();

                        //if bill branch equals to previous dropoff change it to new dropoff
                        if (consignment.BillBranchId == consignment.ToChangedPartyId)
                        {
                            consignment.BillBranchId = newDropoff.Id;
                            consignment.BillingRegionId = newDropoff.RegionId.GetValueOrDefault();
                            consignment.BillingSubRegionId = newDropoff.SubregionId.GetValueOrDefault();
                            consignment.BillingStationId = newDropoff.StationId.GetValueOrDefault();
                        }
                        try
                        {
                            var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                            consignment.Distance += distanceInfo.Item1;
                            consignment.DistanceStatus = distanceInfo.Item2;
                        }
                        catch { }

                        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                        List<ShipmentComment> listOfComments = new();
                        if (consignment.Comments != null)
                            listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);

                        listOfComments.Add(new ShipmentComment()
                        {
                            Description = $"New dropoff to {newDropoff.ShortName}-{newDropoff.FormalName} has been " +
                            (consignment.IsToChangedPartyVerified ? "Approved" : "Rejected") +
                            $" by {User.Identity.Name} at {consignment.ApprovedAt?.ToString("hh:mm tt dd/MM/yyyy")} with No Comments",
                            CreatedAt = MyDateTime.Now,
                            CreatedBy = string.IsNullOrEmpty(user.Name) ? User.Identity.Name : user.Name + " (" + User.Identity.Name + ") ",
                        });

                        consignment.Comments = JsonConvert.SerializeObject(listOfComments);

                        foreach (var item in consignment.ConsignmentDeliveries.Where(x => x.ToPartyId == consignment.ToChangedPartyId))
                        {
                            item.ToPartyId = consignment.ToPartyId;
                        }

                        await context.SaveChangesAsync();
                        await ResetShipmentCacheAndPublishUpdate(consignment.Id);
                        //var cachedShipment = await shipmentsCacheService.GetShipment(consignment.Id);
                        //cachedShipment.Comments = consignment.Comments;
                        //await shipmentsCacheService.SetShipment(cachedShipment.Id, cachedShipment);
                        await GetShipmentFromCachePublishUpdate(consignment.Id);
                        var notificationService = scope.ServiceProvider.GetService<NotificationService>();

                        await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);

                        var notificationId1 = await notificationService.CreateShipmentApprovedNotification(consignment.Id, NotificationType.ShipmentApproved, consignment.ShipmentCode);
                        NotificationAgent.WebPushNotificationsQueue.Add(notificationId1);

                        var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                        NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);

                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
                return ex.Message.ToString();
            }
        }

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetCPCBranches(int id)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var query = await (from p in context.Parties
                               join r in context.PartyRelationships on p.Id equals r.FromPartyId
                               where r.ToPartyId == id &&
                                   r.FromPartyRole == RoleType.ChildOrganization
                                   && r.ToPartyRole == RoleType.BankCPC
                               select new SelectListItem(p.Id, p.ShortName + "-" + p.FormalName)).ToListAsync();
            return query;
        }

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetCrews()
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var crews = await (from o in context.Parties
                               join r in context.PartyRelationships on o.Id equals r.FromPartyId
                               where (r.FromPartyRole == RoleType.Crew)
                               && r.IsActive
                               select new SelectListItem()
                               {
                                   Value = o.Id.ToString(),
                                   Text = o.FormalName
                               }).ToListAsync();

            return crews;
        }

        //[HttpGet]
        public async Task<IEnumerable<CrewWithLocation>> GetCrewsWithLocationMatrix(int consignmentId, bool IsALl = false, string SearchKey = null)
        {

            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var partiesCache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();

            var consignment = await (from c in context.Consignments
                                     join f in context.Parties on c.FromPartyId equals f.Id
                                     join d in context.Parties on c.ToPartyId equals d.Id
                                     where c.Id == consignmentId
                                     select new ConsignmentLocationsViewModel()
                                     {
                                         DeliveryId = 0,
                                         PickupPartyId = c.FromPartyId,
                                         PickupStationId = f.StationId,
                                         DropoffStationId = d.StationId,
                                         FromPartyCode = c.FromParty.ShortName,
                                         ToPartyCode = c.ToParty.ShortName,
                                         DropoffPartyId = c.ToPartyId
                                     }).FirstAsync();

            consignment.PickupPoint = await partiesCache.GetGeoCoordinate(consignment.PickupPartyId);
            consignment.DropoffPoint = await partiesCache.GetGeoCoordinate(consignment.DropoffPartyId);

            var crewsWithCommonDestination = await (from s in context.Consignments
                                                    join d in context.ConsignmentDeliveries on s.Id equals d.ConsignmentId
                                                    where s.DeliveryStationId == consignment.PickupStationId
                                                    && d.CrewId != null
                                                    && s.CreatedAt >= MyDateTime.Today && s.CreatedAt <= MyDateTime.Today.AddDays(1)
                                                    select d.CrewId).ToListAsync();

            var crews = await (from p in context.Parties
                               join o in context.Orgnizations on p.Id equals o.Id
                               join r in context.PartyRelationships on p.Id equals r.FromPartyId
                               where o.OrganizationType == OrganizationType.Crew
                               && r.FromPartyRole == RoleType.Crew
                               && r.IsActive
                               && (string.IsNullOrEmpty(SearchKey) || p.FormalName.Contains(SearchKey))
                                && (IsALl || (p.StationId == consignment.PickupStationId || p.StationId == consignment.DropoffStationId)
                                || crewsWithCommonDestination.Contains(p.Id))
                               select new CrewWithLocation()
                               {
                                   IntValue = p.Id,
                                   Text = p.FormalName,
                                   // CrewLocation = new Point(o.Geolocation.Y, o.Geolocation.X),
                               }).ToListAsync();


            double? distance = null;
            try
            {
                var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.PickupPartyId, consignment.PickupPoint, consignment.DropoffPartyId, consignment.DropoffPoint);
                distance = distanceInfo.Item1;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            var crewIds = crews.Select(x => x.IntValue).ToList();


            var allShipmentsOfPotentialCrews = context.ConsignmentDeliveries
                     .Include(x => x.Consignment).Include(x => x.ToParty.Orgnization)
                     .Where(x => crewIds.Contains(x.CrewId) && x.DeliveryState < Web.Shared.Enums.ConsignmentDeliveryState.Delivered && x.Consignment.DueTime > MyDateTime.Today).ToList();


            foreach (var crew in crews)
            {
                crew.CrewLocation = await partiesCache.GetTemporatlGeoCoordinate(crew.IntValue.GetValueOrDefault());
                crew.ConsignmentDistance = distance.GetValueOrDefault() == 0 ? "Not Available, please check lat/long" : $"{Math.Round(distance.Value / 1000d, 1)} km consignemt distance";
                crew.ConsignmentDistance_ = distance.GetValueOrDefault() == 0 ? double.MaxValue : Math.Round(distance.Value / 1000d, 1);
                crew.PickupLocation = consignment.PickupPoint;
                try
                {
                    var result = CalculateDistanceLocally(crew.CrewLocation, consignment.PickupPoint);
                    crew.PickeupStats = $"{result}Km to reach pickup";
                    crew.PickeupStats_ = result;
                }
                catch (Exception ex)
                {
                    crew.PickeupStats = ex.Message;
                }

                var deliveries = allShipmentsOfPotentialCrews.Where(x => x.CrewId == crew.IntValue).ToList();
                crew.PickupUpConsignments = deliveries.Count == 0 ?
                "**No Shipments**" : $"{deliveries.Sum(x => x.Consignment.Amount) / 1000000}M cash, {deliveries.Count} consignments";

                var distances = new List<double>(deliveries.Count);
                foreach (var d in deliveries)
                {
                    try
                    {
                        var toLocation = new Point(d.ToParty.Orgnization.Geolocation?.Y, (d.ToParty.Orgnization.Geolocation?.X));

                        distances.Add((await GetDistanceOrCalculateUsingGoogle(d.ToPartyId, toLocation, consignment.DropoffPartyId, consignment.DropoffPoint)).Item1);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex.Message);
                    }
                }
                if (distances.Count > 0)
                {
                    crew.ClosetToDropffDistance_ = Math.Round(distances.Min() / 1000d, 1);
                    crew.ClosetToDropffDistance = $"{crew.ClosetToDropffDistance_} kms difference in destinations";

                }
                else
                {
                    crew.ClosetToDropffDistance_ = 0;
                    crew.ClosetToDropffDistance = "This crew has no destinations";
                }
            }

            return crews.OrderBy(x => x.PickeupStats_ == 0).ThenBy(x => x.PickeupStats_);
        }

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetLocations(LocationType? locationType)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var query = (from o in context.Locations

                         select new
                         {
                             Value = o.Id,
                             Text = o.Name,
                             o.Type
                         });

            if (locationType.HasValue)
            {
                query = query.Where(x => x.Type == locationType);
            }

            return await query.Select(x => new SelectListItem()
            {
                Text = x.Text,
                IntValue = x.Value
            }).ToListAsync();
        }

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetSiblingBranches(int id1, int id2)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var results = await (
                               from c in context.PartyRelationships
                               join r in context.PartyRelationships on c.ToPartyId equals r.ToPartyId
                               join p in context.Parties on r.FromPartyId equals p.Id
                               join o in context.Orgnizations on p.Id equals o.Id

                               where o.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
                                 && (id1 == c.FromPartyId || id2 == c.FromPartyId)
                                 && r.ToPartyRole == RoleType.ParentOrganization
                                 && c.ToPartyRole == RoleType.ParentOrganization
                                 && p.IsActive
                               select new SelectListItem(o.Id, p.ShortName, p.FormalName, "-")).Distinct()
             .ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.ToString() + ex.InnerException?.ToString());
            }
        }

        //[HttpPost]
        public async Task<int> AssignVault(VaultNowViewModel viewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var dummyLocation = context.Locations.First();
                await shipmentsCacheService.SetShipment(viewModel.ConsignmentId, null);

                var lastDelivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.ConsignmentId == viewModel.ConsignmentId);

                if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                    throw new BadRequestException("You need to assign this consignment to one of crews before including additional crews or vault");

                if (lastDelivery.CrewId.GetValueOrDefault() == viewModel.VaultId)
                    throw new BadRequestException("You cannot assign two Consective deliveries to same crew or vault");

                //   var dummyLocation = context.Locations.First();
                var deliveryId = context.Sequences.GetNextDeliverySequence();
                var newDelivery = new ConsignmentDelivery()
                {
                    Id = deliveryId,
                    ParentId = lastDelivery.Id,
                    ConsignmentId = viewModel.ConsignmentId,
                    CrewId = viewModel.VaultId,
                    DestinationLocationId = dummyLocation.Id,
                    PickupLocationId = dummyLocation.Id,
                    FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                    ToPartyId = 0,
                    PlanedPickupTime = MyDateTime.Now,
                    PickupCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Handover",
                    DropoffCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Dropoff",
                    DeliveryState = Web.Shared.Enums.ConsignmentDeliveryState.InTransit,
                    IsVault = true
                };

                context.ConsignmentDeliveries.Add(newDelivery);

                lastDelivery.ToPartyId = viewModel.VaultId;
                lastDelivery.DestinationLocationId = dummyLocation.Id;
                lastDelivery.PlanedDropTime = null;
                lastDelivery.DeliveryState = Web.Shared.Enums.ConsignmentDeliveryState.Delivered;
                lastDelivery.DropoffCode = newDelivery.PickupCode;

                lastDelivery.Consignment.ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.InVault;
                lastDelivery.Consignment.VaultInTime = MyDateTime.Now;
                lastDelivery.Consignment.IsVault = true;

                var consignmentState = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == lastDelivery.ConsignmentId
                    && x.DeliveryId == newDelivery.Id
                    && x.ConsignmentStateType == Web.Shared.Enums.ConsignmentDeliveryState.InVault);
                if (consignmentState == null)
                {
                    consignmentState = new ConsignmentState()
                    {
                        DeliveryId = newDelivery.Id,
                        ConsignmentId = newDelivery.ConsignmentId,
                        ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.InVault
                    };
                    context.ConsignmentStates.Add(consignmentState);
                }
                consignmentState.Status = StateTypes.Confirmed;
                consignmentState.TimeStamp = MyDateTime.Now;

                await context.SaveChangesAsync();
                await ResetShipmentCacheAndPublishUpdate(lastDelivery.ConsignmentId);
                var notificationService = scope.ServiceProvider.GetService<NotificationService>();

                await notificationService.CreateFirebaseNotification(lastDelivery.Id, lastDelivery.ConsignmentId, lastDelivery.CrewId,
                    lastDelivery.Consignment.ShipmentCode, NotificationType.Delivered, NotificationCategory.CIT);

                await notificationService.CreateFirebaseNotification(newDelivery.Id, newDelivery.ConsignmentId, newDelivery.CrewId,
                    newDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);

                return deliveryId;


            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
            }
        }

        public async Task<int> VaultOutShipment(DeliveryCrewFormViewModel viewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var dummyLocation = context.Locations.First();

                var lastDelivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.ConsignmentId == viewModel.ConsignmenId);

                if (lastDelivery.CrewId.GetValueOrDefault() == 0)
                    throw new BadRequestException("You need to assign this consignment to one of crews before including additional crews or vault");

                //if (lastDelivery.CrewId.GetValueOrDefault() == viewModel.VaultId)
                //    throw new BadRequestException("You cannot assign two Consective deliveries to same crew or vault");

                //   var dummyLocation = context.Locations.First();
                var deliveryId = context.Sequences.GetNextDeliverySequence();
                var newDelivery = new ConsignmentDelivery()
                {
                    Id = deliveryId,
                    ParentId = lastDelivery.Id,
                    ConsignmentId = viewModel.ConsignmenId,
                    CrewId = viewModel.CrewId,
                    DestinationLocationId = dummyLocation.Id,
                    PickupLocationId = dummyLocation.Id,
                    FromPartyId = lastDelivery.CrewId.GetValueOrDefault(),
                    ToPartyId = lastDelivery.Consignment.ToPartyId,
                    PlanedPickupTime = MyDateTime.Now,
                    PickupCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Handover",
                    DropoffCode = $"{lastDelivery.Consignment.ShipmentCode}{deliveryId}-Dropoff",
                    DeliveryState = Web.Shared.Enums.ConsignmentDeliveryState.InTransit

                };

                context.ConsignmentDeliveries.Add(newDelivery);

                lastDelivery.ToPartyId = viewModel.CrewId.GetValueOrDefault();
                lastDelivery.DestinationLocationId = dummyLocation.Id;
                lastDelivery.PlanedDropTime = null;
                lastDelivery.DeliveryState = Web.Shared.Enums.ConsignmentDeliveryState.Delivered;
                lastDelivery.DropoffCode = newDelivery.PickupCode;

                lastDelivery.Consignment.ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.InTransit;
                lastDelivery.Consignment.VaultOutTime = MyDateTime.Now;
                lastDelivery.Consignment.IsVault = false;

                var consignmentState = context.ConsignmentStates.FirstOrDefault(x => x.ConsignmentId == lastDelivery.ConsignmentId
                && x.DeliveryId == newDelivery.Id
                && x.ConsignmentStateType == Web.Shared.Enums.ConsignmentDeliveryState.InTransit);
                if (consignmentState == null)
                {
                    consignmentState = new ConsignmentState()
                    {
                        DeliveryId = newDelivery.Id,
                        ConsignmentId = newDelivery.ConsignmentId,
                        ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.InTransit
                    };
                    context.ConsignmentStates.Add(consignmentState);
                }
                consignmentState.Status = StateTypes.Confirmed;
                consignmentState.TimeStamp = MyDateTime.Now;

                await context.SaveChangesAsync();
                await ResetShipmentCacheAndPublishUpdate(lastDelivery.ConsignmentId);
                var notificationService = scope.ServiceProvider.GetService<NotificationService>();

                await notificationService.CreateFirebaseNotification(lastDelivery.Id, lastDelivery.ConsignmentId, lastDelivery.CrewId,
                    lastDelivery.Consignment.ShipmentCode, NotificationType.Delivered, NotificationCategory.CIT);

                await notificationService.CreateFirebaseNotification(newDelivery.Id, newDelivery.ConsignmentId, newDelivery.CrewId,
                    newDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);
                return deliveryId;


            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
            }
        }

        private async Task<Tuple<double, DataRecordStatus>> GetDistanceOrCalculateUsingGoogle(int pickupPartyId, int dropoffPartyId)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var branches = await context.Orgnizations.Where(x => x.Id == pickupPartyId || x.Id == dropoffPartyId).ToListAsync();
            var collectionPoint = new Point(branches.First(x => x.Id == pickupPartyId).Geolocation?.Y, branches.First(x => x.Id == pickupPartyId).Geolocation?.X);
            var deliveryPoint = new Point(branches.First(x => x.Id == dropoffPartyId).Geolocation?.Y, branches.First(x => x.Id == pickupPartyId).Geolocation?.X);
            return await GetDistanceOrCalculateUsingGoogle(pickupPartyId, collectionPoint, dropoffPartyId, deliveryPoint);
        }

        private async Task<Tuple<double, DataRecordStatus>> GetDistanceOrCalculateUsingGoogle(int pickupPartyId, Point pickupPoint, int dropoffPartyId, Point dropoffPoint)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var intraPartyDistance = context.IntraPartyDistances
                .OrderByDescending(x => x.DistanceStatus)
                .ThenBy(x => x.UpdateAt)
                .FirstOrDefault(x => x.FromPartyId == pickupPartyId && x.ToPartyId == dropoffPartyId);

            if (intraPartyDistance == null || intraPartyDistance.DistanceStatus < DataRecordStatus.Approved)
            {
                var distance = context.IntraPartyDistances
                  .OrderByDescending(x => x.DistanceStatus)
                  .ThenBy(x => x.UpdateAt)
                  .FirstOrDefault(x => x.FromPartyId == dropoffPartyId && x.ToPartyId == pickupPartyId);
                if (distance != null && distance.DistanceStatus == DataRecordStatus.Approved || intraPartyDistance == null)
                    intraPartyDistance = distance;
            }
            if (intraPartyDistance == null)
            {
                var result = GMapsService.ClaculateDistanceUsinGoogle(pickupPoint, dropoffPoint, string.Empty);

                if (intraPartyDistance == null)
                {
                    intraPartyDistance = new IntraPartyDistance()
                    {
                        FromPartyId = pickupPartyId,
                        ToPartyId = dropoffPartyId
                    };
                    context.IntraPartyDistances.Add(intraPartyDistance);
                }
                intraPartyDistance.AverageTravelTime = (int)result.duration.Value;
                intraPartyDistance.Distance = (int)result.distance.Value;
                intraPartyDistance.UpdatedBy = intraPartyDistance.UpdatedBy == null ? "System" : $"System at {DateTime.Now}_{intraPartyDistance.UpdatedBy}";
                intraPartyDistance.UpdateAt = DateTime.Now;
                await context.SaveChangesAsync();
            }
            return new Tuple<double, DataRecordStatus>(intraPartyDistance.Distance, intraPartyDistance.DistanceStatus);
        }


        protected double CalculateDistanceLocally(Point p1, Point p2)
        {
            if ((p1?.Lat).GetValueOrDefault() == 0 || (p2?.Lat).GetValueOrDefault() == 0)
                throw new InvalidDataException("Geolocation data is not available");

            var d1 = p1?.Lat * (Math.PI / 180.0);
            var num1 = p1?.Lng * (Math.PI / 180.0);
            var d2 = p2?.Lat * (Math.PI / 180.0);
            var num2 = p2?.Lng * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2.Value - d1.Value) / 2.0), 2.0) + Math.Cos(d1.Value) * Math.Cos(d2.Value) * Math.Pow(Math.Sin(num2.Value / 2.0), 2.0);

            return Math.Round((6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)))) / 1000d, 1);
        }

        //[HttpPost]
        public async Task<int> PostConsignmentStatus(ConsignmentStatusViewModel vm)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
            var notificationService = scope.ServiceProvider.GetService<NotificationService>();
            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == vm.ConsignmentId);
            List<ShipmentComment> listOfComments = new();
            try
            {
                if (consignment.Comments != null)
                    listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                if (vm.Comments == null)
                    throw new BadRequestException("Please enter something in commention section to add!");
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                listOfComments.Add(new ShipmentComment()
                {
                    Description = vm.Comments + "," + vm.ConsignmentStatus,
                    CreatedAt = MyDateTime.Now,
                    CreatedBy = string.IsNullOrEmpty(user.Name) ? User.Identity.Name : user.Name + " (" + User.Identity.Name + ") ",
                    ViewedAt = MyDateTime.Now,
                    ViewedBy = User.Identity.Name
                });
                consignment.ConsignmentStatus = vm.ConsignmentStatus;
                consignment.Comments = JsonConvert.SerializeObject(listOfComments);
                await context.SaveChangesAsync();
                await shipmentsCacheService.SetShipment(vm.ConsignmentId, null);
                if (vm.ConsignmentStatus == ConsignmentStatus.Declined || vm.ConsignmentStatus == ConsignmentStatus.Cancelled)
                {
                    var notificationId = await notificationService.CreateShipmentDeclinedNotification(consignment.Id, consignment.ShipmentCode);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId);

                    var lastDelivery = context.ConsignmentDeliveries.Where(x => x.ConsignmentId == consignment.Id && x.CrewId != null).OrderBy(x => x.Id).LastOrDefault();
                    if (lastDelivery != null)
                        await notificationService.CreateFirebaseNotification(lastDelivery.Id, lastDelivery.ConsignmentId, lastDelivery.CrewId,
        consignment.ShipmentCode, vm.ConsignmentStatus == ConsignmentStatus.Declined ? NotificationType.Declined : NotificationType.Cancel, NotificationCategory.CIT);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return consignment.Id;
        }



        public async Task<int> ApproveConsignmentStatus(ConsignmentApprrovalViewModel consignmentStatusViewModel)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
            var notificationService = scope.ServiceProvider.GetService<NotificationService>();
            var consignment = await context.Consignments.Include(x => x.ConsignmentDeliveries).FirstOrDefaultAsync(x => x.Id == consignmentStatusViewModel.ConsignmentId);

            if (!string.IsNullOrEmpty(consignment.ApprovedBy) && consignment.ApprovedBy != User.Identity.Name)
            {
                throw new BadRequestException($"Only {consignment.ApprovedBy} can supervise/authenticate this shipment");
            }

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                if (consignment.ToChangedPartyId != null)
                {
                    var newDropoff = await context.Parties.Where(x => x.Id == consignment.ToChangedPartyId).FirstOrDefaultAsync();
                    consignment.ApprovedAt = MyDateTime.Now;

                    if (consignmentStatusViewModel.ApprovalState == ConsignmentApprovalState.Declined)
                    {
                        consignment.IsToChangedPartyVerified = false;
                        consignment.ToChangedPartyId = null;
                    }
                    else
                    {
                        consignment.IsToChangedPartyVerified = true;
                        //changing dropoff, making new dropoff and setting it to main dropoff
                        consignment.ToChangedPartyId = consignment.ToPartyId;

                        consignment.ToPartyId = newDropoff.Id;
                        consignment.DeliveryRegionId = newDropoff.RegionId.GetValueOrDefault();
                        consignment.DeliverySubRegionId = newDropoff.SubregionId.GetValueOrDefault();
                        consignment.DeliveryStationId = newDropoff.StationId.GetValueOrDefault();

                        //if bill branch equals to previous dropoff change it to new dropoff
                        if (consignment.BillBranchId == consignment.ToChangedPartyId)
                        {
                            consignment.BillBranchId = newDropoff.Id;
                            consignment.BillingRegionId = newDropoff.RegionId.GetValueOrDefault();
                            consignment.BillingSubRegionId = newDropoff.SubregionId.GetValueOrDefault();
                            consignment.BillingStationId = newDropoff.StationId.GetValueOrDefault();
                        }
                        try
                        {
                            var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                            consignment.Distance += distanceInfo.Item1;
                            consignment.DistanceStatus = distanceInfo.Item2;
                        }
                        catch { }

                    }

                    List<ShipmentComment> listOfComments = new();
                    if (consignment.Comments != null)
                        listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);


                    listOfComments.Add(new ShipmentComment()
                    {
                        Description = $"New dropoff to {newDropoff.ShortName}-{newDropoff.FormalName} has been " +
                        (consignment.IsToChangedPartyVerified ? "Approved" : "Rejected") +
                        $" by {User.Identity.Name} at {consignment.ApprovedAt?.ToString("hh:mm tt dd/MM/yyyy")} with "
                        + (string.IsNullOrEmpty(consignmentStatusViewModel.Comments) ? "No Comments" : $"Comments {consignmentStatusViewModel.Comments}"),
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = string.IsNullOrEmpty(user.Name) ? User.Identity.Name : user.Name + " (" + User.Identity.Name + ") ",
                    });

                    consignment.Comments = JsonConvert.SerializeObject(listOfComments);

                    foreach (var item in consignment.ConsignmentDeliveries.Where(x => x.ToPartyId == consignment.ToChangedPartyId))
                    {
                        item.ToPartyId = consignment.ToPartyId;
                    }
                    await context.SaveChangesAsync();
                    await ResetShipmentCacheAndPublishUpdate(consignment.Id);
                    //var cachedShipment = await shipmentsCacheService.GetShipment(consignment.Id);
                    //cachedShipment.Comments = consignment.Comments;
                    //await shipmentsCacheService.SetShipment(cachedShipment.Id, cachedShipment);
                    await GetShipmentFromCachePublishUpdate(consignment.Id);

                    await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);

                }
                else
                {
                    consignment.ApprovalState = consignmentStatusViewModel.ApprovalState;
                    consignment.ApprovedBy = User.Identity.Name;
                    consignment.ApprovedAt = MyDateTime.Now;

                    List<ShipmentComment> listOfComments = new();
                    if (consignment.Comments != null)
                        listOfComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);

                    listOfComments.Add(new ShipmentComment()
                    {
                        Description = $"Approved by {User.Identity.Name} at {consignment.ApprovedAt?.ToString("hh:mm tt dd/MM/yyyy")} with "
                        + (string.IsNullOrEmpty(consignmentStatusViewModel.Comments) ? "No Comments" : $"Comments {consignmentStatusViewModel.Comments}"),
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = string.IsNullOrEmpty(user.Name) ? User.Identity.Name : user.Name + " (" + User.Identity.Name + ") ",
                    });

                    consignment.Comments = JsonConvert.SerializeObject(listOfComments);

                    var rowsEffected = await context.SaveChangesAsync();

                    var cachedShipment = await shipmentsCacheService.GetShipment(consignment.Id);
                    cachedShipment.Comments = consignment.Comments;
                    cachedShipment.ApprovalState = consignment.ApprovalState;
                    cachedShipment.ApprovedBy = consignment.ApprovedBy;
                    await shipmentsCacheService.SetShipment(cachedShipment.Id, cachedShipment);
                    await GetShipmentFromCachePublishUpdate(consignment.Id);

                }
                if (consignment.ApprovalState != ConsignmentApprovalState.Declined)
                {
                    var notificationId1 = await notificationService.CreateShipmentApprovedNotification(consignment.Id, NotificationType.ShipmentApproved, consignment.ShipmentCode, consignment.ToChangedPartyId != null);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId1);
                    var notificationId2 = await notificationService.CreateNewShipmentNotification(consignment.Id, NotificationType.New, consignment.ShipmentCode);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId2);
                }
                else
                {
                    var notificationId1 = await notificationService.CreateShipmentRefusedNotification(consignment.Id, consignment.ShipmentCode, consignment.ToChangedPartyId != null);
                    NotificationAgent.WebPushNotificationsQueue.Add(notificationId1);
                }

                var consignmentDelivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .FirstAsync(x => x.ConsignmentId == consignment.Id);
                if (consignmentDelivery != null && consignmentDelivery.DeliveryState == ConsignmentDeliveryState.CrewAssigned)
                    await notificationService.CreateFirebaseNotification(consignmentDelivery.Id, consignmentDelivery.ConsignmentId, consignmentDelivery.CrewId,
                      consignmentDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return consignment.Id;
        }

        public async Task<DistanceUpdateResult> UpdateShipmentDistance(ShipmentAdministrationViewModel model)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var partiesCache = scope.ServiceProvider.GetRequiredService<PartiesCacheService>();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

            var consignment = await (from c in context.Consignments where c.Id == model.ConsignmentId select c).FirstOrDefaultAsync();
            var distance = await context.IntraPartyDistances
                .FirstOrDefaultAsync(x => (x.FromPartyId == consignment.FromPartyId && x.ToPartyId == consignment.ToPartyId)
                || (x.FromPartyId == consignment.ToPartyId && x.ToPartyId == consignment.FromPartyId));
            if (distance == null)
            {
                distance = new IntraPartyDistance()
                {
                    FromPartyId = consignment.FromPartyId,
                    ToPartyId = consignment.ToPartyId
                };
                context.IntraPartyDistances.Add(distance);
            }
            else
            {
                var distanceHistory = new IntraPartyDistanceHistory()
                {
                    Id = sequenceService.GetNextIntraPartyDistanceSequence(),
                    FromPartyId = distance.FromPartyId,
                    ToPartyId = distance.ToPartyId,
                    CreatedAt = distance.UpdateAt.GetValueOrDefault(),
                    CreatedBy = distance.CreatedBy,
                    UpdatedBy = distance.UpdatedBy,
                    UpdateAt = MyDateTime.Now,
                    DistanceSource = distance.DistanceSource,
                    DistanceStatus = distance.DistanceStatus,
                    Distance = distance.Distance,
                };
                context.IntraPartyDistancesHistory.Add(distanceHistory);

            }

            distance.Distance = model.Distance * 1000;
            distance.DistanceStatus = model.DistanceStatus;
            distance.UpdateAt = DateTime.Now;
            distance.UpdatedBy = User.Identity.Name;
            await context.SaveChangesAsync();
            var distanceUpdateResult = new DistanceUpdateResult
            {
                Repushed = await context.Consignments.FromSqlInterpolated($"Select * FROM Consignments WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))")
                .Select(x => new SelectListItem(x.Id, x.ShipmentCode)).ToListAsync(),

                Updated = await context.Consignments.FromSqlInterpolated($"Select * FROM Consignments WHERE (ConsignmentStatus = '0' OR ConsignmentStatus = '3' OR ConsignmentStatus = '5') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))")
                .Select(x => new SelectListItem(x.Id, x.ShipmentCode)).ToListAsync()
            };

            var v1 = await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET ConsignmentStatus = '97' WHERE (ConsignmentStatus = '9' OR ConsignmentStatus = '17' OR ConsignmentStatus = '33' OR ConsignmentStatus = '128' OR ConsignmentStatus = '640') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))");
            var v2 = await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Consignments SET Distance = {model.Distance * 1000}, DistanceStatus = {(byte)model.DistanceStatus} WHERE (ConsignmentStatus = '0' OR ConsignmentStatus = '3' OR ConsignmentStatus = '5') AND ((FromPartyId = {consignment.FromPartyId} AND ToPartyId = {consignment.ToPartyId}) OR (FromPartyId = {consignment.ToPartyId} AND ToPartyId = {consignment.ToPartyId}))");

            await partiesCache.SetGeoStatus(consignment.Id, model.DistanceStatus);

            foreach (var item in distanceUpdateResult.Updated)
            {
                await shipmentsCacheService.SetShipment(item.IntValue.GetValueOrDefault(), null);
            }

            return distanceUpdateResult;

        }

        //[HttpPost]
        public async Task<int> PostBulkShipments(BulkShipmentsViewModel viewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                for (int i = 0; i < 10; i++)
                {
                    if (viewModel.FromPartyDic.ContainsKey(i) && viewModel.ToPartyDic.ContainsKey(i) && viewModel.BillBranchDic.ContainsKey(i))
                    {
                        if (viewModel.FromPartyDic[i] > 0 && viewModel.ToPartyDic[i] > 0 && viewModel.BillBranchDic[i] > 0)
                        {
                            var consignmentId = sequenceService.GetNextCitOrdersSequence();
                            var now = MyDateTime.Now;
                            var consignment = new Consignment()
                            {
                                Id = consignmentId,
                                CreatedAt = now,
                                DueTime = now,
                                ShipmentCode = $"CIT/{MyDateTime.Now.Year}/" + consignmentId.ToString("D4"),
                                Type = ShipmentExecutionType.Live,
                                ApprovalState = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved,
                                ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.Created,
                                CreatedBy = User.Identity.Name,
                                ServiceType = ServiceType.ByRoad,
                                ShipmentType = ShipmentType.Unknown,
                                ConsignmentStatus = ConsignmentStatus.TobePosted,
                                CustomerId = viewModel.BillBranchDic[i],
                                FromPartyId = viewModel.FromPartyDic[i],
                                ToPartyId = viewModel.ToPartyDic[i],
                                BillBranchId = viewModel.BillBranchDic[i],
                                MainCustomerId = context.PartyRelationships
                                    .First(x => x.FromPartyId == viewModel.BillBranchDic[i] && x.ToPartyRole == RoleType.ParentOrganization)
                                    .ToPartyId,
                                CurrencySymbol = CurrencySymbol.PKR,
                                IsFinalized = User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN)
                            };

                            var parties = await context.Parties.Where(x => x.Id == viewModel.BillBranchDic[i] || x.Id == viewModel.FromPartyDic[i] || x.Id == viewModel.ToPartyDic[i])
                            .ToListAsync();
                            var collectionBranch = parties.FirstOrDefault(x => x.Id == viewModel.FromPartyDic[i]);
                            var deliveryBranch = parties.FirstOrDefault(x => x.Id == viewModel.ToPartyDic[i]);
                            var billingBranch = parties.FirstOrDefault(x => x.Id == viewModel.BillBranchDic[i]);

                            consignment.CustomerId = viewModel.BillBranchDic[i];

                            consignment.FromPartyId = viewModel.FromPartyDic[i];
                            consignment.CollectionRegionId = collectionBranch.RegionId.GetValueOrDefault();
                            consignment.CollectionSubRegionId = collectionBranch.SubregionId.GetValueOrDefault();
                            consignment.CollectionStationId = collectionBranch.StationId.GetValueOrDefault();

                            consignment.ToPartyId = viewModel.ToPartyDic[i];
                            consignment.DeliveryRegionId = deliveryBranch.RegionId.GetValueOrDefault();
                            consignment.DeliverySubRegionId = deliveryBranch.SubregionId.GetValueOrDefault();
                            consignment.DeliveryStationId = deliveryBranch.StationId.GetValueOrDefault();


                            consignment.BillBranchId = viewModel.BillBranchDic[i];
                            consignment.BillingRegionId = billingBranch.RegionId.GetValueOrDefault();
                            consignment.BillingSubRegionId = billingBranch.SubregionId.GetValueOrDefault();
                            consignment.BillingStationId = billingBranch.StationId.GetValueOrDefault();

                            var PickTime = !viewModel.PickupTimeDic.ContainsKey(i) ? null : viewModel.PickupTimeDic[i];
                            var DropoffTime = !viewModel.DropoffTimeDic.ContainsKey(i) ? null : viewModel.DropoffTimeDic[i];

                            consignment.PlanedCollectionTime = PickTime;
                            consignment.PlanedDeliveryTime = DropoffTime;
                            consignment.DueTime = PickTime.HasValue ? PickTime.Value : MyDateTime.Now;

                            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                            if (User.IsInRole(BANK_CPC))
                                consignment.OriginPartyId = consignment.FromPartyId;
                            else
                                consignment.OriginPartyId = user.PartyId;

                            consignment.CounterPartyId = consignment.OriginPartyId == consignment.FromPartyId ? consignment.ToPartyId : consignment.OriginPartyId;

                            consignment.MainCustomerId = context.PartyRelationships
                                .First(x => x.FromPartyId == consignment.CustomerId && x.ToPartyRole == RoleType.ParentOrganization)
                                .ToPartyId;

                            try
                            {
                                var distanceInfo = await GetDistanceOrCalculateUsingGoogle(consignment.FromPartyId, consignment.ToPartyId);
                                consignment.Distance = distanceInfo.Item1;
                                consignment.DistanceStatus = distanceInfo.Item2;

                            }
                            catch { }

                            context.Consignments.Add(consignment);

                            var dinomination = new Denomination()
                            {
                                ConsignmentId = consignmentId,
                                Id = context.Sequences.GetNextDenominationSequence(),
                            };
                            context.Denominations.Add(dinomination);



                            int deliveryId = context.Sequences.GetNextDeliverySequence();
                            var dummyLocation = context.Locations.First();

                            var consignmentDelivery = new ConsignmentDelivery()
                            {
                                FromPartyId = viewModel.FromPartyDic[i],
                                ToPartyId = viewModel.ToPartyDic[i],
                                DestinationLocationId = dummyLocation.Id,
                                PickupLocationId = dummyLocation.Id,
                                ConsignmentId = consignmentId,
                                Id = deliveryId,
                                PlanedPickupTime = PickTime.HasValue ? PickTime.Value : MyDateTime.Now,
                                PlanedDropTime = DropoffTime.HasValue ? DropoffTime.Value : MyDateTime.Now.AddHours(1),
                                PickupCode = $"{consignment.ShipmentCode}{deliveryId}-Pickup",
                                DropoffCode = $"{consignment.ShipmentCode}{deliveryId}-Dropoff"
                            };
                            context.ConsignmentDeliveries.Add(consignmentDelivery);

                            var consignmentState = new ConsignmentState()
                            {
                                ConsignmentId = consignmentId,
                                DeliveryId = deliveryId,
                                ConsignmentStateType = Web.Shared.Enums.ConsignmentDeliveryState.Created,
                                TimeStamp = MyDateTime.Now,
                                Status = StateTypes.Confirmed,
                            };
                            context.ConsignmentStates.Add(consignmentState);


                            foreach (var item in context.ShipmentChargeType.ToList())
                            {
                                context.ShipmentCharges.Add(new ShipmentCharge()
                                {
                                    ChargeTypeId = item.Id,
                                    ConsignmentId = consignmentId,
                                    Amount = 0,
                                    Status = 1
                                });
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return 1;
        }

        //[HttpPost]
        public async Task<int> PostDeliveryTime(DeliveryTimeViewModel deliveryTimeViewModel)
        {
            try
            {
                if (deliveryTimeViewModel.PickupTime.GetValueOrDefault() < MyDateTime.Now
                    || deliveryTimeViewModel.DropOffTime.GetValueOrDefault() < MyDateTime.Now)
                    throw new BadRequestException("You cannot select previous Pickup or Dropoff Time");

                if (deliveryTimeViewModel.PickupTime.GetValueOrDefault() >= deliveryTimeViewModel.DropOffTime.GetValueOrDefault())
                    throw new BadRequestException("DropOff Time should be greater then Pickup Time!");

                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var consignmentDelivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .FirstAsync(x => x.ConsignmentId == deliveryTimeViewModel.ConsignmentId);

                consignmentDelivery.Consignment.PlanedCollectionTime = consignmentDelivery.PlanedPickupTime = deliveryTimeViewModel.PickupTime;
                consignmentDelivery.Consignment.PlanedDeliveryTime = consignmentDelivery.PlanedDropTime = deliveryTimeViewModel.DropOffTime;
                consignmentDelivery.Consignment.DueTime = deliveryTimeViewModel.PickupTime.GetValueOrDefault();
                await shipmentsCacheService.SetShipment(consignmentDelivery.ConsignmentId, null);
                return await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex?.Message + ex?.InnerException?.Message);
            }
        }
        public async Task<string> AssignCrewFromAssetId(int AssetId, int consignmentId)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var crewId = (from o in context.Parties
                              join r in context.PartyRelationships on o.Id equals r.FromPartyId
                              from va in context.AssetAllocations.Where(x => x.PartyId == o.Id).DefaultIfEmpty()
                              orderby o.FormalName

                              where va.AssetId == AssetId && r.FromPartyRole == RoleType.Crew && r.IsActive
                              select o.Id).FirstOrDefault();
                if (crewId == 0) return "No active crew found against this vehicle";

                var consignmentDelivery = await context.ConsignmentDeliveries
                    .Include(x => x.Consignment)
                    .FirstAsync(x => x.ConsignmentId == consignmentId);
                consignmentDelivery.CrewId = crewId;
                consignmentDelivery.DeliveryState = ConsignmentDeliveryState.CrewAssigned;
                consignmentDelivery.Consignment.ConsignmentStateType = ConsignmentDeliveryState.CrewAssigned;
                consignmentDelivery.Consignment.ShipmentType = ShipmentType.Dedicated;
                if (!context.ConsignmentStates.Where(x => x.ConsignmentId == consignmentId && x.ConsignmentStateType == ConsignmentDeliveryState.CrewAssigned).Any())
                {
                    var consignmentState = new ConsignmentState()
                    {
                        ConsignmentId = consignmentId,
                        ConsignmentStateType = ConsignmentDeliveryState.CrewAssigned,
                        TimeStamp = MyDateTime.Now,
                        Status = StateTypes.Confirmed,
                        CreatedBy = User.Identity.Name,
                        DeliveryId = consignmentDelivery.Id,
                    };
                    context.ConsignmentStates.Add(consignmentState);
                }
                await context.SaveChangesAsync();
                await ResetShipmentCacheAndPublishUpdate(consignmentId);
                if (User.IsSupervisor())
                {
                    var notificationService = scope.ServiceProvider.GetService<NotificationService>();
                    await notificationService.CreateFirebaseNotification(consignmentDelivery.Id, consignmentDelivery.ConsignmentId, consignmentDelivery.CrewId,
      consignmentDelivery.Consignment.ShipmentCode, NotificationType.New, NotificationCategory.CIT);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex?.Message + ex?.InnerException?.Message);
            }
        }

        //[HttpGet]
        public async Task<DeliveryTimeViewModel> GetDeliveryTime(int consignmentId)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DeliveryTimeViewModel deliveryTime = null;
            var delivery = await context.ConsignmentDeliveries.FirstOrDefaultAsync(x => x.ConsignmentId == consignmentId);
            if (delivery != null)
            {
                deliveryTime = new DeliveryTimeViewModel()
                {
                    ConsignmentId = consignmentId,
                    PickupTime = delivery.PlanedPickupTime,
                    DropOffTime = delivery.PlanedDropTime,
                };
            }
            return deliveryTime;
        }
        public async Task<List<SelectListItem>> GetDedicatedVehicles(int? consignmentId = null)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            decimal? distance = 0;

            if (consignmentId != null)
            {
                var consignment = await context.Consignments.Include(x => x.ConsignmentDeliveries).Where(x => x.Id == consignmentId).FirstOrDefaultAsync();

                if (consignment.ConsignmentDeliveries.Where(x => x.ConsignmentId == consignmentId && x.CrewId != null).Any())
                    return null;
                if (consignment.Distance > 0)
                    distance = Convert.ToDecimal(consignment.Distance / 1000); // distance in Kms
                else
                    return null; //for un-specified distance between both
            }


            var PartyId = context.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.PartyId).FirstOrDefault();
            var todaysTrips = await context.Consignments.Where(x => x.FromPartyId == PartyId && x.ShipmentType == ShipmentType.Dedicated && x.DueTime.Date == DateTime.Now.Date).CountAsync();

            var organizationCapacity = await context.DedicatedVehiclesCapacities.Where(x => x.OrganizationId == PartyId && x.IsActive && x.VehicleCapacity > 0 && todaysTrips < x.TripPerDay && (distance == null || x.RadiusInKm >= distance) && (((x.ToDate == null || x.ToDate >= MyDateTime.Now) && x.FromDate <= MyDateTime.Now))).FirstOrDefaultAsync();
            if (organizationCapacity == null)
                return null;

            var query = from a in context.AssetAllocations
                        join aset in context.Assets on a.AssetId equals aset.Id
                        where a.PartyId == PartyId && aset.AssetType == AssetType.Vehicle && ((a.AllocatedThru == null || a.AllocatedThru >= MyDateTime.Now) && a.AllocatedFrom <= MyDateTime.Now)
                        orderby a.AllocatedFrom descending
                        select new SelectListItem()
                        {
                            IntValue = aset.Id,
                            Text = aset.Description
                        };
            return query.ToList();
        }
        //[HttpGet]
        public async Task<ShipmentCommentsViewModel> GetComments(int consignmentId)
        {
            ShipmentCommentsViewModel viewModel = new ShipmentCommentsViewModel()
            {
                ConsignmentId = consignmentId,
                ShipmentComments = new List<ShipmentComment>(),
            };

            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == consignmentId);
            if (!string.IsNullOrEmpty(consignment?.Comments))
            {
                try
                {
                    viewModel.ShipmentComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                }
                catch
                {
                    viewModel.ShipmentComments.Add(new ShipmentComment()
                    {
                        CreatedAt = consignment.CreatedAt,
                        CreatedBy = "---",
                        Description = consignment.Comments
                    });
                }
            }
            return viewModel;
        }
        //[HttpPost]
        public async Task<int> PostComment(ShipmentCommentsViewModel viewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == viewModel.ConsignmentId);

                if (consignment?.Comments != null)
                {
                    try
                    {
                        viewModel.ShipmentComments = JsonConvert.DeserializeObject<List<ShipmentComment>>(consignment.Comments);
                    }
                    catch
                    {
                    }
                }

                if (viewModel.CommentText == null)
                    throw new BadRequestException("Please enter something in comment section to add!");

                var Name = context.Users.Where(x => x.UserName == User.Identity.Name).Select(x => x.Name).FirstOrDefault();

                viewModel.ShipmentComments.Add(new ShipmentComment()
                {
                    Description = viewModel.CommentText,
                    CreatedAt = MyDateTime.Now,
                    CreatedBy = string.IsNullOrEmpty(Name) ? User.Identity.Name : Name + " (" + User.Identity.Name + ") ",
                    ViewedAt = MyDateTime.Now,
                    ViewedBy = User.Identity.Name
                });

                consignment.Comments = JsonConvert.SerializeObject(viewModel.ShipmentComments);
                await context.SaveChangesAsync();
                await shipmentsCacheService.SetShipment(consignment.Id, null);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message + ex.InnerException?.ToString());
            }
            return 1;
        }
        #endregion

        //[HttpPost]
        public async Task<int> PostRatingCategories(RatingCategoriesViewModel categoriesViewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                List<Category> categories = await (from c in context.Categories select c).ToListAsync();

                ComplaintCategory complaintCategory = default;
                Common.Data.Models.ComplaintStatus complaintStatus = default;//await context.Complaints.FirstOrDefaultAsync(x => x.ConsignmentId == categoriesViewModel.ConsignmentId);
                List<string> categoryNames = new();

                if (categoriesViewModel.IsBadBehaviour)
                    categoryNames.Add("Bad Behaviour");
                if (categoriesViewModel.IsBadQuality)
                    categoryNames.Add("Bad Quality");
                if (categoriesViewModel.IsShipmentDelayed)
                    categoryNames.Add("Shipment Delayed");

                var complaint = await context.Complaints.FirstOrDefaultAsync(x => x.ConsignmentId == categoriesViewModel.ConsignmentId);
                if (complaint == default)
                {
                    complaint = new()
                    {
                        Id = context.Complaints.OrderByDescending(x => x.Id).FirstOrDefault()?.Id == default ? 1 : context.Complaints.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        ConsignmentId = categoriesViewModel.ConsignmentId,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name
                    };
                    context.Complaints.Add(complaint);
                }
                var complaints = await context.ComplaintCategories.Where(x => categories.Select(x => x.Id).Contains(x.CategoryId)
                && x.ComplaintId == complaint.Id).ToListAsync();
                if (complaints != default)
                {
                    context.ComplaintCategories.RemoveRange(complaints);
                    await context.SaveChangesAsync();
                }
                foreach (var categoryName in categoryNames)
                {
                    var category = categories.FirstOrDefault(x => x.Name == categoryName);
                    complaintCategory = context.ComplaintCategories.FirstOrDefault(x => x.CategoryId == category.Id && x.ComplaintId == complaint.Id);
                    if (complaintCategory == default)
                    {
                        complaintCategory = new()
                        {
                            CategoryId = category.Id,
                            ComplaintId = complaint.Id
                        };
                        context.ComplaintCategories.Add(complaintCategory);
                    }
                }
                complaintStatus = context.ComplaintStatuses.FirstOrDefault(x => x.ComplaintId == complaint.Id);
                if (complaintStatus == default)
                {
                    complaintStatus = new()
                    {
                        Id = context.ComplaintStatuses.OrderByDescending(x => x.Id).FirstOrDefault()?.Id == default ? 1 : context.ComplaintStatuses.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        ComplaintId = complaint.Id,
                        CreatedAt = MyDateTime.Now,
                        CreatedBy = User.Identity.Name,
                    };
                    context.ComplaintStatuses.Add(complaintStatus);
                }

                complaint.Description = categoriesViewModel.Description;
                complaintStatus.Comments = categoriesViewModel.Description;
                complaintStatus.Status = categoriesViewModel.Status;

                Consignment consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == categoriesViewModel.ConsignmentId);
                consignment.Rating = (byte)categoriesViewModel.RatingValue;
                await shipmentsCacheService.SetShipment(consignment.Id, null);
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }
        //[HttpGet]
        public async Task<RatingCategoriesViewModel> GetRatingCategories(int consignmentId)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ratings = await (from c in context.Complaints
                                 join co in context.Consignments on c.ConsignmentId equals co.Id
                                 join s in context.ComplaintStatuses on c.Id equals s.ComplaintId
                                 where c.ConsignmentId == consignmentId
                                 select new RatingCategoriesViewModel()
                                 {
                                     ConsignmentId = consignmentId,
                                     ComplaintId = c.Id,
                                     RatingValue = co.Rating,
                                     Description = c.Description,
                                     Status = s.Status
                                 }).FirstOrDefaultAsync();

            if (ratings != default)
            {
                var categories = await (from cc in context.ComplaintCategories
                                        join cat in context.Categories on cc.CategoryId equals cat.Id
                                        where cc.ComplaintId == ratings.ComplaintId
                                        select new RatingCategoriesViewModel()
                                        {
                                            IsShipmentDelayed = cat.Name == "Shipment Delayed",
                                            IsBadBehaviour = cat.Name == "Bad Behaviour",
                                            IsBadQuality = cat.Name == "Bad Quality"
                                        }).ToListAsync();
                foreach (var cat in categories)
                {
                    if (cat.IsBadBehaviour)
                        ratings.IsBadBehaviour = true;
                    if (cat.IsBadQuality)
                        ratings.IsBadQuality = true;
                    if (cat.IsShipmentDelayed)
                        ratings.IsShipmentDelayed = true;
                }
            }

            return ratings;
        }

        //[HttpPost]
        public async Task<int> PostRatings(RatingControlViewModel ratingControl)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                Consignment consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == ratingControl.ConsignmentId);
                consignment.Rating = (byte)ratingControl.RatingValue;
                await shipmentsCacheService.SetShipment(consignment.Id, null);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return 1;
        }

        public async Task<MixedCurrencyViewModel> GetMixCurrency(int consignmentId)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var mixCurrency = await context.Consignments.Where(x => x.Id == consignmentId).Select(x => new MixedCurrencyViewModel()
                {
                    ConsignmentId = consignmentId,
                    CurrencyType = x.CurrencySymbol.ToString(),
                    Description = x.Valueables,
                    AmountPKR = x.AmountPKR,
                    IsFinalized = x.IsFinalized
                }).FirstOrDefaultAsync();

                return mixCurrency;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        //[HttpPost]
        public async Task<int> UpdateMixCurrency(MixedCurrencyViewModel mixedCurrencyViewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == mixedCurrencyViewModel.ConsignmentId);
                if (consignment != default)
                {
                    var denominationChanged = consignment.Valueables != mixedCurrencyViewModel.Description || consignment.AmountPKR != consignment.Amount;

                    consignment.Valueables = mixedCurrencyViewModel.Description;
                    consignment.AmountPKR = consignment.Amount = mixedCurrencyViewModel.AmountPKR;

                    if (mixedCurrencyViewModel.Finalize)
                    {
                        consignment.IsFinalized = mixedCurrencyViewModel.Finalize;
                        consignment.FinalizedAt = MyDateTime.Now;
                        consignment.FinalizedBy = User.Identity.Name;
                    }
                    await shipmentsCacheService.SetShipment(consignment.Id, null);
                    await context.SaveChangesAsync();


                    if (denominationChanged)
                    {
                        if (consignment.ApprovalState == ConsignmentApprovalState.Approved)
                        {
                            consignment.ApprovalState = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.ReApprove : ConsignmentApprovalState.Approved;
                        }
                        else
                        {
                            consignment.ApprovalState = User.IsInRole(BANK_BRANCH) || User.IsInRole(BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;
                        }
                    }


                    var notificationService = scope.ServiceProvider.GetService<NotificationService>();

                    if (consignment.ApprovalState.HasFlag(ConsignmentApprovalState.Draft))
                    {
                        var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                        notificationIds.ForEach(x => NotificationAgent.WebPushNotificationsQueue.Add(x));
                    }

                    if (consignment.IsFinalized)
                    {
                        await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.ShipmentFinalized, NotificationCategory.CIT);
                    }
                    else
                    {
                        await notificationService.CreateShipmentNotificationForMobile(consignment.Id, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                    }

                    await ResetShipmentCacheAndPublishUpdate(consignment.Id);


                }
                return 1;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        //[HttpGet]
        public async Task<TransitTimeViewModel> GetTransitTime(int consignmentId)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            TransitTimeViewModel transitTimeViewModel = new()
            {
                ListOfTransitTime = new List<TransitTime>(),
                ConsignmentId = consignmentId,
                IsCrewAssigned = context.ConsignmentStates.Where(x => x.ConsignmentStateType == Web.Shared.Enums.ConsignmentDeliveryState.CrewAssigned
                                  && x.Status == StateTypes.Confirmed
                                  && x.ConsignmentId == consignmentId).Any(),
            };

            var times = await (from c in context.Consignments
                               join d in context.ConsignmentDeliveries on c.Id equals d.ConsignmentId
                               join cr in context.Parties on d.CrewId equals cr.Id
                               where c.Id == consignmentId
                               select new TransitTime()
                               {
                                   CrewId = cr.Id,
                                   CrewName = cr.FormalName,
                                   ActualPickupTime = d.ActualPickupTime,
                                   DeliveryState = d.DeliveryState,
                                   DeliveryId = d.Id,
                                   CollectionPoint = new Point((d.CollectionPoint).Y,
                                                            (d.CollectionPoint).X)
                               }).ToListAsync();

            transitTimeViewModel.ListOfTransitTime.AddRange(times);

            //else
            //{
            //transitTimeViewModel.ListOfTransitTime.Add(new TransitTime()
            //{
            //    ActualDropoffTime = DateTime.Now,
            //    DeliveryState = Web.Shared.Enums.ConsignmentState.InTransit,
            //    CrewId = 1,
            //    CrewName = "FirstCrew"
            //});
            //transitTimeViewModel.ListOfTransitTime.Add(new TransitTime()
            //{
            //    ActualDropoffTime = DateTime.Now.AddMinutes(3),
            //    DeliveryState = Web.Shared.Enums.ConsignmentState.ReachedDestination,
            //    CrewId = 2,
            //    CrewName = "SecondCrew"
            //});
            //transitTimeViewModel.ListOfTransitTime.Add(new TransitTime()
            //{
            //    ActualDropoffTime = DateTime.Now.AddMinutes(5),
            //    DeliveryState = Web.Shared.Enums.ConsignmentState.ReachedPickup,
            //    CrewId = 3,
            //    CrewName = "ThirdCrew"
            //});
            //}

            return transitTimeViewModel;
        }


        public async Task<CitDenominationViewModel> GetDenomination(int id)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var denom = await (from d in context.Denominations
                               join c in context.Consignments on d.ConsignmentId equals c.Id
                               where d.ConsignmentId == id //&& d.CashCountType == cashCountType
                               select new CitDenominationViewModel()
                               {
                                   Id = d.Id,
                                   Type = d.DenominationType,
                                   ConsignmentId = d.ConsignmentId,
                                   ShipmentCode = c.ShipmentCode,
                                   Currency1x = d.Currency1x,
                                   Currency2x = d.Currency2x,
                                   Currency5x = d.Currency5x,
                                   Currency10x = d.Currency10x,
                                   Currency20x = d.Currency20x,
                                   Currency50x = d.Currency50x,
                                   Currency75x = d.Currency75x,
                                   Currency100x = d.Currency100x,
                                   Currency500x = d.Currency500x,
                                   Currency1000x = d.Currency1000x,
                                   Currency5000x = d.Currency5000x,

                                   Currency200x = d.Currency200x,
                                   Currency750x = d.Currency750x,
                                   Currency1500x = d.Currency1500x,
                                   Currency7500x = d.Currency7500x,
                                   Currency15000x = d.Currency15000x,
                                   Currency25000x = d.Currency25000x,
                                   Currency40000x = d.Currency40000x,

                                   PrizeMoney100x = d.PrizeMoney100x,
                                   PrizeMoney200x = d.PrizeMoney200x,
                                   PrizeMoney750x = d.PrizeMoney750x,
                                   PrizeMoney1500x = d.PrizeMoney1500x,
                                   PrizeMoney7500x = d.PrizeMoney7500x,
                                   PrizeMoney15000x = d.PrizeMoney15000x,
                                   PrizeMoney25000x = d.PrizeMoney25000x,
                                   PrizeMoney40000x = d.PrizeMoney40000x,
                                   TotalAmount = c.Amount,
                                   AmountPKR = c.AmountPKR,
                                   CurrencySymbol = c.CurrencySymbol,
                                   ExchangeRate = c.ExchangeRate,
                                   Valuables = c.Valueables
                               }).FirstOrDefaultAsync();
            return denom;
        }

        [HttpPost]
        public async Task<int> PostDenomination(CitDenominationViewModel viewModel)
        {
            if (viewModel.ConsignmentId == 0)
            {
                throw new InvalidOperationException("Denomination cannot be tracked without Consignment...");
            }

            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
            if (viewModel.CurrencySymbol == CurrencySymbol.PKR || viewModel.CurrencySymbol == CurrencySymbol.USD || viewModel.CurrencySymbol == CurrencySymbol.EURO)
            {
                viewModel.Valuables = null;
                viewModel.Currency200x = 0;
                viewModel.Currency750x = 0;
                viewModel.Currency1500x = 0;
                viewModel.Currency7500x = 0;
                viewModel.Currency15000x = 0;
                viewModel.Currency25000x = 0;
                viewModel.Currency40000x = 0;

                viewModel.PrizeMoney100x = 0;
                viewModel.PrizeMoney200x = 0;
                viewModel.PrizeMoney750x = 0;
                viewModel.PrizeMoney1500x = 0;
                viewModel.PrizeMoney7500x = 0;
                viewModel.PrizeMoney15000x = 0;
                viewModel.PrizeMoney25000x = 0;
                viewModel.PrizeMoney40000x = 0;
            }
            else if (viewModel.CurrencySymbol == CurrencySymbol.PrizeBond)
            {
                viewModel.Valuables = null;
                viewModel.Currency1x = 0;
                viewModel.Currency2x = 0;
                viewModel.Currency5x = 0;
                viewModel.Currency10x = 0;
                viewModel.Currency20x = 0;
                viewModel.Currency50x = 0;
                viewModel.Currency75x = 0;
                viewModel.Currency500x = 0;
                viewModel.Currency1000x = 0;
                viewModel.Currency5000x = 0;

            }
            else
            {
                viewModel.Type = DenominationType.Leafs;
                viewModel.Currency1x = 0;
                viewModel.Currency2x = 0;
                viewModel.Currency5x = 0;
                viewModel.Currency10x = 0;
                viewModel.Currency20x = 0;
                viewModel.Currency50x = 0;
                viewModel.Currency75x = 0;
                viewModel.Currency100x = 0;
                viewModel.Currency500x = 0;
                viewModel.Currency1000x = 0;
                viewModel.Currency5000x = 0;

                viewModel.Currency200x = 0;
                viewModel.Currency750x = 0;
                viewModel.Currency1500x = 0;
                viewModel.Currency7500x = 0;
                viewModel.Currency15000x = 0;
                viewModel.Currency25000x = 0;
                viewModel.Currency40000x = 0;

                viewModel.PrizeMoney100x = 0;
                viewModel.PrizeMoney200x = 0;
                viewModel.PrizeMoney750x = 0;
                viewModel.PrizeMoney1500x = 0;
                viewModel.PrizeMoney7500x = 0;
                viewModel.PrizeMoney15000x = 0;
                viewModel.PrizeMoney25000x = 0;
                viewModel.PrizeMoney40000x = 0;
            }
            try
            {

                var cpcService = await context.CPCServices.FirstOrDefaultAsync(x => x.CitShipmentId == viewModel.ConsignmentId);
                if (cpcService != null)
                {

                    cpcService.CurrencyByCustomer10x = viewModel.Currency10x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer20x = viewModel.Currency20x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer50x = viewModel.Currency50x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer75x = viewModel.Currency75x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer100x = viewModel.Currency100x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer500x = viewModel.Currency500x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer1000x = viewModel.Currency1000x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer5000x = viewModel.Currency5000x.GetValueOrDefault();

                    cpcService.CurrencyByCustomer200x = viewModel.Currency200x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer750x = viewModel.Currency750x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer1500x = viewModel.Currency1500x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer7500x = viewModel.Currency7500x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer15000x = viewModel.Currency15000x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer25000x = viewModel.Currency25000x.GetValueOrDefault();
                    cpcService.CurrencyByCustomer40000x = viewModel.Currency40000x.GetValueOrDefault();

                    cpcService.PrizeMoney100x = viewModel.PrizeMoney100x.GetValueOrDefault();
                    cpcService.PrizeMoney200x = viewModel.PrizeMoney200x.GetValueOrDefault();
                    cpcService.PrizeMoney750x = viewModel.PrizeMoney750x.GetValueOrDefault();
                    cpcService.PrizeMoney1500x = viewModel.PrizeMoney1500x.GetValueOrDefault();
                    cpcService.PrizeMoney7500x = viewModel.PrizeMoney7500x.GetValueOrDefault();
                    cpcService.PrizeMoney15000x = viewModel.PrizeMoney15000x.GetValueOrDefault();
                    cpcService.PrizeMoney25000x = viewModel.PrizeMoney25000x.GetValueOrDefault();
                    cpcService.PrizeMoney40000x = viewModel.PrizeMoney40000x.GetValueOrDefault();

                    cpcService.AmountByCustomer = viewModel.AmountPKR;
                    cpcService.AmountByCustomer = viewModel.TotalAmount;
                    cpcService.DenominationTypeByCustomer = viewModel.Type;

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }

            Consignment consignment = await context.Consignments.FirstOrDefaultAsync(x => x.Id == viewModel.ConsignmentId);

            if (consignment.IsFinalized && !(User.IsInRole(ADMIN) || User.IsInRole(REGIONAL_ADMIN) || User.IsInRole(SUBREGIONAL_ADMIN)))
            {
                throw new InvalidOperationException("Cannot change demonination once shipment is finalized");
            }

            Denomination denom = null;
            try
            {
                denom = await context.Denominations.FirstOrDefaultAsync(x => x.Id == viewModel.Id);
                if (denom == null)
                {
                    denom = new Denomination()
                    {
                        Id = context.Denominations.Max(x => (int?)x.Id).GetValueOrDefault() + 1,
                        ConsignmentId = viewModel.ConsignmentId
                    };
                }
                bool denominationChanged =
                    (
                 //denom.DenominationType != viewModel.Type ||
                 //denom.Currency1x != viewModel.Currency1x.GetValueOrDefault() ||
                 //denom.Currency2x != viewModel.Currency2x.GetValueOrDefault() ||
                 //denom.Currency5x != viewModel.Currency5x.GetValueOrDefault() ||
                 //denom.Currency10x != viewModel.Currency10x.GetValueOrDefault() ||
                 //denom.Currency20x != viewModel.Currency20x.GetValueOrDefault() ||
                 //denom.Currency50x != viewModel.Currency50x.GetValueOrDefault() ||
                 //denom.Currency100x != viewModel.Currency100x.GetValueOrDefault() ||
                 //denom.Currency500x != viewModel.Currency500x.GetValueOrDefault() ||
                 //denom.Currency1000x != viewModel.Currency1000x.GetValueOrDefault() ||
                 //denom.Currency5000x != viewModel.Currency5000x.GetValueOrDefault() ||
                 consignment.CurrencySymbol != viewModel.CurrencySymbol ||
                //consignment.ExchangeRate != viewModel.ExchangeRate ||
                consignment.Amount != viewModel.TotalAmount);


                denom.DenominationType = viewModel.Type;
                denom.Currency1x = viewModel.Currency1x.GetValueOrDefault();
                denom.Currency2x = viewModel.Currency2x.GetValueOrDefault();
                denom.Currency5x = viewModel.Currency5x.GetValueOrDefault();
                denom.Currency10x = viewModel.Currency10x.GetValueOrDefault();
                denom.Currency20x = viewModel.Currency20x.GetValueOrDefault();
                denom.Currency50x = viewModel.Currency50x.GetValueOrDefault();
                denom.Currency75x = viewModel.Currency75x.GetValueOrDefault();
                denom.Currency100x = viewModel.Currency100x.GetValueOrDefault();
                denom.Currency500x = viewModel.Currency500x.GetValueOrDefault();
                denom.Currency1000x = viewModel.Currency1000x.GetValueOrDefault();
                denom.Currency5000x = viewModel.Currency5000x.GetValueOrDefault();

                denom.Currency200x = viewModel.Currency200x.GetValueOrDefault();
                denom.Currency750x = viewModel.Currency750x.GetValueOrDefault();
                denom.Currency1500x = viewModel.Currency1500x.GetValueOrDefault();
                denom.Currency7500x = viewModel.Currency7500x.GetValueOrDefault();
                denom.Currency15000x = viewModel.Currency15000x.GetValueOrDefault();
                denom.Currency25000x = viewModel.Currency25000x.GetValueOrDefault();
                denom.Currency40000x = viewModel.Currency40000x.GetValueOrDefault();

                denom.PrizeMoney100x = viewModel.PrizeMoney100x.GetValueOrDefault();
                denom.PrizeMoney200x = viewModel.PrizeMoney200x.GetValueOrDefault();
                denom.PrizeMoney750x = viewModel.PrizeMoney750x.GetValueOrDefault();
                denom.PrizeMoney1500x = viewModel.PrizeMoney1500x.GetValueOrDefault();
                denom.PrizeMoney7500x = viewModel.PrizeMoney7500x.GetValueOrDefault();
                denom.PrizeMoney15000x = viewModel.PrizeMoney15000x.GetValueOrDefault();
                denom.PrizeMoney25000x = viewModel.PrizeMoney25000x.GetValueOrDefault();
                denom.PrizeMoney40000x = viewModel.PrizeMoney40000x.GetValueOrDefault();


                consignment.CurrencySymbol = viewModel.CurrencySymbol;
                consignment.ExchangeRate = viewModel.ExchangeRate;
                consignment.IsFinalized = consignment.IsFinalized || viewModel.FinalizeShipment;
                consignment.Valueables = viewModel.Valuables;
                if (denominationChanged)
                {
                    if (consignment.ApprovalState == ConsignmentApprovalState.Approved)
                    {
                        consignment.ApprovalState = User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_CPC) ? ConsignmentApprovalState.ReApprove : ConsignmentApprovalState.Approved;
                    }
                    else
                    {
                        consignment.ApprovalState = User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_CPC) ? ConsignmentApprovalState.Draft : ConsignmentApprovalState.Approved;
                    }
                }

                consignment.Amount = viewModel.TotalAmount;
                consignment.AmountPKR = viewModel.AmountPKR;

                if (consignment.IsFinalized)
                {
                    consignment.FinalizedAt = MyDateTime.Now;
                    consignment.FinalizedBy = User.Identity.Name;
                }
                context.Consignments.Update(consignment);
                await context.SaveChangesAsync();
                var notificationService = scope.ServiceProvider.GetService<NotificationService>();

                if (consignment.ApprovalState.HasFlag(ConsignmentApprovalState.Draft))
                {
                    var notificationIds = await notificationService.CreateApprovalPromptNotification(consignment.Id, NotificationType.ApprovalRequired, consignment.ShipmentCode);
                    notificationIds.ForEach(x => NotificationAgent.WebPushNotificationsQueue.Add(x));
                }

                if (consignment.IsFinalized)
                {
                    await notificationService.CreateShipmentNotificationForMobile(viewModel.ConsignmentId, NotificationType.ShipmentFinalized, NotificationCategory.CIT);
                }
                else
                {
                    await notificationService.CreateShipmentNotificationForMobile(viewModel.ConsignmentId, NotificationType.UpdatedConsignment, NotificationCategory.CIT);
                }

                await ResetShipmentCacheAndPublishUpdate(consignment.Id);
                return denom.Id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }


        }

        public async Task<DeliveryChargesViewModel> GetServiceCharges(int id)
        {
            //Return denominations identified by the given Consignment FK value

            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var charges = await (from d in context.ShipmentCharges
                                 join c in context.Consignments on d.ConsignmentId equals c.Id
                                 where d.ConsignmentId == id
                                 select d).ToArrayAsync();

            DeliveryChargesViewModel chargesVm = charges.ToViewModel(id);
            return (chargesVm);
        }

        public async Task<int> PostServiceCharges(DeliveryChargesViewModel viewModel)
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var shipmentsCacheService = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();

                var charges = context.ShipmentCharges
                    .Where(c => c.ConsignmentId == viewModel.ConsignmentId);

                foreach (var c in charges)
                {
                    c.Amount = (c.ChargeTypeId == 1) ? viewModel.WaitingCharges : c.Amount;
                    c.Amount = (c.ChargeTypeId == 2) ? viewModel.TollCharges : c.Amount;
                }
                await context.SaveChangesAsync();
                await shipmentsCacheService.SetShipment(viewModel.ConsignmentId, null);
                //return Delivery ID for which charges are updated
                return (viewModel.ConsignmentId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<IndexViewModel<CrewMemberListModel>> GetCrewMembers(int crewId)
        {
            if (crewId == 0)
                throw new BadRequestException("Please select Crew"); //NoContent();

            var context = scopeFactory.CreateScope().ServiceProvider.GetService<AppDbContext>();
            var query = (from r in context.PartyRelationships
                         join p in context.Parties on r.FromPartyId equals p.Id
                         join e in context.People on p.Id equals e.Id
                         where r.ToPartyId == crewId
                         select new CrewMemberListModel()
                         {
                             CrewId = r.ToPartyId,
                             RelationshipType = r.FromPartyRole,
                             EmployeeName = p.FormalName,
                             EmployeeCode = p.ShortName,
                             EmployeeId = p.Id,
                             ImageLink = p.ImageLink,
                             StartDate = r.StartDate,
                             EndDate = r.ThruDate,
                             Id = r.Id,
                             IsActive = r.IsActive,
                             NationalId = e.NationalId,

                             CheckinTime = context.EmployeeAttendance.FirstOrDefault(x => x.RelationshipId == r.Id
                             && r.IsActive && (x.AttendanceDate.Date.Equals(MyDateTime.Now.Date) && x.AttendanceState == AttendanceState.Present)).CheckinTime,

                             CheckoutTime = context.EmployeeAttendance.FirstOrDefault(x => x.RelationshipId == r.Id
                             && r.IsActive && (x.AttendanceDate.Date.Equals(MyDateTime.Now.Date) && x.AttendanceState == AttendanceState.Present)).CheckoutTime
                         });


            query = query.Where(x => x.IsActive);


            var totalRows = query.Count();

            var items = await query.ToArrayAsync();

            return new IndexViewModel<CrewMemberListModel>(items, totalRows);
        }

        public async Task<string> PostSealCodesAndNoOfBags(List<Seal> sealCodes, int ConsignmentId, int NoOfBags)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                var existingCodes = context.ShipmentSealCodes.Where(x => x.ConsignmentId == ConsignmentId).Select(x => x.SealCode).ToArray();
                var codes = from sealcode in sealCodes
                            where !existingCodes.Contains(sealcode.SealCode)
                            select new ShipmentSealCode
                            {
                                ConsignmentId = ConsignmentId,
                                SealCode = sealcode.SealCode,
                                CreatedAt = MyDateTime.Now,
                                CreatedBy = User.Identity.Name
                            };
                if (codes.Any())
                {
                    await context.ShipmentSealCodes.AddRangeAsync(codes);
                    await context.SaveChangesAsync();
                    var shipmentsCache = scope.ServiceProvider.GetRequiredService<ShipmentsCacheService>();
                    var cachedShipment = await shipmentsCache.GetShipment(ConsignmentId);
                    if (cachedShipment != null)
                    {
                        cachedShipment.SealCodes = context.ShipmentSealCodes.Where(x => x.ConsignmentId == cachedShipment.Id).Select(x => x.SealCode).ToList();
                        await shipmentsCache.SetShipment(cachedShipment.Id, cachedShipment);
                    }
                }

                var consignment = await context.Consignments.Where(x => x.Id == ConsignmentId).FirstOrDefaultAsync();
                consignment.NoOfBags = NoOfBags;
                await context.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

}
