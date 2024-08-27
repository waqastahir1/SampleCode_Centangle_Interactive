
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class CPCConsignments
    {
        public override string ApiControllerName => "CPCConsignments";

        public string FormTitle => "Create CPC Consignment";

        public int CustomerId
        {
            get
            {
                return SelectedItem.CustomerId;
            }
            set
            {
                SelectedItem.CustomerId = value;
                Branches = BranchesCache.Where(x => x.ParentId == value);
            }
        }

        public IEnumerable<OrganizationModel> Customers
        {
            get; set;
        }

        public IEnumerable<OrganizationModel> Branches
        {
            get; set;
        }

        public IEnumerable<OrganizationModel> BranchesCache
        {
            get; set;
        }

        public CitDenominationViewModel CitDenominationViewModel
        {
            get; set;
        }

        public DeliveryChargesViewModel DeliveryChargesModel
        {
            get; set;
        }

        protected override async Task OnInitializedAsync()
        {
            Customers = await Http.GetFromJsonAsync<List<OrganizationModel>>
            ($"v1/organization/getbanks");

            BranchesCache = await Http.GetFromJsonAsync<List<OrganizationModel>>
                ($"v1/organization/getbranches");

            Branches = new List<OrganizationModel>();

            await base.OnInitializedAsync();
        }

        private async Task OnDenominationClicked(int consignmentid)
        {
            CitDenominationViewModel = await Http.GetFromJsonAsync<CitDenominationViewModel>
              ($"v1/Denomination/get?id={consignmentid}");
        }

        private async Task SaveDenomination()
        {
            try
            {
                CitDenominationViewModel = null;
            }
            catch (Exception ex) { }
        }

        private async Task SaveCharges()
        {
            try
            {
                DeliveryChargesModel = null;
            }
            catch (Exception ex) { }
        }
    }
}
