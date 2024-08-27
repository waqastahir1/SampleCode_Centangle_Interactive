namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class DeliveryChargesViewModel
    {
        //public decimal BaseCharges
        //{
        //    get; set;
        //}
        //public decimal Surcharge
        //{
        //    get; set;
        //}
        //public decimal AdditionalCharges
        //{
        //    get; set;
        //}
        //public decimal SealCharges
        //{
        //    get; set;
        //}
        //public decimal OverTimeCharges
        //{
        //    get; set;
        //}
        //public decimal DistanceCharges
        //{
        //    get; set;
        //}
        //public decimal ExtraCharges
        //{
        //    get; set;
        //}
        public decimal WaitingCharges
        {
            get; set;
        }
        public decimal TollCharges
        {
            get; set;
        }
        public int ConsignmentId
        {
            get; set;
        }
        public int DeliveryId
        {
            get; set;
        }

    }

    public class DeliveryChargesModel
    {

        public int ChargeTypeId { get; set; }

        public int ConsignmentId { get; set; }


        public decimal Amount { get; set; }

        public int Status { get; set; }

        public string Tag { get; set; }

        public string ChargeType { get; set; }
    }
}
