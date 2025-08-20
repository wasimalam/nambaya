using Patient.Contracts.Models;
using System.Collections.Generic;

namespace Patient.Contracts.Interfaces
{
    public interface IEvaluationService
    {
        PatientEDFFileBO GetPatientEDFFile(long patientcaseid);
        PatientEDFFileBO DownloadEDFFile(long patientcaseid);
        //byte[] DownloadEDFFileData(long patientcaseid);
        void UploadEDFFile(PatientEDFFileBO patientEDFFile);
        void DeleteEDFFiles(long patientcaseid);
        DetailEvaluationBO GetDetailEvaluationData(long patientcaseid);
        DetailEvaluationBO UpdateDetailEvaluationData(DetailEvaluationBO detailEvaluationBO);
        DetailEvaluationBO DownloadDetailEvaluationFile(long patientcaseid);
        void UploadDetailEvaluationFile(DetailEvaluationBO patientEcgFile);
        void DeleteDetailEvaluationFiles(long patientcaseid);
        DetailEvaluationBO GetPatientEvaluationByCaseID(long patientcaseid);
        QuickEvaluationResultBO GetQuickEvaluationResultByCaseID(long patientcaseid);
        long AddQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResultBO);
        void UpdateQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResultBO);
        QuickEvaluationFileBO GetQuickEvalFile(long patientcaseid);
        QuickEvaluationFileBO DownloadQuickEvalFile(long patientcaseid);
        List<string> DownloadQuickEvalImages(long patientcaseid);
        void UploadQuickEvalFile(QuickEvaluationFileBO quickEvalFileBO);
    }
}
