using AutoMapper;
using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Logging.API.RabbitMQ;
using Logging.Contracts.Interfaces;
using Logging.Repository.Interfaces;
using Logging.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
namespace Logging.API
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
            var apiConf = services.AddApiAuthentication(Configuration);

            var rabbitmqsetting = new RabbitMQSettings();
            Configuration.GetSection(ConfigurationConsts.RabbitMQSettings).Bind(rabbitmqsetting);
            services.AddSingleton(rabbitmqsetting);
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
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitmqsetting.Host
                };
                factory.UserName = rabbitmqsetting.Username;
                factory.Password = rabbitmqsetting.Password;
                factory.Port = rabbitmqsetting.Port;
                return factory;
            });
            services.Scan(scan => scan
                .FromAssemblyOf<DatabaseSession>()
                .AddClasses(x => x.AssignableTo(typeof(IDatabaseSession)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()

                .AddClasses(x => x.AssignableTo(typeof(IUnitOfWork)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()

                .AddClasses(x => x.AssignableTo(typeof(IDapperRepositoryBase<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<ISerilogsRepository>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .FromAssemblyOf<SeriLogsService>()
                .AddClasses(x => x.AssignableTo(typeof(ILoggingService)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .FromAssemblyOf<NotificationsLogService>()
                .AddClasses(x => x.AssignableTo(typeof(INotificationsLogService)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .FromAssemblyOf<RabbitMQPersistentConnection>()
                .AddClasses(x => x.AssignableTo(typeof(IRabbitMQPersistentConnection)))
                .AsSelfWithInterfaces()
                .WithSingletonLifetime()

                .FromAssemblyOf<RabbitMQPersistentConnection>()
                .AddClasses(x => x.AssignableTo(typeof(EventBusRabbitMQ)))
                .AsSelfWithInterfaces()
                .WithSingletonLifetime()
            );
            services.AddAutoMapper(typeof(SeriLogsService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseRabbitListener();
        }
    }
    public static class ApplicationBuilderExtentions
    {
        public static RabbitMQPersistentConnection Listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<RabbitMQPersistentConnection>();
            var life = app.ApplicationServices.GetService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
            life.ApplicationStarted.Register(OnStarted);
            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            life.ApplicationStopping.Register(OnStopping);
            return app;
        }

        private static void OnStarted()
        {
            Listener.CreateConsumerChannel();
        }

        private static void OnStopping()
        {
            Listener.Disconnect();
        }
    }
}
