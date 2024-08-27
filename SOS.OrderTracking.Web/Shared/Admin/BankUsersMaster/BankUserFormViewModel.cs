namespace SOS.OrderTracking.Web.Shared.ViewModels.BankUser
{
    public class BankUserFormViewModel
    {
        public string SupervisorUserId { get; set; }
        public string InitiatorUserId { get; set; }
        public string SupervisorUserName { get; set; }
        public string InitiatorUserName { get; set; }
        public string SupervisorEmail { get; set; }
        public string InitiatorEmail { get; set; }
        public bool? IsSupervisorEnabled { get; set; }
        public bool? IsInitiatorEnabled { get; set; }

        //   public string UserName { get; set; }
        //[EmailAddress(ErrorMessage = "Please Provide Valid Email Address")]
        //public string Email { get; set; }
        public string AspNetUserId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int? PartyId { get; set; }
        public string? RoleName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
