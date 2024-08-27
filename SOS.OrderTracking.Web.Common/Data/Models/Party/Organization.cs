
using NetTopologySuite.Geometries;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class Organization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public Point Geolocation { get; set; }

        public DateTime? GeolocationUpdateAt { get; set; }

        public short GeolocationVersion { get; set; }

        public OrganizationType OrganizationType { get; set; }

        [StringLength(128)]
        public string ExternalCustomerType { get; set; }

        [StringLength(128)]
        public string ExternalBranchType { get; set; }

        public bool IsCPCBranch { get; set; }

        public byte AtmCitBill { get; set; }

        public DataRecordStatus LocationStatus { get; set; }

        public virtual Party Party { get; set; }
    }
}
