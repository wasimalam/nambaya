using Common.DataAccess.Interfaces;
using Patient.Repository.Models;

namespace Patient.Repository.Interfaces
{
    public interface ICaseDispatchDetailRepository : IDapperRepositoryBase<CaseDispatchDetail>
    {
        CaseDispatchDetail GetByPatientCaseId(long patientcaseid);
        void DeleteForCaseId(long patientCaseId);
    }
}
