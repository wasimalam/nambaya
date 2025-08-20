import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

import { AuthenticationService, CredentialsService, I18nService } from '@app/core';
import { OAuthService } from '@app/shared/OAuth.Service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { LoggedUserInfoService } from '@app/shared/services/logged-user-info.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  menuHidden = true;
  loggedUserName: string;
  public render = false;
  public applicationId: any = 'user';
  public settingsLink: any = '/user/accounts';
  public preferencesLink: any = '/user/settings';
  public renderName = false;
  public isCardiologist = false;
  initialClient: string;
  public urlPrefix = '';
  @Output() onNavClicked: EventEmitter<any> = new EventEmitter<any>();

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private loggedUserService: LoggedUserInfoService,
    private credentialsService: CredentialsService,
    private i18nService: I18nService,
    public oAuthService: OAuthService,
    public oidcSecurityService: OidcSecurityService
  ) {
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
      if (this.applicationId === 'pharmacist') {
        this.settingsLink = '/pharmacist/accounts';
        this.preferencesLink = '/pharmacist/settings';
      } else if (this.applicationId === 'cardiologist') {
        this.settingsLink = '/cardiologist/accounts';
        this.preferencesLink = '/cardiologist/settings';
        this.isCardiologist = true;
      } else if (this.applicationId === 'centralgroupuser') {
        this.settingsLink = '/center/accounts';
        this.preferencesLink = '/center/settings';
      }
    }
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.oAuthService.checkIfAuthenticated();
    this.initialClient = localStorage.getItem('initialClient');
    this.render = true;
    this.loggedUserService.currentLoggedUserName.subscribe(username => {
      this.loggedUserName = username;
      this.renderName = true;
    });

    //this.loggedUserName = this.oAuthService.userData.firstname;
    // this.oAuthService.observableUser.subscribe(user => {
    //   if (user != undefined) {
    //     this.loggedUserName = user.firstname + ' ' + user.lastname;
    //     this.renderName = true;
    //   }
    // });
  }

  toggleMenu() {
    this.onNavClicked.emit(this.menuHidden);
    this.menuHidden = !this.menuHidden;
  }

  public toggleLanguage() {
    const currentLanguge = this.i18nService.language;
    if (currentLanguge === 'en-US') {
      this.i18nService.language = 'de-DE';
    } else {
      this.i18nService.language = 'en-US';
    }

    localStorage.setItem('lang', this.i18nService.language);
    window.location.reload();
  }

  get alternateLanguage(): string {
    let lang = this.i18nService.language;
    if (lang === 'en-US') {
      lang = 'DE';
    } else {
      lang = 'EN';
    }

    return lang;
  }

  get currentLanguage(): string {
    const lang = this.i18nService.language;
    return lang;
  }

  get languages(): string[] {
    const languages = this.i18nService.supportedLanguages;
    return languages;
  }
  public goToHome() {
    if (this.initialClient === 'Pharmacist') {
      this.router.navigate(['/pharmacist/dashboard']);
    } else if (this.initialClient === 'Cardiologist') {
      this.router.navigate(['/cardiologist/dashboard']);
    } else if (this.initialClient === 'User') {
      this.router.navigate(['/user/dashboard']);
    } else if (this.initialClient === 'Center') {
      this.router.navigate(['/center/dashboard']);
    }
  }

  logout() {
    this.oAuthService.logout();

    // const pageURL = this.router.url;
    // if (pageURL === '/home') {
    //   this.authenticationService.logout().subscribe(() => this.router.navigate(['user/login'], { replaceUrl: true }));
    // } else {
    //   this.authenticationService
    //     .logout()
    //     .subscribe(() => this.router.navigate(['pharmacist/login'], { replaceUrl: true }));
    // }
  }
  get username(): string | null {
    const credentials = this.credentialsService.credentials;
    return credentials ? credentials.username : null;
  }
}
