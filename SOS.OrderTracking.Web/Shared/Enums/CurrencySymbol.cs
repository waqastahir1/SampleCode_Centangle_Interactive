namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum CurrencySymbol : byte
    {
        PKR = 1,
        USD = 2,
        // UAD = 3
        EURO = 4,
        MixCurrency = 64,
        PrizeBond = 96,
        Other = 127
    }
}
