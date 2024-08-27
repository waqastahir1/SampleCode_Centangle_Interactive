using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Parties
{
    public class AdditionalRequestFormViewModel
    {
        public int Quantity { get; set; }
        [Required]
        public DateTime? FromDate { get; set; }
        public DateTime? ThruDate { get; set; }
        public int Type { get; set; }
        public RequestType RequestType { get; set; }
        public AllocationType AllocationType { get; set; }
        public int Operation { get; set; }
        public string Remarks { get; set; }
    }
}
