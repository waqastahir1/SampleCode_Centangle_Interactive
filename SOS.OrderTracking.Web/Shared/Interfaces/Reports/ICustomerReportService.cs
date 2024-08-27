using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Reports;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Reports
{
    public interface ICustomerReportService
        : ICrudService<CustomerReportViewModel, CustomerReportViewModel, int, CustomerReportIndexViewModel>
    {

    }
}
