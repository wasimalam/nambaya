import { APIUrls } from '@app/shared/Addresses';
import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface PatientContext {
  FirstName: string;
  LastName: string;
  Email: string;
  dateOfBirth: string;
  genderID: number;
  InsuranceNumber: number;
  Phone: string;
  Street: string;
  Zip: number;
  Address: string;
  County: string;
  CityID: string;
  isActive: boolean;
  PharmacyPatientID: string;
  PharmacyID: number;
  id?: number;
  doctorId: number;
  caseID: number;
}

export interface DoctorContext {
  FirstName: string;
  LastName: string;
  Email: string;
  Phone: string;
  Street: string;
  Zip: number;
  Address: string;
  id?: number;
  doctorId: number;
  companyId: number;
}

export interface ReportContext {
  isMedicationPlanAttached: boolean;
  isDetailEvaluationAttached: boolean;
  patientCaseID: number;
  dispatchDate: string;
}
@Injectable()
export class PatientsService {
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;

  private URLs = {
    getGenderOptions: `${this.patientServiceUrl}/api/v1/lookups/genders`,
    getPatient: `${this.patientServiceUrl}/api/v1/patient`,
    deletePatient: `${this.patientServiceUrl}/api/v1/patient`,
    getDoctors: `${this.patientServiceUrl}/api/v1/doctor`,
    getDoctor: `${this.patientServiceUrl}/api/v1/doctor/`,
    getDispatchDetails: `${this.patientServiceUrl}/api/v1/patient/cases/caseDispatchDetails/`,
    savePatient: `${this.patientServiceUrl}/api/v1/patient`,
    saveDoctor: `${this.patientServiceUrl}/api/v1/doctor`,
    deleteDoctor: `${this.patientServiceUrl}/api/v1/doctor/`,
    importPatient: `${this.pharmacistServiceUrl}/api/v1/patient/ImportPatientFile`,
    getPharmacyIdPrefixUrl: `${this.pharmacistServiceUrl}/api/v1/pharmacy/GetPharmacyId`,
    sendCompletedStudyUrl: `${this.patientServiceUrl}/api/v1/patient/cases/casedispatchdetails`
  };

  constructor(private http: HttpClient) {}

  public getGenderOptions(): Observable<any> {
    const url = `${this.URLs.getGenderOptions}`;

    return this.http.get(url);
  }

  public getPatientData(id?: number): Observable<any> {
    if (id) {
      const url = `${this.URLs.getPatient}/${id}`;
      return this.http.get(url);
    }
  }

  public getDoctors() {
    return this.http.get(`${this.URLs.getDoctors}`);
  }

  public getDoctor(doctorId: number) {
    return this.http.get(this.URLs.getDoctor + doctorId, { observe: 'response' });
  }

  public getDispatchDetails(caseId: number) {
    return this.http.get(this.URLs.getDispatchDetails + caseId, { observe: 'response' });
  }

  createPatient(patientContext: PatientContext): any {
    patientContext.isActive = true;
    return this.http.post(this.URLs.savePatient, patientContext, { observe: 'response' });
  }

  createDoctor(doctorContext: DoctorContext): any {
    return this.http.post(this.URLs.saveDoctor, doctorContext, { observe: 'response' });
  }

  updateDoctor(doctorContext: DoctorContext): any {
    return this.http.put(this.URLs.saveDoctor, doctorContext, { observe: 'response' });
  }

  deleteDoctor(doctorId: number): any {
    return this.http.delete(this.URLs.deleteDoctor + doctorId, { observe: 'response' });
  }

  deleteDeactivatedPatient(patientId: number): any {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      body: { id: patientId }
    };

    return this.http.delete(this.URLs.deletePatient, options);
  }

  public sendCompletedStudy(study: any): any {
    return this.http.post(this.URLs.sendCompletedStudyUrl, study, { observe: 'response' }).pipe();
  }

  updatePatient(patientContext: PatientContext): any {
    patientContext.isActive = true;

    return this.http.put(this.URLs.savePatient, patientContext, { observe: 'response' });
  }

  getPharmacyIdPrefix(): Observable<string> {
    return this.http.get(this.URLs.getPharmacyIdPrefixUrl, { responseType: 'text' });
  }

  importPatient(fileToUpload: File, pharmacyPatientID: string): Observable<any> {
    const formData: FormData = new FormData();

    formData.append('fileKey', fileToUpload, fileToUpload.name);
    const url = this.URLs.importPatient + '?pharmacypatientid=' + pharmacyPatientID;
    return this.http
      .post(url, formData, {
        reportProgress: true,
        observe: 'events'
      })
      .pipe(
        map(event => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              const progress = Math.round((100 * event.loaded) / event.total);
              // const progress = event.loaded / 100;
              return { status: 'progress', message: progress };
            case HttpEventType.Response:
              return event.body;
            default:
              return `Unhandled event: ${event.type}`;
          }
        })
      );
  }
}
