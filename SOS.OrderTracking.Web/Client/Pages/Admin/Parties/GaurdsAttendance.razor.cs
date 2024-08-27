using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Components;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Parties
{
    public partial class GaurdsAttendance
    {
        public override string ApiControllerName => "GaurdsAttendance";
        private IEnumerable<SelectListItem> Banks { get; set; }

        private int _bankId;

        public int BankId
        {
            get { return _bankId; }
            set
            {
                _bankId = value;
                NotifyPropertyChanged();
            }
        }
         

        private DateTime? _attendanceDate;
        private DateTime? AttendanceDate { 
            get
            {
                return _attendanceDate;
            }
            set
            {
                _attendanceDate = value;
                NotifyPropertyChanged();
            }
        }

        public GaurdsAttendance()
        {
            PropertyChanged += async(p, q) =>
            {
                if(q.PropertyName == nameof(BankId) || q.PropertyName == nameof(AttendanceDate))
                {
                    if(AttendanceDate.HasValue && BankId > 0)
                    {
                        try
                        {
                            AdditionalParams = $"&bankId={BankId}&Date={AttendanceDate}";

                            await LoadItems();
                          await InvokeAsync(()=> {
                                StateHasChanged();
                            });
                        }catch(Exception ex)
                        {
                            Logger.LogError(ex.ToString());
                        }
                    }
                }
            };
        }
        

        protected override async Task OnInitializedAsync()
        { 
           // BankId = 3;
               Banks = await Http.GetFromJsonAsync<IEnumerable<SelectListItem>>
             ($"v1/Common/GetCustomers"); 

            await base.OnInitializedAsync();
        }
        private async Task MarkAttendance(ChangeEventArgs e,int id)
        {
            //mark attendance by clicking on checkbox
            GaurdsAttendanceViewModel gaurdsAttendanceViewModel = new GaurdsAttendanceViewModel();
            gaurdsAttendanceViewModel.RelationshipId = id;
            if ((bool)e.Value)
            {
                gaurdsAttendanceViewModel.AttendanceState = AttendanceState.Present;
            }
            else
            {
                gaurdsAttendanceViewModel.AttendanceState = AttendanceState.Absent;
            }
                var response = await Http.PostAsJsonAsync($"v1/{ApiControllerName}/post", gaurdsAttendanceViewModel);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Operation Failed, please try again!!!" + "  " + response.StatusCode + "  " + (await response.Content.ReadAsStringAsync()));
            
        }
    }
}
