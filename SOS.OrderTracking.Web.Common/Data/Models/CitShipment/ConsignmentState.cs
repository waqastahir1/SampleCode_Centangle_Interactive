using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    [Table("ConsignmentStates")]
    public class ConsignmentState
    {
        public int ConsignmentId { get; set; }

        public int DeliveryId { get; set; }

        public Shared.Enums.ConsignmentDeliveryState ConsignmentStateType { get; set; }

        public StateTypes Status { get; set; }

        public string Tag { get; set; }

        public DateTime? TimeStamp { get; set; }


        [StringLength(450)]
        public string CreatedBy { get; set; }
    }
}
