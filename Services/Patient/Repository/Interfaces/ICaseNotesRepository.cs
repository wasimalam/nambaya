using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface ICaseNotesRepository : IDapperRepositoryBase<CaseNotes>
    {
        IEnumerable<CaseNotes> GetByPatientCaseId(long patientcaseid);
    }
}
