using System;

namespace SOS.OrderTracking.Web.Shared.ATMR
{

    public class ATMRBaseModel
    {
        public int AtmrServiceId { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public ATMRBaseModel()
        {

        }

        public ATMRBaseModel(int atmrServiceId)
        {
            AtmrServiceId = atmrServiceId;

        }

    }

    public class AtmrQrCodeModel : ATMRBaseModel
    {
        public string QrCode { get; set; }


    }

    public class CheckListPostModel
    {
        public int AtmrServiceId { get; set; }

        public ChecklistStatus[] ChecklistStatuses { get; set; }
    }

    public class ChecklistStatus
    {
        public int Id { get; set; }

        public bool Status { get; set; }
    }

    public class AtmrSealCodesModel : ATMRBaseModel
    {
        public string[] SealCodes { get; set; }
    }

    public class AtmrCashCounterModel : ATMRBaseModel
    {
        public int Currency500x { get; set; }
        public int Currency1000x { get; set; }
        public int Currency5000x { get; set; }

        public string CounterId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Snap { get; set; }

        public int Iteration { get; set; }
    }

    public class AtmCarImage : ATMRBaseModel
    {
        public string Image { get; set; }
    }

    public class AtmCarImages : ATMRBaseModel
    {
        public string[] Images { get; set; }
    }


    public class RequestCITViewModel : ATMRBaseModel
    {
        public string CITPickupQrCode { get; set; }

        public string CITDropoffQrCode { get; set; }

        public int ShipmentId { get; set; }

    }

    public class RequestReturnCITViewModel : ATMRBaseModel
    {
        public string ReturnCITPickupQrCode { get; set; }
    }


}
