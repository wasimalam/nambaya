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
using Pharmacist.API.Consumers;
using Pharmacist.Repository.Interfaces;
using Pharmacist.Service;
using System;
using System.Threading.Tasks;
using MassTransit.Scheduling;
using Common.Services.Extensions;
using Pharmacist.API.CronJob;

namespace Pharmacist.API
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
            services.AddControllers();
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddHttpContextAccessor();
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
            // to only include classes which can be assigned to a specific interface (or implement a specific interface
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

                .FromAssemblyOf<IPharmacistRepository>()
                    .AddClasses()
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()

                .FromAssemblyOf<PharmacistService>()
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
            services.AddAutoMapper(typeof(PharmacyService), typeof(LookupService));

            // for rabbitmq
            services.AddMassTransit(x =>
            {
                x.AddConsumer<QuickEvaluationNotifier>();
                x.AddConsumer<DeviceReminderConsumer>();
                x.AddConsumer<customReminder>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}"),
                        rabbitmqsetting.ConnectionName, hostConfig =>
                        {
                            hostConfig.Username(rabbitmqsetting.Username);
                            hostConfig.Password(rabbitmqsetting.Password);
                        });
                    cfg.UseMessageScheduler(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}/quartz"));
                    cfg.ReceiveEndpoint(KnownChannels.QUICK_EVALUATION_COMPLETED_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<QuickEvaluationNotifier>(provider);
                    });
                    cfg.ReceiveEndpoint(KnownChannels.DEVICE_REMINDER_EVENT_CHANNEL, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<DeviceReminderConsumer>(provider);
                    });

                    cfg.ReceiveEndpoint("custom_channel", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));
                        ep.ConfigureConsumer<customReminder>(provider);
                    });
                    

                }));
            });

            services.AddCronJob<ChargeDeviceReminderJob>(c =>
            {
                c.CronExpression = Configuration.GetValue<string>("DeviceReminderCronExpression"); ;
                c.TimeZoneInfo = TimeZoneInfo.Local;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
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

    public class PollExternalSystemSchedule : DefaultRecurringSchedule
    {
        public PollExternalSystemSchedule()
        {
            CronExpression = "* * * * *"; // this means every minute
        }
    }

    public class CustomMessage
    {
        public string a { get; set; }
    }

    public class customReminder : IConsumer<CustomMessage>
    {
        
        public Task Consume(ConsumeContext<CustomMessage> context)
        {
            throw new NotImplementedException();
        }
    }
}
