using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class RegionalHeadViewModel
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }

        [Required(ErrorMessage = "Please Select Employee")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Please Enter Start Date.")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
