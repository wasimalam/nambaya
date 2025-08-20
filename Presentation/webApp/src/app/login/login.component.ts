import { environment } from './../../environments/environment';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Logger, I18nService, AuthenticationService, untilDestroyed } from '@app/core';
import { ToastrService } from 'ngx-toastr';
import { CountdownComponent } from 'ngx-countdown';
import { TranslateService } from '@ngx-translate/core';

const log = new Logger('Login');

@Component({
  selector: 'app-login',
  templateUrl: './Views/login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  get alternateLanguage(): string {
    let lang = this.i18nService.language;
    if (lang === 'en-US') {
      lang = 'DE';
    } else {
      lang = 'EN';
    }
    return lang;
  }

  get languages(): string[] {
    const languages = this.i18nService.supportedLanguages;
    return languages;
  }

  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  public pageName = '';

  /*igx*/
  public loginForm: FormGroup;
  public otpForm: FormGroup;
  public showLogin = true;
  public showSuccessLogin = false;
  public param1: string;
  public clientId: string;
  public showOtpForm = false;
  public token: string;
  public otp: string;
  public firstParam: string;
  public mode: string;
  public isAllowedToSendAgain = true;
  public passwordFieldType = 'password';
  public config = {
    leftTime: 60,
    demand: true
  };
  initialClient: string;
  roles: string;

  @ViewChild('timer', { static: false }) private countdown: CountdownComponent;

  /*igx*/

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private i18nService: I18nService,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    fb: FormBuilder,
    public oauthservice: OAuthService,
    public oidcSecurityService: OidcSecurityService,
    private translateService: TranslateService
  ) {
    if (localStorage.getItem('showOtpForm')) {
      this.showOtpForm = true;
    } else {
      this.showOtpForm = false;
    }
    if (localStorage.getItem('logintoken')) {
      this.token = localStorage.getItem('logintoken');
    }
    if (localStorage.getItem('mode')) {
      this.mode = localStorage.getItem('mode');
    }
    this.createForm(fb);
  }

  onTimerFinished(e: Event) {
    if (e['action'] == 'done') {
      this.isAllowedToSendAgain = true;
    }
  }

  sendAgain() {
    this.isAllowedToSendAgain = false;
    this.countdown.restart();
    this.config.demand = false;
    this.countdown.begin();
    this.login();
  }

  public showPassword() {
    if (this.passwordFieldType == 'password') {
      this.passwordFieldType = 'text';
    } else {
      this.passwordFieldType = 'password';
    }
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

  ngOnInit() {
    this.initialClient = localStorage.getItem('initialClient');
    this.firstParam = this.route.snapshot.queryParamMap.get('ReturnUrl');
    this.pageName = localStorage.getItem('loginPageTitle');
    if (this.initialClient === 'Pharmacist') {
      this.roles = 'pharmacy,pharmacist';
    } else if (this.initialClient === 'Cardiologist') {
      this.roles = 'cardiologist,nurse';
    } else if (this.initialClient === 'User') {
      this.roles = 'nambayauser,StakeHolder,PharmacyTrainer';
    } else if (this.initialClient === 'Center') {
      this.roles = 'centralgroupuser';
    }
  }

  ngOnDestroy() {}

  login() {
    this.isLoading = true;

    const user = {
      payload: {
        username: this.loginForm.controls.email.value,
        password: this.loginForm.controls.password.value,
        roles: this.roles
      },
      token: '',
      returnUrl: ''
    };
    const login$ = this.authenticationService.login(user);
    login$
      .pipe(
        finalize(() => {
          this.loginForm.markAsPristine();
          this.isLoading = false;
        }),
        untilDestroyed(this)
      )
      .subscribe(
        authenticationResult => {
          this.token = authenticationResult.verify_token;
          this.mode = authenticationResult.parameters[1];
          this.passwordFieldType = 'password';
          this.showOtpForm = true;
          localStorage.setItem('showOtpForm', 'true');
          localStorage.setItem('logintoken', this.token);
          localStorage.setItem('mode', this.mode);
          this.isLoading = false;
        },
        error => {
          if (error.error === 'USER_ID_INACTIVE') {
            log.debug(`Login error: ${error}`);
            this.translateService.get('User is inavtive').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.loginForm.reset();
            this.isLoading = false;
          } else if (error.error === 'USER_ID_LOCKED') {
            this.translateService.get('User is Locked').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.loginForm.reset();
            this.isLoading = false;
          } else if (error.error === 'USER_ID_DELETED') {
            this.translateService.get('User is Deleted').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.loginForm.reset();
            this.isLoading = false;
          } else if (error.error === 'INVALID_USER_ID_PASSWORD') {
            this.translateService.get('Invalid Password').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.loginForm.reset();
            this.isLoading = false;
          } else if (error.error === 'PHARMACY_IS_NOT_ACCESSIBLE') {
            this.translateService.get('PHARMACY_IS_NOT_ACCESSIBLE').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.loginForm.reset();
            this.isLoading = false;
          } else if (error.error === 'PASSWORD_RESET_REQUIRED') {
            this.error = error;
            localStorage.setItem('changepasswordemail', this.loginForm.controls.email.value);
            this.isLoading = false;
            this.navigateToChangePassword();
          } else {
            log.debug(`Login error: ${error}`);
            this.translateService.get('Login Failed, Credentials Invalid').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.loginForm.reset();
            this.isLoading = false;
          }
        }
      );
  }

  public navigateToForgetPassword() {
    localStorage.removeItem('otpSent');
    localStorage.removeItem('allowedEnterNewPassword');
    localStorage.removeItem('forgettoken');
    localStorage.setItem('requstNewOtp', 'true');
    this.router.navigate(['/forgetpassword', this.firstParam]);
  }

  public navigateToChangePassword() {
    localStorage.setItem('changepassword', 'true');
    localStorage.removeItem('otpSent');
    localStorage.removeItem('allowedEnterNewPassword');
    localStorage.removeItem('forgettoken');
    localStorage.setItem('requstNewOtp', 'true');
    this.router.navigate(['/changepassword', this.firstParam]);
  }

  verifyOtp() {
    this.isLoading = true;
    const otp = {
      payload: {
        otp: this.otpForm.controls.otp.value
      },
      token: this.token,
      returnUrl: this.firstParam
    };
    this.authenticationService.verifyOtop(otp).subscribe(
      result => {
        this.oauthservice.urlRedirectAfterLogin = result.redirectUrl;
        localStorage.removeItem('showOtpForm');
        localStorage.removeItem('logintoken');
        localStorage.removeItem('mode');

        localStorage.setItem('login', 'true');
        window.location = result.redirectUrl;
      },
      error => {
        if (error.error === 'Value does not fall within the expected range.') {
          this.translateService.get('OTP does not match').subscribe(text => {
            this.toastr.error(text);
            this.isLoading = false;
          });
        } else if (!(typeof error.error === 'string') && !(error.error instanceof String)) {
          this.translateService.get('OTP has Expired').subscribe(text => {
            this.toastr.error(text);
            localStorage.removeItem('showOtpForm');
            localStorage.removeItem('mode');
            localStorage.removeItem('otpSent');
            localStorage.removeItem('allowedEnterNewPassword');
            localStorage.removeItem('forgettoken');
            localStorage.removeItem('requstNewOtp');

            this.loginForm.reset();
            this.otpForm.reset();

            this.isLoading = false;
            this.showLogin = true;
            this.showSuccessLogin = false;
            this.showOtpForm = false;
          });
        }
      }
    );
  }

  goToHome() {
    this.router.navigate(['/']);
  }

  private createForm(fb: FormBuilder) {
    this.loginForm = fb.group({
      email: ['', Validators.email],
      password: ['', Validators.required]
    });
    this.otpForm = fb.group({
      otp: ['', Validators.required]
    });
  }
}
