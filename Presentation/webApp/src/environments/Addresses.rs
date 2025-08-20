import { Injectable } from '@angular/core';

@Injectable()
export class Addresses {
  stsServerAddress: string = 'https://identity.qt-life.nambaya-medical.de';
  physicanRedirectUrl: string = 'https://qt-life.nambaya-medical.de/center/dashboard';
  centerGroupRedirectUrl: string = 'https://qt-life.nambaya-medical.de/center/dashboard';
  nambayaUserRedirectUrl: string = 'https://qt-life.nambaya-medical.de/user/dashboard';
  cardiologistRedirectUrl: string = 'https://qt-life.nambaya-medical.de/cardiologist/dashboard';
  pharmacistRedirectUrl: string = 'https://qt-life.nambaya-medical.de/pharmacist/dashboard';
  logoutUrl: string = 'https://qt-life.nambaya-medical.de/logout';
  postLogoutRedirectUri: 'https://qt-life.nambaya-medical.de/logout';
}

@Injectable()
export class APIUrls {
  PharmacistServiceBaseUrl: string = 'https://services.qt-life.nambaya-medical.de/pharmacist';
  CardiologistServiceBaseUrl: string = 'https://services.qt-life.nambaya-medical.de/cardiologist';
  centralServiceBaseUrl: string = 'https://services.qt-life.nambaya-medical.de/centralgroup';
  NambayaUserServiceBaseUrl: string = 'https://services.qt-life.nambaya-medical.de/nambayauser';
  IdentityServerBaseUrl: string = 'https://identity.qt-life.nambaya-medical.de';
  PatientServiceBaseUrl: string = 'https://services.qt-life.nambaya-medical.de/patient';
  LogginServiceBaseUrl : string = 'https://services.qt-life.nambaya-medical.de/logging';
  UserManagementServiceBaseUrl: string = 'https://services.qt-life.nambaya-medical.de/usermanagement';
}
