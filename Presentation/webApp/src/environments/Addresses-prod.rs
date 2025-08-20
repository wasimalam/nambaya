import { Injectable } from '@angular/core';

@Injectable()
export class Addresses {
  stsServerAddress: string = 'https://identity.nambaya-medical.de';
  physicanRedirectUrl: string = 'https://www.nambaya-medical.de/center/dashboard';
  centerGroupRedirectUrl: string = 'https://www.nambaya-medical.de/center/dashboard';
  nambayaUserRedirectUrl: string = 'https://www.nambaya-medical.de/user/dashboard';
  cardiologistRedirectUrl: string = 'https://www.nambaya-medical.de/cardiologist/dashboard';
  pharmacistRedirectUrl: string = 'https://www.nambaya-medical.de/pharmacist/dashboard';
  logoutUrl: string = 'https://www.nambaya-medical.de/logout';
  postLogoutRedirectUri: 'https://www.nambaya-medical.de/logout';
}

@Injectable()
export class APIUrls {
  PharmacistServiceBaseUrl: string = 'https://services.nambaya-medical.de/pharmacist';
  CardiologistServiceBaseUrl: string = 'https://services.nambaya-medical.de/cardiologist';
  centralServiceBaseUrl: string = 'https://services.nambaya-medical.de/centralgroup';
  NambayaUserServiceBaseUrl: string = 'https://services.nambaya-medical.de/nambayauser';
  IdentityServerBaseUrl: string = 'https://identity.nambaya-medical.de';
  PatientServiceBaseUrl: string = 'https://services.nambaya-medical.de/patient';
  LogginServiceBaseUrl : string = 'https://services.nambaya-medical.de/logging';
}
