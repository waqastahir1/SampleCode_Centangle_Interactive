using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbVehicle
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XDescription { get; set; } = null!;
        public string XVehicleType { get; set; } = null!;
        public string? XVehicleTypeDescription { get; set; }
        public string XGasolineType { get; set; } = null!;
        public string? XGasolineTypeDescription { get; set; }
        public string XLocation { get; set; } = null!;
        public string? XLocationDescription { get; set; }
        public string? XStation { get; set; }
        public string? XStationDescription { get; set; }
        public string? XMake { get; set; }
        public string? XModel { get; set; }
        public string? XCapacity { get; set; }
        public string? XOrigin { get; set; }
        public string? XChasisNo { get; set; }
        public string? XEngineNo { get; set; }
        public string? XRegistration { get; set; }
        public string? XLeasingCompany { get; set; }
        public string? XTagNumber { get; set; }
        public string? XPurchaseDate { get; set; }
        public DateTime? DPurchaseDate { get; set; }
        public string? XFuelCardNumber { get; set; }
        public decimal? XPurchasePrice { get; set; }
        public decimal? XRegistrationCost { get; set; }
        public decimal? XOtherCosts { get; set; }
        public decimal? XTotalCost { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
