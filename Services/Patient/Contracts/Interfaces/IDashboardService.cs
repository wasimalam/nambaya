using Patient.Contracts.Models;
using System.Collections.Generic;

namespace Patient.Contracts.Interfaces
{
    public interface IDashboardService
    {
        long GetNewCasesCount(long pharmacyid);
        long GetPendingCasesCount(long pharmacyid);
        long GetQECompletedCount(long pharmacyid);
        long GetDetailedCompletedCount(long pharmacyid);
        GoalCompletedBO GetGoalCompletedPercent();
        IEnumerable<MonthlyCasesBO> GetMonthlyCasesStarted();
        IEnumerable<MonthlyCasesBO> GetMonthlyCasesCompleted();
        IEnumerable<MonthlyCasesBO> GetMonthlyCasesDispatched();
        IEnumerable<CaseStatsBO> GetCardiologistNotesStats();
        long GetTotalCardiologistActiveCases();
        long GetTotalCardiologistAssignedCases();
        IEnumerable<QEResultStatBO> GetQEResultStats();
    }
}
