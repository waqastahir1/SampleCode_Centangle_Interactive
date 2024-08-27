using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace SOS.OrderTracking.Web.Portal.Components
{
    public partial class DatePicker : InputBase<DateTime?>
    {
        [Parameter]
        public string Id
        {
            get; set;
        }

        [Inject]
        IJSRuntime JSRuntime
        {
            get; set;
        }

        public DotNetObjectReference<DatePicker> DotNetRef;

        protected override void OnInitialized()
        {
            DotNetRef = DotNetObjectReference.Create(this);
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("datePicker.init", Id, DotNetRef, "Change_Date");
            }
        }

        [JSInvokable("Change_Date")]
        public void Change(string value, string key = null)
        {
            if (value == "null")
            {
                value = null;
            }
            CurrentValue = DateTime.ParseExact(value, "dd-MM-yyyy", null);
        }

        protected override bool TryParseValueFromString(string value, out DateTime? result, out string validationErrorMessage)
        {
            if (!string.IsNullOrEmpty(value) && DateTime.TryParseExact(value, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedValue))
            {
                result = parsedValue;
                validationErrorMessage = null;
                return true;
            }
            validationErrorMessage = "Unable to parse date/time string";
            result = default;
            return false;
        }

        protected override string FormatValueAsString(DateTime? value)
        {
            if (value == null)
                return null;

            return Convert.ToDateTime(value).ToString("dd-MM-yyyy");
        }
    }
}
