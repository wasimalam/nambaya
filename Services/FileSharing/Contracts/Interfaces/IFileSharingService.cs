namespace FileSharing.Contracts.Interfaces
{
    public interface IFileSharingService
    {
        void UploadFile(string fileName, byte[] fileData);
        byte[] DownloadFile(string fileName);
        void DeleteFile(string fileName);
        void DeleteFolder(string folderName);
    }
}
