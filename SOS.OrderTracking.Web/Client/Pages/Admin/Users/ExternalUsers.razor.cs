using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Users
{
    public partial class ExternalUsers
    {

        private IEnumerable<SelectListItem> Customers { get; set; }

        private int _mainCustomerId;

        public int MainCustomerId
        {
            get { return _mainCustomerId; }
            set
            {
                _mainCustomerId = value;
                NotifyPropertyChanged();
            }
        }

        private ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        private List<SelectListItem> RoleTypes = new List<SelectListItem>()
        {
        new SelectListItem() { Text = "Bank Branch", Value = "BankBranch" },
        new SelectListItem() { Text = "Bank CPC", Value = "BankCPC" },
        new SelectListItem() { Text = "Bank Headoffice", Value = "BANK" },
        new SelectListItem() { Text = "Bank Gaurding", Value = "BankGaurding" },
        new SelectListItem() { Text = "Bank BranchManager", Value = "BankBranchManager" },
        new SelectListItem() { Text = "Bank CPCManager", Value = "BankCPCManager" },
        new SelectListItem() { Text = "Initiator + Supervisor", Value = "BankHybrid" }
        };

        public IEnumerable<SelectListItem> MainCustomers { get; set; } = Array.Empty<SelectListItem>();

        private string _rolyTypeId;
        private string RoleTypeId    //used to get users which are connected to specific role
        {
            get
            {
                return _rolyTypeId;
            }
            set
            {
                if (RoleTypeId != value)
                {
                    _rolyTypeId = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public ExternalUsers()
        {
            //  GetAllRoleTypes();
            PropertyChanged += async (p, q) =>
            {
                try
                {

                    if (q.PropertyName == nameof(RoleTypeId) || q.PropertyName == nameof(MainCustomerId))
                    {
                        AdditionalParams = $"&roleTypeId={RoleTypeId}&mainCustomerId={MainCustomerId}";
                        await LoadItems();
                        await InvokeAsync(() =>
                        {
                            StateHasChanged();
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            };

        }
        public async Task SearchString(string searchKey)
        {
            AdditionalParams = $"&searchKey={searchKey}";
            await LoadItems();
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        protected override async Task OnInitializedAsync()
        {
            MainCustomers = await ApiService.GetMainCustomers();//GetFromJsonAsync<IEnumerable<SelectListItem>>("v1/Organization/GetMainCustomers");
            await base.OnInitializedAsync();
        }

        private void OnPasswordChangeClicked(string id)
        {
            Error = null;
            ChangePasswordViewModel = new ChangePasswordViewModel();
            ChangePasswordViewModel.Id = id;
        }
        private async Task ChangePassword()
        {
            IsModalBusy = true;
            try
            {
                var response = await ApiService.ChangePassword(ChangePasswordViewModel);
                 //    var response = await Http.PostAsJsonAsync
                 //     ($"v1/{ApiControllerName}/ChangePassword", ChangePasswordViewModel);

                //it produces error when as response is not serializeable
                //  var resposneString = await response.Content.ReadAsStringAsync();
                //  if (!response.IsSuccessStatusCode)
                //   throw new Exception(await response.Content.ReadAsStringAsync());
                if (!response)
                    throw new Exception("Something went wrong in changing password!");

                //  if (response.IsSuccessStatusCode)
                // ChangePasswordViewModel = null;
                if (response)
                    ChangePasswordViewModel = null;

                ValidationError = null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ValidationError = ex.Message;
            }
            IsModalBusy = false;
        }
    }
}
