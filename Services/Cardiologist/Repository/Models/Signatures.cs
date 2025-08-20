using Common.DataAccess.Models;

namespace Cardiologist.Repository.Models
{
    public class Signatures : BaseModel
    {
        #region Data Member
        private long _cardiologistid;
        private string _filepath;
        private long _filelength;
        #endregion

        #region Properties
        public long CardiologistID
        {
            get { return _cardiologistid; }
            set
            {
                if (_cardiologistid != value)
                {
                    _cardiologistid = value;
                    SetField(ref _cardiologistid, value, "CardiologistID");
                }
            }
        }
        public string FilePath
        {
            get { return _filepath; }
            set
            {
                if (_filepath != value)
                {
                    _filepath = value;
                    SetField(ref _filepath, value, "FilePath");
                }
            }
        }
        public long FileLength
        {
            get { return _filelength; }
            set
            {
                if (_filelength != value)
                {
                    _filelength = value;
                    SetField(ref _filelength, value, "FileLength");
                }
            }
        }
        #endregion
    }
}
