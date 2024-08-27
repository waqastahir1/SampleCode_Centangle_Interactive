using Microsoft.AspNetCore.Components.Forms;

namespace SOS.OrderTracking.Web.Portal.Components
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
