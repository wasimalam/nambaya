using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace Patient.Repository.Models
{
    public partial class PharmacyPatient : BaseModel
    {
        #region Data Members
        private string _pharmacypatientid;
        #endregion
        #region dapper ignore
        [DapperIgnore]
        public override long ID { get => base.ID; set => base.ID = value; }
        [DapperIgnore]
        public override string UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
        [DapperIgnore]
        public override DateTime? UpdatedOn { get => base.UpdatedOn; set => base.UpdatedOn = value; }
        #endregion
        #region Properties
        [DapperKey]
        public string PharmacyPatientID
        {
            get { return _pharmacypatientid; }
            set
            {
                if (_pharmacypatientid != value)
                {
                    _pharmacypatientid = value;
                    SetField(ref _pharmacypatientid, value, "PharmacyPatientID");
                }
            }
        }    
        #endregion        
    }
}
