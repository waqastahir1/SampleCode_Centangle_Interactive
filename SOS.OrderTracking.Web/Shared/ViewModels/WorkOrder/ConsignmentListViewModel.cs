using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class ConsignmentListViewModel
    {
        public int Id
        {
            get; set;
        }
         public int DeliveryId
        {
            get; set;
        }

        public string ShipmentCode
        {
            get; set;
        }

        public string ManualShipmentCode { get; set; }
        public int FromPartyId { get; set; }
        public int ToPartyId { get; set; }
        public string FromPartyCode { get; set; }

        public string FromPartyName { get; set; }

        public string FromPartyAddress { get; set; }

        public string FromPartyContact
        {
            get; set;
        }

        public string FromPartyStationName { get; set; }

        public DataRecordStatus FromPartyGeoStatus { get; set; }

        public int? CollectionRegionId { get; set; }

        public int? CollectionSubRegionId { get; set; }

        public int? CollectionStationId { get; set; }


        public int? DeliveryRegionId { get; set; }

        public int? DeliverySubRegionId { get; set; }

        public int? DeliveryStationId { get; set; }

        public string ToPartyCode
        {
            get; set;
        }

        public string ToPartyName
        {
            get; set;
        }


        public string ToPartyStationName { get; set; }

        public string ToPartyAddress
        {
            get; set;
        }

        public string ToPartyContact
        {
            get; set;
        }

        public DataRecordStatus ToPartyGeoStatus { get; set; }


        public int CustomerId
        {
            get; set;
        }
        public string CustomerCode
        {
            get; set;
        }

        public string CustomerName
        {
            get; set;
        }

        public int? BillBranchId { get; set; }
        public string BillingRegion { get; set; }

        public CurrencySymbol CurrencySymbol
        {
            get; set;
        }

        public decimal ExchangeRate { get; set; }

        public int OriginPartyId { get; set; }

        public int CounterPartyId { get; set; }

        public byte ApprovalMode { get; set; }

        public string CustomerLogoUrl
        {
            get; set;
        }

        public int Amount { get; set; }
        public int AmountPKR { get; set; }

        public double Distance { get; set; }

        public DataRecordStatus DistanceStatus { get; set; }


        public bool IsFinalized { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PlanedCollectionTime { get; set; }

        public DateTime? PlanedDeliveryTime { get; set; }
        public DateTime? ActualCollectionTime { get; set; }

        public DateTime? ActualDeliveryTime { get; set; }

        public DateTime DueTime { get; set; }

        public string CreatedBy { get; set; }


        public string ApprovedBy { get; set; }

        public string PostingMessages { get; set; }
        public int Rating { get; set; }

        public ConsignmentApprovalState ApprovalState { get; set; }

        public ShipmentExecutionType Type
        {
            get; set;
        }

        public Point FromPartyGeolocation { get; set; }

        public Point ToPartyGeolocation { get; set; }


        public ConsignmentDeliveryState ConsignmentStateType { get; set; }

        public ConsignmentStatus ConsignmentStatus { get; set; }

        public ShipmentType ShipmentType { get; set; }

        public IEnumerable<DeliveryListViewModel> Deliveries { get; set; }
        public List<string> SealCodes { get; set; }

        public string Comments { get; set; }

        //AB: Use here ViewModel instead of directly using Model (Check CIT Order UI as well)
        public CitDenominationViewModel Denomination
        {
            get; set;
        }

        public IEnumerable<DeliveryChargesModel> Charges// => new DeliveryChargesViewModel();
        {
            get; set;
        }

        public IList<ConsignmentStateViewModel> DeliveryStates { get; set; }
        public ChangedDropOff ChangedDropOff { get; set; }
        public string Valueables { get; set; }

        public bool IsVault { get; set; }
        public bool IsClubbed { get; set; }
        public int? ToChangedPartyId { get; set; }
        public string ImageUrl { get; set; }
        public ConsignmentListViewModel()
        {

        }
    }
    public class ChangedDropOff
    {
        public int ToPartyId { get; set; }
        public string ToPartyCode { get; set; }
        public string ToPartyName { get; set; }
        public bool IsApproved { get; set; }
    }
}
