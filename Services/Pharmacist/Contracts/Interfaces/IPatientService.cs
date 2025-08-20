using Common.Infrastructure;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Models;
using System.Collections.Generic;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IPatientService
    {
        PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string filter);
        PatientBO GetPatientbyID(long id);
        PatientBO GetPatientbyCaseID(long patientcaseid);
        PatientCaseBO GetPatientCasebyID(long patientcaseid);
        PatientBO AddPatient(PatientBO patient);
        string GenerateDeActivatePatientVerification(long patientid);
        void VerifyDeActivatePatient(VerifyDeactivatePatientOtpRequest req);
        PatientBO UpdatePatient(PatientBO patient);
        PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO);
        PatientBO ImportXml(byte[] fileContent, string pharmacypatientid, string fileName, string fileContentType);
        PatientEDFFileBO GetPatientEDFFile(long patientcaseid);
        byte[] DownloadEDFFile(long patientcaseid);
        void UploadEDFFile(PatientEDFFileBO patientEDFFileBO, byte[] fileData);
        QuickEvaluationFileBO DownloadQuickEvalFile(long patientcaseid);
        List<string> DownloadQuickEvalImages(long patientcaseid);
        void UploadQuickEvalFile(QuickEvaluationFileBO quickEvaluationFileBO, byte[] fileData);
        QuickEvaluationResultBO AddQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult);
        QuickEvaluationResultBO GetQuickEvaluationResultByCaseID(long patientcaseid);
        QuickEvaluationResultBO UpdateQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult);
        #region Additional Info
        PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid);
        PatientAdditionalInfoBO AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo);
        PatientAdditionalInfoBO UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo);
        #endregion
        #region drug related functions
        List<DrugGroupBO> GetDrugGroups(long patientcaseid);
        DrugGroupBO GetDrugGroup(long druggroupid);
        DrugGroupBO AddDrugGroup(DrugGroupBO drugGroupBO);
        DrugGroupBO UpdateDrugGroup(DrugGroupBO drugGroupBO);
        void DeleteDrugGroup(DrugGroupBO drugGroupBO);
        DrugDetailsBO AddDrugDetails(DrugDetailsBO drugDetailsBO);
        DrugDetailsBO UpdateDrugDetails(DrugDetailsBO drugDetailsBO);
        void DeleteDrugDetails(DrugDetailsBO drugDetailsBO);
        DrugIngredientsBO AddDrugIngredients(DrugIngredientsBO drugIngredientsBO);
        DrugFreeTextBO AddDrugFreeText(DrugFreeTextBO drugFreeTextBO);
        DrugFreeTextBO UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO);
        void DeleteDrugFreeText(DrugFreeTextBO drugFreeTextBO);
        DrugReceipeBO AddDrugReceipe(DrugReceipeBO drugReceipeBO);
        DrugReceipeBO UpdateDrugReceipe(DrugReceipeBO drugReceipeBO);
        void DeleteDrugReceipe(DrugReceipeBO drugReceipeBO);
        #endregion
        #region Dashboard
        object GetPharmacyStats();
        GoalCompletedBO GetGoalcompletedPercent();
        List<MonthlyCasesBO> GetMonthlyCasesStarted();
        List<MonthlyCasesBO> GetMonthlyCasesCompleted();
        List<QEResultStatBO> GetQEResultStats();
        #endregion
    }
}
