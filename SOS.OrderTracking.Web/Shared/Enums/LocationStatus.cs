namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum DataRecordStatus : byte
    {
        None = 0,
        Draft = 1,
        Approved = 4
    }

    public enum EscalationStatus : short
    {
        None = 0,
        CrewAssignment = 1,
        MainControlRoom = 2 | 1,

        BankApproval = 128,
        InitialApproval = 256 | BankApproval
    }
}
