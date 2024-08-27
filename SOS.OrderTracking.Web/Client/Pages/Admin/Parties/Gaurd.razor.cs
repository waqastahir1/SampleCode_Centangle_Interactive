using Microsoft.AspNetCore.Components;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class Gaurd
    {

        private int _branchId;
        [Parameter]
        public int BranchId
        {
            get { return _branchId; }
            set { _branchId = value;
                NotifyPropertyChanged();
            }
        }

        [Parameter]
        public string BranchName { get; set; }
        public IEnumerable<SelectListItem> Gaurds { get; set; }
        public Gaurd()
        {
            PropertyChanged += async (p, q) =>
            {
                if(q.PropertyName == nameof(BranchId) && BranchId > 0)
                {
                    AdditionalParams = $"&BranchId={BranchId}";
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());
                }
            };
            OnFormSubmitted = (id) =>
            {
                PubSub.Hub.Default.Publish(this);
            };
        }
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Gaurds = await ApiService.GetGaurds();
        }
    }
}
