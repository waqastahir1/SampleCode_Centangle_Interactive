using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Components;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class ManageGaurds
    {
        private IEnumerable<SelectListItem> Banks { get; set; }
        private IEnumerable<SelectListItem> AllGaurds { get; set; }
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
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        private async Task MainCustomerValue(int id)
        {
            try
            {
                AdditionalParams = $"&MainCustomerId={id}";
                await LoadItems(true);
                //await InvokeAsync(() => {
                //    StateHasChanged();
                //});
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }
        protected override async Task OnInitializedAsync()
        {
           // Banks = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
           //($"v1/{ApiControllerName}/GetCustomers");
           // AllGaurds = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
           //($"v1/{ApiControllerName}/GetAllGaurds");
           // await base.OnInitializedAsync();
        }
        public ManageGaurds()
        {
            PropertyChanged += async (p, q) =>
            {
                //if (q.PropertyName == nameof(MainCustomerId))
                //{
                  
                //        try
                //        {
                //            AdditionalParams = $"&MainCustomerId={MainCustomerId}";
                //            await LoadItems();
                //            await InvokeAsync(() => {
                //                StateHasChanged();
                //            });
                //        }
                //        catch (Exception ex)
                //        {
                //            Logger.LogError(ex.ToString());
                //        }
                    
                //}
            };
            PubSub.Hub.Default.Subscribe<Gaurd>(this, async p =>
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

        public async void SelectGaurds(ChangeEventArgs e)
        {
            AdditionalParams = $"&MainCustomerId={MainCustomerId}";
            await LoadItems();
            this.StateHasChanged();
        }
    }
}
