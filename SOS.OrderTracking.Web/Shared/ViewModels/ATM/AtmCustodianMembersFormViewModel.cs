using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.ATM
{
    public class AtmCustodianMembersFormViewModel : ViewModelBase
    {
        private int _relationshipId;

        public int RelationshipId
        {
            get { return _relationshipId; }
            set
            {
                _relationshipId = value;
                NotifyPropertyChanged();
            }
        }


        public int ATMId { get; set; }
        public int PersonId { get; set; }
        public bool ChangeRelationship { get; set; }
        [Required(ErrorMessage = "Please select Person Type Cashier or Technician")]
        public int PersonType { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
