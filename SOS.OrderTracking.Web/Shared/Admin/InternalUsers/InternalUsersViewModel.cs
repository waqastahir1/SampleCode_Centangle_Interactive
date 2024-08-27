using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class InternalUsersViewModel : ViewModelBase
    {

        //  readonly UserManager<ApplicationUser> userManager;

        private string _Id;

        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                NotifyPropertyChanged();
            }
        }


        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool IsEnabled { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select an option")]
        public int PartyId { get; set; }


    }
}
