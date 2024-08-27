using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class EmployeesListViewModel
    {
        public int UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? PartyId { get; set; }
        public IEnumerable<string> Role { get; set; }
        public string TickColor { get; set; }
        public string OrganizationName { get; set; }
    }
}
