using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IEmployeesService
        : ICrudService<EmployeesViewModel, EmployeesListViewModel, int, EmployeesAdditionalValueViewModel>
    {

    }
}
