using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ATMRSealCode
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AtmrServiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string SealCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
