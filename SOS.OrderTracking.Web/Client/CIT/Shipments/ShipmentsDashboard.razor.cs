using Microsoft.AspNetCore.Components;
using SOS.OrderTracking.Web.Client.Pages.Customer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SOS.OrderTracking.Web.Client.CIT.Shipments
{
    public partial class ShipmentsDashboard
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Parameter]
        public int Type { get; set; }

        public string ShipmentCode { get; set; }
          
        protected override Task OnInitializedAsync()
        { 
            var querystring = NavManager.QueryString();

            // get id from query string
            ShipmentCode = querystring["ShipmentCode"];

            return base.OnInitializedAsync();
        }

     
    }
    public static class ExtensionMethods
    {
        // get entire querystring name/value collection
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        // get single querystring value with specified key
        public static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }
    }

}
