using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.LogContext;
using Common.Services.Handlers;
using FileSharing.Contracts;
using FileSharing.Contracts.Interfaces;
using FileSharing.Service;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using IFileServerWrapper = FileSharing.Contracts.Interfaces.IFileServerWrapper;

namespace FileSharing.API
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
            services.Configure<FormOptions>(o =>  // currently all set to max, configure it to your needs!
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
                o.MultipartBoundaryLengthLimit = int.MaxValue;
                o.MultipartHeadersCountLimit = int.MaxValue;
                o.MultipartHeadersLengthLimit = int.MaxValue;
            });
            var apiConf = services.AddApiAuthentication(Configuration);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read", policy => policy.RequireClaim("scope", "file_sharing_api_full", "file_sharing_api_read"));
                options.AddPolicy("readwrite", policy => policy.RequireClaim("scope", "file_sharing_api_full"));
            });
            services.AddControllers();
            services.AddSingleton(_ => Configuration);
            services.AddTransient<RequestHandler>();
            services.AddTransient<SecureServiceHandler>();
            services.AddHttpContextAccessor();
            services.AddMassTransitSupport(Configuration);
            var fileserverSettingsSection = Configuration.GetSection("FileServerSettings");
            services.Configure<FileServerSettings>(fileserverSettingsSection);
            var fileServerWrapperAssembly = LoadAssembly(Configuration.GetSection("FileServerWrapper")["assembly"]);
            var firstInterfaceImplementationType = fileServerWrapperAssembly.DefinedTypes.Where(t => t.ImplementedInterfaces.Contains(typeof(IFileServerWrapper))).FirstOrDefault();
            var interfaceType = firstInterfaceImplementationType.GetInterface("IFileServerWrapper");
            services.Add(new ServiceDescriptor(interfaceType, firstInterfaceImplementationType, ServiceLifetime.Singleton));
            services.AddScoped<IFileSharingService, FileSharingService>();
            services.Scan(scan => scan
                .FromAssemblyOf<RabbitMQClient>()
                    .AddClasses(x => x.AssignableTo(typeof(RabbitMQClient)))
                        .AsSelfWithInterfaces()
                        .WithSingletonLifetime()
            );
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
        public static System.Reflection.Assembly LoadAssembly(string path)
        {
            //System.Reflection.Assembly ass = null;
            var assembiles = Directory.GetFiles(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, path, SearchOption.TopDirectoryOnly).Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
            //string fullPath = Path.Combine(this.folderPath, path);
            // Requires nuget - System.Runtime.Loader
            //ass = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            return assembiles.FirstOrDefault();
        }
    }
}
