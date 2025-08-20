import { APIUrls } from '@app/shared/Addresses';
import { PharmacyModel } from './pharmacy.model';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '@env/environment';

@Injectable()
export class PharmacyService {
  public remoteData: BehaviorSubject<any[]>;
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;
  private urls = {
    saveUrl: `${this.pharmacistServiceUrl}/api/v1/Pharmacy`,
    getAllPharmacyUrl: `${this.pharmacistServiceUrl}/api/v1/pharmacy `
  };

  constructor(private http: HttpClient) {}

  save(pharmacy: PharmacyModel) {
    pharmacy.role = 'Pharmacy';
    return this.http.post(`${this.urls.saveUrl}`, pharmacy, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  update(pharmacy: PharmacyModel): any {
    return this.http.put(`${this.urls.saveUrl}`, pharmacy, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  getOne(pharmacyId: number): Observable<any> {
    return this.http.get(`${this.urls.saveUrl}/` + pharmacyId).pipe(catchError(this.handleError));
  }

  getPharmacies(): any {
    return this.http.get(`${this.urls.saveUrl}`).pipe(catchError(this.handleError));
  }

  getAllPharmacies(): any {
    return this.http.get(`${this.urls.getAllPharmacyUrl}`).pipe(catchError(this.handleError));
  }

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
