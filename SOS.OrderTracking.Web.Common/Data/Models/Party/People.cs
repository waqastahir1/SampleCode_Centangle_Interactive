using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string NationalId { get; set; }

        public Gender Gender { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }


        public bool Status { get; set; }

        public int? DesignationId { get; set; }

        public string DesignationDesc { get; set; }


        [Column(TypeName = "date")]
        public DateTime? JoiningDate { get; set; }

        public EmploymentType EmploymentType { get; set; }


        public virtual Party Origin { get; set; }


    }
}
