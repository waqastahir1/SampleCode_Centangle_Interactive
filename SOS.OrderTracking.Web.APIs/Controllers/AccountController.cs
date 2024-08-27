using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SOS.OrderTracking.Web.APIs.Models;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SOS.OrderTracking.Web.APIs.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly SignInManager<ApplicationUser> signInManager;
        readonly IConfiguration configuration;
        private readonly SequenceService sequenceService;
        readonly ILogger<AccountController> logger;
        private readonly AppDbContext context;

        public AccountController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           IConfiguration configuration,
           RoleManager<IdentityRole> roleManager,
           SequenceService sequenceService,
           ILogger<AccountController> logger,
           AppDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.sequenceService = sequenceService;
            this.logger = logger;
            this.context = context;
        }


        [HttpPost]
        [Route("login")]
        [Produces(typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
#if DEBUG
                var user1 = await userManager.FindByNameAsync(loginModel.Username);
                await signInManager.SignInAsync(user1, true);
                var loginReponse1 = new LoginResponse()
                {
                    Token = CreateJwtToken(user1)
                };
                return Ok(loginReponse1);
#endif

                var loginResult = await signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, isPersistent: false, lockoutOnFailure: false);
                logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(loginResult));

               
                if (!loginResult.Succeeded)
                {
                    if (loginResult.IsLockedOut)
                    {
                        return BadRequest("This user is locked, please contact administrator");
                    }
                    if (loginResult.IsNotAllowed)
                    {
                        return BadRequest("This user is disabled, please contact administrator");
                    }
                    else
                    {
                        return BadRequest("Invalid UserName/Password");
                    }
                  
                }

                var user = await userManager.FindByNameAsync(loginModel.Username);

                if (string.IsNullOrEmpty(user.IMEINumber))
                {
                    user.IMEINumber = loginModel.IMEI?.FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(loginModel.FCMToken))
                {
                    user.FCMToken = loginModel.FCMToken;
                    await userManager.UpdateAsync(user);
                }

                var loginReponse = new LoginResponse()
                {
                    Token = CreateJwtToken(user)
                };
                return Ok(loginReponse);
            }
            return BadRequest(ModelState);
        }


        [HttpPost]
        [Route("loginv2")]
        [Produces(typeof(LoginV2Response))]
        public async Task<IActionResult> LoginV2([FromBody] LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
#if DEBUG
                var user1 = await userManager.FindByNameAsync(loginModel.Username);
                await signInManager.SignInAsync(user1, true);
                var loginReponse1 = new LoginV2Response()
                {
                    Token = CreateJwtToken(user1),
                    RefreshToken = CreateJwtToken(user1)
                };
                return Ok(loginReponse1);
#endif

                var loginResult = await signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, isPersistent: false, lockoutOnFailure: false);
                logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(loginResult));


                if (!loginResult.Succeeded)
                {
                    if (loginResult.IsLockedOut)
                    {
                        return BadRequest("This user is locked, please contact administrator");
                    }
                    if (loginResult.IsNotAllowed)
                    {
                        return BadRequest("This user is disabled, please contact administrator");
                    }
                    else
                    {
                        return BadRequest("Invalid UserName/Password");
                    }

                }

                var user = await userManager.FindByNameAsync(loginModel.Username);

                if (string.IsNullOrEmpty(user.IMEINumber))
                {
                    user.IMEINumber = loginModel.IMEI?.FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(loginModel.FCMToken))
                {
                    user.FCMToken = loginModel.FCMToken;
                    await userManager.UpdateAsync(user);
                }

                var loginReponse = new LoginV2Response()
                {
                    Token = CreateJwtToken(user),
                    RefreshToken = CreateJwtToken(user)
                };
                return Ok(loginReponse);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginModel loginModel)
        {
            var user = await userManager.FindByNameAsync(
               loginModel.Username);
            return Ok(new
            {
              Token =  CreateJwtToken(user)
            });

        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await userManager.FindByNameAsync(
                User.Identity.Name ??
                User.Claims.Where(c => c.Properties.ContainsKey("unique_name")).Select(c => c.Value).FirstOrDefault()
                );
            user.FCMToken = null;
            await userManager.UpdateAsync(user);
            return Ok();
        }


        [Authorize]
        [HttpGet]
        [Route("GetUserInfoBase64")]
        [Produces(typeof(UserInfo))]
        public async Task<IActionResult> GetUserInfoBase64()
        {
            UserInfo result = new UserInfo()
            {
                CrewMembers = new List<CrewMember>()
            };
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var name = User.Claims.Where(c => c.Properties.ContainsKey(JwtRegisteredClaimNames.UniqueName)).Select(c => c.Value).FirstOrDefault();
            var role = context.UserRoles.FirstOrDefault(x => x.UserId == user.Id)?.RoleId;
            if (role == "ATMR")
            {
                var me = await (from p in context.Parties
                                join u in context.Users on p.Id equals u.PartyId
                                from m in context.Parties.Where(x => x.Id == u.PartyId)

                                where u.UserName == User.Identity.Name

                                select new
                                {
                                    Name = p.FormalName,
                                    EmpCode = m.ShortName,
                                    CrewId = p.Id,
                                    m.ImageLink,
                                    u.UserName,
                                    u.Email,
                                    Roles = context.UserRoles.Where(x => x.UserId == u.Id).Select(x => x.RoleId).ToList()
                                }).FirstOrDefaultAsync();

                HttpClient http = new();

                string imageLink = null;

                try
                {
                    imageLink = me.ImageLink == null ? null : Convert.ToBase64String(http.GetByteArrayAsync("http://176.57.184.247:100/F137_SOS/data/" + me.ImageLink.Split("data").LastOrDefault()).Result, Base64FormattingOptions.None);
                }
                catch { }

                return Ok(new UserInfo
                {
                    Name = me.Name,
                    EmpCode = me.EmpCode,
                    ImageLink = imageLink,
                    UserName = me.UserName,
                    Email = me.Email,
                    Roles = me.Roles,
                    CrewId = me.CrewId,
                    CrewName = "ATM Team",
                    CrewMembers = new List<CrewMember> { new CrewMember()
                    {
                        ImageLink = imageLink,
                        Name = me.Name
                    } }
                });
            }
            else
            {
                var crewMembers = await (from crewAssembly in context.Parties
                                         join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                         from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                         from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)

                                         from region in context.Parties.Where(x => x.Id == crewAssembly.RegionId).DefaultIfEmpty()
                                         from station in context.Parties.Where(x => x.Id == crewAssembly.StationId).DefaultIfEmpty()

                                         where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                                 && crewAsseemblyUser.UserName == User.Identity.Name
                                                 && crewMembersRelation.IsActive
                                         select new
                                         {
                                             CrewName = crewAssembly.FormalName,
                                             CrewMemberName = crewMember.FormalName,
                                             EmpCode = crewMember.ShortName,
                                             crewMembersRelation.FromPartyId,
                                             CrewMemberRole = crewMembersRelation.FromPartyRole,
                                             CrewId = crewAssembly.Id,
                                             crewMember.ImageLink,
                                             Region = region.FormalName,
                                             Station = station.FormalName,
                                             crewAsseemblyUser.UserName,
                                             crewMembersRelation.FromPartyRole,
                                             crewAsseemblyUser.Email,
                                             Roles = context.UserRoles.Where(x => x.UserId == crewAsseemblyUser.Id).Select(x => x.RoleId).ToList()
                                         }).ToListAsync();

                //if (crewMembers.Count > 0)
                {
                    var me = crewMembers.OrderBy(x => x.FromPartyRole).FirstOrDefault();

                    HttpClient http = new HttpClient();
                    string imageLink = null;

                try
                {
                    
                    imageLink = me?.ImageLink == null ? null : Convert.ToBase64String(http.GetByteArrayAsync("http://176.57.184.247:100/F137_SOS/data/" + me.ImageLink.Split("data").LastOrDefault()).Result, Base64FormattingOptions.None);
                }
                catch { }

                 result = new UserInfo
                {
                    Name = me?.CrewMemberName ?? "Dummy",
                    EmpCode = me?.EmpCode ?? "Dummy",
                    ImageLink = imageLink,
                    UserName = me?.UserName ?? "Dummy",
                    Email = me?.Email ?? "Dummy",
                    Roles = me?.Roles?? new List<string> { "CIT"},
                    CrewId = crewMembers.FirstOrDefault()?.CrewId?? 0,
                    CrewName = crewMembers.FirstOrDefault()?.CrewName ?? "Dummy",
                    RegionName = crewMembers.FirstOrDefault()?.Region ?? "Dummy",
                    StationName = crewMembers.FirstOrDefault()?.Station ?? "Dummy",
                    CrewMembers = crewMembers.Select(x => new CrewMember()
                    {
                        ImageLink = x.ImageLink,
                        Name = x.CrewMemberName,
                        Designation =  x.FromPartyRole.ToString()
                    }).ToList()
                };

                    for (int i = 0; i < result.CrewMembers.Count(); i++)
                    {

                        try
                        {
                            result.CrewMembers[i].ImageLink = result.CrewMembers[i].ImageLink == null ? null : Convert.ToBase64String(http.GetByteArrayAsync("http://176.57.184.247:100/F137_SOS/data/" + result.CrewMembers[i].ImageLink.Split("data").LastOrDefault()).Result, Base64FormattingOptions.None);
                        }
                        catch { }
                    }
                }
                if (!result.CrewMembers.Any(x => x.Designation == "CheifCrewAgent"))
                {
                    result.CrewMembers.Add(new CrewMember()
                    {
                        Designation = "Cheif Crew Agent"
                    });
                }
                if (!result.CrewMembers.Any(x => x.Designation == "AssistantCrewAgent"))
                {
                    result.CrewMembers.Add(new CrewMember()
                    {
                        Designation = "Assistant Crew Agent"
                    });
                }
                if (!result.CrewMembers.Any(x => x.Designation == "CrewDriver"))
                {
                    result.CrewMembers.Add(new CrewMember()
                    {
                        Designation = "Crew Driver"
                    });
                }
                if (!result.CrewMembers.Any(x => x.Designation == "CrewGuard"))
                {
                    result.CrewMembers.Add(new CrewMember()
                    {
                        Designation = "Crew Guard"
                    });
                }
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("UpdateCrew")]
        [Produces(typeof(UserInfo))]
        [Authorize]
        public async Task<IActionResult> UpdateCrew([FromBody] CrewAssemblyDto vm)
        {
           var crewId =   (from crewAssembly in context.Parties
                   join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                   where crewAsseemblyUser.UserName == User.Identity.Name
                   select crewAssembly.Id).FirstOrDefault();


            var crewMembers = await (from crewAssembly in context.Parties
                                     join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                     from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                     from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)


                                     where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                             && crewAsseemblyUser.UserName == User.Identity.Name
                                             && (crewMembersRelation.IsActive )
                                     select new
                                     {
                                         RelationshipId = crewMembersRelation.Id,
                                         CrewName = crewAssembly.FormalName,
                                         CrewMemberName = crewMember.FormalName,
                                         EmpCode = crewMember.ShortName,
                                         crewMembersRelation.FromPartyId,
                                         crewMembersRelation.FromPartyRole,
                                         crewMember.ImageLink,
                                         crewMember.ShortName,
                                         crewAsseemblyUser.UserName, 
                                         crewAsseemblyUser.Email, 
                                     }).ToListAsync();
            // check only crew members 
            //if (!await (from p in context.People
            //            join party in context.Parties on p.Id equals party.Id
            //            where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.CheifCrew && party.IsActive == true
            //            select p).AnyAsync()) { return BadRequest($"{vm.CheifCrew} is invalid crew member"); }

            //if (!await (from p in context.People
            //            join party in context.Parties on p.Id equals party.Id
            //            where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.AssitantCrew && party.IsActive == true
            //            select p).AnyAsync()) { return BadRequest($"{vm.AssitantCrew} is invalid crew member"); }

            //if (!await (from p in context.People
            //            join party in context.Parties on p.Id equals party.Id
            //            where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.Gaurd && party.IsActive == true
            //            select p).AnyAsync()) { return BadRequest($"{vm.Gaurd} is invalid crew member"); }

            //if (!await (from p in context.People
            //            join party in context.Parties on p.Id equals party.Id
            //            where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.Driver && party.IsActive == true
            //            select p).AnyAsync()) { return BadRequest($"{vm.Driver} is invalid crew member"); }


            ////Vehicle Crew  matched with the relevant region.
            //var CheckRegionIdcrewMembers = await (from crewAssembly in context.Parties
            //                                      join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
            //                                      where crewAsseemblyUser.UserName == User.Identity.Name
            //                                      select new { RegionId = crewAssembly.RegionId, SubregionId = crewAssembly.SubregionId, StationId = crewAssembly.StationId }).FirstOrDefaultAsync();
            //var CheifCrew = await context.Parties.Where(x => x.ShortName == vm.CheifCrew)
            //                    .Select(x => new { RegionId = x.RegionId, SubregionId = x.SubregionId, StationId = x.StationId }).FirstOrDefaultAsync();
            //var AssitantCrew = await context.Parties.Where(x => x.ShortName == vm.AssitantCrew)
            //                        .Select(x => new { RegionId = x.RegionId, SubregionId = x.SubregionId, StationId = x.StationId }).FirstOrDefaultAsync();
            //var Gaurd = await context.Parties.Where(x => x.ShortName == vm.Gaurd)
            //                 .Select(x => new { RegionId = x.RegionId, SubregionId = x.SubregionId, StationId = x.StationId }).FirstOrDefaultAsync();
            //var Driver = await context.Parties.Where(x => x.ShortName == vm.Driver)
            //                  .Select(x => new { RegionId = x.RegionId, SubregionId = x.SubregionId, StationId = x.StationId }).FirstOrDefaultAsync();


            //if (CheifCrew != null && CheckRegionIdcrewMembers != null)
            //{
            //    if (CheckRegionIdcrewMembers.RegionId != CheifCrew.RegionId)
            //    {
            //        return BadRequest("The cheif-crew is not from the same region.");
            //    }
            //    if (CheckRegionIdcrewMembers.RegionId != AssitantCrew.RegionId)
            //    {
            //        return BadRequest("The assitant-crew is not from the same region.");
            //    }
            //    if (CheckRegionIdcrewMembers.RegionId != Gaurd.RegionId)
            //    {
            //        return BadRequest("The gaurd is not from the same region.");
            //    }
            //    if (CheckRegionIdcrewMembers.RegionId != Driver.RegionId)
            //    {
            //        return BadRequest("The driver is not from the same region.");
            //    }
            //}








            //        var overlap = crewMembers.Where(x =>

            //        (x.FromPartyRole == RoleType.CheifCrewAgent && x.ShortName == vm.CheifCrew)
            //         || (vm.AssitantCrew != x.ShortName && x.FromPartyRole == RoleType.AssistantCrewAgent)
            //            || (vm.Gaurd != x.ShortName && x.FromPartyRole == RoleType.Gaurd)
            //            || (vm.Driver != x.ShortName && x.FromPartyRole == RoleType.CrewDriver)

            //     );

            //var disjoint = crewMembers.Where(x => !overlap.Contains(x));


            foreach (var crewMember in crewMembers)
            {

                var relationship = context.PartyRelationships.Find(crewMember.RelationshipId);
                relationship.IsActive = false;
                relationship.ThruDate = MyDateTime.Today.AddSeconds(-1);
                context.SaveChanges();
            }

              crewMembers = await (from crewAssembly in context.Parties
                                      join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                     from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                     from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)


                                     where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                             && crewAsseemblyUser.UserName == User.Identity.Name
                                             && crewMembersRelation.IsActive
                                     select new
                                     {
                                         RelationshipId = crewMembersRelation.Id,
                                         CrewName = crewAssembly.FormalName,
                                         CrewMemberName = crewMember.FormalName,
                                         EmpCode = crewMember.ShortName,
                                         crewMembersRelation.FromPartyId,
                                         crewMembersRelation.FromPartyRole,
                                         crewMember.ImageLink,
                                         crewMember.ShortName,
                                         crewAsseemblyUser.UserName,
                                         crewAsseemblyUser.Email,
                                     }).ToListAsync();


            if (!crewMembers.Any(x => x.ShortName == vm.CheifCrew && x.FromPartyRole == RoleType.CheifCrewAgent))
            {
                AddMember(vm.CheifCrew, crewId, RoleType.CheifCrewAgent, context);
            }
            if (!crewMembers.Any(x => x.ShortName == vm.AssitantCrew && x.FromPartyRole == RoleType.AssistantCrewAgent))
            {
                AddMember(vm.AssitantCrew, crewId, RoleType.AssistantCrewAgent, context);
            }
            if (!crewMembers.Any(x => x.ShortName == vm.Gaurd && x.FromPartyRole == RoleType.CrewGuard))
            {
                AddMember(vm.Gaurd, crewId, RoleType.CrewGuard, context);
            }
            if (!crewMembers.Any(x => x.ShortName == vm.Driver && x.FromPartyRole == RoleType.CrewDriver))
            {
                AddMember(vm.Driver, crewId,  RoleType.CrewDriver, context);
            }
             
            var crewMembersUpdatedList = await (from crewAssembly in context.Parties
                                     join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                     from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                     from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)
                                      
                                     where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                             && crewAsseemblyUser.UserName == User.Identity.Name
                                             && crewMembersRelation.IsActive
                                     select new CrewAssemblyResponseDto
                                     { 
                                         EmpCode = crewMember.ShortName, 
                                         EmpName = crewMember.FormalName
                                     }).ToListAsync();

            return Ok(await GetUserInfoBase64());
        }

        /// <summary>
        /// Updates crew members of the logged in crew if all are different, active, actual crew members and are of same station as of the logged in crew.
        /// Developed on 26-01-2023
        /// </summary>
        /// <param name="vm">Crew member Codes</param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateCrewV2")]
        [Produces(typeof(UserInfo))]
        [Authorize]
        public async Task<IActionResult> UpdateCrewV2([FromBody] CrewAssemblyDto vm)
        {
            var crew = (from crewAssembly in context.Parties
                        join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                        where crewAsseemblyUser.UserName == User.Identity.Name
                        select new { crewAssembly.Id, crewAssembly.StationId }).FirstOrDefault();

            // check only crew members 
            if (!string.IsNullOrEmpty(vm.CheifCrew))
            {
                var member = await (from p in context.People
                                    join party in context.Parties on p.Id equals party.Id
                                    where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.CheifCrew
                                    select party).FirstOrDefaultAsync();
                if (vm.CheifCrew == vm.AssitantCrew && !string.IsNullOrEmpty(vm.AssitantCrew)) return BadRequest("Chief Crew Agent and Assistant Chief Crew Agent Can't be same.");
                else if (vm.CheifCrew == vm.Driver && !string.IsNullOrEmpty(vm.Driver)) return BadRequest("Chief Crew Agent and Driver Can't be same.");
                else if (vm.CheifCrew == vm.Gaurd && !string.IsNullOrEmpty(vm.Gaurd)) return BadRequest("Chief Crew Agent and Guard Can't be same.");

                else if (member == null) return BadRequest($"{vm.CheifCrew} is invalid crew member");
                else if (member.StationId != crew.StationId) return BadRequest($"{vm.CheifCrew}-{member.FormalName} is from different station");
                else if (!member.IsActive) return BadRequest($"{vm.CheifCrew}-{member.FormalName} is a inactive crew member");
            }

            if (!string.IsNullOrEmpty(vm.AssitantCrew))
            {
                var member = await (from p in context.People
                                    join party in context.Parties on p.Id equals party.Id
                                    where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.AssitantCrew
                                    select party).FirstOrDefaultAsync();

                if (vm.AssitantCrew == vm.Driver && !string.IsNullOrEmpty(vm.Driver)) return BadRequest("Assistant Chief Crew Agent and Driver Can't be same.");
                else if (vm.AssitantCrew == vm.Gaurd && !string.IsNullOrEmpty(vm.Gaurd)) return BadRequest("Assistant Chief Crew Agent and Guard Can't be same.");

                else if (member == null) return BadRequest($"{vm.AssitantCrew} is invalid crew member");
                else if (member.StationId != crew.StationId) return BadRequest($"{vm.AssitantCrew}-{member.FormalName} is from different station");
                else if (!member.IsActive) return BadRequest($"{vm.AssitantCrew}-{member.FormalName} is a inactive crew member");
            }

            if (!string.IsNullOrEmpty(vm.Driver))
            {
                var member = await (from p in context.People
                                    join party in context.Parties on p.Id equals party.Id
                                    where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.Driver
                                    select party).FirstOrDefaultAsync();

                if (vm.Driver == vm.Gaurd && !string.IsNullOrEmpty(vm.Gaurd)) return BadRequest("Driver and Guard Can't be same.");

                else if (member == null) return BadRequest($"{vm.Driver} is invalid crew member");
                else if (member.StationId != crew.StationId) return BadRequest($"{vm.Driver}-{member.FormalName} is from different station");
                else if (!member.IsActive) return BadRequest($"{vm.Driver}-{member.FormalName} is a inactive crew member");
            }
            if (!string.IsNullOrEmpty(vm.Gaurd))
            {
                var member = await (from p in context.People
                                    join party in context.Parties on p.Id equals party.Id
                                    where p.EmploymentType == (EmploymentType)2 && party.ShortName == vm.Gaurd
                                    select party).FirstOrDefaultAsync();

                if (member == null) return BadRequest($"{vm.Gaurd} is invalid crew member");
                else if (member.StationId != crew.StationId) return BadRequest($"{vm.Gaurd}-{member.FormalName} is from different station");
                else if (!member.IsActive) return BadRequest($"{vm.Gaurd}-{member.FormalName} is a inactive crew member");
            }


            var crewMembers = await (from crewAssembly in context.Parties
                                     join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                     from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                     from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)


                                     where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                             && crewAsseemblyUser.UserName == User.Identity.Name
                                             && (crewMembersRelation.IsActive)
                                     select new
                                     {
                                         RelationshipId = crewMembersRelation.Id,
                                         CrewName = crewAssembly.FormalName,
                                         CrewMemberName = crewMember.FormalName,
                                         EmpCode = crewMember.ShortName,
                                         crewMembersRelation.FromPartyId,
                                         crewMembersRelation.FromPartyRole,
                                         crewMember.ImageLink,
                                         crewMember.ShortName,
                                         crewAsseemblyUser.UserName,
                                         crewAsseemblyUser.Email,
                                     }).ToListAsync();



            //        var overlap = crewMembers.Where(x =>

            //        (x.FromPartyRole == RoleType.CheifCrewAgent && x.ShortName == vm.CheifCrew)
            //         || (vm.AssitantCrew != x.ShortName && x.FromPartyRole == RoleType.AssistantCrewAgent)
            //            || (vm.Gaurd != x.ShortName && x.FromPartyRole == RoleType.Gaurd)
            //            || (vm.Driver != x.ShortName && x.FromPartyRole == RoleType.CrewDriver)

            //     );

            //var disjoint = crewMembers.Where(x => !overlap.Contains(x));


            foreach (var crewMember in crewMembers)
            {

                var relationship = context.PartyRelationships.Find(crewMember.RelationshipId);
                relationship.IsActive = false;
                relationship.ThruDate = MyDateTime.Today.AddSeconds(-1);
                context.SaveChanges();
            }

            crewMembers = await (from crewAssembly in context.Parties
                                 join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                 from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                 from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)


                                 where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                         && crewAsseemblyUser.UserName == User.Identity.Name
                                         && crewMembersRelation.IsActive
                                 select new
                                 {
                                     RelationshipId = crewMembersRelation.Id,
                                     CrewName = crewAssembly.FormalName,
                                     CrewMemberName = crewMember.FormalName,
                                     EmpCode = crewMember.ShortName,
                                     crewMembersRelation.FromPartyId,
                                     crewMembersRelation.FromPartyRole,
                                     crewMember.ImageLink,
                                     crewMember.ShortName,
                                     crewAsseemblyUser.UserName,
                                     crewAsseemblyUser.Email,
                                 }).ToListAsync();


            if (!crewMembers.Any(x => x.ShortName == vm.CheifCrew && x.FromPartyRole == RoleType.CheifCrewAgent))
            {
                AddMember(vm.CheifCrew, crew.Id, RoleType.CheifCrewAgent, context);
            }
            if (!crewMembers.Any(x => x.ShortName == vm.AssitantCrew && x.FromPartyRole == RoleType.AssistantCrewAgent))
            {
                AddMember(vm.AssitantCrew, crew.Id, RoleType.AssistantCrewAgent, context);
            }
            if (!crewMembers.Any(x => x.ShortName == vm.Gaurd && x.FromPartyRole == RoleType.CrewGuard))
            {
                AddMember(vm.Gaurd, crew.Id, RoleType.CrewGuard, context);
            }
            if (!crewMembers.Any(x => x.ShortName == vm.Driver && x.FromPartyRole == RoleType.CrewDriver))
            {
                AddMember(vm.Driver, crew.Id, RoleType.CrewDriver, context);
            }

            var crewMembersUpdatedList = await (from crewAssembly in context.Parties
                                                join crewAsseemblyUser in context.Users on crewAssembly.Id equals crewAsseemblyUser.PartyId
                                                from crewMembersRelation in context.PartyRelationships.Where(x => x.ToPartyId == crewAsseemblyUser.PartyId)
                                                from crewMember in context.Parties.Where(x => x.Id == crewMembersRelation.FromPartyId)

                                                where (crewMembersRelation.ToPartyRole == RoleType.Crew || crewMembersRelation.ToPartyRole == RoleType.Vault)
                                                        && crewAsseemblyUser.UserName == User.Identity.Name
                                                        && crewMembersRelation.IsActive
                                                select new CrewAssemblyResponseDto
                                                {
                                                    EmpCode = crewMember.ShortName,
                                                    EmpName = crewMember.FormalName
                                                }).ToListAsync();

            return Ok(await GetUserInfoBase64());
        }
        private void AddMember(string empId, int crewId, RoleType role, AppDbContext context)
        {
            if (empId == null)
                return;

            var employee = (from p in context.Parties
                            join e in context.People on p.Id equals e.Id
                            where p.ShortName == empId && p.IsActive
                            select p).FirstOrDefault();
            if (employee == null)
                return;

            var relationship = new PartyRelationship();
            relationship.Id = sequenceService.GetNextPartiesSequence();
            relationship.ToPartyRole = RoleType.Crew; 
            relationship.ToPartyId = crewId;
            relationship.FromPartyId = employee.Id;
            relationship.FromPartyRole = role;
            relationship.StartDate = MyDateTime.Today;
            relationship.IsActive = true;

            context.PartyRelationships.Add(relationship);
            context.SaveChanges();
        }

        

        private string CreateJwtToken(IdentityUser user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Tokens:Key")));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddHours(configuration.GetValue<int>("Tokens:Lifetime")),
                audience: configuration.GetValue<string>("Tokens:Audience"),
                issuer: configuration.GetValue<string>("Tokens:Issuer")
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("Version")]
        public  IActionResult Version()
        {
            return Ok(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("MakeReadonly")]
        public IActionResult MakeReadonly()
        {
            DatabaseActionController.ChangeReadOnly();
            return Ok();
        }
    }
}