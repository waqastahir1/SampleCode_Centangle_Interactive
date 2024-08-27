using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Portal.Components
{
    public partial class Select2<TValue> : InputBase<TValue>
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string ModalId { get; set; }

        [Parameter]
        public bool ShowDefaultOption { get; set; } = true;

        [Parameter]
        public IEnumerable<SelectListItem> Datasource
        {
            get
            {
                return _datasource;
            }
            set
            {
                _datasource = value;
            }
        }
        private IEnumerable<SelectListItem> _datasource;

        [Inject]
        ILogger<Select2<TValue>> logger { get; set; }

        [Inject]
        IJSRuntime JSRuntime
        {
            get; set;
        }

        public DotNetObjectReference<Select2<TValue>> DotNetRef;
        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {

            try
            {
                if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                {
                    result = (TValue)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(TValue)));
                }
                else
                {
                    result = (TValue)Convert.ChangeType(value, typeof(TValue));
                }
                validationErrorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                validationErrorMessage = ex.ToString();
            }

            throw new InvalidOperationException($"{GetType()} does not support the type '{typeof(TValue)}'.");
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
                await JSRuntime.InvokeVoidAsync("select2Component.init", Id, ModalId);
                await JSRuntime.InvokeVoidAsync("select2Component.onChange", Id, DotNetRef, "Change_SelectWithFilterBase");
            }
        }

        [JSInvokable("Change_SelectWithFilterBase")]
        public void Change(string value, string key = null)
        {
            logger.LogWarning($"{Id}-{value}");
            try
            {
                if (string.IsNullOrWhiteSpace(value) || value.Contains("Select a"))
                {
                    Value = default;
                    ValueChanged.InvokeAsync(Value);
                }
                else
                {
                    TValue temp;
                    if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                    {
                        temp = (TValue)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(TValue)));
                    }
                    else
                    {
                        temp = (TValue)Convert.ChangeType(value, typeof(TValue));
                    }
                    if (!(Value?.Equals(temp)).GetValueOrDefault())
                    {
                        Value = temp;
                        ValueChanged.InvokeAsync(Value);
                    }
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
}
