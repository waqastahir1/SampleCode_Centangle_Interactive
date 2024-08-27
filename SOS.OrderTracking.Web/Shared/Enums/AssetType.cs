namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum AssetType : short
    {
        Unknown = 0,
        DepreciatingAsset = 1,
        Vehicle = 2,
        Weapon = 4,
        Uniform = 8,

        FixedAsset = 1024
    }
}
