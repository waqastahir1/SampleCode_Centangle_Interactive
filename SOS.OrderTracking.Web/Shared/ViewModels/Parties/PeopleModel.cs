namespace SOS.OrderTracking.Web.Shared.ViewModels.Parties
{
    public class PartyModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactNo { get; set; }

        public int ParentId { get; set; }
    }

}
