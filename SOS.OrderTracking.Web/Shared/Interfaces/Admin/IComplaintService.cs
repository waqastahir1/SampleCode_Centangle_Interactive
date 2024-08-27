using SOS.OrderTracking.Web.Shared.ViewModels.Complaint;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IComplaintService
          : ICrudService<ComplaintFormViewModel, ComplaintListViewModel, int, ComplaintAdditionalValueViewModel>
    {

    }
}
