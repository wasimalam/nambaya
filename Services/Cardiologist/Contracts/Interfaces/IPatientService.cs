using Cardiologist.Contracts.Models;
using Common.Infrastructure;
using Patient.Contracts.Models;
using System.Collections.Generic;

namespace Cardiologist.Contracts.Interfaces
{
    public interface IPatientService
    {
        #region Patient Functions 
        PagedResults<PatientBO> GetPatients(int limit, int offset, string orderby, string filter);
        PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string filter);
        PatientBO GetPatientbyID(long id);
        PatientBO GetPatientbyCaseID(long patientcaseid);
        PatientCaseBO GetPatientCasebyID(long patientcaseid);
        PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO);
        #endregion
        #region Evaluation Functions
        byte[] DownloadEDFFile(long patientcaseid);
        DetailEvaluationBO GetDetailReport(long patientcaseid);
        DetailEvaluationBO UpdateDetailEvaluationData(DetailEvaluationBO detailEvaluationBO);
        byte[] DownloadDetailReport(long patientcaseid);
        void UploadDetailEvaluationFile(DetailEvaluationBO detailEvaluationBO, byte[] fileData);
        string GenerateDESignatureOTP(long patientcaseid);
        void VerifyECGFileUploadSignature(VerifyECGUploadOtpRequest req, DetailEvaluationBO detailEvaluationBO);
        void VerifyExistingECGFileWithSignature(VerifyECGUploadOtpRequest req);
        #endregion
        #region Additional Info
        PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid);
        #endregion
        #region drug related functions
        List<DrugGroupBO> GetDrugGroups(long patientcaseid);
        DrugGroupBO GetDrugGroup(long druggroupid);
        #endregion
        #region Dashboard
        GoalCompletedBO GetGoalcompletedPercent();
        List<MonthlyCasesBO> GetMonthlyCasesCompleted();
        long GetTotalCardiologistAssignedCases();
        long GetTotalCardiologistActiveCases();
        List<CaseStatsBO> GetCardiologistNotesStats();
        List<QEResultStatBO> GetQEResultStats();
        #endregion
    }
}
