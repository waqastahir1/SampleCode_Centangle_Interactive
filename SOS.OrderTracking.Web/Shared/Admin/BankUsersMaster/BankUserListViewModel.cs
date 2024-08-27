using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.BankUser
{
    public class BankUserListViewModel
    {
        public int PartyId { get; set; }
        public int? ToPartyId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string RegionName { get; set; }
        public string SubRegionName { get; set; }
        public string StationName { get; set; }
        public int? RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }
        public List<string> Users { get; set; }
    }
    public class BankUserReportListViewModel
    {
        public string MainCustomer { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Role { get; set; }
        public string Station { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordExpiry { get; set; }
        public string EmailStatus { get; set; }
        public string Status { get; set; }
    }
}
