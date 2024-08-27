using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Reports
{
    public class CustomerReportIndexViewModel : BaseIndexModel
    {
        public int BillBranchId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ThruDate { get; set; }

        public ConsignmentStatus ConsignmentStatus { get; set; }

        public string WriterFormat { get; set; }
    }
}
