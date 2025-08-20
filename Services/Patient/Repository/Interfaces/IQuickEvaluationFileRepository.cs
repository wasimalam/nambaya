using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IQuickEvaluationFileRepository : IDapperRepositoryBase<Models.QuickEvaluationFile>
    {
        IEnumerable<Models.QuickEvaluationFile> GetByPatientCaseId(long patientcaseid);
    }
}
