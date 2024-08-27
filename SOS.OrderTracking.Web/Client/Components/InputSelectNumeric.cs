using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Components
{
    public class InputSelectNumeric<T> : InputSelect<T>
    {
        protected override bool TryParseValueFromString(string value, out T result, out string validationErrorMessage)
        {
            if (int.TryParse(value, out var resultInt))
            {
                result = (T)(object)resultInt;
                validationErrorMessage = null;
                return true;
            }
            validationErrorMessage = null;
            result = default(T);
            return true;
        }
    }
}
