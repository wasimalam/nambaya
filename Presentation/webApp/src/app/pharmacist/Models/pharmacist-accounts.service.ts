import { OAuthService } from './../../shared/OAuth.Service';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PharmacistModel } from './Pharmacist.Model';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { APIUrls } from '@app/shared/Addresses';

export interface PharmacistAccountsData {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  street: string;
  pharmacyID?: number;
}

export interface PharmacistAccountsContext {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  phone?: string;
  street: string;
  zipCode: string;
  pharmacyID: number;
  id: number;
  isActive: boolean;
}

@Injectable()
export class PharmacistAccountsService {
  // TODO: Centralize URLS
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;
  private urls = {
    getDataUrl: `${this.pharmacistServiceUrl}/api/v1`,
    urlSavePharmacyAccounts: `${this.pharmacistServiceUrl}/api/v1/pharmacy`,
    urlSavePharmacistAccounts: `${this.pharmacistServiceUrl}/api/v1/pharmacist`,
    saveUrl: `${this.pharmacistServiceUrl}/api/v1/Pharmacist`,
    updateUrl: `${this.pharmacistServiceUrl}/api/v1/Pharmacist`,
    getOneUrl: `${this.pharmacistServiceUrl}/api/v1/Pharmacist/`,
    getPharmacistsUrl: `${this.pharmacistServiceUrl}/api/v1/Pharmacist`,
    getSettings: `${this.pharmacistServiceUrl}/api/v1/Pharmacist/settings`,
    saveSettings: `${this.pharmacistServiceUrl}/api/v1/Pharmacist/updateSettings`,
    getLanguages: `${this.pharmacistServiceUrl}/api/v1/lookups/languages`,
    get2FOptions: `${this.pharmacistServiceUrl}/api/v1/lookups/items?code=2FactorNotificationType`
  };

  constructor(private http: HttpClient) {}

  public getData(id?: number, applicationId?: string): any {
    const roleCode = localStorage.getItem('roleCode');
    this.urls.getDataUrl = `${this.pharmacistServiceUrl}/api/v1/${roleCode}`;
    const url = `${this.urls.getDataUrl}/${id}`;
    return this.http.get(url);
  }

  updatePharmacistSettings(pharmacistAccountsContext: PharmacistAccountsContext) {
    let url = this.urls.urlSavePharmacyAccounts;
    if (pharmacistAccountsContext.pharmacyID) {
      url = this.urls.urlSavePharmacistAccounts;
    }
    return this.http.put(url, pharmacistAccountsContext, { observe: 'response' });
  }

  save(Pharmacist: PharmacistModel) {
    Pharmacist.role = 'Pharmacist';

    Pharmacist.pharmacyId = parseInt(localStorage.getItem('appUserId'));
    return this.http
      .post(`${this.urls.saveUrl}`, Pharmacist, { observe: 'response' })
      .pipe(catchError(this.handleError));
  }

  update(pharmacist: PharmacistModel): any {
    pharmacist.pharmacyId = parseInt(localStorage.getItem('appUserId'));
    return this.http
      .put(`${this.urls.updateUrl}`, pharmacist, { observe: 'response' })
      .pipe(catchError(this.handleError));
  }

  getOne(PharmacistId: number): Observable<any> {
    return this.http.get(`${this.urls.getOneUrl}` + PharmacistId).pipe(catchError(this.handleError));
  }

  getPharmacists(): any {
    return this.http.get(`${this.urls.getPharmacistsUrl}`).pipe(catchError(this.handleError));
  }

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }

  getSettingsData() {
    const roleCode = localStorage.getItem('roleCode');
    this.urls.getSettings = `${this.pharmacistServiceUrl}/api/v1/${roleCode}/settings`;

    return this.http.get(`${this.urls.getSettings}`);
  }
  getLanguages = (): any => this.http.get(`${this.urls.getLanguages}`);
  get2FOptions = (): any => this.http.get(`${this.urls.get2FOptions}`);

  updatePharmacistPreferences(pharmacistPreferences: any): any {
    const roleCode = localStorage.getItem('roleCode');
    this.urls.saveSettings = `${this.pharmacistServiceUrl}/api/v1/${roleCode}/updateSettings`;

    return this.http.put(this.urls.saveSettings, pharmacistPreferences, { observe: 'response' });
  }
}
