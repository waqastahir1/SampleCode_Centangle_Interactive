using Microsoft.AspNetCore.Components;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Vault
{
    public partial class VaultMembers
    {
        private bool State { get; set; }
        private bool InverseState()
        {
            State = !State;
            return State;
        }
        private int _id { get; set; }
        [Parameter]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }
        [Parameter]
        public string vaultName { get; set; }
        [Inject]
      
        IEnumerable<SelectListItem> People { get; set; }
        public RelationshipDetailViewModel RelationshipDetailViewModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AdditionalParams = $"&vaultId={Id}";
            People = await ApiService.GetPotentialVaultMembers(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId, BaseIndexModel.StationId);
            await base.OnInitializedAsync();
        }
        public VaultMembers()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(Id) && Id > 0)
                {
                    AdditionalParams = $"&vaultId={Id}";
                    await LoadItems(true);
                }
            };
        }
        protected override VaultMembersViewModel CreateSelectedItem()
        {
            return new VaultMembersViewModel()
            {
                VaultId = Id
            };
        }
        protected async Task ShowConformation()
        {
            RelationshipDetailViewModel = await ApiService.GetRelationshipDetail(SelectedItem.EmployeeId, SelectedItem.StartDate);//GetRelationshipDetail?employeeId={SelectedItem.EmployeeId}&startDate={SelectedItem.StartDate?.ToString("o")}");

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
