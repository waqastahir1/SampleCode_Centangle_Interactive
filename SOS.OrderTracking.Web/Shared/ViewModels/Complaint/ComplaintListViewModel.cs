using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Complaint
{
    public class ComplaintListViewModel
    {
        public int ComplaintId { get; set; }
        public ComplaintStatus ComplaintStatus { get; set; }
        public string ConsignmentCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }
}
