import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class AdditionalMedicationService {
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;

  private URLs = {
    additionalMedication: `${this.patientServiceUrl}/api/v1/patient/drugGroups/patientCase`,
    drugTypes: `${this.patientServiceUrl}/api/v1/lookups/items?code=DRUGGROUPCODE`,
    updateDrugDetails: `${this.patientServiceUrl}/api/v1/patient/drugGroups/details`,
    deleteDrugDetails: `${this.patientServiceUrl}/api/v1/patient/drugGroups/details`,
    addDrugGroup: `${this.patientServiceUrl}/api/v1/patient/drugGroups`,
    deleteDrugGroup: `${this.patientServiceUrl}/api/v1/patient/drugGroups`,
    updateDrugFreeText: `${this.patientServiceUrl}/api/v1/patient/drugGroups/freeText`,
    deleteDrugFreeText: `${this.patientServiceUrl}/api/v1/patient/drugGroups/freeText`,
    updateDrugRecipe: `${this.patientServiceUrl}/api/v1/patient/drugGroups/recipe`,
    deleteDrugRecipe: `${this.patientServiceUrl}/api/v1/patient/drugGroups/recipe`
  };

  constructor(private http: HttpClient) {}

  getDrugGroups(): Observable<any[]> {
    return this.http.get<any[]>(`${this.URLs.drugTypes}`).pipe(catchError(this.handleError));
  }

  // tslint:disable-next-line:typedef
  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }

  getAdditionalMedications(caseId?: number): any {
    const url = `${this.URLs.additionalMedication}/${caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  updateDrugDetails(drugDetails: any, isPost: boolean): any {
    if (isPost) {
      return this.http.post(this.URLs.updateDrugDetails, drugDetails, { observe: 'response' });
    } else {
      return this.http.put(this.URLs.updateDrugDetails, drugDetails, { observe: 'response' });
    }
  }

  updateDrugFreeText(freeTextDetails: any, isPost: boolean): any {
    if (isPost) {
      return this.http.post(this.URLs.updateDrugFreeText, freeTextDetails, { observe: 'response' });
    } else {
      return this.http.put(this.URLs.updateDrugFreeText, freeTextDetails, { observe: 'response' });
    }
  }

  updateDrugRecipe(recipeDetails: any, isPost: boolean): any {
    if (isPost) {
      return this.http.post(this.URLs.updateDrugRecipe, recipeDetails, { observe: 'response' });
    } else {
      return this.http.put(this.URLs.updateDrugRecipe, recipeDetails, { observe: 'response' });
    }
  }
}
