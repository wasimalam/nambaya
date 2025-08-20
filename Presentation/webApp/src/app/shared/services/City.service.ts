import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { APIUrls } from '@app/shared/Addresses';

@Injectable()
export class CityService {
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;
  private urls = {
    getCitiesUrl: `${this.pharmacistServiceUrl}/api/v1/lookups/Cities`
  };

  constructor(private http: HttpClient) {}

  getCities(): Observable<any[]> {
    return this.http.get<any[]>(`${this.urls.getCitiesUrl}`).pipe(catchError(this.handleError));
  }

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
