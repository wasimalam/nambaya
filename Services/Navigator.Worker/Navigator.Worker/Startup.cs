using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Services.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navigator.Service;

namespace Navigator.Worker
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
            services.AddApiAuthentication(Configuration);

            var rabbitmqsetting = new RabbitMQSettings();
            Configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);
            services.AddSingleton(rabbitmqsetting);

            var edfFilePathConfiguration = new EdfFilePathConfiguration();
            Configuration.GetSection(ConfigurationConsts.EdfFilePathConfiguration).Bind(edfFilePathConfiguration);
            services.AddSingleton(edfFilePathConfiguration);

            var scheduleConfiguration = new ScheduleConfiguration();
            Configuration.GetSection("ScheduleConfiguration").Bind(scheduleConfiguration);
            services.AddSingleton(scheduleConfiguration);

            var restartProcessConfiguration = new RestartNavigatorConfiguration();
            Configuration.GetSection("RestartProcessConfiguration").Bind(restartProcessConfiguration);
            services.AddSingleton(restartProcessConfiguration);

            var navigatorConfiguration = new NavigatorConfiguration();
            Configuration.GetSection(ConfigurationConsts.NavigatorConfiguration).Bind(navigatorConfiguration);
            services.AddSingleton(navigatorConfiguration);
            
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();

            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
            // to only include classes which can be assigned to a specific interface (or implement a specific interface
            services.Scan(scan => scan
                .FromAssemblyOf<PatientService>()
                    .AddClasses()
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                .FromAssemblyOf<RabbitMQClient>()
                    .AddClasses(x => x.AssignableTo(typeof(RabbitMQClient)))
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime()
            );
            //services.AddAutoMapper(typeof(PharmacyService), typeof(LookupService));

            // for rabbitmq
            //services.AddMassTransit(x =>
            //{
            //    x.AddConsumer<QuickEvaluationNotifier>();
            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        var host = cfg.Host(new Uri($"rabbitmq://{rabbitmqsetting.Host}"), hostConfig =>
            //        {
            //            hostConfig.Username(rabbitmqsetting.Username);
            //            hostConfig.Password(rabbitmqsetting.Password);
            //        });

            //        cfg.ReceiveEndpoint(KnownChannels.QUICK_EVALUATION_COMPLETED_EVENT_CHANNEL, ep =>
            //        {
            //            ep.PrefetchCount = 16;
            //            ep.UseMessageRetry(mr => mr.Interval(2, 100));
            //            ep.ConfigureConsumer<QuickEvaluationNotifier>(provider);
            //        });
            //    }));
            //});
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
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            //var bus = app.ApplicationServices.GetService<IBusControl>();
            //var busHandle = TaskUtil.Await(() =>
            //{
            //    return bus.StartAsync();
            //});

            //lifetime.ApplicationStopping.Register(() =>
            //{
            //    busHandle.Stop();
            //});
        }
    }
}

