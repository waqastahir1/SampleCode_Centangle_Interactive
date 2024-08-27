using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class RelationshipDetailViewModel
    {
        public int? EmployeeId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationTypeAsString { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
