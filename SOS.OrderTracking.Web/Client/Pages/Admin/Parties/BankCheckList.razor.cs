using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class BankCheckList
    {
        public override string ApiControllerName => "BankCheckList";
        private IEnumerable<SelectListItem> Banks { get; set; }
        public string valu;
        private int _bankId;

        public int BankId
        {
            get
            {
                return _bankId;
            }
            set
            {
                _bankId = value;
                NotifyPropertyChanged();
            }
        }
        private IEnumerable<SelectListItem> checkList { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Banks = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
           ($"v1/Common/GetCustomers");
            checkList = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
           ($"v1/{ApiControllerName}/GetCheckList");
        }
        public async Task<IEnumerable<SelectListItem>> searchValue(string val)
        {
            return await Task.FromResult(Banks.Where(x=> x.Text.ToLower().Contains(val.ToLower())).ToList());
        }
        public BankCheckList()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(BankId))
                {

                    try
                    {
                        AdditionalParams = $"&bankId={BankId}";
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
            };
        }
        public async Task MarkAsActive(ChangeEventArgs e, int checkListId)
        {
            CheckListFormViewModel checkListFormViewModel = new CheckListFormViewModel();
            checkListFormViewModel.isActive = (bool)e.Value;
            checkListFormViewModel.checkListId = checkListId;
            var response = await Http.PostAsJsonAsync($"v1/{ApiControllerName}/post",checkListFormViewModel);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Operation Failed, please try again!!!" + "  " + response.StatusCode + "  " + (await response.Content.ReadAsStringAsync()));

        }
        IEnumerable<SelectListItem> Customers;


        Dictionary<DateTime, string> events = new Dictionary<DateTime, string>();

        void Change(object value, string name)
        {
            events.Add(DateTime.Now, $"{name} value changed to {value}");
            SelectedItem.BankId = value.ToString();
            StateHasChanged();
        }

        public async Task LoadData(LoadDataArgs args)
        {
            Customers = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
           ($"v1/Common/SearchCustomers?search={args.Filter}");

            await InvokeAsync(() => StateHasChanged()); ;
        }
         
        
    private async Task<IEnumerable<SelectListItem>> SearchValues(string searchText)
    {
        return await Task.FromResult(await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
           ($"v1/{ApiControllerName}/GetFewCustomers?search={searchText}"));
    }


    }
}
