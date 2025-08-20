import { Injectable } from '@angular/core';

@Injectable()
export class Addresses {
  stsServerAddress: string = 'http://identity.qtlife.com:5000';
  physicanRedirectUrl: string = 'http://app.qtlife.com:4200/center/dashboard';
  centerGroupRedirectUrl: string = 'http://app.qtlife.com:4200/center/dashboard';
  nambayaUserRedirectUrl: string = 'http://app.qtlife.com:4200/user/dashboard';
  cardiologistRedirectUrl: string = 'http://app.qtlife.com:4200/cardiologist/dashboard';
  pharmacistRedirectUrl: string = 'http://app.qtlife.com:4200/pharmacist/dashboard';
  logoutUrl: string = 'http://app.qtlife.com:4200/logout';
  postLogoutRedirectUri: 'http://app.qtlife.com:4200/logout';
}

@Injectable()
export class APIUrls {
  PharmacistServiceBaseUrl: string = 'http://service.qtlife.com:5002';
  CardiologistServiceBaseUrl: string = 'http://service.qtlife.com:5004';
  centralServiceBaseUrl: string = 'http://service.qtlife.com:5005';
  NambayaUserServiceBaseUrl: string = 'http://service.qtlife.com:5007';
  IdentityServerBaseUrl: string = 'http://identity.qtlife.com:5000';
  PatientServiceBaseUrl: string = 'http://service.qtlife.com:5001';
  LogginServiceBaseUrl: string = 'http://service.qtlife.com:5010';
  UserManagementServiceBaseUrl: string = 'http://service.qtlife.com:5003';
}
