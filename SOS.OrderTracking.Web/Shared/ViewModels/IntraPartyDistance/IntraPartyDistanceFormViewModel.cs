using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance
{
    public class IntraPartyDistanceFormViewModel
    {
        [Required(ErrorMessage = "please select from Branch")]
        public int? FromPartyId { get; set; }
        [Required(ErrorMessage = "please select to Branch")]
        public int? ToPartyId { get; set; }
        public string FromPartyName { get; set; }

        public string ToPartyName { get; set; }

        [Required(ErrorMessage = "please Enter Distance")]
        public double? Distance { get; set; }

        public int? AverageTravelTime { get; set; }

        [Range(minimum: 1, maximum: 100, ErrorMessage = "please Select Distance Status")]
        public DataRecordStatus DistanceStatus { get; set; }

        [Range(minimum: 1, maximum: 100, ErrorMessage = "please Select DistanceSource")]
        public DistanceSource DistanceSource { get; set; }

    }
}
