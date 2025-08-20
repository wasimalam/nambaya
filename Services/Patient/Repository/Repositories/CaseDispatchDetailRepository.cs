using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class CaseDispatchDetailRepository : DapperRepositoryBase<CaseDispatchDetail>, ICaseDispatchDetailRepository
    {
        public CaseDispatchDetailRepository(IDatabaseSession session) : base(session)
        {

        }

        public void DeleteForCaseId(long patientCaseId)
        {
            Execute(System.Data.CommandType.Text, $"delete from {TableName} where patientCaseId = @patientCaseId ", new { patientCaseId = patientCaseId });
        }

        public CaseDispatchDetail GetByPatientCaseId(long patientcaseid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientcaseid = @patientcaseid ", new { patientcaseid = patientcaseid }).FirstOrDefault();
        }
    }
}