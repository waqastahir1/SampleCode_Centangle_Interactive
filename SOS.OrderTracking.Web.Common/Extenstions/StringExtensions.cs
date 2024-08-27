using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SOS.OrderTracking.Web.Common.Extenstions
{
    public static class StringExtensions
    {
        public static string FirstFromCommaSeperated(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            else if (value.Contains(", ")) return value.Split(", ").First();
            else return value;
        }
        public static List<string> SeperateFromComma(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            else if (value.Contains(',')) return value.Split(',').ToList();
            else return new List<string> { value };
        }

        public static int TransformIntFromNumerics(this string value)
        {
            try
            {
                var doubleValue= Convert.ToDouble(Regex.Replace(value, "[^0-9]", ""));
                var intValue = Convert.ToInt32(doubleValue);
                return intValue;
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }
        public static double TransformdoubleFromNumerics(this string value)
        {
            try
            {
                return Convert.ToDouble(Regex.Replace(value, "[^0-9]", ""));
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }
    }
}
