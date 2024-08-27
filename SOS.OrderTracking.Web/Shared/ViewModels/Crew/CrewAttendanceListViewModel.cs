
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewAttendanceListViewModel
    {
        public int RelationshipId { get; set; }

        [Column(TypeName = "date")]
        public DateTime AttendanceDate { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string CNIC { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public bool isChecked { get; set; }
        public AttendanceState? AttendanceState { get; set; }
    }
}
