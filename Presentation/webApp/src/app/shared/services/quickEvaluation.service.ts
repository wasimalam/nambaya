import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

export interface QuickEvaluationContext {
  id: number;
  patientCaseID: number;
  quickResultID: number;
  measurementTime: string;
  quickResult: boolean;
  notes: string;
}
@Injectable()
export class QuickEvaluationService {
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;

  private URLs = {
    quickEvaluation: `${this.patientServiceUrl}/api/v1/patient/cases/quickevaluationresult`,
    quickOptions: `${this.patientServiceUrl}/api/v1/lookups/items?code=QUICKREPORT`,
    getImagesUrl: `${this.patientServiceUrl}/api/v1/Patient/DownloadQuickEvaluationImages?patientcaseid=`
  };

  constructor(private http: HttpClient) {}

  getQuickOptions(): any {
    return this.http.get(`${this.URLs.quickOptions}`) /*.pipe(catchError(this.handleError))*/;
  }

  getQuickEvaluation(caseId?: number): any {
    const url = `${this.URLs.quickEvaluation}/${caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  updateQuickEvaluation(quickEvaluationContext: QuickEvaluationContext, isFirstSave: boolean): any {
    if (isFirstSave) {
      return this.http.post(this.URLs.quickEvaluation, quickEvaluationContext, { observe: 'response' });
    } else {
      return this.http.put(this.URLs.quickEvaluation, quickEvaluationContext, { observe: 'response' });
    }
  }

  getQuickEvaluationImages(patientCaseId: number) {
    return this.http.get(`${this.URLs.getImagesUrl + patientCaseId}`, { observe: 'response' });
  }

  // tslint:disable-next-line:typedef
  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
