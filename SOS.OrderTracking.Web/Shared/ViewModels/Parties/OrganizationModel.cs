using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.Parties;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    /// <summary>
    /// This is actually PartyModel which can hold data both from People
    /// and Organization, this should later be converted to PeopleModel
    /// </summary>
    public class OrganizationModel : PartyModel
    {
        public OrganizationType OrganizationType { get; set; }

        public OrganizationModel()
        {

        }
        public OrganizationModel(int id, string shortName, string formalName, OrganizationType organizationType)
        {
            Id = id;
            Code = shortName;
            Name = formalName;
            OrganizationType = organizationType;
        }
    }
}
