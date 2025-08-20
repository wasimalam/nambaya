import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { OAuthService } from '../OAuth.Service';

export interface PhoneVerificationContext {
  otp: string;
  token: string;
  email?: string;
  PhoneNumber: string;
}

export interface PasswordChangeContext {
  OldPassword: string;
  NewPassword: string;
}

@Injectable()
export class PhoneVerificationService {
  serviceUrl: string;
  userEmail: string;

  private URLs = {
    phoneVerify: '',
    phoneVerifyWithOTP: '',
    changePassword: ''
  };
  constructor(private http: HttpClient, private oAuthService: OAuthService) {
    this.oAuthService.observableUser.subscribe(user => {
      if (user) {
        const applicationId = this.oAuthService.userData.rolecode;
        if (applicationId === 'Pharmacist') {
          this.serviceUrl = `${new APIUrls().PharmacistServiceBaseUrl}/api/v1/pharmacist`;
        } else if (applicationId === 'Pharmacy') {
          this.serviceUrl = `${new APIUrls().PharmacistServiceBaseUrl}/api/v1/pharmacy`;
        } else if (applicationId === 'Cardiologist') {
          this.serviceUrl = `${new APIUrls().CardiologistServiceBaseUrl}/api/v1/cardiologist`;
        } else if (applicationId === 'Nurse') {
          this.serviceUrl = `${new APIUrls().CardiologistServiceBaseUrl}/api/v1/nurse`;
        } else if (applicationId === 'NambayaUser' || applicationId === 'StakeHolder' || applicationId === 'PharmacyTrainer') {
          this.serviceUrl = `${new APIUrls().NambayaUserServiceBaseUrl}/api/v1/nambayaUser`;
        } else if (applicationId === 'CentralGroupUser') {
          this.serviceUrl = `${new APIUrls().centralServiceBaseUrl}/api/v1/centralGroupUser`;
        }

        this.userEmail = this.oAuthService.userData.email;
        this.URLs.phoneVerify = `${this.serviceUrl}/generatephoneverification`;
        this.URLs.phoneVerifyWithOTP = `${this.serviceUrl}/phoneVerification`;
        this.URLs.changePassword = `${this.serviceUrl}/changepassword`;
      }
    });
  }

  public generatePhoneVerificationOtp(phoneNumber: string): any {
    const url = `${this.URLs.phoneVerify}`;

    return this.http.post(url, { PhoneNumber: phoneNumber }, { observe: 'response' });
  }

  public verifyOtp(context: PhoneVerificationContext): any {
    context.email = this.userEmail;

    return this.http.post(`${this.URLs.phoneVerifyWithOTP}`, context, { observe: 'response' });
  }

  public changePassword(passwordContext: PasswordChangeContext): any {
    return this.http.post(`${this.URLs.changePassword}`, passwordContext, { observe: 'response' });
  }
}
