using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public class PatientEDFFile : BaseModel
    {
        #region Data Members
        private long _patientcaseid;
        private string _filepath;
        private string _filename;
        private long _filelenght;
        private string _contenttype;
        private long? _duration;
        #endregion

        #region Properties
        public long PatientCaseID
        {
            get { return _patientcaseid; }
            set
            {
                if (_patientcaseid != value)
                {
                    _patientcaseid = value;
                    SetField(ref _patientcaseid, value, "PatientCaseID");
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
        public string FileName
        {
            get { return _filename; }
            set
            {
                if (_filename != value)
                {
                    _filename = value;
                    SetField(ref _filename, value, "FileName");
                }
            }
        }
        public long FileLength
        {
            get { return _filelenght; }
            set
            {
                if (_filelenght != value)
                {
                    _filelenght = value;
                    SetField(ref _filelenght, value, "FileLength");
                }
            }
        }
        public string ContentType
        {
            get { return _contenttype; }
            set
            {
                if (_contenttype != value)
                {
                    _contenttype = value;
                    SetField(ref _contenttype, value, "ContentType");
                }
            }
        }
        public long? Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    SetField(ref _duration, value, "Duration");
                }
            }
        }
        #endregion
    }
}
