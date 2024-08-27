using Microsoft.AspNetCore.Components;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Portal.Pages.CIT.Shipments.Forms
{
    public partial class ShipmentForm
    {
        [Parameter]
        public ShipmentFormViewModel SelectedItem { get; set; }

        [Parameter]
        public OrganizationUitViewModel OrganizationalUnit { get; set; }

        [Parameter]
        public IEnumerable<SelectListItem> BillBranches { get; set; }

        public bool SearchAll { get; set; }

        protected override void OnParametersSet()
        {
            if (SelectedItem != null)
            {
                SelectedItem.PropertyChanged += (p, q) =>
                {
                    if (q.PropertyName == "Amount" || q.PropertyName == "PropertyName")
                    {
                        StateHasChanged();
                    }
                };
            }

            base.OnParametersSet();
        }
    }
}