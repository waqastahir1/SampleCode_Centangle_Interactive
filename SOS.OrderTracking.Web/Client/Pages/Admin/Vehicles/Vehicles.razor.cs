using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Vehicles
{
    public partial class Vehicles
    {
        public async Task SearchString(string searchKey)
        {
            AdditionalParams = $"&searchKey={searchKey}";
            await LoadItems();
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
    }
}
