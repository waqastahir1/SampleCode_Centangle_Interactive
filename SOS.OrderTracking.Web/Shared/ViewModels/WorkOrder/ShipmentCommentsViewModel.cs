using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ShipmentCommentsViewModel
    {
        public int ConsignmentId { get; set; }
        public string CommentText { get; set; }
        public List<ShipmentComment> ShipmentComments { get; set; }

    }
    public class ShipmentComment
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ViewedBy { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
