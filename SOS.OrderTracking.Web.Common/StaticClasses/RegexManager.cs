using System;
using System.Text.RegularExpressions;

namespace SOS.OrderTracking.Web.Common.StaticClasses
{
    public class RegexManager
    {
        public static bool IsEmailValid(string pEmail)
        {
            return Regex.IsMatch(pEmail,
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        public static bool IsPhnoValid(string phno)
        {
            return Regex.IsMatch(phno, @"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        public static bool IsCNICValid(string cnic)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(cnic))
            {
                Regex check = new Regex(@"^[0-9]{5}-[0-9]{7}-[0-9]{1}$");
                Regex onlyDigits = new Regex(@"^[0-9]{13}$");
                isValid = check.IsMatch(cnic) || onlyDigits.IsMatch(cnic);
            }
            return isValid;
        }
    }
}
