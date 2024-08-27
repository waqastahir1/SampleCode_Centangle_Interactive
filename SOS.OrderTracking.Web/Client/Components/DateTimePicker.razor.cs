using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Components
{
    public partial class DateTimePicker
    {
        [Parameter] public string Id { get; set; }


        [Inject] IJSRuntime JSRuntime { get; set; }

        public DotNetObjectReference<DateTimePicker> DotNetRef;

        protected override bool TryParseValueFromString(string value, out DateTime? result, out string validationErrorMessage)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                validationErrorMessage = null;
                return true;
            }

            else if (DateTime.TryParse(value, out DateTime dateTime))
            {
                result = dateTime;
                validationErrorMessage = null;
                return true;
            }
            else
            {
                result = null;
                validationErrorMessage = $"Cannot convert {value} to date-time";
                return false;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DotNetRef = DotNetObjectReference.Create(this);
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("KTDateTimePicker.set", Id, Value?.ToString(), DotNetRef, "change_DateTimePicker");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// This method is called on an event related to the control
        /// </summary>
        /// <param name="value"></param>
        [JSInvokable("change_DateTimePicker")]
        public void Change(string value)
        {

            CurrentValue = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.Parse(value);
        }
    }
}
