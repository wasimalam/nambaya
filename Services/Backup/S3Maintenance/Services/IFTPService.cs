namespace S3Maintenance.Services
{
    internal interface IFTPService
    {
        public void UploadFtpFile(string folderName, string fileName, byte[] buffer);
        public void DeleteFile(string fileName);
    }
}