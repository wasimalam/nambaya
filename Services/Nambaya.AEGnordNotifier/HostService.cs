using System;
using System.ServiceProcess;
using Nambaya.AEGnordNotifier.Logger;
using Nambaya.AEGnordNotifier.Service;


namespace Nambaya.AEGnordNotifier
{
    public partial class HostService : ServiceBase
    {
       
        private readonly RabbitMqService _rabbitMqService;
        public HostService()
        {

            _rabbitMqService = new RabbitMqService();
            LogHelper.ConfigureSeriLog();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        protected override void OnStop()
        {
            StopService();
        }

        public  bool StartService()
        {
            try
            {
                new CryptShareService().LogClientId();
                var result =_rabbitMqService.Start().Result;
                LogHelper.Log.Information($"AEGnord service started successfully. RabbitMq status: {result}");
            }
            catch (Exception e)
            {
                LogHelper.Log.Error($"AEGnord service not started_{e}");
                throw;
            }

            return true;
        }

        public bool StopService()
        {
            try
            {
                _rabbitMqService.Stop();
                LogHelper.Log.Information("AEGnord Service stopped successfully.");
            }
            catch (Exception e)
            {
                LogHelper.Log.Error($"Error on service stopped. {e}");
                throw;
            }
           
            return true;
        }

        
    }
}



    


