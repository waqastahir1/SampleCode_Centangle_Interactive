namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class ListItem
    {
        public ListItem()
        {

        }


        public ListItem(int id, string code, string name, string AdditionalValue = null)
        {
            IntId = id;
            Code = code;
            Name = name;
            this.AdditionalValue = AdditionalValue;
        }

        public ListItem(string id, string code, string name, string AdditionalValue = null)
        {
            Id = id;
            Code = code;
            Name = name;
            this.AdditionalValue = AdditionalValue;
        }

        public int? IntId
        {
            get
            {
                if (int.TryParse(Id, out int value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                Id = value?.ToString();
            }
        }

        public string AdditionalValue { get; set; }

        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
