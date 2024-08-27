using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.Enums;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class WorkOrderLisViewtModel
    {

        public int Id { get; set; }

        public string ShipmentCode { get; set; }

        public string FromPartyCode { get; set; }

        public string FromPartyName { get; set; }

        public string FromPartyAddress { get; set; }

        public string FromPartyContact { get; set; }

        public string ToPartyCode { get; set; }

        public string ToPartyName { get; set; }

        public string ToPartyAddress { get; set; }

        public string ToPartyContact { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public CurrencySymbol CurrencySymbol { get; set; }

        public byte[] CustomerLogo { get; set; }

        public int Amount { get; set; }

        public ShipmentExecutionType Type { get; set; }

        //AB: Use here ViewModel instead of directly using Model (Check CIT Order UI as well)
        public Denomination Denomination { get; set; }

        public IEnumerable<ShipmentCharge> Charges { get; set; }

        public IEnumerable<Shared.Enums.ConsignmentDeliveryState> DeliverStatuses { get; set; }

        public WorkOrderLisViewtModel()
        {
        }
    }
}
