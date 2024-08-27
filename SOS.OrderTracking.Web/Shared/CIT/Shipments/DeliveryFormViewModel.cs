using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class DeliveryFormViewModel
    {
        /// <summary>
        /// Delivery Id
        /// </summary>
        public int Id { get; set; }

        public int ConsignmentId { get; set; }

        [Required(ErrorMessage = "Select Crew")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Select Crew")]
        public int? CrewId { get; set; }

        //[Required(ErrorMessage = "Select Location")]
        // [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Select Location")]
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public int AddLocationClick { get; set; }
        [Range(-90, 90, ErrorMessage = "Latitude value is not valid")]
        public double Latitude { get; set; }
        [Range(-180, 180, ErrorMessage = "Longitude value is not valid")]
        public double Longitude { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }

        public ConsignmentDeliveryState DeliveryState { get; set; }

        [Required(ErrorMessage = "Select Date/Time")]
        public DateTime? PlanedPickupTime { get; set; }

        public IEnumerable<SelectListItem> Crews { get; set; }


    }
}
