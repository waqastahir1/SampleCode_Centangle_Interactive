namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum ShipmentExecutionType
    {
        All = 0,

        Live = 4,

        Scheduled = 8,

        Recurring = 16,

        Declined = 32,
        //Dashboard = 64
    }

    public enum ATMRServiceType : byte
    {
        ATMR = 1,
        FLM = 2,
        SLM = 3
    }
}
