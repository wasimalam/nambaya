import { APIUrls } from '@app/shared/Addresses';
import { CardiologistModel } from './cardiologist.model';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface NurseContext {
  id?: number;
  CardiologistId: number;
  CompanyId: string;
  firstName: string;
  lastName: string;
  email: string;
  street: string;
  zipCode: string;
  address: string;
  phone: string;
  role: string;
}

@Injectable()
export class CardiologistService {
  public remoteData: BehaviorSubject<any[]>;
  private cardiologistApiUrl = new APIUrls().CardiologistServiceBaseUrl;
  private urls = {
    saveUrl: `${this.cardiologistApiUrl}/api/v1/cardiologist`,
    saveNurse: `${this.cardiologistApiUrl}/api/v1/nurse`,
    getNurse: `${this.cardiologistApiUrl}/api/v1/nurse/`,
    getOneUrl: `${this.cardiologistApiUrl}/api/v1/cardiologist/`
  };

  constructor(private http: HttpClient) {}

  save(cardiologist: CardiologistModel) {
    cardiologist.role = 'Cardiologist';
    return this.http.post(this.urls.saveUrl, cardiologist, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  saveNurse(nurseContext: NurseContext) {
    return this.http
      .post(this.urls.saveNurse, nurseContext, { observe: 'response' })
      .pipe(catchError(this.handleError));
  }

  updateNurse(nurseContext: NurseContext) {
    return this.http.put(this.urls.saveNurse, nurseContext, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  update(cardiologist: CardiologistModel): any {
    return this.http.put(this.urls.saveUrl, cardiologist, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  getOne(cardiologistId: number): Observable<any> {
    return this.http.get(this.urls.getOneUrl + cardiologistId).pipe(catchError(this.handleError));
  }

  getNurse(nurseId: number): Observable<any> {
    return this.http.get(this.urls.getNurse + nurseId).pipe(catchError(this.handleError));
  }

  getCardiologists(): any {
    return this.http.get(this.urls.saveUrl).pipe(catchError(this.handleError));
  }

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
