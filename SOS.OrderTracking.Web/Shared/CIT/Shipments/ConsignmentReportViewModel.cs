using SOS.OrderTracking.Web.Shared.Enums;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
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
        public string? AcceptedByName1 { get; set; }
        public string AcceptedByName2 { get; set; }
        public string RecievedByName1 { get; set; }
        public string RecievedByName2 { get; set; }
        public string PickupBranchSupervisorEmail { get; set; }
        public string DeliveryBranchSupervisorEmail { get; set; }
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

        public int? Currency75xAmount { get; set; }
        public int? Currency100xAmount { get; set; }

        public int? Currency500xAmount { get; set; }

        public int? Currency1000xAmount { get; set; }

        public int? Currency5000xAmount { get; set; }

        public int? Currency200x { get; set; }
        public int? Currency750x { get; set; }
        public int? Currency1500x { get; set; }
        public int? Currency7500x { get; set; }
        public int? Currency15000x { get; set; }
        public int? Currency25000x { get; set; }
        public int? Currency40000x { get; set; }

        public int? PrizeMoney100x { get; set; }
        public int? PrizeMoney200x { get; set; }
        public int? PrizeMoney750x { get; set; }
        public int? PrizeMoney1500x { get; set; }
        public int? PrizeMoney7500x { get; set; }
        public int? PrizeMoney15000x { get; set; }
        public int? PrizeMoney25000x { get; set; }
        public int? PrizeMoney40000x { get; set; }

        public int? Currency200xAmount { get; set; }
        public int? Currency750xAmount { get; set; }
        public int? Currency1500xAmount { get; set; }
        public int? Currency7500xAmount { get; set; }
        public int? Currency15000xAmount { get; set; }
        public int? Currency25000xAmount { get; set; }
        public int? Currency40000xAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string AmountType { get; set; }
        public string AC_Code { get; set; }
        public string CustomerToBeBilled { get; set; }

        public string Comments { get; set; }

        public string Valueables { get; set; }

        public string CreatedBy { get; set; }

        public ShipmentType ShipmentType { get; set; }

        public string DeliveryMode
        {
            get
            {
                return ShipmentType == ShipmentType.ByAir ? "By Air" : "By Road";
            }
        }
    }
}
