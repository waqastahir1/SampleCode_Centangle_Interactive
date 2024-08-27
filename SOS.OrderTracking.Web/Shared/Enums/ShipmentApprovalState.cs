namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum ConsignmentApprovalState : byte
    {
        All = 0,

        Draft = 1,
        Approved = 2,

        ReApprove = 4 | Draft | Approved,

        Declined
    }


    public enum ShipmentApprovalMode : byte
    {
        SameBranch = 0,

        CounterParty = 1,

        ThirdParty = 2
    }
}
