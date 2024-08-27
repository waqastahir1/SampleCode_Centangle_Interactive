using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{

    public class DeliveryCrewFormViewModel
    {
        public int ConsignmenId { get; set; }

        public int DeliveryId { get; set; }

        public int? CrewId { get; set; }
    }

    public class DeliveryCrewFormExtendedViewModel : DeliveryCrewFormViewModel
    {

        public string CrewName { get; set; }

        public string Vehicle { get; set; }

        public List<CrewMemberListViewModel> CrewMembers { get; set; }

        public string CurrentLocation { get; set; }

        public int AmountCarried { get; set; }

        public string CurrentDestination { get; set; }

        public decimal DropoffDistanceFromCurrentDestination { get; set; }

        public decimal PickupDistanceFromCurrentLocation { get; set; }

    }

    public class CrewMemberListViewModel
    {
        public string Name { get; set; }

        public string Designation { get; set; }

        public byte Status { get; set; }
    }
}
