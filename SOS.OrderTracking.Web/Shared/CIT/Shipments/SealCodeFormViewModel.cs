using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class SealCodeFormViewModel
    {
        public SealCodeFormViewModel()
        {
            SealCodes = new List<Seal>();
        }
        public string NoOfBags { get; set; }
        public bool IsPosted { get; set; }
        public List<Seal> SealCodes { get; set; }
    }
    public class Seal
    {
        [Required]
        [StringLength(50)]
        public string SealCode { get; set; }

        public string Error { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
