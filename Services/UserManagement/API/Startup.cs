using System;
using AutoMapper;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.LogContext;
using Common.Services.Extensions;
using Common.Services.Handlers;
using Common.Services.Services;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagement.API.CronJob;
using UserManagement.Repository.Interfaces;
using UserManagement.Service;
namespace UserManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiAuthentication(Configuration);
            services.AddMassTransitSupport(Configuration);
            var apiConf = services.AddApiAuthentication(Configuration);

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    if (apiConf.CorsAllowAnyOrigin || apiConf.CorsAllowOrigins.Length == 0)
                        builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                    else
                        builder.AllowAnyHeader().WithOrigins(apiConf.CorsAllowOrigins).AllowAnyMethod();
                });
            });

            services.AddSingleton(_ => Configuration);
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddUserLoginPolicy(Configuration);
            services.AddHttpContextAccessor();
            // to only include classes which can be assigned to a specific interface (or implement a specific interface)
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

                .FromAssemblyOf<IApplicationRepository>()
                    .AddClasses()
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<UserService>()
                    .AddClasses()
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<LookupService>()
                    .AddClasses(x => x.AssignableTo(typeof(ILookupService)))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<RabbitMQClient>()
                    .AddClasses(x => x.AssignableTo(typeof(RabbitMQClient)))
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime()
            );
            services.AddAutoMapper(typeof(UserService), typeof(LookupService));
            services.AddLazyCache();

            services.AddCronJob<WelcomeMessageJob>(c =>
            {
                c.CronExpression = Configuration.GetValue<string>("WelcomeMessageCronExpression"); ;
                c.TimeZoneInfo = TimeZoneInfo.Local;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<LogHeaderMiddleware>();
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
}
