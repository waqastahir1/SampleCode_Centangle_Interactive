namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum TemporalState
    {
        Active = 2,

        InActive = 8,
        Past = 16 | InActive,
        Future = 32 | InActive
    }
}
