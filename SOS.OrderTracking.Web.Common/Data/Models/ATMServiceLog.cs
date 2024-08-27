using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ATMServiceLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ATMServiceId { get; set; }

        public byte ATMServiceState { get; set; }

        public StateTypes StateType { get; set; }

        public string UserId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public ATMService ATMService { get; set; }

    }
}
