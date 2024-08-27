using NetTopologySuite.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string FCMToken { get; set; }


        public string[] IMEI { get; set; }

    }

    public class RegisterModel : LoginModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Compare("Password")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string PasswordConfirmation { get; set; }

    }

    public class LoginResponse
    {
        public string Token { get; set; }

    }

    public class LoginV2Response
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

    }

    public class UserInfo
    {
        [Required]
        public string UserName { get; set; }

        public string Email { get; set; }
         
        public string Name { get; set; }

        public string EmpCode { get; set; }

        public string ImageLink { get; set; }

        public string RegionName { get; set; }
         

        public string StationName { get; set; }
         
        public int? CrewId { get; set; }

        /// <summary>
        /// Base64 Encoded display picture
        /// </summary>
        public string Avatar { get; set; }

        [Required]
        public IEnumerable<string> Roles { get; set; }
        public string CrewName { get; internal set; }

        public List<CrewMember> CrewMembers { get; set; }
    }

    public class CrewMember
    {  
        public string Name { get; set; }

        public string Designation { get; set; }

        public string ImageLink { get; set; }
    }

    public class CrewAssemblyDto
    {
        [Required]
        public string CheifCrew { get; set; }
         
        public string AssitantCrew { get; set; }
         
        public string Gaurd { get; set; }
         
        public string Driver { get; set; }

    }
    public class CrewAssemblyResponseDto
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }

    }
}
