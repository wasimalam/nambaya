using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using Patient.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Service
{
    public class DashboardService : BaseService, IDashboardService
    {
        private readonly IPatientCasesRepository _patientCasesRepository;
        private readonly IConfiguration _configuration;
        public DashboardService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _patientCasesRepository = _serviceProvider.GetRequiredService<IPatientCasesRepository>();
            _configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        }

        public long GetNewCasesCount(long pharmacyid)
        {
            return _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text, $"select count(pc.id) from [PatientCases]  pc " +
                $"inner join [Patient] p on pc.PatientID = p.ID and p.PharmacyID = @pharmacyid" +
                $" where pc.StatusID = @casestatus and pc.IsActive <> 0", new { casestatus = CaseStatus.CaseStarted, pharmacyid = pharmacyid });
        }
        public long GetPendingCasesCount(long pharmacyid)
        {
            return _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text, $"select count(pc.id) from [PatientCases]  pc " +
                $"inner join [Patient] p on pc.PatientID = p.ID and p.PharmacyID = @pharmacyid" +
                $" where ((pc.StatusID = @casestatus1 OR pc.StatusID = @casestatus2 OR pc.StatusID = @casestatus3)  and pc.IsActive <> 0)", new
                {
                    casestatus1 = CaseStatus.DeviceAllocated,
                    casestatus2 = CaseStatus.DeviceReturned,
                    casestatus3 = CaseStatus.QuickEvalInQueue,
                    pharmacyid
                });
        }

        public long GetQECompletedCount(long pharmacyid)
        {
            return _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text, $"select count(pc.id) from [PatientCases]  pc " +
                $"inner join [Patient] p on pc.PatientID = p.ID and p.PharmacyID = @pharmacyid" +
                $" where ((pc.StatusID = @casestatus1 OR pc.StatusID = @casestatus2)  and pc.IsActive <> 0)", new
                {
                    casestatus1 = CaseStatus.QuickEvalCompleted,
                    casestatus2 = CaseStatus.DetailEvalLocked,
                    pharmacyid
                });
        }
        public long GetDetailedCompletedCount(long pharmacyid)
        {
            return _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text, $"select count(pc.id) from [PatientCases]  pc " +
                $"inner join [Patient] p on pc.PatientID = p.ID and p.PharmacyID = @pharmacyid" +
                $" where ((pc.EndDate is not null and pc.StatusID >= @casestatus1)  and pc.IsActive <> 0)", new
                {
                    casestatus1 = CaseStatus.DetailEvalCompleted,
                    pharmacyid
                });
        }

        public GoalCompletedBO GetGoalCompletedPercent()
        {
            var completed = _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text, $"select count(pc.id) from [PatientCases]  pc " +
                $" where (pc.EndDate is not null and pc.StatusID >= @casestatus1)", new
                {
                    casestatus1 = CaseStatus.DetailEvalCompleted
                });
            var strGoal = _configuration.GetSection(ConfigurationConsts.TotalCasesGoal).Value;
            long goal = 3000;
            if (string.IsNullOrWhiteSpace(strGoal) == false)
                long.TryParse(strGoal, out goal);
            return new GoalCompletedBO()
            {
                NumberCompleted = completed,
                PercentCompleted = ((double)completed * 100 / (double)goal),
                Goal = goal
            };
        }

        public IEnumerable<MonthlyCasesBO> GetMonthlyCasesStarted()
        {
            return _patientCasesRepository.GetCustomItems<MonthlyCasesBO>(System.Data.CommandType.Text,
                $"SELECT DATEPART(month, StartDate) AS Month, DATEPART(year, StartDate) AS [Year], COUNT(ID) AS CasesCount" +
                $" FROM [PatientCases] GROUP BY DATEPART(month, StartDate),  DATEPART(year, StartDate)").ToList();
        }

        public IEnumerable<MonthlyCasesBO> GetMonthlyCasesCompleted()
        {
            return _patientCasesRepository.GetCustomItems<MonthlyCasesBO>(System.Data.CommandType.Text,
                $"SELECT DATEPART(month, EndDate) AS Month, DATEPART(year, EndDate) AS [Year], COUNT(ID) AS CasesCount" +
                $" FROM [PatientCases] where EndDate is not null and StatusID >= @casestatus1 GROUP BY DATEPART(month, EndDate),  DATEPART(year, EndDate)", new
                {
                    casestatus1 = CaseStatus.DetailEvalCompleted
                }).ToList();
        }

        public IEnumerable<MonthlyCasesBO> GetMonthlyCasesDispatched()
        {
            return _patientCasesRepository.GetCustomItems<MonthlyCasesBO>(System.Data.CommandType.Text,
                $"SELECT DATEPART(month, DispatchDate) AS Month, DATEPART(year, DispatchDate) AS [Year], COUNT(cdd.ID) AS CasesCount" +
                $" FROM [CaseDispatchDetail] cdd inner join PatientCases pc on cdd.PatientCaseID = pc.ID" +
                $" where DispatchDate is not null and pc.StatusID=@casestatus1 GROUP BY DATEPART(month, DispatchDate),  DATEPART(year, DispatchDate)", 
                new
                {
                    casestatus1 = CaseStatus.ReportDispatched
                }).ToList();
        }

        public IEnumerable<CaseStatsBO> GetCardiologistNotesStats()
        {
            var sessionContext = GetSessionContext();
            if (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse)
                return _patientCasesRepository.GetCustomItems<CaseStatsBO>(System.Data.CommandType.Text,
                    $"SELECT COALESCE(NotesTypeID,0) as Code, Count(qer.ID) AS CasesCount FROM [DetailEvaluation] qer" +
                    $" inner join patientcases pc on qer.PatientCaseID = pc.ID and pc.CardiologistID = @cardioid" +
                    $" GROUP By NotesTypeID",
                    new
                    {
                        cardioid = sessionContext.CardiologistID
                    });
            else
                return _patientCasesRepository.GetCustomItems<CaseStatsBO>(System.Data.CommandType.Text,
                $"SELECT COALESCE(NotesTypeID,0) as Code, Count(ID) AS CasesCount FROM [DetailEvaluation] GROUP By NotesTypeID");
        }
        public long GetTotalCardiologistActiveCases()
        {
            return _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text,
                $"SELECT Count(pc.ID) AS CasesCount" +
                $" FROM [PatientCases] pc where StatusID = @casestatus" +
                $" and pc.IsActive <> 0", new
                {
                    casestatus = CaseStatus.QuickEvalCompleted
                });
        }
        public long GetTotalCardiologistAssignedCases()
        {
            var sessionContext = GetSessionContext();
            if (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse)
                return _patientCasesRepository.ExecuteScalar<long>(System.Data.CommandType.Text,
                    $"SELECT Count(pc.ID) AS CasesCount FROM [PatientCases] pc where CardiologistID = @cardioid and pc.IsActive <> 0",
                    new
                    {
                        cardioid = sessionContext.CardiologistID
                    });
            else
                return 0;
        }
        public IEnumerable<QEResultStatBO> GetQEResultStats()
        {
            var sessionContext = GetSessionContext();
            if (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse)
                return _patientCasesRepository.GetCustomItems<QEResultStatBO>(System.Data.CommandType.Text,
                    $"SELECT count(PatientCaseID) AS CasesCount, QuickResultID as QEResultID FROM QuickEvaluationResult qer" +
                    $" inner join patientcases pc on qer.PatientCaseID = pc.ID and pc.CardiologistID = @cardioid" +
                    $" Group by QuickResultID",
                    new
                    {
                        cardioid = sessionContext.CardiologistID
                    });
            else
                return _patientCasesRepository.GetCustomItems<QEResultStatBO>(System.Data.CommandType.Text,
                    $"SELECT count(PatientCaseID) AS CasesCount, QuickResultID as QEResultID FROM QuickEvaluationResult Group by QuickResultID");
        }
    }
}