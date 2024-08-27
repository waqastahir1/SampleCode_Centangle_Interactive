using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Complaint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class ComplaintService : ServiceBase, IComplaintService
    {
        public ComplaintService(ApiService apiService, ILogger<ComplaintService> logger) : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/complaint";

        public async Task<ComplaintFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<ComplaintFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<ComplaintListViewModel>> GetPageAsync(ComplaintAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<ComplaintListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(ComplaintFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, ComplaintFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
