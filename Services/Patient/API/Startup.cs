using AutoMapper;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.LogContext;
using Common.Services.Handlers;
using Common.Services.Services;
using GreenPipes;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Patient.API.Consumers;
using Patient.API.Hubs;
using Patient.Repository.Interfaces;
using Patient.Service;
using System;

namespace Patient.API
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
            var rabbitmqsetting = new RabbitMQSettings();
            Configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);
            services.AddSingleton(rabbitmqsetting);
            services.AddSingleton(_ => Configuration);
            services.AddWebServiceConfiguration(Configuration);
            services.AddApiRequestConfiguration(Configuration);
            var apiConf = services.AddApiAuthentication(Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("patientimport", policy => policy.RequireClaim("scope", "patient_api", "patient_import"));
                options.AddPolicy("dashboard", policy => policy.RequireClaim("scope", "patient_dashboard", "patient_api"));
                options.AddPolicy("all", policy => policy.RequireClaim("scope", "patient_api"));
            });
            services.AddControllers();
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddHttpContextAccessor();
            services.AddCustomKeyStoreProvider(Configuration);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyHeader().AllowCredentials().WithOrigins(apiConf.CorsAllowOrigins).AllowAnyMethod().WithExposedHeaders("Content-Disposition");
                });
            });

            services.Scan(scan => scan
                .FromAssemblyOf<DatabaseSession>()
                .AddClasses(x => x.AssignableTo(typeof(IDatabaseSession)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()

                .AddClasses(x => x.AssignableTo(typeof(IUnitOfWork)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()

                .AddClasses(x => x.AssignableTo(typeof(IDapperRepositoryBase<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<IPatientRepository>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<PatientService>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<LookupService>()
                .AddClasses(x => x.AssignableTo(typeof(ILookupService)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<LanguageService>()
                .AddClasses(x => x.AssignableTo(typeof(ILanguageService)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<RabbitMQClient>()
                .AddClasses(x => x.AssignableTo(typeof(RabbitMQClient)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()

            //.FromAssemblyOf<QuickEvaluationProgressHub>()
            //.AddClasses(x => x.AssignableTo(typeof(QuickEvaluationProgressHub)))
            //.AsSelfWithInterfaces()
            //.WithSingletonLifetime()
            );
            services.AddAutoMapper(typeof(PatientService), typeof(LookupService));
            services.AddSignalR();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<QuickEvalProgressConsumer>();
                x.AddConsumer<PatientCaseStatusConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}"),
                        rabbitmqsetting.ConnectionName, hostConfig =>
                        {
                            hostConfig.Username(rabbitmqsetting.Username);
                            hostConfig.Password(rabbitmqsetting.Password);
                        });
                   
                    cfg.UseMessageScheduler(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}/quartz"));
                    cfg.ReceiveEndpoint(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<QuickEvalProgressConsumer>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.AEGNORD_PATIENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<PatientCaseStatusConsumer>(provider);
                    });
                }));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                endpoints.MapHub<QuickEvalProgressHub>("/quickevaluationprogresshub");
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
