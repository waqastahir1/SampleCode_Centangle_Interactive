using System;

namespace SOS.OrderTracking.Web.Common.GBMS.Models
{
    public class XShipmentAttachmentForPortal
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; }
        public string WorkflowStatus { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public decimal XNumber { get; set; }
        public DateTime DDate { get; set; }
        public string XShipmentNo { get; set; }
        public string XLink { get; set; }
        public string FileTitle { get; set; }
        public string FileType { get; set; }
    }
}
