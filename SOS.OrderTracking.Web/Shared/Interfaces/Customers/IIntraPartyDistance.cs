using SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance;
using System;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface IIntraPartyDistance
   : ICrudService<IntraPartyDistanceFormViewModel, IntraPartyDistanceListViewModel, Tuple<int, int>, IntraPartyDistanceAdditionalValueViewModel>
    {

    }
}
