using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IPatientEDFFileRepository : IDapperRepositoryBase<Models.PatientEDFFile>
    {
        IEnumerable<Models.PatientEDFFile> GetByPatientCaseId(long patientcaseid);
    }
}
