namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum NotificationCssClass
    {
        Danger,
        Success,
        Warning
    }

    public enum NotificationType : byte
    {
        New = 1,
        UpdatedDenomination = 2,
        UpdatedDropoff = 4,
        UpdatedSealCode = 8,
        UpdatedConsignment = 16,
        CrewReached = 20,
        ShipmentFinalized = 24,
        Delivered = 32,
        Cancel = 64,
        Declined = 65,
        ApprovalRequired = 96,
        ShipmentApproved = 100,
        ShipmentAlert = 112,
    }

    public enum NotificationCategory : byte
    {
        CIT = 4,
        ATMR = 8,
        CPC = 16
    }

    public enum NotificationMedium : byte
    {
        Firebase = 4,
        Web = 8,
        Email=16
    }


    public enum NotificationStatus : byte
    {
        Undelivered = 1,
        Waiting = 2 | Undelivered,
        Sent = 4 | Undelivered,
        Delivered = 8,
        Read = 16,
        Error = 32 | Undelivered,
        FirbaseError = 64 | Undelivered,
        UndefinedUser = 68 | Undelivered
    }
}
