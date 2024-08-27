using System;

namespace SOS.OrderTracking.Web.Common.Exceptions
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException()
        {

        }
        public UnAuthorizedException(string message) : base(message)
        {

        }
    }
}
