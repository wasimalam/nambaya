using Patient.Contracts.Models;
using System.Collections.Generic;

namespace Patient.Contracts.Interfaces
{
    public interface IDrugService
    {
        List<DrugGroupBO> GetDrugGroups(long patientcaseid);
        DrugGroupBO GetDrugGroupExist(long druggroupid);
        DrugGroupBO GetDrugGroup(long druggroupid);
        long AddDrugGroup(DrugGroupBO drugGroupBO);
        void UpdateDrugGroup(DrugGroupBO drugGroupBO);
        void DeleteDrugGroup(DrugGroupBO drugGroupBO);
        DrugDetailsBO GetDrugDetails(long drugdetailsid);
        long AddDrugDetails(DrugDetailsBO drugDetailsBO);
        void UpdateDrugDetails(DrugDetailsBO drugDetailsBO);
        void DeleteDrugDetails(DrugDetailsBO drugDetailsBO);
        DrugIngredientsBO GetDrugIngredients(long drugingredientsid);
        long AddDrugIngredients(DrugIngredientsBO drugIngredientsBO);
        DrugFreeTextBO GetDrugFreeText(long drugfreetextid);
        long AddDrugFreeText(DrugFreeTextBO drugFreeTextBO);
        void UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO);
        void DeleteDrugFreeText(DrugFreeTextBO drugFreeTextBO);
        DrugReceipeBO GetDrugReceipe(long drugreceipeid);
        long AddDrugReceipe(DrugReceipeBO drugReceipeBO);
        void UpdateDrugReceipe(DrugReceipeBO drugReceipeBO);
        void DeleteDrugReceipe(DrugReceipeBO drugReceipeBO);
        void UploadMedicationFile(MedicationPlanFileBO medicationPlanFileBO);
        MedicationPlanFileBO GetMedicationFile(long patientcaseid);
        MedicationPlanFileBO DownloadMedicationFile(long patientcaseid);
        void DeleteMedicationPlanFiles(long patientcaseid);
    }
}
