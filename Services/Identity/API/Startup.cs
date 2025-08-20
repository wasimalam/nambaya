using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.LogContext;
using Common.Services.Handlers;
using Identity.Contracts.Interfaces;
using Identity.Repository.Interfaces;
using Identity.Service;
using IdentityServer4.Services;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Identity.API
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsDevelopment())
                services.ConfigureNonBreakingSameSiteCookies();
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            var authorityKeys = AuthorityKeys.GetAuthorityKeys();
            Configuration.GetSection("AuthorityKeys").Bind(authorityKeys);
            services.AddSingleton(_ => Configuration);
            services.AddWebServiceConfiguration(Configuration);
            services.AddApiRequestConfiguration(Configuration);
            services.AddMassTransitSupport(Configuration);
            // to only include classes which can be assigned to a specific interface (or implement a specific interface
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddHttpContextAccessor();
            services.Scan(scan => scan
                .FromAssemblyOf<DatabaseSession>()
                    .AddClasses(x => x.AssignableTo(typeof(IDatabaseSession)))
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime()

                    .AddClasses(x => x.AssignableTo(typeof(IUnitOfWork)))
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime()

                    .AddClasses(x => x.AssignableTo(typeof(IDapperRepositoryBase<>)))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<IHostRegistryRepository>()
                    .AddClasses()
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<IdentityService>()
                    .AddClasses(x => x.AssignableTo(typeof(IIdentityService)))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<RabbitMQClient>()
                    .AddClasses(x => x.AssignableTo(typeof(RabbitMQClient)))
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime()
            );
            services.AddCustomeIdentityServer(Configuration);
            var cors = new DefaultCorsPolicyService(new LoggerFactory().CreateLogger<DefaultCorsPolicyService>())
            {
                AllowAll = true
            };
            services.AddControllers();
            services.AddSingleton<ICorsPolicyService>(cors);
            services.AddTransient<IReturnUrlParser, ReturnUrlParser>();
            /* services.Configure<ForwardedHeadersOptions>(options =>
             {
                 options.ForwardedHeaders =
                     ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                 options.RequireHeaderSymmetry = false;
                 options.KnownNetworks.Clear();
                 options.KnownProxies.Clear();
             });*/

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseCookiePolicy();
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseIdentityServer();
            app.UseMiddleware<LogHeaderMiddleware>();
            if (Configuration["enablehttps"].ToLower() != "false")
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var bus = app.ApplicationServices.GetService<IBusControl>();
            var busHandle = TaskUtil.Await(() =>
            {
                return bus.StartAsync();
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                busHandle.Stop();
            });
        }
    }

    public static class StartUpExtensions
    {
        public static void AddCustomeIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serviceProvider = services.BuildServiceProvider();
            ILogger logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
            IWebHostEnvironment env = serviceProvider.GetService<IWebHostEnvironment>();
            IIdentityService identityService = new IdentityService(serviceProvider);
            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.WithOrigins(identityService.GetCorsUrls());
                    policy.AllowCredentials();
                });
            });

            var identityServerBuilder = services.AddIdentityServer(
                    options =>
                    {
                        options.UserInteraction.LoginUrl = identityService.GetLogInUrl();
                        options.UserInteraction.ErrorUrl = identityService.GetErrorUrl();
                        options.UserInteraction.LogoutUrl = identityService.GetLogOutUrl();
                        if (!string.IsNullOrEmpty(configuration["Issuer"]))
                        {
                            options.IssuerUri = configuration["Issuer"];
                        }
                    });
            if (env.IsDevelopment())
                identityServerBuilder.AddDeveloperSigningCredential();
            else
            {
                var certificate = new X509Certificate2("certificate.pfx", configuration["CertificatePassword"]);
                identityServerBuilder.AddSigningCredential(certificate);
                logger.LogInformation($"Certificate {certificate.Subject} loaded successfully!");
            }
            // configure identity server with default stores, keys, clients and scopes,which use the standard SQL
            identityServerBuilder.AddConfigurationStore(options =>
               {
                   options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
               })
               // configure identity server with default Operationalstores,which use the standard SQL
               .AddOperationalStore(option =>
               {
                   option.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                   option.EnableTokenCleanup = true;
                   option.TokenCleanupInterval = 3600;
               });//.AddTestUsers(Config.GetTestUsers());

            services.AddScoped<IProfileService, ProfileService>();
        }
    }

    public class AuthorityKeys
    {
        public string jwtkey { get; set; }
        public string authenticationkey { get; set; }
        public int tokentimeout { get; set; }
        public int rptokentimeout { get; set; }
        public int passwordtokentimeout { get; set; }
        public string otpMessage { get; set; }
        public string testOTPEmail { get; set; }
        public string testOTP { get; set; }
        private static AuthorityKeys _authorityKeys;
        private AuthorityKeys()
        {

        }
        public static AuthorityKeys GetAuthorityKeys()
        {
            if (_authorityKeys == null)
                _authorityKeys = new AuthorityKeys();
            return _authorityKeys;
        }
    }
}
