using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IPatientCasesRepository : IDapperRepositoryBase<Models.PatientCases>
    {
        List<Models.PatientCases> GetByPatientId(long patientid);
    }
}
