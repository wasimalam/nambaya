using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IMedicationPlanFileRepository : IDapperRepositoryBase<Models.MedicationPlanFile>
    {
        IEnumerable<Models.MedicationPlanFile> GetByPatientCaseId(long patientcaseid);
    }
}
