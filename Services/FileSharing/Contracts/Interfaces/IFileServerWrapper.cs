namespace FileSharing.Contracts.Interfaces
{
    public interface IFileServerWrapper
    {
        void UploadFile(string filepath, byte[] data);
        byte[] DownloadFile(string filepath);
        bool FileExists(string filepath);
        void DeleteFile(string filepath);
        void DeleteFolder(string folderpath);
    }
}
