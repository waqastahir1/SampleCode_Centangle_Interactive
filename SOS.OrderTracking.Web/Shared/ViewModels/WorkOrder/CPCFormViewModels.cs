using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class CPCFormViewModel : ConsignmentFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select Customer")]
        public int CustomerId
        {
            get; set;
        }

        public int? Currency10x
        {
            get; set;
        }

        public int? Currency20x
        {
            get; set;
        }

        public int? Currency50x
        {
            get; set;
        }
        public int? Currency75x
        {
            get; set;
        }

        public int? Currency100x
        {
            get; set;
        }

        public int? Currency500x
        {
            get; set;
        }

        public int? Currency1000x
        {
            get; set;
        }

        public int? Currency5000x
        {
            get; set;
        }
    }
    public class CPCListViewModel : ConsignmentListViewModel
    {

    }
}


