using FileSharing.Contracts.Models;
using Microsoft.Extensions.Logging;
using Minio;
using System.IO;

namespace S3Maintenance.Services
{
    internal class SyncService : IFileSyncService
    {
        private readonly S3Configuration _s3Configuration;
        private readonly IFTPService _ftpService;
        //private readonly ILogger<SyncService> _logger;
        private readonly MinioClient minioClient;
        public SyncService(S3Configuration configuration, IFTPService fTPService, ILogger<SyncService> logger)
        {
            _s3Configuration = configuration;
            //_logger = logger;
            _ftpService = fTPService;
            minioClient = new MinioClient()
            .WithEndpoint(_s3Configuration.EndPoint)
            .WithCredentials(_s3Configuration.AccessKey, _s3Configuration.SecretKey)
            .WithSSL().Build();
        }
        public void DoSync(FileEventBO fileEventBO)
        {
            if (fileEventBO.FileEvent == 0) //insert
            {
                minioClient.GetObjectAsync(_s3Configuration.BucketName, fileEventBO.FilePath,
                        s =>
                        {
                            byte[] buffer = new byte[fileEventBO.FileSize];
                            s.Read(buffer, 0, (int)fileEventBO.FileSize);
                            _ftpService.UploadFtpFile($"{_s3Configuration.BucketName}/{Path.GetDirectoryName(fileEventBO.FilePath).Replace("\\", "/")}", Path.GetFileName(fileEventBO.FilePath), buffer);
                        }).GetAwaiter().GetResult();
            }
            else if (fileEventBO.FileEvent == 1) //delete
            {
                _ftpService.DeleteFile(fileEventBO.FilePath);
            }

        }
    }
}
