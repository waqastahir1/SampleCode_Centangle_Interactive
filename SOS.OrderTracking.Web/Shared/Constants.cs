namespace SOS.OrderTracking.Web.Shared
{
    public static class Constants
    {
        public static class Roles
        {
            public const string SUPER_ADMIN = "Super-Admin";

            public const string ADMIN = "SOS-Admin";

            public const string REGIONAL_ADMIN = "SOS-Regional-Admin";

            public const string SUBREGIONAL_ADMIN = "SOS-SubRegional-Admin";

            public const string HEADOFFICE_BILLING = "SOS-Headoffice-Billing";

            public const string BANK_BRANCH = "BankBranch";

            public const string BANK_BRANCH_MANAGER = "BankBranchManager";

            public const string BANK_CPC = "BankCPC";

            public const string BANK_CPC_MANAGER = "BankCPCManager";

            public const string BANK_HYBRID = "BankHybrid";

            public const string BANK = "BANK";
            public const string VAULT_MANAGER = "VAULT";

            public static readonly string[] Initiators;
            public static readonly string[] Supervisors;

            static Roles()
            {
                Initiators = new string[] { BANK_BRANCH, BANK_CPC, BANK_HYBRID };
                Supervisors = new string[] { BANK_BRANCH_MANAGER, BANK_CPC_MANAGER, BANK_HYBRID };
            }
        }
    }
}
