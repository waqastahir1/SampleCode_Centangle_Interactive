using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class CrewCheckin
    {
        private DateTime _checkinTime;
        public DateTime CheckinTime
        {
            get { return _checkinTime; }
            set { _checkinTime = new DateTime(AttendanceDate.GetValueOrDefault().Year, AttendanceDate.GetValueOrDefault().Month, AttendanceDate.GetValueOrDefault().Day, value.Hour, value.Minute, value.Second); }
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
                CheckinTime = new DateTime(value.GetValueOrDefault().Year, value.GetValueOrDefault().Month, value.GetValueOrDefault().Day, CheckinTime.Hour, CheckinTime.Minute, CheckinTime.Second);
                NotifyPropertyChanged();
            }
        }

        public CrewCheckin()
        {
            AttendanceDate = DateTime.Now;
            CheckinTime = DateTime.Now;
            PropertyChanged += async (p, q) =>
            {
             if(q.PropertyName == nameof(AttendanceDate))
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
            if (!AttendanceDate.HasValue || CheckinTime.Equals("00:00"))
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
                    employee.CheckinTime = CheckinTime;
                    ListOfCrewAttendanceFormViewModels.Add(new CrewAttendanceFormViewModel()
                    {
                        RelationshipId = employee.RelationshipId,
                        AttendanceState = employee.AttendanceState,
                        AttendanceDate = employee.AttendanceDate,
                        CheckinTime = employee.CheckinTime
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