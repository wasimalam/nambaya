using Patient.Contracts.Models;
using System.Collections.Generic;

namespace Patient.Contracts.Interfaces
{
    public interface ICaseNotesService
    {
        IEnumerable<CaseNotesBO> GetCaseNotes(long patientcaseid);
        CaseNotesBO GetCaseNote(long casenoteid);
        long AddCaseNotes(CaseNotesBO caseNotesBO);
    }
}
