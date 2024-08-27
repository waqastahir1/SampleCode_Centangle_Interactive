using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.AdditionalRequests
{
    public partial class PendingRequest
    {
        public override string ApiControllerName => "PendingRequest";
        private IEnumerable<SelectListItem> AllRegions;
        protected override async Task OnInitializedAsync()
        {
            AllRegions = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>($"v1/{ApiControllerName}/GetRegions");
            await base.OnInitializedAsync();
        }
        private async Task SelectRequest(ChangeEventArgs e)
        {
            try
            {
                AdditionalParams = $"&regionId={Convert.ToInt32(e.Value)}";
                await LoadItems();
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

        }
    }
}

