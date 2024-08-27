using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class CrewViewModel
    {
        public int Id { get; set; }

        public int RelationshipId { get; set; }

        [Required(ErrorMessage = "Please Enter Name.")]
        public string Name { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        public DateTime? ThruDate { get; set; }
        public MemoryStream MemoryStream { get; set; }
        public bool Uploaded { get; set; }
    }
}
