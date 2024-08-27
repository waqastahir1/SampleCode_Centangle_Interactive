using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum ConsignmentDeliveryState : byte
    {
        All = 127,

        Created = 0,

        [Display(Name = "Crew Assigned")]
        CrewAssigned = 1,

        [Display(Name = "Approaching Pickup")]
        Acknowldged = 2,

        [Display(Name = "Reached Pickup")]
        ReachedPickup = 4,

        [Display(Name = "In Transit")]
        InTransit = 8,

        [Display(Name = "Clubbed")]
        Clubbed = 16,

        [Display(Name = "In Vault")]
        InVault = 20,

        [Display(Name = "Reached Destination")]
        ReachedDestination = 24,

        Delivered = 64
    }


    public enum StateTypes : byte
    {
        Waiting = 0,
        Confirmed = 4,
        Expected = 16
    }
}
