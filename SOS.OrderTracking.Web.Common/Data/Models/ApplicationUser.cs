using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public int PartyId { get; set; }

        public string IMEINumber { get; set; }

        /// <summary>
        /// Firebase token
        /// </summary>
        [MaxLength(450)]
        public string FCMToken { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdateddAt { get; set; }

        public DateTime? PasswordResetAt { get; set; }

        public DateTime? ExpireDate { get; set; }

        public int PasswordExpiryInDays { get; set; }

        public string PasswordHistory { get; set; }

        //[MaxLength(450)]
        public string JsonDate { get; set; }
        public virtual ICollection<AllocatedBranch> AllocatedBranches { get; set; }

    }

    public class UserJsonData
    {
        /// <summary>
        /// in case of headoffice login
        /// </summary>
        public List<int> AllocatedBranches { get; set; }
    }
}
