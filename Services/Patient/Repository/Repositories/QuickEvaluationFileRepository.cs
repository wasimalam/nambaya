using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Repositories
{
    public class QuickEvaluationFileRepository : DapperRepositoryBase<QuickEvaluationFile>, IQuickEvaluationFileRepository
    {
        public QuickEvaluationFileRepository(IDatabaseSession session) : base(session)
        {

        }
        public IEnumerable<QuickEvaluationFile> GetByPatientCaseId(long patientcaseid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientcaseid = @patientcaseid order by 1 desc", new { patientcaseid = patientcaseid });
        }
    }
}