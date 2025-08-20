using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Navigator.Contracts.Interfaces;
using System;
using System.Diagnostics;

namespace Navigator.Service
{
    public class CardiologistService : BaseService, ICardiologistService
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ApiRequestConfiguration _apiSecretConf;
        private readonly EdfFilePathConfiguration _edfFileConfiguration;
        private readonly NavigatorConfiguration _navigatorConfiguration;
        public CardiologistService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _rabbitMQClient = _serviceProvider.GetRequiredService<RabbitMQClient>();
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _apiSecretConf = serviceProvider.GetRequiredService<ApiRequestConfiguration>();
            _edfFileConfiguration = serviceProvider.GetRequiredService<EdfFilePathConfiguration>();
            _navigatorConfiguration = serviceProvider.GetRequiredService<NavigatorConfiguration>();
        }

        /*public void ExecuteInsert(CardiologistBO cardiologistBO)
        {
            try
            {
                string path = _navigatorConfiguration.NavigatorExecutableJarPath;
                using Process process = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = _navigatorConfiguration.JrePath,
                        FileName = "java.exe",
                        Arguments = "-jar " + '"' + _navigatorConfiguration.DatabaseManagerExecutableJarPath
                                                  + " \"" + cardiologistBO.Email.Substring(0, cardiologistBO.Email.IndexOf("@")) + "\""
                                                  + " \"" + cardiologistBO.Name + "\""
                    }
                };
                if (process.Start())
                {
                    bool isClosed = false;
                    while (isClosed == false)
                    {
                        if (process.HasExited)
                        {
                            isClosed = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public void ExecuteUpdate(CardiologistBO cardiologistBO)
        {
            try
            {
                string path = _navigatorConfiguration.NavigatorExecutableJarPath;
                using Process process = new Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = _navigatorConfiguration.JrePath,
                        FileName = "java.exe",
                        Arguments = "-jar " + '"' + _navigatorConfiguration.DatabaseManagerExecutableJarPath
                                                  + " \"" + cardiologistBO.Email.Substring(0, cardiologistBO.Email.IndexOf("@")) + "\""
                                                  + " \"" + cardiologistBO.Name + "\""
                    }
                };
                if (process.Start())
                {
                    bool isClosed = false;
                    while (isClosed == false)
                    {
                        if (process.HasExited)
                        {
                            isClosed = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }*/
    }
}
