using Common.DataAccess.Interfaces;

namespace Patient.Repository.Interfaces
{
    public interface IQuickEvaluationResultRepository : IDapperRepositoryBase<Models.QuickEvaluationResult>
    {
        Models.QuickEvaluationResult GetByPatientCaseId(long patientcaseid);
    }
}
