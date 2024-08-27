using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewUserViewModel
    {
        public int CrewId { get; set; }
        [Required(ErrorMessage = "Please select Name!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please select Password!")]
        public string Password { get; set; }
        [StringLength(19, ErrorMessage = "IMEI number cannot exceed from 14 digit!")]
        public string IMEINumber { get; set; }
        public bool IsEnabled { get; set; }
    }
}
