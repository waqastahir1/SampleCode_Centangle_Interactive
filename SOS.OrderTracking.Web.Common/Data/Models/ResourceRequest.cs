using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ResourceRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public RequestType RequestType { get; set; }
        public int Quantity { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime? ThruDate { get; set; }

        public RequestStatus RequestStatus { get; set; }
        public AllocationType AllocationType { get; set; }

        public string Remarks1 { get; set; }

        public string Remarks2 { get; set; }

        public string RequestedById { get; set; } // Bank/Branch Id

        public DateTime RequestedAt { get; set; }

    }
}
