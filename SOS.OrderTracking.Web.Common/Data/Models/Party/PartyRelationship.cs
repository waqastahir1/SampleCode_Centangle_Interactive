using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class PartyRelationship
    {
        public PartyRelationship()
        {
            EmployeeAttendances = new HashSet<EmployeeAttendance>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int FromPartyId { get; set; }

        public int ToPartyId { get; set; }

        ///// <summary>
        ///// The station id is to indicate if this relationship has a station constraint
        ///// e.g. If a vehicle of Islamabad is declared in Islamabad station, this field might be redundant
        ///// but if Islamabad vehicle is declared as Vault in Karachi, this field shall hold Karachi Id.
        ///// </summary>
        //public int? StationId { get; set; }

        public RoleType FromPartyRole { get; set; }

        public RoleType ToPartyRole { get; set; }


        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ThruDate { get; set; }

        public bool IsActive { get; set; }

        public virtual Party FromParty { get; set; }
        public virtual Party ToParty { get; set; }

        public virtual ICollection<EmployeeAttendance> EmployeeAttendances { get; set; }

    }
}
