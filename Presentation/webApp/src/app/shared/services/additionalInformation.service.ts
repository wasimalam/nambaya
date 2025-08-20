import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

export interface AdditionalInformationContext {
  id: number;
  patientCaseID: number;
  allergiesFreeText: string;
  isPregnant: boolean;
  isFeeding: boolean;
  weight: any;
  height: any;
  creatinineValue: any;
  additionalFreeText: string;
}
@Injectable()
export class AdditionalInformationService {
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;

  private URLs = {
    additionalinfo: `${this.patientServiceUrl}/api/v1/patient/cases/additionalinfo`
  };

  constructor(private http: HttpClient) {}

  getAdditionalInfo(caseId?: number): any {
    const url = `${this.URLs.additionalinfo}/${caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  updateAdditionalInfo(additionalInformationContext: AdditionalInformationContext, isFirstSave: boolean): any {
    if (isFirstSave) {
      return this.http.post(this.URLs.additionalinfo, additionalInformationContext, { observe: 'response' });
    } else {
      return this.http.put(this.URLs.additionalinfo, additionalInformationContext, { observe: 'response' });
    }
  }
}
