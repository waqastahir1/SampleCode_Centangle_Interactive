using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Gaurds
{
    public class GaurdListViewModel
    {
        public int RelationshipId { get; set; }
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
