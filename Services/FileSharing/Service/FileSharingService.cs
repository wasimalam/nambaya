
using Common.Infrastructure;
using FileSharing.Contracts.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FileSharing.Service
{
    public class FileSharingService : IFileSharingService
    {
        //private readonly IMapper _mapper;
        private readonly IFileServerWrapper _ifileserverwrapper;
        private readonly IConfiguration _configuration;
        private readonly string _passphrase;

        public FileSharingService(IFileServerWrapper fileServerWrapper, IConfiguration configuration)
        {
            _ifileserverwrapper = fileServerWrapper;
            _configuration = configuration;
            _passphrase = _configuration.GetSection(ConfigurationConsts.CypherKey).Value;
        }
        byte[] IFileSharingService.DownloadFile(string fileName)
        {
            byte[] downloadBytes = _ifileserverwrapper.DownloadFile(fileName);
            if (downloadBytes == null)
                return null;
            return Cryptography.Decrypt(downloadBytes, _passphrase);
        }
        void IFileSharingService.UploadFile(string fileName, byte[] fileData)
        {
            byte[] fileBytes = Cryptography.Encrypt(fileData, _passphrase);
            _ifileserverwrapper.UploadFile(fileName, fileBytes);
        }
        void IFileSharingService.DeleteFile(string fileName)
        {
            _ifileserverwrapper.DeleteFile(fileName);
        }
        void IFileSharingService.DeleteFolder(string folderName)
        {
            _ifileserverwrapper.DeleteFolder(folderName);
        } 
    }
}
