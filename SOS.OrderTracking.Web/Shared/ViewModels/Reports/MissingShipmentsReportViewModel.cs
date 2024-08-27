using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Reports
{
    public class MissingShipmentsReportViewModel
    {
        public MissingShipmentsReportViewModel()
        {
            MissingShipmentsList = new List<double>();
        }
        public double Id { get; set; }
        public double MissingShipmentNumberFrom { get; set; }
        public double? MissingShipmentNumberTo { get; set; }
        public double PreviousShipmentNumberProcessed { get; set; }
        public string PreviousShipmentNumber { get; set; }
        public string PreviousShipmentPickup { get; set; }
        public string PreviousShipmentDropOff { get; set; }
        public string PreviousShipmentCity { get; set; }
        public DateTime PreviousShipmentDate { get; set; }
        public string PreviousShipmentDateString { get { return PreviousShipmentDate.ToString("dd/MM/yyyy"); } }
        public double NextShipmentNumberProcessed { get; set; }
        public string NextShipmentNumber { get; set; }
        public string NextShipmentPickup { get; set; }
        public string NextShipmentDropOff { get; set; }
        public string NextShipmentCity { get; set; }
        public DateTime NextShipmentDate { get; set; }
        public string NextShipmentDateString { get { return NextShipmentDate.ToString("dd/MM/yyyy"); } }
        public string MissingShipments { get { return (MissingShipmentNumberTo == null ? MissingShipmentNumberFrom.ToString() : $"{MissingShipmentNumberFrom} - {MissingShipmentNumberTo} ({MissingShipmentNumberTo - MissingShipmentNumberFrom} Shipments Missing)"); } }
        public string SimilarRecords { get; set; }
        public List<double> MissingShipmentsList { get; set; }
    }

    public class RangeMissingShipmentReportViewModel
    {
        public DateTime ForMonth { get; set; }
        public string ForMonthString { get { return ForMonth.ToString("MMM, yyyy"); } }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string Station { get; set; }
        public string CrewOrClient { get; set; }
        public string MissingShipmentNumber { get; set; }
    }
}
