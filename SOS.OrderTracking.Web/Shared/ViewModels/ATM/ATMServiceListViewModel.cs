using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class ATMServiceListViewModel
    {
        public int Id
        {
            get; set;
        }

        public string ConsignmentNo
        {
            get; set;
        }

        public string AtmName
        {
            get; set;
        }

        public string AtmCode { get; set; }

        public string AtmAddress { get; set; }

        public string TechnitianName { get; set; }

        public string CachierName { get; set; }

        public ATMRServiceType ATMRServiceType
        {
            get; set;
        }

        public DateTime? Deadline
        {
            get; set;
        }

        public IEnumerable<AtmServiceLogListViewModel> AtmServiceLogs { get; set; }

        public string CashBranchCode { get; set; }

        public string CashBranchName { get; set; }

        public ATMServiceState? ATMServiceState { get; set; }

        public string CashBranchAddress { get; set; }

        public string CashBranchContact { get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int Currency500x { get; set; }

        public int Currency1000x { get; set; }

        public int Currency5000x { get; set; }

        public string QrCode { get; set; }

    }

    public class AtmServiceLogListViewModel
    {
        public ATMServiceState ATMReplanishmentState { get; set; }

        public StateTypes Status { get; set; }

        public string Tag { get; set; }

        public DateTime? TimeStamp { get; set; }

        public int ATMServiceId { get; set; }

    }
}
