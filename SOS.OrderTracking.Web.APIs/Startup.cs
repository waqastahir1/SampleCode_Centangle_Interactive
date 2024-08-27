using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SOS.OrderTracking.Web.APIs.Services;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Extenstions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Shared.ViewModels;
using ConsignmentService = SOS.OrderTracking.Web.APIs.Services.ConsignmentService;

namespace SOS.OrderTracking.Web.APIs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
           x => x.UseNetTopologySuite()).EnableSensitiveDataLogging());
            services.AddDbContext<SOS_VIEWSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SOSViews"))
                .EnableSensitiveDataLogging());

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    // ProtectedData
                    //options.Stores.ProtectPersonalData = true;
                })
             .AddSignInManager<SignInManager<ApplicationUser>>()
               .AddUserManager<UserManager<ApplicationUser>>()
               .AddRoleManager<RoleManager<IdentityRole>>()
               .AddEntityFrameworkStores<AppDbContext>();

            // ProtectedData
            services.AddScoped<ILookupProtectorKeyRing, CustomLookupProtectorKeyRing>();
            services.AddScoped<ILookupProtector, CustomLookupProtector>();
            services.AddScoped<IPersonalDataProtector, PersonalDataProtector>();

            #region Add Authentication  
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = signingKey,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Tokens:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
            #endregion

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetConnectionString("RedisConnectionString");
            });

            services.AddCors(options => options.AddPolicy("Cors", builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            services.AddControllers();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SOS Web APIs", Version = $"v{PlatformServices.Default.Application.ApplicationVersion}" });
                c.CustomSchemaIds(type => type.ToString());
                // Configure Swagger to use the JWT bearer token authorization
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    });
            });

            services.AddHttpClient("SOS.OrderTracking.Web.ServerAPI",
              client => client.BaseAddress = new Uri(Configuration["ServerUrl"]));

            services.AddTransient(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("SOS.OrderTracking.Web.ServerAPI"));

            services.AddSingleton<SmtpEmailManager>();
            services.AddTransient<ConsignmentService>();
            services.AddTransient<UserService>();
            services.AddTransient<NotificationService>();
            services.AddTransient<AtmrService>();
            services.AddTransient<PartiesCacheService>();
            services.AddTransient<ShipmentsCacheService>();
            services.AddTransient<ShipmentDeliveryLogService>();
            services.AddTransient<SequenceService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext context, ILogger<Startup> logger)
        {
            Bold.Licensing.BoldLicenseProvider.RegisterLicense("1FD/EutgcHmWk8UEM5biVllmW4Sp885wuoggYKoiCd4=");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            //app.LogRequestHeaders(logger);
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"SOS APIs V{PlatformServices.Default.Application.ApplicationVersion}");
            });

            app.UseCors("Cors");

            //app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            context.Database.Migrate();


        }
    }


    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Logs all request headers
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>
        /// <param name="loggerFactory"<see cref="ILoggerFactory"/></param>
        public static void LogRequestHeaders(this IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.Use(async (context, next) =>
            {
                var builder = new StringBuilder(Environment.NewLine);
                foreach (var header in context.Request.Headers)
                {
                    builder.AppendLine($"{header.Key}:{header.Value}");
                }
                logger.LogInformation(builder.ToString());
                await next.Invoke();
            });
        }
    }
}
