namespace SOS.OrderTracking.Web.Common
{
    public enum AttendanceState : byte
    {
        Absent = 1,
        OnLeave = 2 | Absent,
        Present = 16
    }

}
