namespace SOS.OrderTracking.Web.Portal.Helpers
{
    internal static class UrlHelper
    {
        internal static string BaseAddress;

        internal static void UpdateUrl(string baseAddress)
        {
            BaseAddress= baseAddress;
        }
    }
}
