using System;

namespace SOS.OrderTracking.Web.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {

        }
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
