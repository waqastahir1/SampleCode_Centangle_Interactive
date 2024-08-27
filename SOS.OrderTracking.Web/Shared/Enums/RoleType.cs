using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{
    [Flags]
    public enum RoleType
    {

        ParentOrganization = 1,
        ChildOrganization = 2,

        Employer = 4 | ParentOrganization,
        Crew = 8 | ParentOrganization,
        Vault = 16 | ParentOrganization,
        ServiceProvider = 32 | ParentOrganization,
        RegionalOrg = 36 | ParentOrganization,
        SubRegionalOrg = 38 | ParentOrganization,
        StationOrg = 40 | ParentOrganization,
        
        Consignment = 52,
        ChangedDropoff = 56,

        Employee = 64,

        [Display(Name = "Cheif Crew")]
        CheifCrewAgent = 128 | Employee,

        [Display(Name = "Assistant Crew")]
        AssistantCrewAgent = 256 | Employee,

        [Display(Name = "Driver")]
        CrewDriver = 512 | Employee,

        [Display(Name = "Gaurd")]
        CrewGuard = 1024 | Employee,


        /// <summary>
        /// Cit Caurd
        /// </summary>
        Gaurd = 2048 | Employee,

        RegionalHead = 4096 | Employee,
        SubRegionalHead = 8192 | Employee,

        /// <summary>
        /// ATM Roles
        /// </summary>
        ATMCashier = 16384 | Employee,
        ATMTechnician = 32768 | Employee,

        /// <summary>
        /// 65600
        /// </summary>
        VaultIncharge = 65536 | Employee,

        VaultOfficer = 131072 | Employee,

        Customer = 262144,
        BankCPC = 524288,
        ATM = 1048576,
        ATMBranch = 2097152,


        SubOrdinate = 4194304,
        SuperOrdinate = 8388608,

        /// <summary>
        /// 16777280
        /// </summary>
        CPCIncharge = 16_777_216 | Employee,

        CPCManualSorter = 33_554_432 | Employee,

        CPCAutoSorter = 67_108_864 | Employee
    }
}
