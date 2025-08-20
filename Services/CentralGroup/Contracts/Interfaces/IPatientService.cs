using Common.Infrastructure;
using Patient.Contracts.Models;
using System.Collections.Generic;

namespace CentralGroup.Contracts.Interfaces
{
    public interface IPatientService
    {
        PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string filter);
        PatientBO GetPatientbyID(long id);
        PatientBO GetPatientbyCaseID(long patientcaseid);
        PatientCaseBO GetPatientCasebyID(long patientcaseid);
        PatientBO AddPatient(PatientBO patient);
        PatientBO UpdatePatient(PatientBO patient);
        PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO);
        #region Evaluation Funtions
        QuickEvaluationFileBO DownloadQuickEvalFile(long patientcaseid);
        QuickEvaluationResultBO GetQuickEvaluationResultByCaseID(long patientcaseid);
        DetailEvaluationBO GetDetailEvaluationByCaseID(long patientcaseid);
        byte[] DownloadDetailEvaluationFile(long patientcaseid);
        #endregion
        PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid);
        PatientAdditionalInfoBO AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo);
        PatientAdditionalInfoBO UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo);
        CaseDispatchDetailBO AddCaseDispatchDetails(CaseDispatchDetailBO caseDispatchDetailBO);
        CaseDispatchDetailBO GetCaseDispatchDetails(long patientCaseId);
        #region drug related functions
        List<DrugGroupBO> GetDrugGroups(long patientcaseid);
        DrugGroupBO GetDrugGroup(long druggroupid);
        DrugGroupBO AddDrugGroup(DrugGroupBO drugGroupBO);
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
