using NetTopologySuite.Geometries;
using SOS.OrderTracking.Web.Common;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Models.WorkOrder
{
    public class CitConsignmentV3Model
    {
        public int ConsignmentId { get; set; }

        public int DeliveryId { get; set; }

        public int? CrewId { get; set; }

        public string ConsignmentNo { get; set; }

        public string ManualShipmentCode { get; set; }

        public int FromPartyId { get; set; }

        public string FromPartyName { get; set; }

        public double? FromPartyLat { get; set; }

        public double? FromPartyLong { get; set; }

        public string FromPartyAddress { get; set; }

        public string FromPartyMobileNo { get; set; }

        public string FromPartyLandlineNo { get; set; }

        public int ToPartyId { get; set; }

        public string ToPartyName { get; set; }

        public string ToPartyAddress { get; set; }

        public string ToPartyMobileNo { get; set; }

        public string ToPartyLandlineNo { get; set; }

        public double? ToPartyLat { get; set; }
        public double? ToPartyLong { get; set; }

        public bool IsFinalized { get; set; }

        public int NoOfBags { get; set; }

        public bool SealedBags { get; set; }

        public string CurrencySymbol { get; set; }

        public int Amount { get; set; }

        public string ConsignmentType { get; set; }

        public ConsignmentDeliveryState ConsignmentStateType { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public DateTime CreatedAt { get; set; }
        public string ConsignmentDeliveryStatus { get; set; }
        public int DeliveryState { get; set; }

        public IEnumerable<string> SealsCodes { get; set; }
        public DenominationType DenominationType { get; set; }
        public string DenominationTypeStr { get; set; }
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
        public string PickupQRCode { get; set; }
        public string DropoffQRCode { get; set; }

        public int PickupPinCode { get; set; }
        public int DropoffPinCode { get; set; }

        public string Type { get; set; }
        public bool IsVault { get; set; }

        public bool IsCds { get; set; }

        [JsonIgnore]
        public string CollectiionMainCustomerJsonData { get; set; }

        [JsonIgnore]
        public string DeliveryMainCustomerJsonData { get; set; }

        public bool SkipQRCodeOnCollection { get; set; }
        public bool SkipQRCodeOnDelivery { get; set; }
        public bool EnableManualShipmentNo { get; set; }
        public string Description { get; internal set; }
    }


    public class CitConsignmentModel : CitConsignmentV3Model
    {
        public string FromPartyCode { get; set; }

        public string FromPartyAddress { get; set; }

        public string FromPartyContact { get; set; }

        public string ToPartyCode { get; set; }

        public string ToPartyAddress { get; set; }

        public string ToPartyContact { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int CashBundleCount { get; set; }

        public string DeliveryTypeStr { get; set; }
    }
}
