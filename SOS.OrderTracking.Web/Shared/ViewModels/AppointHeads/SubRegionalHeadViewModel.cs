using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class SubRegionalHeadViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private int _regionId;

        public int RegionId
        {
            get { return _regionId; }
            set
            {
                _regionId = value;
                NotifyPropertyChanged();
            }
        }

        public string RegionName { get; set; }
        public int SubRegionId { get; set; }

        public string SubRegionName { get; set; }
        [Required(ErrorMessage = "Please Select Employee")]
        public int EmployeeId { get; set; }
        public bool ChangeRelationship { get; set; }
        public int RelationshipId { get; set; }

        [Required(ErrorMessage = "Please Enter Start Date.")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
