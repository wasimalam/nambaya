using Common.BusinessObjects;
using Common.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace DBMaintenance
{
    internal class BackupService
    {
        private DBConfiguration _configuration;
        private FTPConfiguration _ftpConfiguration;
        private readonly ILogger<BackupService> _logger;
        private readonly RabbitMQClient _rabbitMQClient;
        public BackupService(DBConfiguration configuration, ILogger<BackupService> logger,
            RabbitMQClient rabbitMQClient, FTPConfiguration ftpConfiguration)
        {
            _configuration = configuration;
            _logger = logger;
            _rabbitMQClient = rabbitMQClient;
            _ftpConfiguration = ftpConfiguration;
        }
        public void DoWork()
        {
            if (Directory.Exists($"{_configuration.BackupFolder}") == false)
                Directory.CreateDirectory($"{_configuration.BackupFolder}");
            var frequencies = Enum.GetValues(typeof(Frequency));
            foreach (var freq in frequencies)
            {
                if (Directory.Exists($"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{freq}") == false)
                    Directory.CreateDirectory($"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{freq}");
            }
            var s = new Server(new ServerConnection(_configuration.Server, _configuration.UserName, _configuration.Password));
            var dbs = _configuration.Databases.Split(',');
            DateTime operationDateTime = DateTime.Now;
            //DateTime operationDateTime = Now();
            foreach (var dbName in dbs)
            {
                BackupDB(dbName, s, operationDateTime);
            }
        }
        public void BackupDB(string dbName, Server myServer, DateTime operationDateTime)
        {
            _logger.LogInformation($"Going to check backup for {dbName} at {operationDateTime}");
            try
            {
                if (myServer.Databases[dbName].StoredProcedures.Contains("PreBackup"))
                    myServer.Databases[dbName].ExecuteNonQuery("exec PreBackup");
                var freq = Frequency.Daily;
                if (GetLastBackUpDate(dbName, (Frequency)freq).AddDays((int)freq) > operationDateTime)
                    return;
                Backup bkpDBFull = new Backup();
                string filename = $"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{freq}{Path.DirectorySeparatorChar}{dbName}__{operationDateTime.ToString("yyyyMMdd")}.bak";
                _logger.LogInformation($"Going to {freq} backup for {dbName} in file {filename}");
                /* Specify whether you want to back up database or files or log */
                bkpDBFull.Action = BackupActionType.Database;
                /* Specify the name of the database to back up */
                bkpDBFull.Database = dbName;
                /* You can take backup on several media type (disk or tape), here I am using File type and storing backup on the file system */
                bkpDBFull.Devices.AddDevice(filename, DeviceType.File);
                bkpDBFull.BackupSetName = $"{dbName} {freq} Backup";
                bkpDBFull.BackupSetDescription = $"{dbName} - Full Backup";
                /* You can specify the expiration date for your backup data after that date backup data would not be relevant */
                //bkpDBFull.ExpirationDate = DateTime.Today.AddDays(10);
                /* You can specify Initialize = false (default) to create a new backup set which will be appended as last backup set on the media. You
                 * can specify Initialize = true to make the backup as first set on the medium and to overwrite any other existing backup sets if the all the
                 * backup sets have expired and specified backup set name matches with the name on the medium */
                bkpDBFull.Initialize = true;
                bkpDBFull.CompressionOption = BackupCompressionOptions.On;
                /* Wiring up events for progress monitoring */
                bkpDBFull.PercentComplete += CompletionStatusInPercent;
                bkpDBFull.Complete += Backup_Completed;
                bkpDBFull.SqlBackup(myServer);
                if (myServer.Databases[dbName].StoredProcedures.Contains("PostBackup"))
                    myServer.Databases[dbName].ExecuteNonQuery("exec PostBackup");
            }
            catch (Exception ex)
            {
                _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    LogIt = false,
                    Address = _configuration.NotificationAddress,
                    Subject = $"DB Backup Creation Issue - {dbName}",
                    Body = $"{ex.Message} \n {ex.StackTrace}"
                });
                _logger.LogError(ex, ex.Message);
            }
        }

        private void Backup_Completed(object sender, ServerMessageEventArgs e)
        {
            _logger.LogInformation("Backup completed.");
            _logger.LogInformation(e.Error.Message);

            try
            {
                string dbName = (sender as Backup).Database;
                string filename = (sender as Backup).Devices[0].Name;
                UploadFtpFile("Daily", filename);
                DateTime operationDateTime = DateTime.ParseExact(filename.Substring(filename.LastIndexOf(dbName + "__") + dbName.Length + 2, 8), "yyyyMMdd", null);
                if (GetLastBackUpDate(dbName, Frequency.Weekly, operationDateTime.AddDays(-(int)Frequency.Weekly)).AddDays((int)Frequency.Weekly).Date == operationDateTime.Date
                     && operationDateTime.DayOfWeek == (DayOfWeek)_configuration.WeeklyDay)
                {
                    RestoreDatabase($"{dbName}__{operationDateTime.ToString("yyyyMMdd")}", filename);
                    string weeklyFilename = $"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{Frequency.Weekly}{Path.DirectorySeparatorChar}{dbName}__{operationDateTime.ToString("yyyyMMdd")}.bak";
                    filename = $"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{Frequency.Daily}{Path.DirectorySeparatorChar}{dbName}__{operationDateTime.ToString("yyyyMMdd")}.bak";
                    File.Copy(filename, weeklyFilename);
                    UploadFtpFile("Weekly", weeklyFilename);

                    //handle Monthly backup
                    if (GetLastBackUpDate(dbName, Frequency.Monthly).AddDays((int)Frequency.Weekly * 4).Date <= operationDateTime.Date
                        && (operationDateTime.Day <= 7))
                    {
                        string monthlyFilename = $"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{Frequency.Monthly}{Path.DirectorySeparatorChar}{dbName}__{operationDateTime.ToString("yyyyMMdd")}.bak";
                        File.Copy(filename, monthlyFilename);
                        UploadFtpFile("Monthly", monthlyFilename);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void CompletionStatusInPercent(object sender, PercentCompleteEventArgs e)
        {
            Console.Write($"\rPercent completed: { e.Percent}%.{(e.Percent == 100 ? "\n" : "")}");
        }
        private DateTime GetLastBackUpDate(string dbName, Frequency freq, DateTime? defaultDateTime = null)
        {
            List<string> files = new List<string>(Directory.EnumerateFiles($"{_configuration.BackupFolder}{Path.DirectorySeparatorChar}{freq}{Path.DirectorySeparatorChar}", $"{dbName}__*.bak"));
            DateTime maxDate = DateTime.MinValue;
            foreach (var file in files)
            {
                var filename = $"{file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1)}";
                DateTime filedate;
                if (DateTime.TryParseExact(filename.Substring(filename.LastIndexOf(dbName + "__") + dbName.Length + 2, 8), "yyyyMMdd", null, DateTimeStyles.None, out filedate))
                    if (filedate > maxDate)
                        maxDate = filedate;
            }
            return (maxDate == DateTime.MinValue && defaultDateTime != null) ? defaultDateTime.Value : maxDate;
        }
        private static DateTime datetimeNow = DateTime.Now.AddDays(-100);
        public static DateTime Now()
        {
            datetimeNow = datetimeNow.AddDays(1);
            return datetimeNow;
        }

        public bool RestoreDatabase(string databaseName, string filePath)
        {
            try
            {
                var conn = new ServerConnection(_configuration.Server, _configuration.UserName, _configuration.Password);
                conn.ServerInstance = _configuration.Server;
                var srv = new Server(conn);
                Restore res = new Restore();
                _logger.LogInformation($"Creating Database {databaseName}");
                Database db = new Database(srv, databaseName);
                db.Create();

                res.Devices.AddDevice(filePath, DeviceType.File);

                RelocateFile DataFile = new RelocateFile();
                string MDF = res.ReadFileList(srv).Rows[0][1].ToString();
                DataFile.LogicalFileName = res.ReadFileList(srv).Rows[0][0].ToString();
                DataFile.PhysicalFileName = srv.Databases[databaseName].FileGroups[0].Files[0].FileName;

                RelocateFile LogFile = new RelocateFile();
                string LDF = res.ReadFileList(srv).Rows[1][1].ToString();
                LogFile.LogicalFileName = res.ReadFileList(srv).Rows[1][0].ToString();
                LogFile.PhysicalFileName = srv.Databases[databaseName].LogFiles[0].FileName;

                res.RelocateFiles.Add(DataFile);
                res.RelocateFiles.Add(LogFile);

                res.Database = databaseName;
                res.NoRecovery = false;
                res.ReplaceDatabase = true;
                _logger.LogInformation($"Restoring Database {databaseName}");
                res.SqlRestore(srv);
                _logger.LogInformation($"Dropping Database {databaseName}");
                db.Drop();
                conn.Disconnect();
                _logger.LogInformation($"Backup {filePath} successfully verified for Database {databaseName}");
                return true;
            }
            catch (SmoException ex)
            {
                _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    LogIt = false,
                    Address = _configuration.NotificationAddress,
                    Subject = $"Backup Restore Issue - {databaseName}",
                    Body = $"{ex.Message} \n {ex.StackTrace}"
                });
                throw new SmoException(ex.Message, ex.InnerException);
            }
            catch (IOException ex)
            {
                _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    LogIt = false,
                    Address = _configuration.NotificationAddress,
                    Subject = $"Backup Restore Issue - {databaseName}",
                    Body = $"{ex.Message} \n {ex.StackTrace}"
                });
                throw new IOException(ex.Message, ex.InnerException);
            }
        }
        private void UploadFtpFile(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(_ftpConfiguration.ServerUrl)) return;
            Console.Write($"FTP Configuration : {_ftpConfiguration.ServerUrl} {_ftpConfiguration.UserName} {_ftpConfiguration.Password}");
            Console.Write($"Uploading Ftp File: {folderName} {fileName}");
            try
            {

                FtpWebRequest request;
                string absoluteFileName = Path.GetFileName(fileName);
                request = WebRequest.Create(new Uri($"{_ftpConfiguration.ServerUrl}/{folderName}/{absoluteFileName}")) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Credentials = new NetworkCredential(_ftpConfiguration.UserName, _ftpConfiguration.Password);
                request.ConnectionGroupName = "group";

                using (FileStream fs = File.OpenRead(fileName))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Flush();
                    requestStream.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    LogIt = false,
                    Address = _configuration.NotificationAddress,
                    Subject = $"FTP Upload Issue - {fileName}",
                    Body = $"{ex.Message} \n {ex.StackTrace}"
                });
            }
        }
        private void CreateFTPFolder(string folder)
        {
            FtpWebRequest request = WebRequest.Create(new Uri($"{_ftpConfiguration.ServerUrl}/{folder}/")) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
        }
    }
}
