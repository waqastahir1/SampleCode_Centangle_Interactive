using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOS.OrderTracking.Web.APIs.Models
{ 
    public class ATMServiceModel
    {
        public int AtmServiceId
        {
            get; set;
        }

        public string ConsignmentNo
        {
            get; set;
        }
         
        public string AtmName
        {
            get; set;
        }

        public string AtmCode { get; set; }

        public string AtmAddress { get; set; }

        public string TechnitianName { get; set; }

        public string CachierName { get; set; }

        public ATMRServiceType ATMRServiceType
        {
            get; set;
        }

        public string ATMRServiceTypeStr { get; set; }

        public ATMServiceState? ATMServiceState { get; set; }

        public string ATMServiceStateStr { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public DateTime? Deadline
        {
            get; set;
        }
         
        public string CashBranchCode { get; set; }

        public string CashBranchName { get; set; }

        public string CashBranchAddress { get; set; }

        public string CashBranchContact { get; set; }
         
        public int Currency500x { get; set; }

        public int Currency1000x { get; set; }

        public int Currency5000x { get; set; }

        public string CashPickupQrCode { get; set; }

        public string CitExchangeQrCode { get; set; }

        public string CitDropoffQrCode { get; set; }


        public string CitRemainingCashReturnCollectionQrCode { get; set; }

        public string CitAccessCashQrCode { get; set; }

        public string CitCardsReturnQrCode { get; set; }
         

        public IEnumerable<ATMChecklist> Checklist { get; set; }

        public IEnumerable<string> SealCodes { get; set; }

        public bool IsFinalized { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime DueAt { get; set; }
        #region
        public int CitConsignmentId { get; set; }

        public int CitDeliveryId { get; set; }

        public int CitRemainingCashReturnConsignmentId { get; set; }

        public int CitRemainingCashReturnDeliveryId { get; set; }


        public int CitAccessCashReturnConsignmentId { get; set; }

        public int CitAccessCashReturnDeliveryId { get; set; }

        public int CitAtmCardsReturnConsignmentId { get; set; }

        public int CitAtmCardsReturnDeliveryId { get; set; }

        #endregion
    }

    public class ATMChecklist
    { 
        public int Id { get; set; }

        public string Title { get; set; }

    }
}
