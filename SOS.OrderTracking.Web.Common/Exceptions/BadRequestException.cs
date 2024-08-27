using System;

namespace SOS.OrderTracking.Web.Common.Exceptions
{

    public class BadRequestException : Exception
    {
        public BadRequestException()
        {

        }
        public BadRequestException(string message) : base(message)
        {

        }
    }
}
