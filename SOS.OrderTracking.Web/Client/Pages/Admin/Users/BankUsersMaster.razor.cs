using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Users
{
    public partial class BankUsersMaster
    {
        public IEnumerable<SelectListItem> MainCustomers { get; set; }
        public string Headings { get; set; }
        private int _mainCustomerId;
        public int MainCustomerId
        {
            get { return _mainCustomerId; }
            set { _mainCustomerId = value;
                NotifyPropertyChanged();
            }
        }
        private int _bankBranchId;

        public int BankBranchId
        {
            get { return _bankBranchId; }
            set { _bankBranchId = value; }
        }

        public string BranchCode { get; set; }


        public int RoleType { get; set; }
        public BankUsersMaster()
        {
            PropertyChanged += async (p,q) =>
            {
                if(q.PropertyName == nameof(MainCustomerId))
                {
                    AdditionalParams = $"&MainCustomerId={MainCustomerId}";
                    await LoadItems(true);
                }
            };
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            MainCustomers = await ApiService.ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>("v1/Organization/GetMainCustomers");

        }
        private async Task OnItemClicked(string id,int roleType,int partyId)
        {
            Error = null;
            ValidationError = null;
            RoleType = roleType;
            SelectedItem = CreateSelectedItem();
            string role = "";
            if (roleType == 1)
                role = "BankBranchManager";
            else if (roleType == 2)
                role = "BankBranch";

            SelectedItem.RoleName = role;
            SelectedItem.PartyId = partyId;
            if (id?.ToString() != "0" && !string.IsNullOrEmpty(id?.ToString()))
            {
                // SelectedItem = await ApiService.GetAsync(id);
                SelectedItem = await ApiService.GetUserAsync(id,roleType);
                SelectedItem.RoleName = role;
            }
        }
    }
}
