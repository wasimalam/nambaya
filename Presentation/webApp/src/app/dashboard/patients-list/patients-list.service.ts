import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable()
export class PatientsListService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  private PatientServiceUrl = new APIUrls().PatientServiceBaseUrl;
  private urls = {
    getGendersUrl: `${this.PatientServiceUrl}/api/v1/lookups/Genders`,
    getPatientsUrl: `${this.PatientServiceUrl}/api/v1/patient`,
    getEvaluationStatusUrl: `${this.PatientServiceUrl}/api/v1/lookups/items?code=EvaluationStatus`,
    getCompletedCasesUrl: `${this.PatientServiceUrl}/api/v1/patient/cases/todispatch`,
    getDeactivatedPatients: `${this.PatientServiceUrl}/api/v1/patient`,
    caseStatusesUrl: `${this.PatientServiceUrl}/api/v1/lookups/items?code=CASESTATUS`,
    qeStatusesUrl: `${this.PatientServiceUrl}/api/v1/lookups/items?code=QUICKREPORT`,
    assignPatientCase: `${this.PatientServiceUrl}/api/v1/patient/assignPatientCase?patientCaseId=`
  };

  constructor(private http: HttpClient) {
    this.remoteData = new BehaviorSubject([]);
  }

  public getGendersList(): any {
    return this.http.get(this.urls.getGendersUrl).pipe(catchError(this.handleError));
  }

  public getEvaluationStatusList(): any {
    return this.http.get(this.urls.caseStatusesUrl).pipe(catchError(this.handleError));
  }

  public getcaseStatuses(): any {
    return this.http.get(this.urls.caseStatusesUrl).pipe(catchError(this.handleError));
  }

  public getQEStatuses(): any {
    return this.http.get(this.urls.qeStatusesUrl).pipe(catchError(this.handleError));
  }

  public getAllPatients(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
    let qS = '';
    if (perPage) {
      qS = `?offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    if (orderBy) {
      qS += `&orderby=` + orderBy;
    }

    this.http
      .get(`${this.urls.getPatientsUrl + qS}`)
      .pipe(
        map((data: any) => {
          return data;
        })
      )
      .subscribe(data => {
        this.dataLength.next(data.totalCount);
        this.remoteData.next(data.data);
      });
  }

  public getCompletedCases(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
    let qS = '';
    if (perPage) {
      qS = `?offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    if (orderBy) {
      qS += `&orderby=` + orderBy;
    }

    this.http
      .get(`${this.urls.getCompletedCasesUrl + qS}`)
      .pipe(
        map((data: any) => {
          return data;
        })
      )
      .subscribe(data => {
        this.remoteData.next(data.data);
        this.dataLength.next(data.totalCount);
      });
  }

  public getDeactivatedPatients(index?: number, perPage?: number, filterObject?: any, orderBy?: string): any {
    let qS = '';
    if (perPage) {
      qS = `?offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    if (orderBy) {
      qS += `&orderby=` + orderBy;
    }

    this.http
      .get(`${this.urls.getDeactivatedPatients + qS}`)
      .pipe(
        map((data: any) => {
          return data;
        })
      )
      .subscribe(data => {
        this.dataLength.next(data.totalCount);
        this.remoteData.next(data.data);
      });
  }

  assignCaseToCardiologist(patientCaseId: number): any {
    const url = this.urls.assignPatientCase + patientCaseId;

    return this.http.put(url, {}, { observe: 'response' });
  }

  // tslint:disable-next-line:typedef
  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
