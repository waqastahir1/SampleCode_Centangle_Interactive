using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.ATM
{
    public class AtmCustodianMembersListViewModel
    {
        public int Id { get; set; }
        public int ATMId { get; set; }
        public string ATMName { get; set; }

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public RoleType RelationshipType { get; set; }

        public string NationalId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }

        public string ImageLink { get; set; }

        public bool IsActive { get; set; }
    }
}
