using FileSharing.Contracts.Models;

namespace S3Maintenance.Services
{
    internal interface IFileSyncService
    {
        public void DoSync(FileEventBO fileEventBO);
    }
}