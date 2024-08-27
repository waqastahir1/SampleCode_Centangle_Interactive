using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class ATMRServices
    {
        public override string ApiControllerName => "ATMRServices";
         

        public IEnumerable<SelectListItem> Atms
        {
            get; set;
        }

        public IEnumerable<SelectListItem> CashSources
        {
            get; set;
        }
        public ATMRServices()
        {
            OnSelectedItemCreated = async (selectedItem) =>
            {
                if (selectedItem.ATMId > 0)
                {
                    await UpdateAtmBranches(selectedItem);
                }

                CashSources = new List<SelectListItem>(0);
                selectedItem.PropertyChanged += async (p, q) =>
                {
                    if (q.PropertyName == nameof(SelectedItem.ATMId))
                    {
                        await UpdateAtmBranches(selectedItem);
                    }
                };
            };
        }

        protected override async Task OnInitializedAsync()
        {  
            Atms = await ApiService.GetFromJsonAsync<List<SelectListItem>>
                ($"v1/organization/getatms");
            
            CashSources = new List<SelectListItem>(0);
          
           await base.OnInitializedAsync();
        }
         
        private async Task UpdateAtmBranches(ATMServiceFormViewModel selectedItem)
        {

            CashSources = await ApiService.GetFromJsonAsync<List<SelectListItem>>(
                $"v1/organization/getatmbranch?id={SelectedItem.ATMId}");

            selectedItem.CashSourceBranchId = CashSources.FirstOrDefault()?.IntValue;
            await InvokeAsync(StateHasChanged);
        }

        private async Task EditViewModel(int consignmentId)
        {
            await OnItemClicked(consignmentId);
        }

    }
}