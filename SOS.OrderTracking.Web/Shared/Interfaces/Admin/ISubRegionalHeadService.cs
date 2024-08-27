using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface ISubRegionalHeadService
        : ICrudService<SubRegionalHeadViewModel, SubRegionalHeadListViewModel, int, AppointHeadsAdditionalValueViewModel>
    {
        public Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate);
        public Task<IEnumerable<SelectListItem>> GetRegularEmployeesAsync();
    }
}
