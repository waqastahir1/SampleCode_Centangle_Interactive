using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Client.Components
{
    public partial class Select2Ajax<TValue> : InputBase<TValue>
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string ModalId { get; set; }

        [Parameter]
        public string AjaxUrl { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string Id1 { get; set; }

        [Parameter]
        public string Id2 { get; set; }

        [Inject]
        ILogger<Select2<TValue>> logger { get; set; }

        [Inject]
        IJSRuntime JSRuntime
        {
            get; set;
        }

        public DotNetObjectReference<Select2Ajax<TValue>> DotNetRef;
        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {

            try
            {
                result = (TValue)Convert.ChangeType(value, typeof(TValue));
                validationErrorMessage = null;
                return true;
            }
            catch(Exception ex)
            {
                validationErrorMessage = ex.ToString();
            }
           
            throw new InvalidOperationException($"v1/{GetType()} does not support the type '{typeof(TValue)}'.");
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
                await JSRuntime.InvokeVoidAsync("selectAjax2Component.init", Id, ModalId, AjaxUrl, Value, Text, Id1, Id2);
                await JSRuntime.InvokeVoidAsync("selectAjax2Component.onChange", Id, DotNetRef, "Change_SelectWithFilterBase");
            }
        }

        [JSInvokable("Change_SelectWithFilterBase")]
        public void Change(string value, string key = null)
        {
          try
            {
                if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                {
                    Value = (TValue)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(TValue)));
                }
                else
                {
                    Value = (TValue)Convert.ChangeType(value, typeof(TValue));
                }
                ValueChanged.InvokeAsync(Value);
                
            }
            catch(Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
    }
     
}
