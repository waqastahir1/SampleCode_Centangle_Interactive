using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{

    public class DenominationBaseViewModel : ViewModelBase
    {

        public int Id { get; set; }


        public int ConsignmentId { get; set; }

        public DenominationType Type { get; set; }

        [Required(ErrorMessage = "Select Currency Type")]
        public CurrencySymbol CurrencySymbol { get; set; }


        public int Factor => (Type == DenominationType.Bundles) ? 1000 :
                    (Type == DenominationType.Packets) ? 100 : 1;

        public string ShipmentCode { get; set; }

        public int? Currency10x { get; set; }

        public int? Currency20x { get; set; }

        public int? Currency50x { get; set; }

        public int? Currency75x { get; set; }
        public int? Currency100x { get; set; }

        public int? Currency500x { get; set; }

        public int? Currency1000x { get; set; }

        public int? Currency5000x { get; set; }
        public int? Currency200x { get; set; }
        public int? Currency750x { get; set; }
        public int? Currency1500x { get; set; }
        public int? Currency7500x { get; set; }
        public int? Currency15000x { get; set; }
        public int? Currency25000x { get; set; }
        public int? Currency40000x { get; set; }
        public int? PrizeMoney100x { get; set; }
        public int? PrizeMoney200x { get; set; }
        public int? PrizeMoney750x { get; set; }
        public int? PrizeMoney1500x { get; set; }
        public int? PrizeMoney7500x { get; set; }
        public int? PrizeMoney15000x { get; set; }
        public int? PrizeMoney25000x { get; set; }
        public int? PrizeMoney40000x { get; set; }



        public int TotalAmount { get; set; }


    }

    public class CitDenominationViewModel : DenominationBaseViewModel
    {
        public decimal ExchangeRate { get; set; }

        public string Valuables { get; set; }

        public bool SaveNewAmount { get; set; }

        public bool FinalizeShipment { get; set; }

        public bool EnableSkip { get; set; }

        public bool NewShipment { get; set; }
        public int? Currency1x { get; set; }

        public int? Currency2x { get; set; }

        public int? Currency5x { get; set; }

        public int AmountPKR { get; set; }
        public int DedicatedVehicle { get; set; }

    }

    public class CpcTransactionFormModel : DenominationBaseViewModel
    {
        public CPCTransactionReason CPCTransactionReason { get; set; }

        public int DrPartyId { get; set; }

        public string DrPartyName { get; set; }

        public RoleType DrPartyRole { get; set; }

        public int CrPartyId { get; set; }

        public string CrPartyName { get; set; }

        public RoleType CrPartyRole { get; set; }

        public int EmployeeId { get; set; }

        [Range(minimum: 1, maximum: 2, ErrorMessage = "Select Status of Cash")]
        public CashNature CashNature { get; set; }

        public CashType CashType { get; set; }

    }
}
