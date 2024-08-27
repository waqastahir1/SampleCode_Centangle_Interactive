using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class EmployeesService : ServiceBase, IEmployeesService
    {
        public EmployeesService(ApiService apiService,ILogger<EmployeesService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/Employees";

        public Task<EmployeesViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IndexViewModel<EmployeesListViewModel>> GetPageAsync(EmployeesAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<EmployeesListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public Task<int> PostAsync(EmployeesViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
