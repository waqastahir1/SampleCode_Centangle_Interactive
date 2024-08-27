using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{
    /// <summary>
    /// Type of organizations, 1-Primary, 9- Internal, 64- External
    /// </summary>
    [Flags]
    public enum OrganizationType
    {
        Unknown = 0,
        /// <summary>
        /// An organization under which all parties (people and organiations) fall i.e. SOS
        /// </summary>
        PrimaryOrganization = 1,

        RegionalControlCenter = 2 | PrimaryOrganization,

        SubRegionalControlStation = 4 | PrimaryOrganization,

        Station = 6 | PrimaryOrganization,

        Crew = 8 | PrimaryOrganization,

        SOSCPC = 16 | PrimaryOrganization,

        Vault = 32 | PrimaryOrganization,

        [Display(Name = "Vault on wheels")]
        VaultOnWheels = 64 | Vault | PrimaryOrganization,

        /// <summary>
        /// An organization rendering services to primary or internal origanizations i.e. Banks, Bank Branches etc
        /// </summary>
        ExternalOrganization = 1024,

        /// <summary>
        /// Main customer is top level entity, like Bank Alfalah, it is an abstract kind of entity which has concrete presence in the form of branches.
        /// </summary>
        MainCustomer = 2048 | ExternalOrganization,

        /// <summary>
        /// 5120, Out of All customer branches, some are billed and we call those organization customers.
        /// </summary>
        Customer = 4096 | ExternalOrganization,

        /// <summary>
        /// 9216 All of bank branches, Shall be used to check flag instead of direct comparison
        /// </summary>
        CustomerBranch = 8192 | ExternalOrganization,

        ATM = 16384 | CustomerBranch,

        CashManager = 32768 | CustomerBranch,

        /// <summary>
        /// 74,752
        /// </summary>
        Dedicated = 65536 | CustomerBranch,

        HillyArea = 131072 | CustomerBranch,

        /// <summary>
        /// 271,360
        /// </summary>
        Normal = 262144 | CustomerBranch
    }
}
