using SOS.OrderTracking.Web.Client.Components;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class Customers
    {
        public override string ApiControllerName => "Customers";

        public IEnumerable<SelectListItem> Areas { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        public IEnumerable<SelectListItem> OrganizationTypes { get; set; }


        public IEnumerable<SelectListItem> ParentOrganizations { get; set; }

        public IEnumerable<SelectListItem> AssociatedBankCPCs { get; set; }

        public IEnumerable<SelectListItem> AssociatedSOSCPCs { get; set; }

        public Customers()
        {
            OrganizationTypes = new SelectListItem[]
            {
                new SelectListItem((int)OrganizationType.Customer, "Customer") ,
                new SelectListItem( (int)OrganizationType.CustomerBranch, "Customer Branch")
            };

            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(SelectedItem.RegionId))
                {
                    
                    Areas = await Http.GetFromJsonAsync<SelectListItem[]>("v1/locations/getchildren");
                    AssociatedBankCPCs = await Http.GetFromJsonAsync<SelectListItem[]>(
                        $"v1/customers/getAssociatedBankCPCs?regionId={SelectedItem.RegionId}&parentId={SelectedItem.ParentId}");
                    AssociatedSOSCPCs = await Http.GetFromJsonAsync<SelectListItem[]>(
                        $"v1/customers/getAssociatedSOSCPCs?regionId={SelectedItem.RegionId}");
                }
            };
        }

        protected override async Task OnInitializedAsync()
        {
            // Abdul Basir to takeup the APIs and integration
            //Areas = await Http.GetFromJsonAsync<SelectListItem[]>("locations/getbanks");
            //ParentOrganizations = await Http.GetFromJsonAsync<SelectListItem[]>("customers/get");
            await base.OnInitializedAsync();
        }

    }
}
