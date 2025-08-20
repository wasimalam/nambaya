using Common.Infrastructure;
using FileSharing.Contracts;
using FileSharing.Contracts.Interfaces;
using FileSharing.Contracts.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using System;
using System.IO;

namespace FileSharing.Wrapper.S3
{
    public class S3 : IFileServerWrapper
    {
        private FileServerSettings _configuration;
        private readonly ILogger<S3> _logger;

        private readonly string endpoint;
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly MinioClient minio;
        private readonly string bucketName;
        private readonly RabbitMQClient _rabbitMQClient;
        public S3(IOptions<FileServerSettings> fileServerSettingsAccessor, ILogger<S3> logger, IConfiguration configuration, IServiceProvider serviceProvider, RabbitMQClient rabbitMQClient)
        {
            _configuration = fileServerSettingsAccessor.Value;
            _logger = logger;
            endpoint = _configuration.BaseUrl;
            accessKey = _configuration.username;
            secretKey = _configuration.Password;
            bucketName = configuration["FileServerSettings:Bucket"];
            _rabbitMQClient = rabbitMQClient;
            minio = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL().Build();
        }
        public void DeleteFile(string filepath)
        {
            try
            {
                var obj = minio.StatObjectAsync(bucketName, filepath).GetAwaiter().GetResult();
                // Remove objectname from the bucket my-bucketname.
                minio.RemoveObjectAsync(bucketName, filepath).GetAwaiter().GetResult();
                _logger.LogDebug($"successfully removed {bucketName}/{filepath}");
                _rabbitMQClient.SendMessage(KnownChannels.FILE_SYNC_EVENT_CHANNEL,
                        new FileEventBO
                        {
                            FileEvent = 1,
                            FilePath = $"{bucketName}/{filepath}"
                        });
            }
            catch (MinioException e)
            {
                _logger.LogError($"File {filepath} Delete Error: " + e);
                throw;
            }
        }

        public void DeleteFolder(string folderpath)
        {
            try
            {
                // Remove objectname from the bucket my-bucketname.
                minio.RemoveObjectAsync(bucketName, folderpath).GetAwaiter().GetResult();
                _logger.LogDebug($"successfully removed {bucketName}/{folderpath}");
            }
            catch (MinioException e)
            {
                _logger.LogError("Error: " + e);
                throw;
            }
        }

        public byte[] DownloadFile(string filepath)
        {
            try
            {
                bool found = FileExists(filepath);
                // Make a bucket on the server, if not already present.
                if (found)
                {
                    using (var mem = new MemoryStream())
                    {
                        minio.GetObjectAsync(bucketName, filepath, (stream) =>
                        {
                            stream.CopyTo(mem);
                        }).GetAwaiter().GetResult();
                        return mem.ToArray();
                    }
                }
                return null;
            }
            catch (MinioException e)
            {
                _logger.LogError("File DownloadFile Error: {0}", e.Message);
                throw e;
            }
        }

        public bool FileExists(string filepath)
        {
            try
            {
                bool found = minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)).Result;
                // Make a bucket on the server, if not already present.
                if (found)
                {
                    var obj = minio.StatObjectAsync(bucketName, filepath).GetAwaiter().GetResult();
                    found = (obj != null);
                }
                return found;
            }
            catch (MinioException e)
            {
                _logger.LogDebug("File Exists Error: {0}", e.Message);
                if (e.Message.ToLower().Contains("not found"))
                    return false;
                throw e;
            }
        }

        public void UploadFile(string filepath, byte[] data)
        {
            try
            {
                var args = new BucketExistsArgs().WithBucket(bucketName);
                bool found = minio.BucketExistsAsync(args).Result;
                // Make a bucket on the server, if not already present.
                if (!found)
                {
                    minio.MakeBucketAsync(bucketName).GetAwaiter().GetResult();
                }
                using (var mem = new MemoryStream(data))
                {
                    // Upload a file to bucket.
                    minio.PutObjectAsync(bucketName, filepath, mem, data.Length).Wait();
                    _logger.LogDebug("Successfully uploaded " + filepath);
                    _rabbitMQClient.SendMessage(KnownChannels.FILE_SYNC_EVENT_CHANNEL,
                        new FileEventBO
                        {
                            FileEvent = 0,
                            FileSize = data.Length,
                            FilePath = filepath
                        });
                }
            }
            catch (MinioException e)
            {
                _logger.LogError("File Upload Error: {0}", e.Message);
                throw;
            }
        }
    }
}
