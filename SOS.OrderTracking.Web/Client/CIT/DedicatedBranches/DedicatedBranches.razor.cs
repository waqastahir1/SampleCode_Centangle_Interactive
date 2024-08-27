using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.CIT.DedicatedBranches
{
    public partial class DedicatedBranches
    {
        public int PartyId { get; set; } // PartyID represents the Branch
        private int BranchId { get; set; }
        public string Branch_Code { get; set; } = string.Empty;
        public string Branch_Name { get; set; } = string.Empty;
        public byte? DedicatedVehicleCapacity { get; set; }
        private IEnumerable<SelectListItem> MainCustomers { get; set; }
        private int _mainCustomerId;
        public int MainCustomerId
        {
            get { return _mainCustomerId; }
            set
            {
                _mainCustomerId = value;
                NotifyPropertyChanged();
            }
        }
        void SetParametersOfBranch(BranchesListViewModel model)
        {
            PartyId = model.Id;
            Branch_Code = model.BranchCode;
            Branch_Name = model.BranchName;
            BranchId = model.Id;
            DedicatedVehicleCapacity = model.DedicatedVehicleCapacity;
        }
        public DedicatedBranches()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(MainCustomerId))
                {
                    AdditionalParams = $"&MainCustomerId={MainCustomerId}";
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());
                }
            };

            PubSub.Hub.Default.Subscribe<DedicatedVehicles>(this, async p =>
            {
                if (p == null)
                {
                    BranchId = 0;
                    await InvokeAsync(() => StateHasChanged());
                }
                else
                {
                    await LoadItems(true);
                }
            });

        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            MainCustomers = await ApiService.ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>("v1/Organization/GetMainCustomers");

        }
    }
}
