using Common.DataAccess.Models;

namespace Pharmacist.Repository.Models
{
    public partial class DeviceAssignment : BaseModel
    {
        #region Data Members
        int _pharamcyid;
        int _deviceid;
        int _patientid;
        #endregion

        #region Properties
        public int PharmacyID
        {
            get { return _pharamcyid; }
            set
            {
                if (_pharamcyid != value)
                {
                    _pharamcyid = value;
                    SetField(ref _pharamcyid, value, "PharmacyID");
                }
            }
        }
        public int DeviceID
        {
            get { return _deviceid; }
            set
            {
                if (_deviceid != value)
                {
                    _deviceid = value;
                    SetField(ref _deviceid, value, "DeviceID");
                }
            }
        }
        public int PatientID
        {
            get { return _patientid; }
            set
            {
                if (_patientid != value)
                {
                    _patientid = value;
                    SetField(ref _patientid, value, "PatientID");
                }
            }
        }
        #endregion
    }
}
