using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class ATMChecklist
    {
        public override string ApiControllerName => "CheckList";
        private IEnumerable<SelectListItem> Banks { get; set; }
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
              //  NotifyPropertyChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

        }
        public ATMChecklist()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(BankId))
                {

                    try
                    {
                        //AdditionalParams = $"&bankId={BankId}";
                        //await LoadItems();
                        //await InvokeAsync(() =>
                        //{
                        //    StateHasChanged();
                        //});
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }

                }
            };
        }
    }
}
