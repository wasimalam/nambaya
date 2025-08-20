namespace FileSharing.Contracts.Models
{
    public class FileEventBO
    {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int FileEvent { get; set; } //0: insert, 1: delete
    }
}
