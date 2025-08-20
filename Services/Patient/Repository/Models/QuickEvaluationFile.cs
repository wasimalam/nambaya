using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class QuickEvaluationFile : BaseModel
    {
        #region Data Members
        private int _patientcaseid;
        private string _filepath;
        private string _filename;
        long _filelenght;
        string _contenttype;
        #endregion

        #region Properties
        public int PatientCaseID
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
        #endregion
    }
}
