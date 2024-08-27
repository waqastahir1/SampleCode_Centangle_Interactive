using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Users
{
    public partial class Employees
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
