using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Interfaces;
using Common.Interfaces.Interfaces;
using Common.Interfaces.Models;
using Common.Services.Handlers;
using GreenPipes;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.API.Consumers;
using System;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IHostApplicationLifetime;

namespace Notification.API
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
            var emailSettings = new EmailSettings();
            Configuration.GetSection(ConfigurationConsts.EmailSettings).Bind(emailSettings);

            var smsSettings = new SmsSettings();
            Configuration.GetSection(ConfigurationConsts.SmsSettings).Bind(smsSettings);

            var rabbitmqsetting = new RabbitMQSettings();
            Configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);

            Configuration.GetSection("OverrideEmail").Bind(SendEmailCosumer.OverrideEmail);
            services.AddSingleton(_ => Configuration);
            services.AddSingleton(_ => emailSettings);
            services.AddSingleton(_ => rabbitmqsetting);
            services.AddSingleton(_ => smsSettings);
            services.AddApiRequestConfiguration(Configuration);
            services.AddWebServiceConfiguration(Configuration);
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddHttpContextAccessor();
            // to only include classes which can be assigned to a specific interface (or implement a specific interface
            services.Scan(scan => scan

                .FromAssemblyOf<ISmsProvider>()
                .AddClasses(x => x.AssignableTo(typeof(ISmsProvider)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<IEmailProvider>()
                .AddClasses(x => x.AssignableTo(typeof(IEmailProvider)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<LoggingService>()
                .AddClasses(x => x.AssignableTo(typeof(ILoggingService)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
            );

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SendEmailCosumer>();
                x.AddConsumer<SendSmsConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqsetting.Host}:{rabbitmqsetting.Port}"),
                        rabbitmqsetting.ConnectionName, hostConfig =>
                    {
                        hostConfig.Username(rabbitmqsetting.Username);
                        hostConfig.Password(rabbitmqsetting.Password);
                    });

                    cfg.ReceiveEndpoint("send_email", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));

                        ep.ConfigureConsumer<SendEmailCosumer>(provider);
                    });
                    cfg.ReceiveEndpoint("send_sms", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(mr => mr.Interval(2, 100));

                        ep.ConfigureConsumer<SendSmsConsumer>(provider);
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
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
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
