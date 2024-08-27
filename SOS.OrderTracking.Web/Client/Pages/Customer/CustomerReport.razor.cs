using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class CustomerReport
    {
        public IEnumerable<OrganizationModel> Customers { get; set; }
        public string DocumentPath { get; set; }

        public int BillBranchId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ThruDate { get; set; }

        private List<SelectListItem> _consignmentStatusTypes;
        private List<SelectListItem> ConsignmentStatusTypes
        {
            get
            {
                return _consignmentStatusTypes ??= new List<SelectListItem>()
                {
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.All),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.TobePosted),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Pushing),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Pushed),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.PushingFailed),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Declined),
                    ToSelectListItem( Web.Shared.Enums.ConsignmentStatus.Cancelled)
            };
            }
        } 

        private string ConsignmentStatus
        {
            get { return _consignmentStatus; }
            set
            {
                _consignmentStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _consignmentStatus;
        public CustomerReport()
        {
            FromDate = DateTime.Today;
            ThruDate = DateTime.Today;
            ConsignmentStatus = "All";
        }
        private async Task ShowData()
        {
            AdditionalParams = $"&BillBranchId={BillBranchId}&FromDate={FromDate}&ThruDate={ThruDate}&ConsignmentStatus={ConsignmentStatus}";
            await LoadItems(true);
            await InvokeAsync(() => StateHasChanged());
            // await JSRuntime.InvokeVoidAsync("hello");
        }
        private SelectListItem ToSelectListItem(ConsignmentStatus status)
        {
            return new SelectListItem(status.ToString(), Enum.GetName(typeof(ConsignmentStatus), status));
        } 
        private async Task DownloadPdf(string fileType)
        { 
            IsTableBusy = true;
            try
            {
                AdditionalParams = $"&BillBranchId={BillBranchId}&FromDate={FromDate}&ThruDate={ThruDate}&ConsignmentStatus={ConsignmentStatus}&writerFormat={fileType}";
                var vm = await Http.GetFromJsonAsync<FileViewModel>($"v1/CustomerReport/Export?{BaseIndexModel?.ToQueryString()}{AdditionalParams}");
                await JSRuntime.InvokeVoidAsync(
            "saveAsFile", "CustomerReport." + fileType,
            Convert.ToBase64String(vm.Data));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex); 
                Error = ex.Message;
            }
            IsTableBusy = false;
        }
    }
}
