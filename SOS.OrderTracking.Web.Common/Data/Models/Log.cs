using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TimeStamp { get; set; }
        [Required]
        [StringLength(200)]
        public string TableName { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [StringLength(100)]
        public string ActionBy { get; set; }
    }
}
