using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{
    [Flags]
    public enum ConsignmentStatus : short
    {
        All = 0,
        ValidShipment = 1,

        [Display(Name = "To be Pushed")]
        TobePosted = 2 | ValidShipment,

        /// <summary>
        /// 5
        /// </summary>
        Pushing = 4 | ValidShipment,

        [Display(Name = "Partially Pushed")]
        PartiallyPushed = 16 | ValidShipment,

        Pushed = 32 | ValidShipment,

        Calculated = 64 | ValidShipment,

        RePush = 96 | ValidShipment,

        PushingFailed = 128,

        DuplicateSeals = 256 | PushingFailed,

        [Display(Name = "Zero Distance")]
        DistanceIssue = 512 | PushingFailed,

        [Display(Name = "Invalid Shipment")]
        InvalidShipment = 1024,

        Cancelled = 2048 | 1024,

        Declined = 4096 | 1024


    }
}
