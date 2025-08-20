using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class DetailEvaluation : BaseModel
    {
        #region Data Members
        private int _patientcaseid;
        private string _resultspath;
        private string _filename;
        private long _filelenght;
        private string _contenttype;
        private string _notes;
        private long? _notestypeid;
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
        public string ResultsPath
        {
            get { return _resultspath; }
            set
            {
                if (_resultspath != value)
                {
                    _resultspath = value;
                    SetField(ref _resultspath, value, "ResultsPath");
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
        public long? NotesTypeID
        {
            get { return _notestypeid; }
            set
            {
                if (_notestypeid != value)
                {
                    _notestypeid = value;
                    SetField(ref _notestypeid, value, "NotesTypeID");
                }
            }
        }
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    SetField(ref _notes, value, "Notes");
                }
            }
        }
        #endregion
    }
}
