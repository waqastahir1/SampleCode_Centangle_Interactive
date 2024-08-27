using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.CIT.Shipments
{
    public partial class ShipmentBreakups
    {
        public IEnumerable<SelectListItem> Crews { get; set; }
        public List<SelectListItem> ConsignmentStateTypes { get; set; } = new List<SelectListItem>();
        private IEnumerable<SelectListItem> CustomerBranches { get; set; }
        private int _crewId;

        public int CrewId
        {
            get { return _crewId; }
            set
            {
                _crewId = value;
                NotifyPropertyChanged();
            }
        }
        private int? _deliveryStateId;

        public int? DeliveryStateId
        {
            get { return _deliveryStateId; }
            set
            {
                _deliveryStateId = value;
                NotifyPropertyChanged();
            }
        }
        private int _billBranchId;

        public int BillBranchId
        {
            get { return _billBranchId; }
            set
            {
                _billBranchId = value;
                NotifyPropertyChanged();
            }
        }

        public int MainCustomerId { get; set; }

        private string _deliveryStateName;

        public string DeliveryStateName
        {
            get { return _deliveryStateName; }
            set
            {
                _deliveryStateName = value;
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
      

        public ShipmentBreakups()
        {
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;

            AdditionalParams = $"&CrewId={CrewId}&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}";
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(CrewId) || q.PropertyName == nameof(StartDate) || q.PropertyName == nameof(EndDate) || q.PropertyName == nameof(DeliveryStateId) || q.PropertyName == nameof(BillBranchId) || q.PropertyName == nameof(MainCustomerId))
                {
                    AdditionalParams = $"&CrewId={CrewId}&StartDate={StartDate.Value:dd-MMM-yyyy}&EndDate={EndDate.Value:dd-MMM-yyyy}&ConsignmentStateType={DeliveryStateId}&BillBranchId={BillBranchId}&MainCustomerId={MainCustomerId}";
                    await LoadItems(true);
                }
                else if (q.PropertyName == nameof(OrganizationalUnit))
                {
                    Crews = await ApiService.GetCrews(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId.GetValueOrDefault(), BaseIndexModel.StationId.GetValueOrDefault());
                }
                await InvokeAsync(() => StateHasChanged());
                Logger.LogInformation($"{q.PropertyName} changed");

            };
        }
        protected override async Task OnInitializedAsync()
        { 
            GetAllConsignmentStateTypes();
            Crews =  await ApiService.GetCrews(BaseIndexModel.RegionId.GetValueOrDefault(), BaseIndexModel.SubRegionId.GetValueOrDefault(), BaseIndexModel.StationId.GetValueOrDefault());
            await base.OnInitializedAsync();
        }
        private void GetAllConsignmentStateTypes()
        {
            ConsignmentStateTypes.Clear();
            Array enumValueArray = Enum.GetValues(typeof(ConsignmentState));
            foreach (object enumValue in enumValueArray)
            {
                ConsignmentStateTypes.Add(new SelectListItem() { IntValue = Convert.ToInt32(enumValue), Text = Enum.GetName(typeof(ConsignmentState), enumValue) });
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
