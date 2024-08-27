using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Enums.CPC;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.CPC.Consignments
{
    public class CpcConsignmentListViewModel
    {
        public int Id
        {
            get; set;
        }

        public string ShipmentCode
        {
            get; set;
        }

        public int FromPartyId { get; set; }

        public int ToPartyId { get; set; }


        public string FromPartyName { get; set; }


        public string FromPartyContact
        {
            get; set;
        }

        public string FromPartyStationName { get; set; }

        public int? CollectionStationId { get; set; }

        public int? DeliveryStationId { get; set; }


        public string ToPartyName
        {
            get; set;
        }


        public string ToPartyStationName { get; set; }


        public int CustomerId
        {
            get; set;
        }
        public string CustomerCode
        {
            get; set;
        }

        public CurrencySymbol CurrencySymbol
        {
            get; set;
        }


        public int TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }


        public DateTime DueTime { get; set; }

        public string CreatedBy { get; set; }

        public string ApprovedBy { get; set; }



        public ConsignmentApprovalState ApprovalState { get; set; }


        public CpcServiceCategory CpcServiceCategory { get; set; }

        public CPCConsignmentDisposalState CPCConsignmentDisposalState { get; set; }
        public CPCConsignmentProcessingState CPCConsignmentProcessingState { get; set; }

        public List<string> SealCodes { get; set; }

        public string Comments { get; set; }

        //AB: Use here ViewModel instead of directly using Model (Check CIT Order UI as well)
        public CitDenominationViewModel DenominationByCustomer
        {
            get; set;
        }
        public int AmountByCustomer { get; set; }

        public CitDenominationViewModel DenominationBySOS
        {
            get; set;
        }
        public int AmountBySOS { get; set; }

        public CitDenominationViewModel LeafsByCustomer
        {
            get; set;
        }


        public CitDenominationViewModel LeafsBySOS
        {
            get; set;
        }
        public List<CpcTransactionFormModel> Transactions { get; set; }

        #region Amounts

        public int ProcessedAmount { get; set; }

        public int DisposedAmount { get; set; }

        public int Balance { get; set; }

        public int ProcessedBalance { get; set; }

        public int UnprocessedBalance { get; set; }

        /// <summary>
        /// Amount in custody of CPC incharge
        /// </summary>
        public int OpenAmountProcessed { get; set; }


        /// <summary>
        /// Amount in custody of CPC incharge
        /// </summary>
        public int OpenAmountUnProcessed { get; set; }

        public int VaultProcessed { get; set; }

        public int VaultUnProcessed { get; set; }

        public int InprocessAmountManualSort { get; set; }

        public int InprocessAmounMachineSort { get; set; }

        public List<Tuple<string, int>> InProcessBreakup { get; set; }

        #endregion

    }
}
