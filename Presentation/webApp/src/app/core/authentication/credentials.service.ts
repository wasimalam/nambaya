import { Injectable } from '@angular/core';

export interface Credentials {
  username: string;
  token: string;
}

export interface AuthenticationResult {
  redirectUrl: string;
  IsOk: boolean;
}
const credentialsKey = 'credentials';

@Injectable({
  providedIn: 'root'
})
export class CredentialsService {
  private _credentials: Credentials | null = null;

  constructor() {
    const savedCredentials = sessionStorage.getItem(credentialsKey) || localStorage.getItem(credentialsKey);
    if (savedCredentials) {
      this._credentials = JSON.parse(savedCredentials);
    }
  }

  isAuthenticated(): boolean {
    return !!this.credentials;
  }

  get credentials(): Credentials | null {
    return this._credentials;
  }

  setCredentials(credentials?: Credentials, remember?: boolean) {
    this._credentials = credentials || null;

    if (credentials) {
      const storage = remember ? localStorage : sessionStorage;
      storage.setItem(credentialsKey, JSON.stringify(credentials));
    } else {
      sessionStorage.removeItem(credentialsKey);
      localStorage.removeItem(credentialsKey);
    }
  }
}
