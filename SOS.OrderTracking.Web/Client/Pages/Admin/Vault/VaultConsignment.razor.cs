using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Vault
{
    public partial class VaultConsignment
    {
        private int _vaultId;
        [Parameter]
        public int VaultId
        {
            get { return _vaultId; }
            set
            {
                _vaultId = value;
                NotifyPropertyChanged();
            }
        }
        [Parameter]
        public string vaultName { get; set; }
        public VaultConsignment()
        {
            PropertyChanged += async (p, q) =>
            {
                if(q.PropertyName == nameof(VaultId))
                {
                    AdditionalParams = $"&VaultId={VaultId}";
                    await LoadItems(true);
                }
            };
        }

    }
}
