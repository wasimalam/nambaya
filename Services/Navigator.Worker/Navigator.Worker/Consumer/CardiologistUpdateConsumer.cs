using Cardiologist.Contracts.Models;
using MassTransit;
using Navigator.Contracts.Interfaces;
using System.Threading.Tasks;

namespace Navigator.Worker.Consumer
{
    public class CardiologistUpdateConsumer : IConsumer<CardiologistBO>
    {
        private readonly ICardiologistService _cardiologistService;

        public CardiologistUpdateConsumer(ICardiologistService cardiologistService)
        {
            _cardiologistService = cardiologistService;
        }

        /// <summary>
        /// When EDF file is uploaded by pharmacist
        /// this consumer will be invoked and it will
        /// launch navigator software
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<CardiologistBO> context)
        {
            await Task.Run(() =>
            {
                //_cardiologistService.ExecuteUpdate(context.Message);
            });
        }
    }
}