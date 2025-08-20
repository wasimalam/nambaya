using Common.Infrastructure;
using Patient.Contracts.Models;
using System.Collections.Generic;

namespace Patient.Contracts.Interfaces
{
    public interface IPatientService
    {
        PagedResults<PatientBO> GetCasesToDispatch(int limit, int offset, string orderby, string filter);
        PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string param);
        PatientBO GetPatientbyCaseID(long patientcaseID);
        PatientCaseBO GetPatientCase(long patientcaseID);
        long AddPatient(PatientBO patientBO, bool bOverrideExisting = false);
        void UpdatePatient(PatientBO patientBO);
        PatientBO GetPatientbyID(long patientID);
        void DeletePatient(PatientBO patientBO);
        void RaiseNavigatorCleanEvent(PatientBO patientBO);
        PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO);
        void AssignDevice(DeviceAssignmentBO deviceAssignment);
        DeviceAssignmentBO GetDeviceAssignmentById(long id);
        DeviceAssignmentBO GetDeviceAssignmentByCaseId(long patientcaseid);
        PatientBO GetPatientByAssignedDevice(long deviceid);
        PatientBO AssignPatientCase(long patientcaseid);
        PatientBO DeActivatePatient(long patientId);
        List<PatientCaseBO> GetPatientCasesByPatient(long patientid);
    }
}
