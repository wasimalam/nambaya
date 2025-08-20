import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

export interface WizardDataContext {
  currentStep: number;
  step522URL: string; // Patient Details
  editPatientStepURL: string; // Patient Details
  step523URL: string; // Medication
  medicationStepURL: string; // Medication
  step525URL: string; // Assign Device
  deviceStepURL: string; // Assign Device
  step526URL: string; // Upload EDF
  edfStepURL: string; // Upload EDF
  step527URL: string; // Quick Evaluation
  qeStepURL: string; // Quick Evaluation
  prevStepURL: string;
  skipStepURL: string;
  patientId: number;
  pharmacyPatientId: string;
  patientName: string;
  caseStatusId: number;
  caseId: number;
}
export interface DeactivatePatientContext {
  otp: string;
  token: string;
  email: string;
  patientId: number;
}

@Injectable()
export class PatientWizardService {
  private pharmacistServiceBaseUrl = new APIUrls().PharmacistServiceBaseUrl;
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;

  private URLs = {
    updateCaseStep: `${this.pharmacistServiceBaseUrl}/api/v1/patient/cases/step`,
    caseSteps: `${this.pharmacistServiceBaseUrl}/api/v1/lookups/items?code=CASESTEP`,
    getCase: `${this.patientServiceUrl}/api/v1/patient/cases`,
    getECGReportData: `${this.patientServiceUrl}/api/v1/patient/GetECGReportData?patientCaseId=`,
    // deactivatePatient: `${this.pharmacistServiceBaseUrl}/api/v1/patient/deActivatePatient`,
    deactivatePatient: `${this.pharmacistServiceBaseUrl}/api/v1/patient/patientdeactivateverification`,
    getDeactivatePatientToken: `${this.pharmacistServiceBaseUrl}/api/v1/patient/patientdeactivateverification?patientid=`
  };

  constructor(private http: HttpClient) {}

  getCaseSteps(): any {
    const url = `${this.URLs.caseSteps}`;

    return this.http.get(url, { observe: 'response' });
  }

  updateCaseStep(patientId: number, stepId: number): any {
    const url = `${this.URLs.updateCaseStep}/${patientId}?stepId=${stepId}`;

    return this.http.put(url, { observe: 'response' });
  }

  generateDeactivatePatientVerificationOtp(patientId?: number): any {
    const url = `${this.URLs.getDeactivatePatientToken + patientId}`;

    return this.http.get(url, { observe: 'response' });
  }

  deActivatePatient(patientId: number): any {
    const url = `${this.URLs.deactivatePatient}/${patientId}`;

    return this.http.put(url, { observe: 'response' });
  }

  deActivatePatientWithOTP(deactivatePatientContext: DeactivatePatientContext): any {
    const url = `${this.URLs.deactivatePatient}`;

    return this.http.post(url, deactivatePatientContext, { observe: 'response' });
  }

  getCase(patientCaseId: number): any {
    const url = `${this.URLs.getCase}/${patientCaseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  getECGReportData(patientCaseId: number): any {
    const url = `${this.URLs.getECGReportData}${patientCaseId}`;

    return this.http.get(url, { observe: 'response' });
  }
}
