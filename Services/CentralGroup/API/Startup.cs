using AutoMapper;
using CentralGroup.API.Consumers;
using CentralGroup.Repository.Interfaces;
using CentralGroup.Service;
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
using System;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IHostApplicationLifetime;

namespace CentralGroup.API
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
            services.AddSingleton(_ => Configuration);
            services.AddWebServiceConfiguration(Configuration);
            services.AddApiRequestConfiguration(Configuration);
            var apiConf = services.AddApiAuthentication(Configuration);

            var rabbitmqsetting = new RabbitMQSettings();
            Configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);
            services.AddSingleton(rabbitmqsetting);
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddHttpContextAccessor();
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

               .FromAssemblyOf<ICentralGroupRepository>()
               .AddClasses()
               .AsImplementedInterfaces()
               .WithScopedLifetime()

               .FromAssemblyOf<CentralGroupService>()
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

               .FromAssemblyOf<CommonUserServce>()
               .AddClasses(x => x.AssignableTo(typeof(ICommonUserServce)))
               .AsImplementedInterfaces()
               .WithScopedLifetime()

               .FromAssemblyOf<RabbitMQClient>()
               .AddClasses(x => x.AssignableTo(typeof(RabbitMQClient)))
               .AsSelfWithInterfaces()
               .WithScopedLifetime()
           );
            services.AddAutoMapper(typeof(CentralGroupService), typeof(LookupService));
            // for rabbitmq
            services.AddMassTransit(x =>
            {
                x.AddConsumer<DECompletedEventConsumer>();
                x.AddConsumer<QEResultEventConsumer>();
                x.AddConsumer<SignatureSaveEventConsumer>();
                x.AddConsumer<SignatureDeleteEventConsumer>();
                x.AddConsumer<DESignatureEventConsumer>();
                x.AddConsumer<DeactivatePatientEventConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}"),
                        rabbitmqsetting.ConnectionName, hostConfig =>
                        {
                            hostConfig.Username(rabbitmqsetting.Username);
                            hostConfig.Password(rabbitmqsetting.Password);
                        });

                    cfg.ReceiveEndpoint(KnownChannels.DETAILED_EVALUATION_UPLOADED_COMPLETED_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<DECompletedEventConsumer>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.QUICK_EVALUATION_RESULT_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<QEResultEventConsumer>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.SIGNATURE_SAVE_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<SignatureSaveEventConsumer>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.SIGNATURE_DELETE_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<SignatureDeleteEventConsumer>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.DETAIL_REPORT_SIGN_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<DESignatureEventConsumer>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.PATIENT_DEACTIVATE_VERIFICATION_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<DeactivatePatientEventConsumer>(provider);
                    });
                }));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationLifetime lifetime)
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
