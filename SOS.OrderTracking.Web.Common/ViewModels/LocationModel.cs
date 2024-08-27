using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Shared.Enums;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class LocationModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public LocationType Type { get; set; }

        public LocationModel()
        {

        }
        public LocationModel(Location model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Type = model.Type;
        }
    }
}
