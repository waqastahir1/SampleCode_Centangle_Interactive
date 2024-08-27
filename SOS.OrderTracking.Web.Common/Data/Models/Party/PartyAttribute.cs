using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class PartyAttribute
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int AttributeType { get; set; }

        public string Value { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ThruDate { get; set; }

        [Column("PartyId")]
        public int FkPartyId { get; set; }

        public virtual Party Party { get; set; }
    }
}
