using Common.DataAccess.Models;

namespace Pharmacist.Repository.Models
{
    public partial class PharmacyPatients : BaseModel
    {
        #region Data Members

        int _pharamcyid;
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
