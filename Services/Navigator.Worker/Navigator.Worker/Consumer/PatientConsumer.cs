using MassTransit;
using Navigator.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Navigator.Worker.Consumer
{
    internal class PatientConsumer : IConsumer<EdfFileUpdatePayLoadBO>
    {
        private readonly IPatientService _patientService;
        public PatientConsumer(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public async Task Consume(ConsumeContext<EdfFileUpdatePayLoadBO> context)
        {
            while (Worker.ProcessingRestart == true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
            Worker.ProcessingPatient = true;
            //try
            //{
            await Task.Run(() => _patientService.Execute(context.Message, context.GetRetryCount()));
            //}
            //catch (ServiceException se)
            //{
            //    _ = context.Defer(TimeSpan.FromMinutes(1));
            //}
            Worker.ProcessingPatient = false;
        }
    }
}
