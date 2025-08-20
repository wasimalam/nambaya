using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IDetailEvaluationRepository : IDapperRepositoryBase<Models.DetailEvaluation>
    {
        IEnumerable<Models.DetailEvaluation> GetByPatientCaseId(long patientcaseid);
    }
}
