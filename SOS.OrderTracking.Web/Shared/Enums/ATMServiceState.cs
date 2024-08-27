using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{

    public enum ATMServiceState : byte
    {
        Created = 0,

        [Display(Name = "Team Checked-in at Cash Branch")]
        ReachedPickup = 4,

        [Display(Name = "Cash In Transit")]
        InTransit = 8,

        [Display(Name = "In Progress")]
        InProgress = 24,

        [Display(Name = "Completed")]
        Completed = 64
    }

}