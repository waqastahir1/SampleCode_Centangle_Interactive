using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Enums
{
    public enum CPCConsignmentDisposalState : byte
    {
        NotStarted = 0,

        [Display(Name = "Partially Disposed")]
        PartiallyDisposed = 1,

        [Display(Name = "Full Disposed")]
        FullyDisposed = 2,


    }

    public enum CPCTransactionReason : short
    {
        CustomerDeposite = 1,
        Step1Verification = 2,
        Step2Verification = 4,
        VaultIn = 8,
        VaultOut = 16,
        ManualProcessingDistibute = 32,
        ManualProcessingCollection = 64,
        MachineProcessingDistibute = 128,
        MachineProcessingCollection = 256,
        CustmerDisposal = 1024

    }

    public enum CashNature : short
    {
        UnProcessed = 1,
        Processed = 2
    }

    public enum CashType : short
    {
        ReIssuable = 1,
        Soiled = 2,
        MachineRejected = 4,
        Counterfeit = 8,
        Others = 16

    }
}
