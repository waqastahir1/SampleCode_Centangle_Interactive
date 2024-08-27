using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Common.Exceptions
{
    public class InvalidShipmentDataException : Exception
    {
        public InvalidShipmentDataException(string message, ConsignmentStatus consignmentStatus) : base(message)
        {
            ConsignmentStatus = consignmentStatus;
        }

        public ConsignmentStatus ConsignmentStatus { get; }
    }

    public class CalculatedShipmentDataException : Exception
    {
        public CalculatedShipmentDataException(string message) : base(message)
        {

        }

    }
}
