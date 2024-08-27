using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class OrganizationViewModel
    {
        public int Id { get; set; }

        public int RelationshipId { get; set; }

        [Required(ErrorMessage = "Please Enter Organization Code.")]
        public string Code { get; set; }


        [Required(ErrorMessage = "Please Enter Name.")]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Organization Type")]
        public OrganizationType OrganizationType { get; set; }

        public int? AssociatedBankCPCId { get; set; }

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select SOS-CPC")]
        public int AssociatedSOSCPCId { get; set; }

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Region")]
        public int RegionId { get; set; }


        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please Select Area")]
        public int AreaId { get; set; }


        [Required(ErrorMessage = "Please Enter Address.")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Please Enter Contact Number.")]
        public string ContactNo { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }

        public MemoryStream MemoryStream { get; set; }

        public bool Uploaded { get; set; }
    }
}
