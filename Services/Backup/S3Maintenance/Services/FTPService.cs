using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;

namespace S3Maintenance.Services
{
    internal class FTPService : IFTPService
    {
        private FTPConfiguration _ftpConfiguration;
        private readonly ILogger<FTPService> _logger;
        public FTPService(ILogger<FTPService> logger, FTPConfiguration ftpConfiguration)
        {
            _logger = logger;
            _ftpConfiguration = ftpConfiguration;
        }
        public void UploadFtpFile(string folderName, string fileName, byte[] buffer)
        {
            if (string.IsNullOrWhiteSpace(_ftpConfiguration.ServerUrl)) return;
            //Console.Write($"FTP Configuration : {_ftpConfiguration.ServerUrl} {_ftpConfiguration.UserName} {_ftpConfiguration.Password}");
            Console.WriteLine($"Uploading Ftp stream: {folderName} {fileName} size {buffer.Length}");
            int retry = 5;
            while (retry > 0)
            {
                try
                {
                    FtpWebRequest request;
                    request = WebRequest.Create(new Uri($"{_ftpConfiguration.ServerUrl}/{folderName}/{fileName}")) as FtpWebRequest;
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.UseBinary = true;
                    request.UsePassive = true;
                    request.KeepAlive = true;
                    request.EnableSsl = false;
                    request.Credentials = new NetworkCredential(_ftpConfiguration.UserName, _ftpConfiguration.Password);
                    request.ConnectionGroupName = "group";
                    CreateFTPFolder($"{folderName}");
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Flush();
                    requestStream.Close();
                    Console.WriteLine($"Uploaded successfully..");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    Console.WriteLine($"Retyring({5 - retry}) again..");
                    /*_rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        LogIt = false,
                        Address = _configuration.NotificationAddress,
                        Subject = $"FTP Upload Issue - {fileName}",
                        Body = $"{ex.Message} \n {ex.StackTrace}"
                    });*/
                }
                retry--;
            }
            _logger.LogError($"****Failed uploading Ftp stream: {folderName} {fileName} size: {buffer.Length} ****");

        }
        private void CreateFTPFolder(string folder)
        {
            if (DoesFtpDirectoryExist(folder))
                return;
            if (Path.GetDirectoryName(folder).Length > 0 && DoesFtpDirectoryExist(Path.GetDirectoryName(folder).Replace("\\", "/")) == false)
                CreateFTPFolder(Path.GetDirectoryName(folder).Replace("\\", "/"));
            FtpWebRequest request = WebRequest.Create(new Uri($"{_ftpConfiguration.ServerUrl}/{folder}/")) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.UseBinary = true;
            request.UsePassive = false;
            request.KeepAlive = false;
            request.EnableSsl = false;
            request.Credentials = new NetworkCredential(_ftpConfiguration.UserName, _ftpConfiguration.Password);
            request.ConnectionGroupName = "group";
            FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
            ftpResponse.Close();
        }
        public bool DoesFtpDirectoryExist(string dirPath)
        {
            try
            {
                FtpWebRequest request = WebRequest.Create(new Uri($"{_ftpConfiguration.ServerUrl}/{dirPath}/")) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                //request.UseBinary = true;
                request.UsePassive = false;
                request.KeepAlive = false;
                request.EnableSsl = false;
                request.Credentials = new NetworkCredential(_ftpConfiguration.UserName, _ftpConfiguration.Password);
                request.ConnectionGroupName = "group";
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //return (response.ResponseUri.AbsolutePath.Contains(dirPath)) ;
                response.Close();
            }
            catch (WebException ex)
            {
                FtpWebResponse ftpResponse = (FtpWebResponse)ex.Response;
                if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
            }
            return true;
        }

        public void DeleteFile(string fileName)
        {
            int retry = 5;
            while (retry > 0)
            {
                try
                {
                    FtpWebRequest request = WebRequest.Create(new Uri($"{_ftpConfiguration.ServerUrl}/{fileName}")) as FtpWebRequest;
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    //request.UseBinary = true;
                    request.UsePassive = false;
                    request.KeepAlive = false;
                    request.EnableSsl = false;
                    request.Credentials = new NetworkCredential(_ftpConfiguration.UserName, _ftpConfiguration.Password);
                    request.ConnectionGroupName = "group";
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    //return (response.ResponseUri.AbsolutePath.Contains(dirPath)) ;
                    response.Close();
                    return;
                }
                catch (WebException ex)
                {
                    FtpWebResponse ftpResponse = (FtpWebResponse)ex.Response;
                    if (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        _logger.LogError($"Delete file : {fileName} failed.");
                        return;
                    }
                    retry--;
                }
            }
        }
    }
}
