using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.ATM
{
    public class ATMCustodianMembersOperationViewModel
    {
        public int RelationshipId { get; set; }

        public int ATMId { get; set; }
        public string PersonName { get; set; }

        public int PersonId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
