using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class DenominationChangeAmountViewModel
    {
        public int prevAmount { get; set; }
        [Range(1, 300000000, ErrorMessage = "Amount cannot be greater then three hundred million.")]
        public int newAmount { get; set; }
    }
}
