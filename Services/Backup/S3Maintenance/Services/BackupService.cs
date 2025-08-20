using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using S3Maintenance.Services;
using System;
using System.IO;

namespace S3Maintenance
{
    internal class BackupService
    {
        private S3Configuration _configuration;
        private IFTPService _ftpService;
        private readonly ILogger<BackupService> _logger;
        private readonly MinioClient minioClient;
        public BackupService(S3Configuration configuration, ILogger<BackupService> logger, IFTPService fTPService)
        {
            _configuration = configuration;
            _logger = logger;
            _ftpService = fTPService;
            minioClient = new MinioClient()
             .WithEndpoint(_configuration.EndPoint)
             .WithCredentials(_configuration.AccessKey, _configuration.SecretKey)
             .WithSSL().Build();

        }
        public void DoWork()
        {
            _logger.LogInformation($"Going to check backup for");
            try
            {
                // Check whether 'mybucket' exists or not.
                bool found = minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_configuration.BucketName)).Result;
                if (found)
                {
                    BackupFolder(_configuration.BucketName);
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
            catch (Exception ex)
            {
                /*
                 * _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    LogIt = false,
                    Address = _configuration.NotificationAddress,
                    Subject = $"DB Backup Creation Issue - {dbName}",
                    Body = $"{ex.Message} \n {ex.StackTrace}"
                });
                */
                _logger.LogError(ex, ex.Message);
            }
        }

        private void BackupFolder(string folder)
        {
            _logger.LogInformation($"Going to check backup for {folder}");
            try
            {
                // List objects from 'my-bucketname'
                IObservable<Item> observable = minioClient.ListObjectsAsync(folder, null, true);
                IDisposable subscription = observable.Subscribe(
                        item =>
                        {
                            Console.WriteLine("OnNext: {0}", item.Key);
                            if (!item.IsDir)
                            {
                                minioClient.GetObjectAsync(_configuration.BucketName, item.Key,
                                    s =>
                                    {
                                        byte[] buffer = new byte[item.Size];
                                        s.Read(buffer, 0, (int)item.Size);
                                        _ftpService.UploadFtpFile($"{folder}/{Path.GetDirectoryName(item.Key).Replace("\\", "/")}", Path.GetFileName(item.Key), buffer);
                                    }).GetAwaiter().GetResult();
                            }
                        },
                        ex => Console.WriteLine("OnError: {0}", ex.Message),
                        () => Console.WriteLine("Backup process Completed ...."));

            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
            catch (Exception ex)
            {
                /*
                 * _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    LogIt = false,
                    Address = _configuration.NotificationAddress,
                    Subject = $"DB Backup Creation Issue - {dbName}",
                    Body = $"{ex.Message} \n {ex.StackTrace}"
                });
                */
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
