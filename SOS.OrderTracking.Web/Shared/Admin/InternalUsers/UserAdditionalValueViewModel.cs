namespace SOS.OrderTracking.Web.Shared.ViewModels.Users
{
    public class UserAdditionalValueViewModel : BaseIndexModel
    {
        public string RoleTypeId { get; set; }
        public string SearchKey { get; set; }
        public int MainCustomerId { get; set; }

        public int BankBranchId { get; set; }

        public override string ToQueryString()
        {
            return base.ToQueryString() + $"&{nameof(RoleTypeId)}={RoleTypeId}&{nameof(SearchKey)}={SearchKey}&{nameof(MainCustomerId)}={MainCustomerId}&{nameof(BankBranchId)}={BankBranchId}";
        }
    }
}
