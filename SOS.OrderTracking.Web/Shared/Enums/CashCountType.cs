namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum DenominationType : byte
    {
        Leafs = 0,

        /// <summary>
        /// Contains 100 leafs
        /// </summary>
        Packets = 4,

        /// <summary>
        /// Contains 10 packets
        /// </summary>
        Bundles = 8
    }
}
