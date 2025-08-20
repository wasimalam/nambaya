import { Center } from './CenterModel';
import { APIUrls} from '@app/shared/Addresses';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface CenterGroupAccountsContext {
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
}

@Injectable()
export class CenterGroupAccountsService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  private CentralServiceUrl = new APIUrls().centralServiceBaseUrl;

  private URLs = {
    getPhysician: `${this.CentralServiceUrl}/api/v1`,
    savePhysicianSettings: `${this.CentralServiceUrl}/api/v1/centralGroupUser`,
    getSettings: `${this.CentralServiceUrl}/api/v1/centralGroupUser/settings`,
    saveSettings: `${this.CentralServiceUrl}/api/v1/centralGroupUser/updateSettings`,
    getLanguages: `${this.CentralServiceUrl}/api/v1/lookups/languages`,
    get2FOptions: `${this.CentralServiceUrl}/api/v1/lookups/items?code=2FactorNotificationType`,
    saveCenterUrl: `${this.CentralServiceUrl}/api/v1/CentralGroupUser`,
    getCitiesUrl: `${this.CentralServiceUrl}/api/v1/lookups/Cities`,
    pharmacyStatsUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/pharmacystats`,
    goalCompletedPercentageUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/goalcompletedpercent`,
    monthlyCaseStartedUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/monthlycasesstarted`,
    monthlycasesCompletedUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/monthlycasescompleted`,
    caseDispatchedStatUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/monthlycasesdispatched`,
    cardiologistNoteStatsUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/cardiologistnotesstats`,
    qeStatusStatsUrl: `${this.CentralServiceUrl}/api/v1/patient/dashboard/qeResultStats`
  };

  constructor(private http: HttpClient) {
    this.remoteData = new BehaviorSubject([]);
  }

  public getData(id?: number, applicationId?: string): any {
    const url = `${this.URLs.getPhysician}/${applicationId}/${id}`;

    return this.http.get(url);
  }

  getSettingsData = (): any => this.http.get(`${this.URLs.getSettings}`);
  getLanguages = (): any => this.http.get(`${this.URLs.getLanguages}`);
  get2FOptions = (): any => this.http.get(`${this.URLs.get2FOptions}`);

  public getGoalsCompleted() {
    return this.http.get(this.URLs.goalCompletedPercentageUrl);
  }

  public getMonthlyCaseStarted() {
    return this.http.get(this.URLs.monthlyCaseStartedUrl);
  }

  public getMonthlyCaseCompleted() {
    return this.http.get(this.URLs.monthlycasesCompletedUrl);
  }

  public getCaseDispatchedStats() {
    return this.http.get(this.URLs.caseDispatchedStatUrl);
  }

  public getCardiologistNoteStats() {
    return this.http.get(this.URLs.cardiologistNoteStatsUrl);
  }

  public getQEStatusStats() {
    return this.http.get(this.URLs.qeStatusStatsUrl);
  }

  updateCenterGroupSettings(centerGroupAccountsContext: CenterGroupAccountsContext): any {
    return this.http.put(this.URLs.savePhysicianSettings, centerGroupAccountsContext, { observe: 'response' });
  }

  updateCenterGroupPreferences(centerGroupPreferences: any): any {
    return this.http.put(this.URLs.saveSettings, centerGroupPreferences, { observe: 'response' });
  }

  public getUserData(id: number): Observable<any> {
    const url = `${this.URLs.saveCenterUrl}/${id}`;

    return this.http.get(url);
  }

  updateCenter(center: Center): any {
    return this.http.put(this.URLs.saveCenterUrl, center, { observe: 'response' });
  }

  createCenter(center: Center): any {
    return this.http.post(this.URLs.saveCenterUrl, center, { observe: 'response' });
  }

  public getCenters(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
    let qS = '';
    if (perPage) {
      qS = `?offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    if (orderBy) {
      qS += `&orderBy=` + orderBy;
    }

    this.http
      .get(`${this.URLs.saveCenterUrl + qS}`)
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

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
