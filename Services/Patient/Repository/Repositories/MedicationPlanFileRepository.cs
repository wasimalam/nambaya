using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Repositories
{
    public class MedicationPlanFileRepository : DapperRepositoryBase<MedicationPlanFile>, IMedicationPlanFileRepository
    {
        public MedicationPlanFileRepository(IDatabaseSession session) : base(session)
        {

        }
        public IEnumerable<MedicationPlanFile> GetByPatientCaseId(long patientcaseid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientcaseid = @patientcaseid ", new { patientcaseid = patientcaseid });
        }
    }
}