using System;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class CrewListModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ThruDate { get; set; }
    }
}
