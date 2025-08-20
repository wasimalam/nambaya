using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public class CaseNotesRepository : DapperRepositoryBase<CaseNotes>, ICaseNotesRepository
    {
        public CaseNotesRepository(IDatabaseSession session) : base(session)
        {
        }
        public IEnumerable<CaseNotes> GetByPatientCaseId(long patientcaseid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientcaseid = @patientcaseid ", new { patientcaseid = patientcaseid });
        }
      
    }
}