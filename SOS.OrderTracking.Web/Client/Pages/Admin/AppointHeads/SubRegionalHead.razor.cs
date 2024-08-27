using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.AppointHeads
{
    public partial class SubRegionalHead
    {
        public SubRegionalHead()
        {
            OnSelectedItemCreated += (i) =>
            {
                SelectedItem.PropertyChanged += async (p, q) =>
                {
                    try
                    {
                        if (q.PropertyName == nameof(SelectedItem.RegionId))
                        {
                            SubRegions = await OrganizationalUnitsService.GetSubRegionsAsync(SelectedItem.RegionId);
                            await InvokeAsync(() => StateHasChanged());
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }
                };
            };
        }
        [Inject]
        public OrganizationalUnitsService OrganizationalUnitsService { get; set; }

        IEnumerable<SelectListItem> Employees { get; set; }
        IEnumerable<SelectListItem> Regions { get; set; }
        private IEnumerable<SelectListItem> SubRegions { get; set; }
        public RelationshipDetailViewModel RelationshipDetailViewModel { get; set; }
        public bool changeRelationship { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Employees = await ApiService.GetRegularEmployeesAsync();
            Regions = await OrganizationalUnitsService.GetRegionsAsync();
            SubRegions = await OrganizationalUnitsService.GetSubRegionsAsync();
           
            await base.OnInitializedAsync();
        }
        public async Task SearchString(string searchKey)
        {
            AdditionalParams = $"&searchKey={searchKey}";
            await LoadItems();
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        protected async Task ShowConformation()
        {
            RelationshipDetailViewModel = await ApiService.GetRelationshipDetail(SelectedItem.EmployeeId,SelectedItem.StartDate);

            if (RelationshipDetailViewModel == null)
            {
                RelationshipDetailViewModel = new RelationshipDetailViewModel()
                {
                    OrganizationName = "...."
                };
            }
        }
        public async Task SearchValues(string val)
        {
            AdditionalParams = $"&searchKey={val}";
            await LoadItems();
        }
    }
}
