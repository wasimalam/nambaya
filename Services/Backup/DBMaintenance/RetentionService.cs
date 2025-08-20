using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DBMaintenance
{
    internal class RetentionService
    {
        private DBConfiguration _configuration;
        private readonly ILogger<RetentionService> _logger;
        public RetentionService(DBConfiguration configuration, ILogger<RetentionService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public void DoWork()
        {
            var s = new Server(new ServerConnection(_configuration.Server, _configuration.UserName, _configuration.Password));
            var dbs = _configuration.Databases.Split(',');
            foreach (var dbName in dbs)
            {
                CleanupDB(dbName, s);
            }
        }
        public void CleanupDB(string dbName, Server myServer)
        {
            if (myServer.Databases[dbName].StoredProcedures.Contains("PreCleanUp"))
                myServer.Databases[dbName].ExecuteNonQuery("exec PreCleanUp");
            _logger.LogInformation($"Going to cleanup backups of {dbName} in {_configuration.BackupFolder}");

            var frequencies = Enum.GetValues(typeof(Frequency));
            foreach (var freq in frequencies)
            {
                int retentationDays = ((Frequency)freq == Frequency.Daily ? _configuration.DailyRetentionAfter :
                    (Frequency)freq == Frequency.Weekly ? (_configuration.WeeklyRetentionAfter) : _configuration.MonthlyRetentionAfter) * (int)freq;
                //_logger.LogInformation($"Going to delete backup for {freq} files of {dbName} in {_configuration.RetentionFolder}{Path.DirectorySeparatorChar}{freq}");
                List<string> files = new List<string>(Directory.EnumerateFiles($"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{freq}{Path.DirectorySeparatorChar}", $"{dbName}__*.bak"));
                foreach (var file in files)
                {
                    var filename = $"{file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1)}";
                    DateTime filedate;
                    if (DateTime.TryParseExact(filename.Substring(filename.LastIndexOf(dbName + "__") + dbName.Length + 2, 8), "yyyyMMdd", null, DateTimeStyles.None, out filedate))
                        if (filedate.AddDays(retentationDays) < DateTime.Now)
                        {
                            _logger.LogInformation($"Going to delete backup {filename}");
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
                        }
                }
            }
            if (myServer.Databases[dbName].StoredProcedures.Contains("PostCleanUp"))
                myServer.Databases[dbName].ExecuteNonQuery("exec PostCleanUp");
        }
    }
}
