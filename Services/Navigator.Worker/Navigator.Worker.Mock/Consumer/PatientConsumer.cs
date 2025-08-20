using MassTransit;
using Navigator.Contracts.Interfaces;
using Patient.Contracts.Models;
using System.Threading.Tasks;

namespace Navigator.Worker.Mock.Consumer
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
            //try
            //{
            await Task.Run(() => _patientService.Execute(context.Message, context.GetRetryCount()));            
            //}
            //catch (ServiceException se)
            //{
            //    _ = context.Defer(TimeSpan.FromMinutes(1));
            //}
        }
    }
}
