namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum AttendanceState : byte
    {
        Unknown = 0,
        Absent = 1,
        OnLeave = 2 | Absent,
        Present = 16
    }
}
