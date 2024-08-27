using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.CIT.Crew
{
    public partial class CrewMembers
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

        [Parameter]
        public string crewName { get; set; }

        private IEnumerable<SelectListItem> People { get; set; }
        private CrewMemberExistViewModel CrewMemberExistViewModel { get; set; }
        private CrewMemberOperationsViewModel CrewMemberOperationsViewModel { get; set; }

        public RelationshipDetailViewModel RelationshipDetailViewModel { get; private set; }

        public CrewMembers()
        {
            OnFilterApplied += async () =>
            {
                People = await ApiService.GetPotentialCrewMembers(OrganizationalUnit.RegionId.GetValueOrDefault(), OrganizationalUnit.SubRegionId, OrganizationalUnit.StationId);
                Items = Array.Empty<CrewMemberListModel>();
            };
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(Id) && Id > 0)
                {
                    AdditionalParams = $"&CrewId={Id}";
                    Logger.LogInformation($"CrewId = {Id}");
                    await LoadItems(true);
                    await InvokeAsync(()=> StateHasChanged());
                }
            };

            OnFormSubmitted = (id) =>
            {
                PubSub.Hub.Default.Publish(this);
            };
        }

        protected override async Task OnInitializedAsync()
        {
            People = await ApiService.GetPotentialCrewMembers(OrganizationalUnit.RegionId.GetValueOrDefault(), OrganizationalUnit.SubRegionId, OrganizationalUnit.StationId);
            await base.OnInitializedAsync();
        }

        protected override CrewMemberViewModel CreateSelectedItem()
        {
            return new CrewMemberViewModel()
            {
                CrewId = Id
            };
        }

        protected async Task ShowConformation()
        {
            if (SelectedItem.Id == 0)
            {
                RelationshipDetailViewModel = await ApiService.GetRelationshipDetail(SelectedItem.EmployeeId, SelectedItem.StartDate);//GetFromJsonAsync<RelationshipDetailViewModel>($"v1/Common/GetRelationshipDetail?employeeId={SelectedItem.EmployeeId}&startDate={SelectedItem.StartDate?.ToString("o")}");

                if (RelationshipDetailViewModel == null)
                {
                    RelationshipDetailViewModel = new RelationshipDetailViewModel()
                    {
                        OrganizationName = "...."
                    };
                }
            }
            else
            {
                await OnFormSubmit();
            }
        }
        private async Task OnRemoveItemClicked(int id)
        {
            Error = null;
            CrewMemberOperationsViewModel = await ApiService.GetMemberDetail(id);
        }
        private async Task RemoveCrewMember()
        {
            Error = null;
            var response = await ApiService.RemoveCrewMember(CrewMemberOperationsViewModel);
            if (response > 0)
            {
                CrewMemberOperationsViewModel = null;
                await LoadItems(true);
            }
        }
    }

}
