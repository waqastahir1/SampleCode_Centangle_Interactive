using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultMembersListViewModel
    {
        public int Id { get; set; }
        public int VaultId { get; set; }
        public string VaultName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public RoleType RelationshipType { get; set; }

        public string NationalId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
