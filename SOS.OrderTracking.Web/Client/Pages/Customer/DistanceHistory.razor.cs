using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class DistanceHistory
    {
        private int _fromPartyId;
        [Parameter]
        public int  FromPartyId
        {
            get { return _fromPartyId; }
            set { _fromPartyId = value;
                NotifyPropertyChanged();
            }
        }
        private int _toPartyId;

        [Parameter]
        public int ToPartyId
        {
            get { return _toPartyId; }
            set { _toPartyId = value;
                NotifyPropertyChanged();
            }
        }
        public DistanceHistory()
        {
            PropertyChanged += async (p, q) =>
            {
                AdditionalParams = $"&FromPartyId={FromPartyId}&ToPartyId={ToPartyId}";
                await LoadItems(true);
            };
        }

    }
}
