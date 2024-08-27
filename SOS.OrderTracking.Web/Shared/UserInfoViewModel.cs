using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared
{
    public class UserInfoViewModel
    {
        public class UserInfo
        {
            [Required]
            public string UserName { get; set; }

            public string Email { get; set; }

            [Required]
            public string Name { get; set; }
            public int PartyId { get; set; }

            public string RegionName { get; set; }

            public string SubRegionName { get; set; }

            public string StationName { get; set; }

            public int RegionId { get; set; }

            public int SubRegionId { get; set; }

            public int StationId { get; set; }

            public int CrewId { get; set; }

            /// <summary>
            /// Base64 Encoded display picture
            /// </summary>
            public string Avatar { get; set; }

            [Required]
            public IList<string> Roles { get; set; }
            public string UserId { get; set; }
        }
    }
}
