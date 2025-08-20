using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navigator.Contracts.Interfaces;
using Navigator.Contracts.Models;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Navigator.Worker
{
    public class ImportWorker : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IPharmacyService _pharmacyService;
        private readonly ImportOptions _importOptions;
        public ImportWorker(ILoggerFactory loggerFactory, IPharmacyService pharmacyService, ImportOptions options)
        {
            _logger = loggerFactory.CreateLogger<Worker>();
            _pharmacyService = pharmacyService;
            _importOptions = options;//
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var reader = new StreamReader(_importOptions.PharmacyFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<PharmacyImportBO>();
                foreach (var rec in records)
                    _pharmacyService.Import(rec);
            }
            _logger.LogInformation("Import process finished!");
            //StopAsync(cancellationToken);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
    internal class ImportParams
    {
        public string PharmacyFileName { get; set; }
    }
}