using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class DistanceUpdateResult
    {
        public List<SelectListItem> Repushed { get; set; }

        public List<SelectListItem> Updated { get; set; }
    }
}
