using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.AppointHeads
{
    public partial class RegionalHead
    {
        [Inject]
        public OrganizationalUnitsService OrganizationalUnitsService { get; set; }
        public RelationshipDetailViewModel RelationshipDetailViewModel { get; set; }
        public bool changeRelationship { get; set; }

        private IEnumerable<SelectListItem> Employees { get; set; }
        private IEnumerable<SelectListItem> Regions { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Employees = await ApiService.GetRegularEmployeesAsync();
            Regions = await OrganizationalUnitsService.GetRegionsAsync();
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

    }
}
