using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.CustomShipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Customer
{
    public partial class CustomShipment
    {
        public List<SelectListItem> ConsignmentStateTypes { get; set; } = new List<SelectListItem>();
        private IEnumerable<SelectListItem> Crews { get; set; }

        private int _consignmentStateType;

        private int ConsignmentStateTypeInt
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
            set
            {
                _crewId = value;
                NotifyPropertyChanged();
            }
        }
        private string _postingMessage;

        public string PostingMessage
        {
            get { return _postingMessage; }
            set
            {
                _postingMessage = value;
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
        public IEnumerable<CustomShipmentListViewModel> CustomItems { get; set; }
        public int CustomItemsCount { get; set; }
        public CustomShipment()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            ConsignmentStateTypeInt = 64;
            ConsignmentStatus = ConsignmentStatus.TobePosted;

            //AdditionalParams = $"&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}";
            //PropertyChanged += async (p, q) =>
            //{
            //    if (q.PropertyName == nameof(StartDate) || q.PropertyName == nameof(EndDate) || q.PropertyName == nameof(CrewId) || q.PropertyName == nameof(ConsignmentStateTypeInt) || q.PropertyName == nameof(ConsignmentStatus))
            //    {
            //        AdditionalParams = $"&CrewId={CrewId}&ConsignmentStateTypeInt={ConsignmentStateTypeInt}&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}&ConsignmentStatus={(int)ConsignmentStatus}";
            //        await LoadItems(true);
            //        await InvokeAsync(() => StateHasChanged());

            //        Logger.LogInformation($"{q.PropertyName} changed");
            //    }
            //    else if (q.PropertyName == nameof(BaseIndexModel))
            //    {
            //        Crews = await ApiService.GetCrews(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId.GetValueOrDefault(), BaseIndexModel.StationId.GetValueOrDefault());
            //    }
            //    Logger.LogInformation($"{q.PropertyName} changed");

            //};
        }
        protected override async Task OnInitializedAsync()
        {
            GetAllConsignmentStateTypes();
            Crews = await ApiService.GetCrews(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId.GetValueOrDefault(), BaseIndexModel.StationId.GetValueOrDefault());
            await base.OnInitializedAsync();
        }
        private void GetAllConsignmentStateTypes()
        {
            ConsignmentStateTypes.Clear();
            Array enumValueArray = Enum.GetValues(typeof(ConsignmentState));
            foreach (object enumValue in enumValueArray)
            {
                ConsignmentStateTypes.Add(new SelectListItem() { IntValue = Convert.ToInt32(enumValue), Text = EnumHelper.GetDisplayValue(enumValue) });
            }
            ConsignmentStateTypes.Reverse();
        }
        private async Task SearchShipments()
        {
            IsTableBusy = true;
            try
            {
                BaseIndexModel.AdditionalParams = $"&CrewId={CrewId}&ConsignmentStateTypeInt={ConsignmentStateTypeInt}&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}&ConsignmentStatus={(int)ConsignmentStatus}&PostingMessage={PostingMessage}";
         
                //Items = (await ApiService.SearchCustomShipments(BaseIndexModel)).ToList();
                CustomItems = await ApiService.SearchCustomShipments(BaseIndexModel);
                CustomItemsCount = CustomItems.Count();
                await InvokeAsync(() => StateHasChanged());
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            IsTableBusy = false;
          
        }
        private bool State { get; set; }
        private bool InverseState()
        {
            State = !State;
            return State;
        }
    }
}
