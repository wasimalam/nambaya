import { APIUrls } from '@app/shared/Addresses';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface PhysicianAccountsData {
  firstName: string;
  lastName: string;
  email: string;
  doctorId: string;
  phone: string;
  street: string;
  city: string;
}

export interface PhysicianAccountsContext {
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
  isActive: boolean;
}

@Injectable()
export class PhysicianAccountsService {
  private centralServiceUrl = new APIUrls().centralServiceBaseUrl;
  private URLs = {
    getPhysician: `${this.centralServiceUrl}/api/v1`,
    savePhysicianSettings: `${this.centralServiceUrl}/api/v1/physician`
  };

  constructor(private http: HttpClient) {}

  public getData(id?: number, applicationId?: string): any {
    const url = `${this.URLs.getPhysician}/${applicationId}/${id}`;

    return this.http.get(url);
  }

  updatePhysicianSettings(physicianAccountsContext: PhysicianAccountsContext, userId: number): any {
    physicianAccountsContext.id = Number(userId);
    physicianAccountsContext.isActive = true;

    return this.http.put(this.URLs.savePhysicianSettings, physicianAccountsContext, { observe: 'response' });
  }
}
