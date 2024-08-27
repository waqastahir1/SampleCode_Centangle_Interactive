using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultConsignmentByQRViewModel
    {
        public VaultConsignmentByQRViewModel()
        {
            Seals = new List<string>();
        }

        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Shipment No Required")]

        public int? ShipmentId { get; set; }
        public string Shipment { get; set; }
        [Required]
        [Range (1, int.MaxValue, ErrorMessage ="Please Select Vault")]
        public int VaultId { get; set; }
        
        [Required]
        [Range (1, int.MaxValue, ErrorMessage ="Please Select Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Crew")]
        public int CrewInId { get; set; }
        public string Crew { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select From Branch")]
        public int FromBranchId { get; set; }
        public string FromBranch { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select To Branch")]
        public int ToBranchId { get; set; }
        public string ToBranch { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Bags Required")]
        public int BagsIn { get; set; }
        public int ShipmentBags { get; set; }
        public string ShipmentAmount { get; set; }
        public decimal AmountIn { get; set; }
        public DateTime TimeIn { get; set; } = DateTime.Now;
        public DateTime RequiredTimeOut { get; set; } = DateTime.Now;
        public string? VaultedBy { get; set; }
        public bool OpenSecondForm { get; set; }
        public List<string> Seals { get; set; }
    }


    public class VaultInManualByQRViewModel
    {
        public VaultInManualByQRViewModel()
        {
            Seals = new List<string>();
        }

        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Shipment No Required")]
        public int ShipmentNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Vault")]
        public int VaultId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select From Branch")]
        public int FromBranchId { get; set; }
        public string FromBranch { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Delivery Branch")]
        public int ToBranchId { get; set; }
        public string ToBranch { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select To Chief Crew")]
        public int ChiefCrewId { get; set; }
        public string ChiefCrew { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Bags Required")]
        public int BagsIn { get; set; }

        public decimal AmountIn { get; set; }

        public DateTime TimeIn { get; set; } = DateTime.Now;
        public DateTime RequiredTimeOut { get; set; } = DateTime.Now.AddDays(1);
        public string? VaultedBy { get; set; }
        public List<string> Seals { get; set; }
    }

}
