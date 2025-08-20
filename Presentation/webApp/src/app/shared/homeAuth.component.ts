import { UserConfig } from './userConfigs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { AuthenticationService } from '@app/core';
import { OAuthService } from '@app/shared/OAuth.Service';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-homeAuth',
  templateUrl: './homeAuth.component.html'
})
export class HomeAuthComponent implements OnInit, OnDestroy {
  initialClient: string;
  userConfigs: UserConfig = new UserConfig();

  constructor(
    public oidcSecurityService: OidcSecurityService,
    public oAuthService: OAuthService,
    private cookieService: CookieService,
    private authenticationService: AuthenticationService
  ) {
    this.initialClient = localStorage.getItem('initialClient');
    localStorage.clear();
    sessionStorage.clear();
    this.setNewModules();
  }

  public deleteCookies() {
    this.oidcSecurityService.logoff();

    this.cookieService.delete('idsrv');
    this.cookieService.delete('idsrv.session');
    this.setNewModules();
  }

  public endSession() {
    let token = this.oidcSecurityService.getIdToken();
    if (token) {
      this.authenticationService.endSession(token, '/abc').subscribe(response => {});
    }
  }

  public setNewModules() {
    this.oAuthService.checkIfAuthenticated();

    if (this.initialClient === 'Pharmacist') {
      this.oAuthService.setUpModule(this.userConfigs.pharmacistConfig);
      localStorage.setItem('clientId', this.userConfigs.pharmacistConfig.client_id);
      localStorage.setItem('applicationId', 'pharmacist');
      localStorage.setItem('loginPageTitle', 'Pharmacist');
      localStorage.setItem('applicationCode', 'Pharma');
      localStorage.setItem('initialClient', 'Pharmacist');
      this.oAuthService.authorize();
    } else if (this.initialClient === 'Cardiologist') {
      this.oAuthService.setUpModule(this.userConfigs.cardiologistConfig);
      localStorage.setItem('clientId', this.userConfigs.cardiologistConfig.client_id);
      localStorage.setItem('loginPageTitle', 'Cardiologist');
      localStorage.setItem('applicationId', 'cardiologist');
      localStorage.setItem('applicationCode', 'Cardio');
      localStorage.setItem('initialClient', 'Cardiologist');
      this.oAuthService.authorize();
    } else if (this.initialClient === 'User') {
      this.oAuthService.setUpModule(this.userConfigs.nambayauserConfig);
      localStorage.setItem('clientId', this.userConfigs.nambayauserConfig.client_id);
      localStorage.setItem('loginPageTitle', 'Nambaya User');
      localStorage.setItem('applicationId', 'nambayauser');
      localStorage.setItem('applicationCode', 'User');
      localStorage.setItem('initialClient', 'User');
      this.oAuthService.authorize();
    } else if (this.initialClient === 'Center') {
      this.oAuthService.setUpModule(this.userConfigs.centerGroupConfig);
      localStorage.setItem('clientId', this.userConfigs.centerGroupConfig.client_id);
      localStorage.setItem('loginPageTitle', 'Center Group');
      localStorage.setItem('applicationId', 'centralgroupuser');
      localStorage.setItem('applicationCode', 'CentralGroupApp');
      localStorage.setItem('initialClient', 'Center');
      this.oAuthService.authorize();
    }
  }

  ngOnInit() {}
  ngOnDestroy() {}
}
