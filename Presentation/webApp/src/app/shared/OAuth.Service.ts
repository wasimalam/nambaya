import {
  OidcSecurityService,
  OpenIdConfiguration,
  AuthWellKnownEndpoints,
  AuthorizationResult
} from 'angular-auth-oidc-client';
import { Addresses } from '@app/shared/Addresses';
import { Injectable } from '@angular/core';
import { userData } from './userData';
import { BehaviorSubject } from 'rxjs';
import { I18nService } from '@app/core';
import { CenterGroupAccountsService } from '@app/centerGroup/Models/center-group-accounts.service';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { PharmacistAccountsService } from '@app/pharmacist/Models/pharmacist-accounts.service';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { LoggedUserInfoService } from '@app/shared/services/logged-user-info.service';
@Injectable()
export class OAuthService {
  isAuthenticated: boolean;
  public userData: userData;
  public observableUser: any;
  public urlRedirectAfterLogin: any;
  settingsData: any;

  addresses: Addresses = new Addresses();

  authWellKnownEndpoints: AuthWellKnownEndpoints = {
    issuer: this.addresses.stsServerAddress,
    jwks_uri: this.addresses.stsServerAddress + '/.well-known/openid-configuration/jwks',
    authorization_endpoint: this.addresses.stsServerAddress + '/connect/authorize',
    token_endpoint: this.addresses.stsServerAddress + '/connect/token',
    userinfo_endpoint: this.addresses.stsServerAddress + '/connect/userinfo',
    end_session_endpoint: this.addresses.stsServerAddress + '/connect/endsession',
    check_session_iframe: this.addresses.stsServerAddress + '/connect/checksession',
    revocation_endpoint: this.addresses.stsServerAddress + '/connect/revocation',
    introspection_endpoint: this.addresses.stsServerAddress + '/connect/introspect'
  };

  constructor(
    public oidcSecurityService: OidcSecurityService,
    private loggedUserService: LoggedUserInfoService,
    private i18nService: I18nService,
    private centerGroupAccountsService: CenterGroupAccountsService,
    private cardiologistAccountsService: CardiologistAccountsService,
    private pharmacistAccountsService: PharmacistAccountsService,
    private userAccountsService: UserAccountsService
  ) {
    this.observableUser = new BehaviorSubject<userData>(this.userData);
  }

  public setUpModule(config: OpenIdConfiguration) {
    this.oidcSecurityService.setupModule(config, this.authWellKnownEndpoints);
  }

  public setUpComponent() {
    if (!this.oidcSecurityService.moduleSetup) {
      this.oidcSecurityService.onModuleSetup.subscribe(() => {
        this.doCallbackLogicIfRequired();
      });
    } else {
      this.doCallbackLogicIfRequired();
    }
  }

  eventChange() {
    if (this.userData.firstname != undefined) {
      this.observableUser.next(this.userData);
    }
  }

  public checkIfAuthenticated() {
    this.oidcSecurityService.getIsAuthorized().subscribe(auth => {
      this.isAuthenticated = auth;
    });

    this.oidcSecurityService.getUserData().subscribe(userData => {
      this.userData = userData;

      this.eventChange();
      if (this.userData.appuserid) {
        localStorage.setItem('appUserId', this.userData.appuserid);
        this.setUpPreferenceLocalStorage();
      }
      if (this.userData.sub) {
        localStorage.setItem('userId', this.userData.sub);
        localStorage.setItem('roleCode', this.userData.rolecode);
      }
      if (this.userData.firstname) {
        localStorage.setItem('userName', this.userData.firstname);
        let userName = this.userData.lastname
          ? this.userData.firstname + ' ' + this.userData.lastname
          : this.userData.firstname;
        this.loggedUserService.updateLoggedUserName(userName);
      }
      if (this.oidcSecurityService.getToken() != '') {
        localStorage.setItem('token', this.oidcSecurityService.getToken());
      }
      const localLanguage = localStorage.getItem('lang');
      if (this.userData.language && !localLanguage) {
        if (this.userData.language === 'EN') {
          this.setLanguage('en-US');
        } else if (this.userData.language === 'DE') {
          this.setLanguage('de-DE');
        } else {
          this.setLanguage('de-DE');
        }
      }
    });
  }

  setLanguage(language: string) {
    this.i18nService.language = language;
  }

  public authorize() {
    this.oidcSecurityService.authorize();
  }

  setUpPreferenceLocalStorage() {
    if (!localStorage.getItem('dateFormat')) {
      const appIdInterval = setInterval(() => {
        const applicationId = localStorage.getItem('applicationId');
        if (applicationId) {
          let settingsServiceCall = this.centerGroupAccountsService.getSettingsData();
          if (applicationId === 'nambayauser') {
            settingsServiceCall = this.userAccountsService.getSettingsData();
          } else if (applicationId === 'cardiologist') {
            settingsServiceCall = this.cardiologistAccountsService.getSettingsData(localStorage.getItem('roleCode'));
          } else if (applicationId === 'pharmacist') {
            settingsServiceCall = this.pharmacistAccountsService.getSettingsData();
          }
          settingsServiceCall.subscribe(result => {
            this.settingsData = result;
            this.persistSettings();
          });
          clearInterval(appIdInterval);
        }
      }, 500);
    }
  }

  public doCallbackLogicIfRequired() {
    if (window.location.hash) {
      this.oidcSecurityService.authorizedImplicitFlowCallback();
    }
  }

  persistSettings() {
    localStorage.setItem('dateFormat', 'DD.MM.YYYY');
    localStorage.setItem('timeFormat', 'HH:mm');
    localStorage.setItem('timeZone', 'Arctic/Longyearbyen');

    for (const result of this.settingsData) {
      if (result.code === 'DateFormat') {
        localStorage.setItem('dateFormat', result.value);
      }
      if (result.code === 'TimeFormat') {
        localStorage.setItem('timeFormat', result.value);
      }
      if (result.code === 'TimeZone') {
        localStorage.setItem('timeZone', result.value);
      }
    }
  }

  public logout() {
    this.oidcSecurityService.logoff();
  }
}
