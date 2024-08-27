using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Radzen;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Users
{
    public partial class InternalUsers
    {
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        private IEnumerable<SelectListItem> Employees;
        private List<InternalUsersViewModel> Roles;
        IEnumerable<string> multipleValues = new string[] { };

        public InternalUsers()
        {
            OnSelectedItemCreated += (i) =>
            {
                Logger.LogInformation("Selected item is initialized in InternalUsers");
                SelectedItem.PropertyChanged += async (p, q) =>
                {
                    {
                        if (q.PropertyName == nameof(SelectedItem.Id))
                        {
                            Logger.LogInformation($"Selected item's Id = {SelectedItem.Id}");
                            Employees = await ApiService.GetEmployees(SelectedItem.Id);
                            await InvokeAsync(() =>
                            {
                                StateHasChanged();
                            });
                        }
                    }
                };
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
        private new async Task OnItemClicked(string id)
        {
            Error = null;
            SelectedItem = CreateSelectedItem();
            SelectedItem.Id = id;
            if (id?.ToString() != "0" && !string.IsNullOrEmpty(id?.ToString()))
            {
                SelectedItem = await ApiService.GetAsync(id);//GetViewModel<UserRolesViewModel>(ApiControllerName, id);
                string[] roles = SelectedItem.RoleName.Split(',');
                var items = new List<string>();
                foreach (var r in roles)
                {
                    items.Add(r);
                    Console.WriteLine(r);
                }
                multipleValues = items;
                StateHasChanged();
            }
        }
        protected async override Task OnInitializedAsync()
        {
            Employees = await ApiService.GetEmployees(null);//await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>($"v1/{ApiControllerName}/GetEmployees?userId={null}");
            Roles = await ApiService.GetRolesAsync();
            await base.OnInitializedAsync();
        }
        public void OnPasswordChangeClicked(string id)
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
        private void Change(object value)
        {
            var str = value is IEnumerable<string> ? string.Join(",", (IEnumerable<string>)value) : value;
            Console.WriteLine($"{str}");
            SelectedItem.RoleId = str.ToString();
            StateHasChanged();
        }
    }

}
