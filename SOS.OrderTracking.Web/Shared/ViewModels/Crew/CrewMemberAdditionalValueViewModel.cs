namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewMemberAdditionalValueViewModel : BaseIndexModel
    {
        public int CrewId { get; set; }

        public bool OnlyActive { get; set; }
    }
}
