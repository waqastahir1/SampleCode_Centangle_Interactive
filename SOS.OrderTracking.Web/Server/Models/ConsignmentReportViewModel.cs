using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOS.OrderTracking.Web.Server.Models
{
    public class ConsignmentReportViewModel
    {
        public int ConsignmentId { get; set; }
        public string ShipmentRecieptNo { get; set; }
        public string VehicleNo { get; set; }
        public string CustomerToBeBilledName { get; set; }
        public string PickupBranch { get; set; }
        public string DeliveryBranch { get; set; }
        public string PickupTime { get; set; }
        public string DeliveryTime { get; set; }
        public string Date { get; set; }
        public string CurrencySymbol { get; set; }
        public CurrencySymbol CurrencySymbol_ { get; set; }
        public string FirstCopyName { get; set; }
        public string SecondCopyName { get; set; }
        public string ThirdCopyName { get; set; }
        public string SealNo { get; set; }
        public List<string> SealNos { get; set; }
        public string Item { get; set; }
        public string Amount { get; set; }
        public string AmountInWords { get; set; }

        public string AmountInWordsPKR { get; set; }
        public string PartialDelivery { get; set; }
        public string WaitingTime { get; set; }
        public int NoOfSeals { get; set; }
        public int NoOfBags { get; set; }

        public bool SealedBags { get; set; }
        public string ConsignedByName1 { get; set; }
        public string ConsignedByName2 { get; set; }
        public string AcceptedByName1 { get; set; }
        public string AcceptedByName2 { get; set; }
        public string RecievedByName1 { get; set; }
        public string RecievedByName2 { get; set; }
        public int Currency1x { get; set; }

        public int Currency2x { get; set; }

        public int Currency5x { get; set; }

        public int Currency10x { get; set; }

        public int Currency20x { get; set; }

        public int Currency50x { get; set; }
        public int Currency75x { get; set; }

        public int Currency100x { get; set; }

        public int Currency500x { get; set; }

        public int Currency1000x { get; set; }

        public int Currency5000x { get; set; }
        public int? Currency1xAmount { get; set; }

        public int? Currency2xAmount { get; set; }

        public int? Currency5xAmount { get; set; }

        public int? Currency10xAmount { get; set; }

        public int? Currency20xAmount { get; set; }

        public int? Currency50xAmount { get; set; }

        public int? Currency100xAmount { get; set; }
        public int? Currency75xAmount { get; set; }

        public int? Currency500xAmount { get; set; }

        public int? Currency1000xAmount { get; set; }

        public int? Currency5000xAmount { get; set; }
        public string AmountType { get; set; }
        public string AC_Code { get; set; }
        public string CustomerToBeBilled { get; set; }

        public string Comments { get; set; }

        public string Valueables { get; set; }

        public string CreatedBy { get; set; }
    }
}