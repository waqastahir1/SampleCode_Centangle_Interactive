namespace SOS.OrderTracking.Web.Shared.Enums.CPC
{
    public enum CPCConsignmentProcessingState : short
    {
        CashAwaited = 0,
        CashRecieved = 2,
        BundlesCounted = 4,
        LeafsCounted = 6,
        InVault = 8,
        Processing = 16,
        PartiallyProcessed = 32,
        Processed = 64,
        Disposed = 128

    }
}
