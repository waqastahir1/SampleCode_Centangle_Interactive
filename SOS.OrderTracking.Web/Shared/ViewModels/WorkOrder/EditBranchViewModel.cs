using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class BranchFormViewModel
    {
        public int BranchId { get; set; }
        public string ShortName { get; set; }
        public string FormalName { get; set; }
        public string Address { get; set; }
        public string PersonalContactNo { get; set; }
        public string OfficialContactNo { get; set; }
        public bool IsLatLngVerified { get; set; }
        public int ConsignmentId { get; set; }
        public DataRecordStatus LocationStatus { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude value is not valid")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude value is not valid")]
        public double? Longitude { get; set; }
    }
}
