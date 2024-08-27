using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{
    /// <summary>
    /// Local, Domestic, Dediated
    /// </summary>
    public enum ShipmentType : byte
    {
        Unknown = 0,
        Local = 10,
        Domestic = 20,
        ByAir = 30,
        Dedicated = 40,

        [Display(Name = "Hilly Areas")]
        HillyAreas = 50,

        [Display(Name = "ATM CIT Local")]
        ATMCITLocal = 80,

        [Display(Name = "ATM CIT Domestic")]
        ATMCITDomestic = 85,

        [Display(Name = "Cash Manager Local")]
        CashManagerLocal = 90,

        [Display(Name = "Cash Manager Domestic")]
        CashManagerDomestic = 95
    }
}