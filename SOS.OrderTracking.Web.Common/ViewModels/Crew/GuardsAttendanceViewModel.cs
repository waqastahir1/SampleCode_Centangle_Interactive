using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class EmployeeAttendanceViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public DateTime? AttendanceDate { get; set; }

        [Required]
        public AttendanceState AttendanceState { get; set; }
    }
}
