using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;

[assembly: HostingStartup(typeof(SOS.OrderTracking.Web.Server.Areas.Identity.IdentityHostingStartup))]
namespace SOS.OrderTracking.Web.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}