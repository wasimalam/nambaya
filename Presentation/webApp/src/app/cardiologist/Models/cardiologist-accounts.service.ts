import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Addresses, APIUrls } from '@app/shared/Addresses';

export interface CardiologistAccountsContext {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  street: string;
  zipCode: string;
  cityID: string;
  pharmacyID: number;
  id: number;
  isActive: boolean;
  companyID?: string;
  cardiologistID?: string;
  role: string;
}

export interface CardioDeleteSignatureContext {
  otp: string;
  token: string;
  email: string;
}

export interface CardioWizardDataContext {
  currentStep: number;
  medicationStepURL: string; // Medication
  ecgStepURL: string; // Upload ECG
  qeStepURL: string; // Quick Evaluation
  patientId: number;
  pharmacyPatientId: string;
  patientName: string;
  caseStatusId: number;
  caseId: number;
  navigatorCaseId: string;
}

@Injectable()
export class CardiologistAccountsService {
  private CardiologistServiceBaseUrl = new APIUrls().CardiologistServiceBaseUrl;
  private URLs = {
    getList: `${this.CardiologistServiceBaseUrl}/api/v1`,
    saveCardiologist: `${this.CardiologistServiceBaseUrl}/api/v1/cardiologist`,
    saveNurse: `${this.CardiologistServiceBaseUrl}/api/v1/nurse`,
    getSettings: `${this.CardiologistServiceBaseUrl}/api/v1/cardiologist/settings`,
    getNurseSettings: `${this.CardiologistServiceBaseUrl}/api/v1/nurse/settings`,
    saveSettings: `${this.CardiologistServiceBaseUrl}/api/v1/cardiologist/updateSettings`,
    saveNurseSettings: `${this.CardiologistServiceBaseUrl}/api/v1/nurse/updateSettings`,
    getLanguages: `${this.CardiologistServiceBaseUrl}/api/v1/lookups/languages`,
    get2FOptions: `${this.CardiologistServiceBaseUrl}/api/v1/lookups/items?code=2FactorNotificationType`,
    assignedCasesUrl: `${this.CardiologistServiceBaseUrl}/api/v1/patient/dashboard/totalcardiologistassignedcases`,
    activeCasesUrl: `${this.CardiologistServiceBaseUrl}/api/v1/patient/dashboard/totalcardiologistactivecases`,
    goalCompletedPercentageUrl: `${this.CardiologistServiceBaseUrl}/api/v1/patient/dashboard/goalcompletedpercent`,
    monthlycasesCompletedUrl: `${this.CardiologistServiceBaseUrl}/api/v1/patient/dashboard/monthlycasescompleted`,
    cardiologistNoteStatsUrl: `${this.CardiologistServiceBaseUrl}/api/v1/patient/dashboard/cardiologistnotesstats`,
    qeStatusStatsUrl: `${this.CardiologistServiceBaseUrl}/api/v1/patient/dashboard/qeResultStats`,
    getSignatureData: `${this.CardiologistServiceBaseUrl}/api/v1/cardiologist/getSignaturesData`,
    getDeleteSignatureToken: `${this.CardiologistServiceBaseUrl}/api/v1/cardiologist/GetDeleteSignaturesToken`,
    deleteSignature: `${this.CardiologistServiceBaseUrl}/api/v1/cardiologist/DeleteSignatureData`
  };

  constructor(private http: HttpClient) {}

  getData(id?: number, applicationId?: string): any {
    const url = `${this.URLs.getList}/${applicationId}/${id}`;

    return this.http.get(url);
  }

  public getAssignedCasesStats() {
    return this.http.get(this.URLs.assignedCasesUrl);
  }

  public getSignatureData() {
    return this.http.get(this.URLs.getSignatureData, { observe: 'response' });
  }

  public getActiveCasesStats() {
    return this.http.get(this.URLs.activeCasesUrl);
  }

  public getGoalsCompleted() {
    return this.http.get(this.URLs.goalCompletedPercentageUrl);
  }

  public getMonthlyCaseCompleted() {
    return this.http.get(this.URLs.monthlycasesCompletedUrl);
  }

  public getCardiologistNoteStats() {
    return this.http.get(this.URLs.cardiologistNoteStatsUrl);
  }

  public getQEStatusStats() {
    return this.http.get(this.URLs.qeStatusStatsUrl);
  }

  public getSettingsData(roleCode: any) {
    if (roleCode === 'Nurse') {
      return this.http.get(`${this.URLs.getNurseSettings}`);
    }

    return this.http.get(`${this.URLs.getSettings}`);
  }

  getLanguages = (): any => this.http.get(`${this.URLs.getLanguages}`);
  get2FOptions = (): any => this.http.get(`${this.URLs.get2FOptions}`);

  updateCardiologistSettings(cardiologistAccountsContext: CardiologistAccountsContext): any {
    if (cardiologistAccountsContext.role === 'Nurse') {
      return this.http.put(this.URLs.saveNurse, cardiologistAccountsContext, { observe: 'response' });
    }

    return this.http.put(this.URLs.saveCardiologist, cardiologistAccountsContext, { observe: 'response' });
  }

  updateCardiologistPreferences(cardiologistPreferences: any, roleCode: any): any {
    if (roleCode === 'Nurse') {
      return this.http.put(this.URLs.saveNurseSettings, cardiologistPreferences, { observe: 'response' });
    }

    return this.http.put(this.URLs.saveSettings, cardiologistPreferences, { observe: 'response' });
  }

  generateDeleteSignatureOtp(): any {
    return this.http.get(this.URLs.getDeleteSignatureToken, { observe: 'response' });
  }

  deleteSignature(cardioDeleteSignatureContext: CardioDeleteSignatureContext): any {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      body: cardioDeleteSignatureContext
    };

    return this.http.delete(this.URLs.deleteSignature, options);
  }
}
