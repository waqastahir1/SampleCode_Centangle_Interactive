using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultAdditionalValueViewModel : BaseIndexModel
    {
        public int vaultId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ThruDate { get; set; }
    }
}
