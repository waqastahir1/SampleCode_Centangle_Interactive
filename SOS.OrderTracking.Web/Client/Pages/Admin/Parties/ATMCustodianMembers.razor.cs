using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class ATMCustodianMembers
    {
        private bool State { get; set; }
        private bool InverseState()
        {
            State = !State;
            return State;
        }
        private int _id;

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

        private IEnumerable<SelectListItem> People { get; set; }

        public RelationshipDetailViewModel RelationshipDetailViewModel { get; private set; }
        public ATMCustodianMembersOperationViewModel ATMCustodianMembersOperationViewModel { get; set; }

        public ATMCustodianMembers()
        {
            OnFilterApplied += async () =>
            {
                People = await ApiService.GetPeople(OrganizationalUnit.RegionId.GetValueOrDefault(), OrganizationalUnit.SubRegionId, OrganizationalUnit.StationId);
                //Items = Array.Empty<AtmCustodianMembersListViewModel>();
            };
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(Id) && Id > 0)
                {
                    AdditionalParams = $"&ATMId={Id}";
                    Logger.LogInformation($"ATMId = {Id}");
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());
                }
            };

            OnFormSubmitted = (id) =>
            {
                PubSub.Hub.Default.Publish(this);
            };
        }

        protected override async Task OnInitializedAsync()
        {
            People = await ApiService.GetPeople(OrganizationalUnit.RegionId.GetValueOrDefault(), OrganizationalUnit.SubRegionId, OrganizationalUnit.StationId);
            await base.OnInitializedAsync();
        }

        //protected override CrewMemberViewModel CreateSelectedItem()
        //{
        //    return new CrewMemberViewModel()
        //    {
        //        ATMId = Id
        //    };
        //}

        //protected async Task ShowConformation()
        //{
        //    RelationshipDetailViewModel = await ApiService.GetRelationshipDetail(SelectedItem.EmployeeId, SelectedItem.StartDate);//GetFromJsonAsync<RelationshipDetailViewModel>($"v1/Common/GetRelationshipDetail?employeeId={SelectedItem.EmployeeId}&startDate={SelectedItem.StartDate?.ToString("o")}");

        //    if (RelationshipDetailViewModel == null)
        //    {
        //        RelationshipDetailViewModel = new RelationshipDetailViewModel()
        //        {
        //            OrganizationName = "...."
        //        };
        //    }
        //}
        private async Task OnRemoveItemClicked(int id)
        {
           // Error = null;
            ATMCustodianMembersOperationViewModel = await ApiService.GetMemberDetail(id);
            //AdditionalParams = $"&ATMId={Id}";
            //Logger.LogInformation($"ATMId = {Id}");
            //await LoadItems(true);
            //await InvokeAsync(() => StateHasChanged());
        }
        private async Task RemoveCrewMember()
        {
           // Error = null;
            var response = await ApiService.RemoveATMMember(ATMCustodianMembersOperationViewModel);
            if (response > 0)
            {
                ATMCustodianMembersOperationViewModel = null;
                await LoadItems(true);
            }
        }
    }

}