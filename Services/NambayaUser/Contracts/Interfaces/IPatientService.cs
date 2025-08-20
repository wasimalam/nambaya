using Patient.Contracts.Models;
using System.Collections.Generic;

namespace NambayaUser.Contracts.Interfaces
{
    public interface IPatientService
    {
        #region dashboard
        GoalCompletedBO GetGoalcompletedPercent();
        List<MonthlyCasesBO> GetMonthlyCasesStarted();
        List<MonthlyCasesBO> GetMonthlyCasesCompleted();
        List<MonthlyCasesBO> GetMonthlyCasesDispatched();
        List<CaseStatsBO> GetCardiologistNotesStats();
        List<QEResultStatBO> GetQEResultStats();
        #endregion
    }
}
