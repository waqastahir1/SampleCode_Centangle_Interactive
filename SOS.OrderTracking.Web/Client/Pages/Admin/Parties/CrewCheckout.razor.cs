using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class CrewCheckout
    {
        private DateTime _checkoutTime;
        public DateTime CheckoutTime
        {
            get { return _checkoutTime; }
            set { _checkoutTime = new DateTime(AttendanceDate.GetValueOrDefault().Year,AttendanceDate.GetValueOrDefault().Month,AttendanceDate.GetValueOrDefault().Day,value.Hour,value.Minute,value.Second); }
        }

        private DateTime? _attendanceDate;
        private DateTime? AttendanceDate
        {
            get
            {
                return _attendanceDate;
            }
            set
            {
                _attendanceDate = value;
                CheckoutTime = new DateTime(value.GetValueOrDefault().Year, value.GetValueOrDefault().Month, value.GetValueOrDefault().Day, CheckoutTime.Hour, CheckoutTime.Minute, CheckoutTime.Second);
                NotifyPropertyChanged();
            }
        }

        public CrewCheckout()
        {
            AttendanceDate = DateTime.Today;
            CheckoutTime = DateTime.Now;
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(AttendanceDate))
                {
                    AdditionalParams = $"&AttendanceDate={AttendanceDate.GetValueOrDefault()}";
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());
                }
            };
        }


        private async Task MarkAttendance()
        {
            List<CrewAttendanceFormViewModel> ListOfCrewAttendanceFormViewModels = new List<CrewAttendanceFormViewModel>();
            if (!AttendanceDate.HasValue || CheckoutTime.Equals("00:00"))
                Error = "please select Attendance Date";
            else
            {
                Error = null;
                foreach (var employee in Items)
                {
                    if (employee.isChecked)
                        employee.AttendanceState = AttendanceState.Present;
                    else
                        employee.AttendanceState = AttendanceState.Absent;


                    employee.AttendanceDate = AttendanceDate.GetValueOrDefault();
                    employee.CheckoutTime = CheckoutTime;
                    ListOfCrewAttendanceFormViewModels.Add(new CrewAttendanceFormViewModel()
                    {
                        RelationshipId = employee.RelationshipId,
                        AttendanceState = employee.AttendanceState,
                        AttendanceDate = employee.AttendanceDate,
                        CheckoutTime = employee.CheckoutTime.GetValueOrDefault()
                    });
                }

                var response = await ApiService.MarkAttendence(ListOfCrewAttendanceFormViewModels);

                if (response != 1)
                    Error = "Operation Failed, please try again";

            }
        }
        private int count = 0;
        private async Task SelectAll()
        {
            count++;
            foreach (var employee in Items)
            {
                if (count % 2 != 0)
                    employee.isChecked = true; //employee.isChecked == true ? false : true;
                else
                    employee.isChecked = false;
            }
        }
    }
}