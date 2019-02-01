using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ticketing_api.Data;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphiQl;
using Microsoft.IdentityModel.Logging;
using Sieve.Services;
using Microsoft.Extensions.FileProviders;
using ticketing_api.Services;
using Sieve.Models;

namespace ticketing_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile("logs/log-{Date}.txt")
                //.WriteTo
                //.MSSqlServer(ConfigurationValues.CoreConnectionString, ConfigurationValues.SeriLoggerTableName, autoCreateSqlTable: true)
                .CreateLogger();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.Configure<SieveOptions>(Configuration.GetSection("Sieve"));
            services.AddScoped<ISieveCustomSortMethods, SieveCustomSortMethods>();
            services.AddScoped<ISieveCustomFilterMethods, SieveCustomFilterMethods>();
            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
            services.AddCors();
            // services.ConfigureCors();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // Configure Entity Framework Identity for Auth
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 1;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.IssuerUri = AppConfiguration.Instance.Security["Authority"];
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(SecurityConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(SecurityConfiguration.GetApiResources())
                .AddInMemoryClients(SecurityConfiguration.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

            services.AddScoped<ApplicationUserManager>();
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IEmailSender, SmtpSender>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<UserResolverService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policyAdmin =>
                {
                    policyAdmin.RequireRole(new List<string> { AppConstants.AppRoles.SysAdmin.ToUpper(), AppConstants.AppRoles.Admin.ToUpper() });
                });
            });

            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = AppConfiguration.Instance.Security["Authority"];
                o.Audience = AppConfiguration.Instance.Security["Audience"];
                o.RequireHttpsMetadata = false;
                //o.TokenValidationParameters = tokenValidationParameters;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddSwaggerDocumentation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
                //app.UseHttpsRedirection();
            }

            app.UseIdentityServer();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSwaggerDocumentation();

            // var origins = AppConfiguration.Instance.Configuration.GetSection("Security:Origins").Get<string[]>();
            app.UseCors(policy => policy
                //.WithOrigins(origins)
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
            );

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));


            app.UseGraphiQl();
            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var helper = (DbContextHelper)ActivatorUtilities.CreateInstance(serviceScope.ServiceProvider, typeof(DbContextHelper));
                //    if (!helper.AllMigrationsApplied())
                //    {
                //        serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
                //    }
                Task.Run(() => helper.EnsureSeededAsync()).Wait();
            }

            app.UseStaticFiles(); // For the wwwroot folder
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });
        }
    }
}
