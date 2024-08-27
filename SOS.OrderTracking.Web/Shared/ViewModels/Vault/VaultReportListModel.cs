using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultReportFormModel
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int VehicleOutId { get; set; }
        public string VehicleOut { get; set; }
    }
    
    public class VaultReportListModel
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int VaultId { get; set; }
        public string Vault { get; set; }
        public string VehicleIn { get; set; }
        public string VehicleOut { get; set; }
        public string ChiefCrewIn { get; set; }
        public string ChiefCrewOut { get; set; }
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }
        public int BagsIn { get; set; }
        public decimal AmountIn { get; set; }
        public DateTime TimeIn { get; set; }
        public string TimeInString { get{ return TimeIn.ToString("dd-MM-yyyy"); } }
        public DateTime? TimeOut { get; set; }
        public string TimeOutString { get { return TimeOut==null?"-": TimeOut.Value.ToString("dd-MM-yyyy"); } }
        public DateTime RequiredTimeOut { get; set; }
        public string VaultedBy { get; set; }
        public bool IsVaulted { get; set; }
        public List<string> VaultedSeals { get; set; }
        public string VaultedSealsString { get { return string.Join(", ", VaultedSeals); } }
    }
}
