using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Common.Extenstions
{
    static public class DeliveryChargesExtensions
    {
        public static DeliveryChargesViewModel ToViewModel(this ShipmentCharge[] charges, int consignmentId)
        {
            var viewModel = new DeliveryChargesViewModel()
            {
                ConsignmentId = consignmentId,

            };

            if (charges == null)
            {
                return viewModel;
            };

            foreach (var d in charges)
            {
                switch (d.ChargeTypeId)
                {
                    case 1: viewModel.WaitingCharges = d.Amount; break;
                    case 2: viewModel.TollCharges = d.Amount; break;
                }
            }
            return viewModel;
        }

        public static ShipmentCharge[] ToModel(this DeliveryChargesViewModel vm)
        {
            if (vm == null)
            {
                throw new ArgumentNullException("View model is passed null Extension method: ToModel");
            }

            List<ShipmentCharge> charges = new List<ShipmentCharge>();
            return charges.ToArray();
        }
    }
}
