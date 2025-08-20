import { APIUrls } from '@app/shared/Addresses';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

export interface UserAccountsData {
  data: any;
}

export interface UserRoles {
  roles: any;
}

export interface UserAccountsContext {
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

export interface UserContext {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  phone?: string;
  street: string;
  zipCode: string;
  city: string;
  pharmacyID: number;
  id: number;
  createdOn: string;
  updatedOn: string;
  createdBy: string;
  updatedBy: string;
}

@Injectable()
export class UserAccountsService {
  private userServiceUrl = new APIUrls().NambayaUserServiceBaseUrl;
  private userRoleServiceUrl = new APIUrls().NambayaUserServiceBaseUrl;
  private loggingServiceUrl = new APIUrls().LogginServiceBaseUrl;

  private URLs = {
    getSettings: `${this.userServiceUrl}/api/v1`,
    getUserData: `${this.userServiceUrl}/api/v1/nambayauser`,
    getUserRoles: `${this.userRoleServiceUrl}/api/v1/role`,
    saveUserAccounts: `${this.userServiceUrl}/api/v1/nambayauser`,
    saveUser: `${this.userServiceUrl}/api/v1/nambayauser`,
    getPreferences: `${this.userServiceUrl}/api/v1/nambayaUser/settings`,
    saveSettings: `${this.userServiceUrl}/api/v1/nambayaUser/updateSettings`,
    getLanguages: `${this.userServiceUrl}/api/v1/lookups/languages`,
    get2FOptions: `${this.userServiceUrl}/api/v1/lookups/items?code=2FactorNotificationType`,
    getNotificationTypesByEventId: `${this.userServiceUrl}/api/v1/nambayauser/notificationtypes/`,
    getEventTypesUrl: `${this.userServiceUrl}/api/v1/nambayauser/notificationeventtypes`,
    getTemplatesUrl: `${this.userServiceUrl}/api/v1/nambayauser/notificationtemplates`,
    getNotificationsLog: `${this.loggingServiceUrl}/api/v1/logging/GetNotificationsLog`,
    getPlaceHoldersByEventId: `${this.userServiceUrl}/api/v1/nambayauser/eventparams/`,
    updateTemplateUrl: `${this.userServiceUrl}/api/v1/nambayauser/notificationtemplates`,
    CreateTemplateUrl: `${this.userServiceUrl}/api/v1/nambayauser/notificationtemplates`,
    getOneTemplate: `${this.userServiceUrl}/api/v1/nambayauser/notificationtemplates`,
    getOneTemplateByEventIdAndTypeUrl: `${this.userServiceUrl}/api/v1/nambayauser/notificationtemplates/eventtype`,
    getLogsUrl: `${this.loggingServiceUrl}/api/v1/logging`,
    userStatsUrl: `${this.userServiceUrl}/api/v1/nambayauser/dashboard/userstats`,
    getTemplateAllowedDomains: `${this.userServiceUrl}/api/v1/nambayauser/GetTemplateAllowedDomains`,
    getLogAllowedDomains: `${this.loggingServiceUrl}/api/v1/logging/GetLogAllowedUserDomains`,
    goalCompletedPercentageUrl: `${this.userServiceUrl}/api/v1/patient/dashboard/goalcompletedpercent`,
    monthlyCaseStartedUrl: `${this.userServiceUrl}/api/v1/patient/dashboard/monthlycasesstarted`,
    monthlycasesCompletedUrl: `${this.userServiceUrl}/api/v1/patient/dashboard/monthlycasescompleted`,
    caseDispatchedStatUrl: `${this.userServiceUrl}/api/v1/patient/dashboard/monthlycasesdispatched`,
    cardiologistNoteStatsUrl: `${this.userServiceUrl}/api/v1/patient/dashboard/cardiologistnotesstats`,
    qeStatusStatsUrl: `${this.userServiceUrl}/api/v1/patient/dashboard/qeResultStats`
  };

  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);

  constructor(private http: HttpClient) {
    this.remoteData = new BehaviorSubject([]);
  }

  public getData(id?: number, applicationId?: string): any {
    const url = `${this.URLs.getSettings}/${applicationId}/${id}`;

    return this.http.get(url) /*.pipe(catchError(error => this.handleError(error)))*/;
  }

  public getuserDashboardStats() {
    return this.http.get(this.URLs.userStatsUrl);
  }

  public getUserData(id?: number): Observable<any> {
    const url = `${this.URLs.getUserData}/${id}`;

    return this.http.get(url);
  }

  public getTemplateAllowedDomains() {
    return this.http.get(this.URLs.getTemplateAllowedDomains);
  }

  public getLogAllowedUserDomains() {
    return this.http.get(this.URLs.getLogAllowedDomains);
  }

  public getLogs(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
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
      .get(`${this.URLs.getLogsUrl + qS}`)
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

  updateUserSettings(userAccountsContext: UserAccountsContext): any {
    return this.http.put(this.URLs.saveUserAccounts, userAccountsContext, { observe: 'response' });
  }

  updateUser(userAccountsContext: UserAccountsContext, id: number): any {
    userAccountsContext.id = Number(id);

    return this.http.put(this.URLs.saveUserAccounts, userAccountsContext, { observe: 'response' });
  }

  createUser(userContext: UserContext): any {
    return this.http.post(this.URLs.saveUser, userContext, { observe: 'response' });
  }

  public getAllTemplates(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
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
      .get(`${this.URLs.getTemplatesUrl + qS}`)
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

  public getAllEmailSMSLogs(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
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
      .get(`${this.URLs.getNotificationsLog + qS}`)
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

  getEvenTypes(): Observable<any[]> {
    return this.http.get<any[]>(this.URLs.getEventTypesUrl);
  }

  getNotificationTypesByEventId(eventId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.URLs.getNotificationTypesByEventId + eventId}`);
  }

  getPlaceHoldersByEventId(eventId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.URLs.getPlaceHoldersByEventId + eventId}`);
  }

  createTemplate(template: any): any {
    return this.http.post(this.URLs.CreateTemplateUrl, template, { observe: 'response' });
  }

  updateTemplate(template: any): any {
    return this.http.put(this.URLs.updateTemplateUrl, template, { observe: 'response' });
  }

  getOneTemplateByEventIdAndType(eventTypeId: number, templateTypeId: number): Observable<any> {
    return this.http.get<any[]>(
      `${this.URLs.getOneTemplateByEventIdAndTypeUrl +
        '?eventtypeid=' +
        eventTypeId +
        '&templatetypeid=' +
        templateTypeId}`
    );
  }

  getOneTemplate(id: number): Observable<any> {
    return this.http.get(`${this.URLs.getOneTemplate}/${id}`);
  }

  getSettingsData = (): any => this.http.get(`${this.URLs.getPreferences}`);
  getLanguages = (): any => this.http.get(`${this.URLs.getLanguages}`);
  get2FOptions = (): any => this.http.get(`${this.URLs.get2FOptions}`);

  updateUserPreferences(userPreferences: any): any {
    return this.http.put(this.URLs.saveSettings, userPreferences, { observe: 'response' });
  }

  public getCaseDispatchedStats() {
    return this.http.get(this.URLs.caseDispatchedStatUrl);
  }

  public getGoalsCompleted() {
    return this.http.get(this.URLs.goalCompletedPercentageUrl);
  }

  public getMonthlyCaseStarted() {
    return this.http.get(this.URLs.monthlyCaseStartedUrl);
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
}
