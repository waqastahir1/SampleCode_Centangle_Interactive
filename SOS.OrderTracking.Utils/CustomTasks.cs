using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Utils
{
    internal class CustomTasks
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        private readonly ILookupProtector dataProtectionProvider;
        private readonly IPersonalDataProtector personalDataProtector;
        private readonly ILookupProtectorKeyRing keyRing;
        public CustomTasks(IServiceScopeFactory serviceScopeFactory,
          Serilog.ILogger logger,
            ILookupProtector _dataProtectionProvider,
            ILookupProtectorKeyRing _keyRing,
            IPersonalDataProtector _personalDataProtector)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;

            dataProtectionProvider = _dataProtectionProvider;
            keyRing = _keyRing;
            personalDataProtector = _personalDataProtector;

        }

        public void StartPopulatingAllocatedBranchesFromUserJson()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    logger.Error("data is already loaded, this will duplicate the data");
                    return;
                    using var scope = serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var usersWithJson = context.Users.Where(x => !string.IsNullOrEmpty(x.JsonDate)).ToList();

                    foreach (var user in usersWithJson)
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var jsonData = user.JsonDate == null ? new UserJsonData() : JsonConvert.DeserializeObject<UserJsonData>(user.JsonDate);

                        if (jsonData?.AllocatedBranches != null && jsonData.AllocatedBranches.Count > 0)
                        {
                            var list = jsonData?.AllocatedBranches.Select(x => new AllocatedBranch() { UserId = user.Id, PartyId = x, IsEnabled = true });
                            await _context.AllocatedBranches.AddRangeAsync(list);
                            await _context.SaveChangesAsync();
                        }

                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

            });
        }

        public void StartEncryptingData()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    logger.Error("recheck data that needs to be encrypted, otherwise already encrypted data can be double encrypted which may cause issues");
                    return;

                    using var scope = serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    // Code to encrypt existingUsers
                    var users = context.Users.Where(x => x.UserName == "testlive@sos.com").ToList();
                    foreach (var user in users)
                    {
                        var key = keyRing.CurrentKeyId;
                        user.NormalizedEmail = dataProtectionProvider.Protect(key, user.NormalizedEmail);
                        user.Email = personalDataProtector.Protect(user.Email);
                        user.NormalizedUserName = user.NormalizedEmail;
                        user.UserName = personalDataProtector.Protect(user.UserName);
                        // Update other properties as needed
                    }

                    await context.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

            });
        }

    }
}
