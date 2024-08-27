using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Crew
{
    public class CrewDetailFormModel
    {

        [Required]
        public string CheifCrew { get; set; }

        public string AssitantCrew { get; set; }

        public string Gaurd { get; set; }

        public string Driver { get; set; }
    }
}
