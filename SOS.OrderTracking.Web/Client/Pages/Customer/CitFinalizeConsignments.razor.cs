using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class CitFinalizeConsignments
    {
        public List<SelectListItem> ConsignmentStateTypes { get; set; } = new List<SelectListItem>();
        private IEnumerable<SelectListItem> Crews { get; set; }

        private ConsignmentState _consignmentStateType;

        private ConsignmentState ConsignmentStateType
        {
            get { return _consignmentStateType; }
            set
            {
                _consignmentStateType = value;
                NotifyPropertyChanged();
            }
        }

        private ConsignmentStatus _consignmentStatus;

        public ConsignmentStatus ConsignmentStatus
        {
            get { return _consignmentStatus; }
            set
            {
                _consignmentStatus = value;
                NotifyPropertyChanged();
            }
        }


        private int _crewId;

        private int CrewId
        {
            get { return _crewId; }
            set { _crewId = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged();
            }
        }
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged();
            }
        }
        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                foreach (var item in Items)
                {
                    item.Selected = value;
                }
                StateHasChanged();
            }

        }
         
        public CitFinalizeConsignments()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            ConsignmentStateType =  ConsignmentState.Delivered;
            ConsignmentStatus = ConsignmentStatus.TobePosted;

            AdditionalParams = $"&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}";
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(StartDate) || q.PropertyName == nameof(EndDate) || q.PropertyName == nameof(CrewId) || q.PropertyName == nameof(ConsignmentStateType) || q.PropertyName == nameof(ConsignmentStatus))
                {
                    AdditionalParams = $"&{nameof(CrewId)}={CrewId}&{nameof(ConsignmentStateType)}={ConsignmentStateType}&{nameof(StartDate)}={StartDate.Value:dd-MMM-yyyy}&{nameof(EndDate)}={EndDate.Value:dd-MMM-yyyy}&{nameof(ConsignmentStatus)}={(int)ConsignmentStatus}";
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());

                    Logger.LogDebug($"{q.PropertyName} changed");
                }
                else if (q.PropertyName == nameof(BaseIndexModel))
                {
                      Crews = await ApiService.GetCrews(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId.GetValueOrDefault(), BaseIndexModel.StationId.GetValueOrDefault());
                }
                Logger.LogDebug($"{q.PropertyName} changed");

            };
        }
        protected override async Task OnInitializedAsync()
        {
            GetAllConsignmentStateTypes();
            Crews = await ApiService.GetCrews(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId.GetValueOrDefault(), BaseIndexModel.StationId.GetValueOrDefault());
            await base.OnInitializedAsync();
        }
        private void GetAllConsignmentStateTypes()
        { 
            Array enumValueArray = Enum.GetValues(typeof(ConsignmentState));
            foreach (object enumValue in enumValueArray)
            {
                ConsignmentStateTypes.Add(new SelectListItem() {  Value = enumValue.ToString(), Text = EnumHelper.GetDisplayValue(enumValue) });
            }
            ConsignmentStateTypes.Reverse();
        }
        private async Task FinalizeShipment()
        {
            try
            {
                bool hasError = false;
                if(Items.Where(x=>x.Selected).Any(x=>x.Amount == 0))
                {
                    await JSRuntime.InvokeVoidAsync("toast.show", "Shipments with zero amoount", $"{string.Join("<br>", (Items.Where(x => x.Selected && x.Amount == 0).Select(x=>x.ShipmentCode)))}", "error");
                    hasError = true;
                }

                if (Items.Where(x => x.Selected).Any(x => x.ShipmentType ==   ShipmentType.Unknown))
                {
                    await JSRuntime.InvokeVoidAsync("toast.show", "Shipment type is not defined", $"{string.Join("<br>", (Items.Where(x => x.Selected && x.ShipmentType == 0).Select(x => x.ShipmentCode)))}", "error");
                    hasError = true;
                }

                if (Items.Where(x => x.Selected).Any(x => x.Distance == 0))
                {
                    await JSRuntime.InvokeVoidAsync("toast.show", "Shipments with zero distance", $"{string.Join("<br>", (Items.Where(x => x.Selected && x.Distance == 0).Select(x => x.ShipmentCode)))}", "error");
                    hasError = true;
                }

                if (Items.Where(x => x.Selected).Any(x => x.ConsignmentStateType == 0))
                {
                    await JSRuntime.InvokeVoidAsync("toast.show", "Undelivered Shipments", $"{string.Join("<br>", (Items.Where(x => x.Selected && x.ConsignmentStateType == 0).Select(x => x.ShipmentCode)))}", "error");
                    hasError = true;
                }

                if (!hasError)
                {
                    IsTableBusy = true;
                    await InvokeAsync(() => StateHasChanged());
                    var response = await ApiService.FinalizeShipment(Items.Where(x => x.Selected).ToList());
                    if (response > 0)
                    {
                        await JSRuntime.InvokeVoidAsync("toast.show", "Pushing to gbms", $"{string.Join("<br>", (Items.Where(x => x.Selected && x.Distance == 0).Select(x => x.ShipmentCode)))}");
                        await LoadItems(true);

                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }
        private bool State { get; set; }
        private bool InverseState()
        {
            State = !State;
            return State;
        }
    }
}
