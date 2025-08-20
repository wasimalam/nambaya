using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class PatientCasesRepository : DapperRepositoryBase<Models.PatientCases>, IPatientCasesRepository
    {
        public PatientCasesRepository(IDatabaseSession session) : base(session)
        {
        }

        List<PatientCases> IPatientCasesRepository.GetByPatientId(long patientid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientid=@patientid;",
             new { patientid = patientid }).ToList();
        }
    }
}
