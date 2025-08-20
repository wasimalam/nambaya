using FileSharing.Contracts.Models;
using MassTransit;
using S3Maintenance.Services;
using System.Threading.Tasks;

namespace S3Maintenance.Consumers
{
    internal class FileEventConsumer : IConsumer<FileEventBO>
    {
        private readonly IFileSyncService _fileSyncService;
        public FileEventConsumer(IFileSyncService fileSyncService)
        {
            _fileSyncService = fileSyncService;
        }

        public async Task Consume(ConsumeContext<FileEventBO> context)
        {
            await Task.Run(() => _fileSyncService.DoSync(context.Message));
        }
    }
}
