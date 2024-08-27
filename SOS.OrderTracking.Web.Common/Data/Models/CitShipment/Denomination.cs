using SOS.OrderTracking.Web.Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    [Table("ConsignmentDenomination")]
    public partial class Denomination
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ConsignmentId { get; set; }

        public int Currency1x { get; set; }
        public int Currency2x { get; set; }
        public int Currency5x { get; set; }
        public int Currency10x { get; set; }
        public int Currency20x { get; set; }
        public int Currency50x { get; set; }
        public int Currency75x { get; set; }
        public int Currency100x { get; set; }
        public int Currency500x { get; set; }
        public int Currency1000x { get; set; }
        public int Currency5000x { get; set; }

        public int Currency200x { get; set; }
        public int Currency750x { get; set; }
        public int Currency1500x { get; set; }
        public int Currency7500x { get; set; }
        public int Currency15000x { get; set; }
        public int Currency25000x { get; set; }
        public int Currency40000x { get; set; }

        public int PrizeMoney100x { get; set; }
        public int PrizeMoney200x { get; set; }
        public int PrizeMoney750x { get; set; }
        public int PrizeMoney1500x { get; set; }
        public int PrizeMoney7500x { get; set; }
        public int PrizeMoney15000x { get; set; }
        public int PrizeMoney25000x { get; set; }
        public int PrizeMoney40000x { get; set; }


        /// <summary>
        /// This flag indicates that we are what type of denomination,
        /// In CIT we will store Cash Leafs (loose cash) count
        /// IN CPC we store Bundle counts and CashLeafs (loose cash)
        /// </summary>
        public DenominationType DenominationType { get; set; }

        public virtual Consignment FkWorkOrder { get; set; }
    }
}
