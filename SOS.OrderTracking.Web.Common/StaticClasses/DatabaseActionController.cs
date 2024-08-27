namespace SOS.OrderTracking.Web.Common.StaticClasses
{
    public static class DatabaseActionController
    {
        public static bool ReadOnly = false;
        public static void ChangeReadOnly()
        {
            ReadOnly = !ReadOnly;
        }
    }
}
