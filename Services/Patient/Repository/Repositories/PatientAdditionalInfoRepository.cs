using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class PatientAdditionalInfoRepository : DapperRepositoryBase<PatientAdditionalInfo>, IPatientAdditionalInfoRepository
    {
        public PatientAdditionalInfoRepository(IDatabaseSession session) : base(session)
        {

        }
        public PatientAdditionalInfo GetByPatientCaseId(long patientcaseid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientcaseid = @patientcaseid ", new { patientcaseid = patientcaseid }).FirstOrDefault();
        }
    }
}