import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { HttpClient, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { map, catchError, tap } from 'rxjs/operators';

import { APIUrls } from '@app/shared/Addresses';

export interface CardioECGFileContext {
  otp: string;
  token: string;
  email: string;
  notes: string;
  notesTypeId: number;
  patientCaseId: number;
}

export interface CardioSigImgContext {
  otp: string;
  token: string;
  email: string;
}

export interface CardioSigStringContext {
  otp: string;
  token: string;
  email: string;
  ImageData: string;
}

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  PatientServiceUrl = new APIUrls().PatientServiceBaseUrl;
  CardioServiceUrl = new APIUrls().CardiologistServiceBaseUrl;
  PharmacistServiceBaseUrl = new APIUrls().PharmacistServiceBaseUrl;

  private urls = {
    postEdfFileUrl: `${this.PatientServiceUrl}/api/v1/patient/uploadedffile?patientcaseid=`,
    getMedicationFile: `${this.PatientServiceUrl}/api/v1/patient/getMedicationFile?patientcaseid=`,
    postMedicationPlan: `${this.PatientServiceUrl}/api/v1/patient/UploadMedicationFile?patientcaseid=`,
    postEcgFileUrl: `${this.CardioServiceUrl}/api/v1/patient/uploadEcgfile`,
    uploadECGFileUnsigned: `${this.CardioServiceUrl}/api/v1/patient/uploadECGFileUnsigned`,
    postSigFileUrl: `${this.CardioServiceUrl}/api/v1/cardiologist/UploadSignatureFile`,
    postSigStringUrl: `${this.CardioServiceUrl}/api/v1/cardiologist/UploadSignatureData`,
    downloadEdfFileUrl: `${this.PatientServiceUrl}/api/v1/patient/downloadedffile?patientcaseid=`,
    downloadEcgFileUrl: `${this.PatientServiceUrl}/api/v1/patient/downloadecgfile?patientcaseid=`,
    downloadMedication: `${this.PatientServiceUrl}/api/v1/patient/DownloadMedicationFile?patientcaseid=`,
    getNoteTypes: `${this.CardioServiceUrl}/api/v1/lookups/items?code=CARDIOREMARKSTYPE`,
    getPatientEDFFile: `${this.PharmacistServiceBaseUrl}/api/v1/patient/GetPatientEDFFile?patientcaseid=`,
    getPatientQuickEvalFile: `${this.PatientServiceUrl}/api/v1/patient/GetQuickEvaluationFile?patientcaseid=`,
    getEcguploadToken: `${this.CardioServiceUrl}/api/v1/patient/UploadEcgToken?patientcaseid=`,
    getSignatureUploadToken: `${this.CardioServiceUrl}/api/v1/cardiologist/GetSaveSignaturesToken`
  };

  constructor(private http: HttpClient) {}

  postFile(fileToUpload: File, patiendId: number): Observable<any> {
    const formData: FormData = new FormData();
    const headers = new HttpHeaders({ 'ngsw-bypass': 'true' });
    formData.append('fileKey', fileToUpload, fileToUpload.name);
    return this.http
      .post(this.urls.postEdfFileUrl + patiendId, formData, {
        reportProgress: true,
        observe: 'events',
        headers: headers
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

  uploadMedicationPlanFile(fileToUpload: File, patientCaseId: number): Observable<any> {
    const formData: FormData = new FormData();
    const headers = new HttpHeaders({ 'ngsw-bypass': 'true' });
    formData.append('fileKey', fileToUpload, fileToUpload.name);
    return this.http
      .post(this.urls.postMedicationPlan + patientCaseId, formData, {
        reportProgress: true,
        observe: 'events',
        headers: headers
      })
      .pipe(
        map(event => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              const progress = Math.round((100 * event.loaded) / event.total);
              return { status: 'progress', message: progress };
            case HttpEventType.Response:
              return event.body;
            default:
              return `Unhandled event: ${event.type}`;
          }
        })
      );
  }

  downloadFile(patiendCaseId: number): any {
    return this.http
      .get(`${this.urls.downloadEdfFileUrl + patiendCaseId}`, { observe: 'response', responseType: 'blob' })
      .pipe();
  }

  downloadEcgFile(patiendCaseId: number): any {
    return this.http
      .get(`${this.urls.downloadEcgFileUrl + patiendCaseId}`, { observe: 'response', responseType: 'blob' })
      .pipe();
  }

  getMedicationFile(caseId?: number): any {
    const url = `${this.urls.getMedicationFile + caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  downloadMedicationFile(patiendCaseId: number): any {
    return this.http
      .get(`${this.urls.downloadMedication + patiendCaseId}`, { observe: 'response', responseType: 'blob' })
      .pipe(tap(resp => console.log('heaeder', resp.headers)));
  }

  generateECGUploadVerificationOtp(caseId?: number): any {
    const url = `${this.urls.getEcguploadToken + caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  generateSigImgUploadVerificationOtp(): any {
    return this.http.get(this.urls.getSignatureUploadToken, { observe: 'response' });
  }

  signECGReport(cardioECGFileContext: CardioECGFileContext): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('req', JSON.stringify(cardioECGFileContext));

    return this.http.post(this.urls.postEcgFileUrl, formData);
  }

  updateECGReportData(cardioECGFileContext: CardioECGFileContext): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('req', JSON.stringify(cardioECGFileContext));

    return this.http.post(this.urls.uploadECGFileUnsigned, formData);
  }

  postEcgFile(fileToUpload: File, cardioECGFileContext: CardioECGFileContext): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('fileKey', fileToUpload, fileToUpload.name);
    formData.append('req', JSON.stringify(cardioECGFileContext));

    let url = this.urls.postEcgFileUrl;

    if (cardioECGFileContext.otp === '') {
      url = this.urls.uploadECGFileUnsigned;
    }

    return this.processFileUpload(url, formData);
  }

  processFileUpload(url: string = '', formData: any) {
    let headers: any = '';
    headers = new HttpHeaders({ 'ngsw-bypass': 'true' });

    return this.http
      .post(url, formData, {
        reportProgress: true,
        observe: 'events',
        headers: headers
      })
      .pipe(
        map(event => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              const progress = Math.round((100 * event.loaded) / event.total);
              return { status: 'progress', message: progress };
            case HttpEventType.Response:
              return event.body;
            default:
              return `Unhandled event: ${event.type}`;
          }
        })
      );
  }

  postSigImgFile(fileToUpload: File, cardioSigImgContext: CardioSigImgContext): Observable<any> {
    const headers = new HttpHeaders({ 'ngsw-bypass': 'true' });
    const formData: FormData = new FormData();
    formData.append('fileKey', fileToUpload, fileToUpload.name);
    formData.append('req', JSON.stringify(cardioSigImgContext));

    return this.http
      .post(this.urls.postSigFileUrl, formData, {
        reportProgress: true,
        observe: 'events',
        headers: headers
      })
      .pipe(
        map(event => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              const progress = Math.round((100 * event.loaded) / event.total);
              return { status: 'progress', message: progress };
            case HttpEventType.Response:
              return event.body;
            default:
              return `Unhandled event: ${event.type}`;
          }
        })
      );
  }

  postSigString(cardioSigStringContext: CardioSigStringContext): Observable<any> {
    return this.http.post(this.urls.postSigStringUrl, cardioSigStringContext, { observe: 'response' });
  }

  public getNoteTypes(): Observable<any> {
    const url = `${this.urls.getNoteTypes}`;

    return this.http.get(url);
  }

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }

  getPatientEDFFile(patientCaseId: number) {
    const url = `${this.urls.getPatientEDFFile + patientCaseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  getPatientQuickEvalFile(patientCaseId: number) {
    const url = `${this.urls.getPatientQuickEvalFile + patientCaseId}`;

    return this.http.get(url, { observe: 'response' });
  }
}
