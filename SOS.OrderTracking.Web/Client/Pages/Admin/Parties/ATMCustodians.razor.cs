using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class ATMCustodians
    {

        public int AtmId { get; set; }
        public ATMCustodians()
        {

            PubSub.Hub.Default.Subscribe<ATMCustodianMembers>(this, async p =>
            {
                if (p == null)
                {
                    AtmId = 0;
                    await InvokeAsync(() => StateHasChanged());
                }
                else
                {
                    await LoadItems(true);
                }
            });
        }
    }
}
