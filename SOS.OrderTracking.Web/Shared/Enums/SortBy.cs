namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum SortBy : byte
    {
        Unknown = 0,
        CreationDateAsc = 1,
        CreationDateDesc = 2,
        DueDateAsc = 4,
        DueDateDesc = 8,
        DeliveryDateAsc = 16,
        DeliveryDateDesc = 32,
        ApprovalAsc = 64,
        ApprovalDesc = 128

    }
}
