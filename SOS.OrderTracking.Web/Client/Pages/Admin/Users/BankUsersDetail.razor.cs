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
    public partial class BankUsersDetail
    {

        private int _bankBranchId;

        [Parameter]
        public int BankBranchId
        {
            get { return _bankBranchId; }
            set
            {
                _bankBranchId = value;
                NotifyPropertyChanged();
            }
        }

        [Parameter]
        public string BranchCode { get; set; }

        private ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        private List<SelectListItem> RoleTypes = new List<SelectListItem>()
        {
        new SelectListItem() { Text = "Branch Initiator", Value = "BankBranch" },
        new SelectListItem() { Text = "Branch Supervisor", Value = "BankBranchManager" },
        new SelectListItem() { Text = "CPC Initiator", Value = "BankCPC" },
        new SelectListItem() { Text = "CPC Supervisor", Value = "BankCPCManager" },
          new SelectListItem() { Text = "Branch Initiator + Supervisor", Value = "BankHybrid" },
        new SelectListItem() { Text = "Bank Headoffice", Value = "BANK" },
        new SelectListItem() { Text = "Bank Gaurding", Value = "BankGaurding" }
        };

        public BankUsersDetail()
        {
            PropertyChanged += async (p, q) =>
            {
                if(q.PropertyName == nameof(BankBranchId))
                {
                    BaseIndexModel.BankBranchId = BankBranchId;
                    await LoadItems();
                }

            };

            OnSelectedItemCreated += (p) =>
            {
                SelectedItem.PartyId = BankBranchId;
                if (!string.IsNullOrWhiteSpace(BranchCode) && string.IsNullOrWhiteSpace(SelectedItem.UserName))
                {
                    int splitIndex = BranchCode.IndexOf('-');
                    SelectedItem.UserName = $"{BranchCode.Substring(0, splitIndex).ToLower()}.{BranchCode.Substring(splitIndex + 1, BranchCode.Length - splitIndex -1)}-0";
                    SelectedItem.AutoName = true;
                }
            };
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadItems();
            await base.OnParametersSetAsync();
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
     
          
    }
}
