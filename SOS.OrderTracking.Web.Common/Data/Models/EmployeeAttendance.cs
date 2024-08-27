using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class EmployeeAttendance
    {
        public EmployeeAttendance()
        {

        }

        /// <summary>
        /// Primary Key
        /// </summary>
        public int RelationshipId { get; set; }

        /// <summary>
        /// Primary key
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime AttendanceDate { get; set; }

        public Shared.Enums.AttendanceState AttendanceState { get; set; }

        public DateTime CheckinTime { get; set; }

        public DateTime? CheckoutTime { get; set; }


        [Required]
        [StringLength(450)]
        public string MarkedBy { get; set; }

        public DateTime MarkedAt { get; set; }

        /// <summary>
        /// Once someone approves this record, no body shall be allowed to change anything
        /// If ApprovedBy is null, the record is considered in draft mode and can be updated by bank users
        /// </summary>
        [StringLength(450)]
        public string Approvedby { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public virtual PartyRelationship AllocationRelationship { get; set; }

    }
}
