using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class IntraPartyDistance
    {

        private int _fromPartyId;

        private int fromPartyId
        {
            get { return _fromPartyId; }
            set
            {
                _fromPartyId = value;
                NotifyPropertyChanged();
            }
        }
        private int _toPartyId;

        private int toPartyId
        {
            get { return _toPartyId; }
            set
            {
                _toPartyId = value;
                NotifyPropertyChanged();
            }
        }
        public int SelectedFromPartyId { get; set; }
        public int SelectedToPartyId { get; set; }
        public IntraPartyDistance()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(fromPartyId) || q.PropertyName == nameof(toPartyId))
                {
                    AdditionalParams = $"&FromPartyId={fromPartyId}&ToPartyId={toPartyId}";
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());
                }
            };
            PubSub.Hub.Default.Subscribe<DistanceHistory>(this, async p =>
            {
                if (p == null)
                {
                    SelectedFromPartyId = 0;
                    await InvokeAsync(() => StateHasChanged());
                }
                else
                {
                    await LoadItems(true);
                }

            });
        }


    }
}
