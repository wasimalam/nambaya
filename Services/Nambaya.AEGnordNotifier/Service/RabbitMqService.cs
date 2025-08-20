using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Common.BusinessObjects.ConsumerMessages;
using GreenPipes;
using MassTransit;
using Nambaya.AEGnordNotifier.Consumer;
using Nambaya.AEGnordNotifier.Logger;
using Nambaya.AEGnordNotifier.Settings;

namespace Nambaya.AEGnordNotifier.Service
{
    
    public class RabbitMqService
    {
        private static IBusControl _busControl;
        private readonly ConfigurationStore _configuration;
        public RabbitMqService()
        {
            _configuration = new ConfigurationStore();
        }

        public async Task<bool>  Start()
        { 
             LogHelper.Log.Information($"{nameof(RabbitMqService.Start)}: About to configure rabbit mq. Host:{_configuration.RabbitMqHost},Port:{_configuration.RabbitMqPort}");
            _busControl = Bus.Factory.CreateUsingRabbitMq(rbq =>
            {

                rbq.Host(host:_configuration.RabbitMqHost, port:ushort.Parse(_configuration.RabbitMqPort),virtualHost: _configuration.RabbitMqVirtualHost,connectionName:_configuration.RabbitMqConnectionName, h =>
                {
                    h.Username(_configuration.RabbitMqUsername);
                    h.Password(_configuration.RabbitMqPassword);
                    if (_configuration.IsSsl)
                    {
                        h.UseSsl(ssl =>
                        {
                            ssl.Protocol = SslProtocols.Tls12;
                            ssl.Certificate = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory +"\\fullchain.pem");
                            ssl.AllowPolicyErrors(SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors);
                            ssl.UseCertificateAsAuthenticationIdentity = false;
                        });
                    }
                });


                rbq.Publish<PatientCaseStatusDetailMessage>(x =>
                {
                    x.AutoDelete = false;

                });

                rbq.ReceiveEndpoint(_configuration.RabbitMqQueueName, ep =>
                {
                    ep.UseMessageRetry(mr => mr.Interval(5, 10000));
                    ep.Consumer<CaseDetailConsumer>();
                });

            });

            LogHelper.Log.Information($"{nameof(RabbitMqService.Start)}: About to start rabbit mq.");
            await _busControl.StartAsync();
            LogHelper.Log.Information($"{nameof(RabbitMqService.Start)}: Rabbitmq started successfully.");

            return true;
        }

        public async void Stop()
        {
            LogHelper.Log.Information($"{nameof(RabbitMqService.Stop)}: About to stop rabbit mq.");
            await _busControl.StopAsync();
        }

        public async void PublishPatientCaseDetail(PatientCaseDispatchMessage message, bool isSuccess)
        {
            LogHelper.Log.Information($"{nameof(RabbitMqService.PublishPatientCaseDetail)}: About to publish message about case detail of patient:{message.PatientCaseId}, status:{isSuccess}");
            await _busControl.Publish(new PatientCaseStatusDetailMessage()
                {
                    PatientCaseId = message.PatientCaseId,
                    IsSuccess = isSuccess,
                    CreatedBy = message.CreatedBy,
                    IsDetailEvaluationFileAttached = message.IsDetailEvaluationFileAttached,
                    IsMedicationFileAttached = message.IsMedicationFileAttached
            }
            );
        }
    }
}
