import { OAuthService } from '@app/shared/OAuth.Service';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { environment } from '@env/environment';
import { I18nService, AuthenticationService } from '@app/core';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { CustomValidatorService } from '../services/custom-validator.service';
import { CountdownComponent } from 'ngx-countdown';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-forget',
  templateUrl: './forgetPassword.component.html',
  styleUrls: ['./forgetPassword.component.scss']
})
export class ForgetPasswordComponent implements OnInit, OnDestroy {
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

  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  public pageName = '';
  public loginid = '';

  /*igx*/
  public forgetPasswordForm: FormGroup;
  public otpForm: FormGroup;
  public confirmPasswordForm: FormGroup;
  public waitingforOtp = false;
  public showLogin = true;
  public showSuccessLogin = false;
  public param1: string;
  public clientId: string;
  public showOtpForm = false;
  public token: string;
  public passwordToken: string;
  public otp: string;
  public returnUrl: string;
  public applicationCode = '';

  public availableModes = [
    {
      modeName: 'email',
      enumValue: 1,
      displayName: 'Email',
      alternateOption: 'Mobile'
    },
    {
      modeName: 'sms',
      enumValue: 2,
      displayName: 'Mobile',
      alternateOption: 'Email'
    }
  ];

  public selectedIndex = 0;
  public selectedMode = this.availableModes[this.selectedIndex];
  public initialClient = '';
  public requstNewOtp = true;
  public otpSent = false;
  public allowedEnterNewPassword = false;
  public hideToggle = false;
  public isAllowedToSendAgain = true;
  public updateToast;
  public config = {
    leftTime: 60,
    demand: true
  };
  public configNewPasswordCountDown = {
    leftTime: 120,
    demand: true
  };
  public passwordFieldType = 'password';
  @ViewChild('timer', { static: false }) private countdown: CountdownComponent;
  @ViewChild('timerNewPassword', { static: false }) private countDownNewPassword: CountdownComponent;
  @ViewChild('sendOTPAgaintimer', { static: false }) private countdownOTP: CountdownComponent;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private i18nService: I18nService,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    fb: FormBuilder,
    public oauthservice: OAuthService,
    public oidcSecurityService: OidcSecurityService,
    private location: Location,
    private customValidator: CustomValidatorService,
    private translateService: TranslateService,
    private router: Router
  ) {

    localStorage.getItem('requstNewOtp');
    localStorage.getItem('otpSent');
    localStorage.getItem('allowedEnterNewPassword');
    localStorage.getItem('forgettoken');

    if (localStorage.getItem('requstNewOtp') === 'true') {
      this.requstNewOtp = true;
    }
    if (localStorage.getItem('requstNewOtp') === 'false') {
      this.requstNewOtp = false;
    }

    if (localStorage.getItem('otpSent') === 'true') {
      this.otpSent = true;
    }
    if (localStorage.getItem('otpSent') === 'false') {
      this.otpSent = false;
    }
    if (localStorage.getItem('changepassword') == 'true') {
      this.requstNewOtp = false;
      this.otpSent = true;
    }

    if (localStorage.getItem('allowedEnterNewPassword') === 'true') {
      this.allowedEnterNewPassword = true;
    }
    if (localStorage.getItem('allowedEnterNewPassword') === 'false') {
      this.allowedEnterNewPassword = false;
    }

    if (localStorage.getItem('forgettoken')) {
      this.token = localStorage.getItem('forgettoken');
    }

    this.applicationCode = localStorage.getItem('applicationCode');
    this.createForm(fb);
  }

  onTimerFinishedForPassword(e: Event) {
    if (e['action'] == 'done') {
      setTimeout(() => {
        const application = localStorage.getItem('initialClient');
        this.router.navigate(['/' + application.toLowerCase()]);
      }, 6000);
      this.toastr.clear();
      this.translateService.get('password_change_failed').subscribe(text => {
        this.toastr.error(text, '', {
          closeButton: true,
          positionClass: 'toast-top-center',
          progressBar: true,
          toastClass: 'toastr-custom-width ngx-toastr'
        });
      });
    }
  }

  onTimerFinished(e: Event) {
    if (e['action'] == 'done') {
      this.confirmPasswordForm.controls.toggle.setValue(false);
    }
  }

  onTimerFinishedOtp(e: Event) {
    if (e['action'] == 'done') {
      this.isAllowedToSendAgain = true;
    }
  }

  toggleOtpOption() {
    this.confirmPasswordForm.controls.toggle.setValue(true);
    this.countdown.restart();
    this.config.demand = false;
    this.countdown.begin();
    if (this.selectedIndex === 0) {
      this.selectedIndex = 1;
      this.selectedMode = this.availableModes[this.selectedIndex];
    } else {
      this.selectedIndex = 0;
      this.selectedMode = this.availableModes[this.selectedIndex];
    }

    this.getPasswordResetOtp();
  }

  sendAgain() {
    this.isAllowedToSendAgain = false;
    this.countdownOTP.restart();
    this.config.demand = false;
    this.countdownOTP.begin();
    this.getPasswordResetOtp();
  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.params.returnUrl;
    this.pageName = localStorage.getItem('loginPageTitle');
    this.initialClient = localStorage.getItem('applicationId');
    if (localStorage.getItem('changepassword') == 'true') {
      this.forgetPasswordForm.controls.email.setValue(localStorage.getItem('changepasswordemail'));
      this.getPasswordResetOtp();
      localStorage.removeItem('changepassword');
      localStorage.removeItem('changepasswordemail');
    }
  }

  ngOnDestroy() {}

  getPasswordResetOtp() {
    this.loginid = this.forgetPasswordForm.controls.email.value;
    localStorage.setItem('loginid', this.loginid);
    const requestParam = {
      payload: {
        loginname: this.forgetPasswordForm.controls.email.value,
        otptype: this.selectedMode.modeName,
        applicationcode: this.applicationCode
      },
      token: '',
      returnUrl: ''
    };

    this.authenticationService.getForgetPasswordOtp(requestParam).subscribe(
      result => {
        if (result) {
          this.passwordFieldType = 'password';
          this.requstNewOtp = false;
          this.otpSent = true;
          this.allowedEnterNewPassword = false;
          this.token = result.verify_token;
          localStorage.setItem('requstNewOtp', 'false');
          localStorage.setItem('otpSent', 'true');
          localStorage.setItem('allowedEnterNewPassword', 'false');
          localStorage.setItem('forgettoken', this.token);
        } else {
          this.translateService.get('Email Not Found').subscribe(text => {
            this.toastr.error(text);
          });
        }
      },
      error => {
        this.translateService.get('Not authorized').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
  }

  verifyOtp() {
    const otp = {
      payload: {
        otp: this.otpForm.controls.otp.value
      },
      token: this.token,
      returnUrl: ''
    };
    this.authenticationService.verifyForgetPasswordOtop(otp).subscribe(
      result => {
        if (result) {
          this.passwordFieldType = 'password';
          this.requstNewOtp = false;
          this.otpSent = false;
          this.allowedEnterNewPassword = true;
          this.passwordToken = result.verify_token;
          if (result.expires_in) {
            this.configNewPasswordCountDown.leftTime = result.expires_in;
          }
          localStorage.setItem('requstNewOtp', 'false');
          localStorage.setItem('otpSent', 'false');
          localStorage.setItem('allowedEnterNewPassword', 'true');
          localStorage.setItem('forgettoken', this.token);

          if (
            this.route.snapshot.url &&
            this.route.snapshot.url.length > 0 &&
            this.route.snapshot.url[0].path === 'changepassword'
          ) {
            this.translateService.get('PASSWORD_RESET_REQUIRED').subscribe(text => {
              this.updateToast = this.toastr.info(text, '', {
                closeButton: true,
                positionClass: 'toast-top-center',
                timeOut: 50000,
                extendedTimeOut: 50000,
                toastClass: 'toastr-custom-width nambaya-blue-bg ngx-toastr'
              });
            });
          }
          setTimeout(() => {
            this.configNewPasswordCountDown.demand = false;
            this.countDownNewPassword.begin();
          }, 500);
        }
      },
      error => {
        this.translateService.get('OTP does not match').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
  }

  public toggleLanguage() {
    const currentLanguge = this.i18nService.language;
    if (currentLanguge === 'en-US') {
      this.i18nService.language = 'de-DE';
    } else {
      this.i18nService.language = 'en-US';
    }
    localStorage.setItem('requstNewOtp', 'false');
    localStorage.setItem('otpSent', 'true');
    localStorage.setItem('allowedEnterNewPassword', 'false');
    localStorage.setItem('lang', this.i18nService.language);
    window.location.reload();
  }

  setLanguage(language: string) {
    this.i18nService.language = language;
    window.location.reload();
  }

  saveNewPassword() {
    if (this.confirmPasswordForm.invalid) {
      this.validateConfirmPasswordFormFields(this.confirmPasswordForm);
    } else {
      let loginId;
      if(!this.loginid) {
        loginId = localStorage.getItem('loginid');
      } else {
        loginId = this.loginid;
      }

      const confirmPass = {
        payload: {
          loginid: loginId,
          password: this.confirmPasswordForm.controls.newPassword.value
        },
        token: this.passwordToken,
        returnUrl: this.returnUrl
      };

      this.authenticationService.saveNewPassword(confirmPass).subscribe(
        result => {
          if (result) {
            this.oauthservice.urlRedirectAfterLogin = result.redirectUrl;
            localStorage.removeItem('requstNewOtp');
            localStorage.removeItem('otpSent');
            localStorage.removeItem('allowedEnterNewPassword');
            localStorage.removeItem('forgettoken');
            localStorage.removeItem('loginid');
            window.location = result.redirectUrl;
          }
        },
        error => {
          if (error.error === 'POLICY_INVALID_PASSWORD') {
            this.translateService.get('POLICY_INVALID_PASSWORD').subscribe(text => {
              this.toastr.error(text);
            });
            this.error = error;
            this.confirmPasswordForm.reset();
            this.isLoading = false;
          } else {
            this.error = error;
            this.confirmPasswordForm.reset();
            this.isLoading = false;
          }
        }
      );
    }
  }

  public showPassword() {
    if (this.passwordFieldType == 'password') {
      this.passwordFieldType = 'text';
    } else {
      this.passwordFieldType = 'password';
    }
  }

  goBack(): void {
    this.location.back();
  }

  private validateConfirmPasswordFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if ((field === 'password' || field === 'confirmPassword') && control.value !== '') {
        control.markAsTouched({ onlySelf: true });
      } else {
        control.markAsTouched({ onlySelf: true });
      }
    });
  }

  private createForm(fb: FormBuilder) {
    this.forgetPasswordForm = fb.group({
      email: ['', Validators.email]
    });

    this.otpForm = fb.group({
      otp: ['', Validators.required]
    });

    this.confirmPasswordForm = fb.group(
      {
        newPassword: ['', Validators.compose([this.customValidator.patternValidator()])],
        confirmPassword: ['', Validators.required],
        toggle: ['']
      },
      { validator: this.customValidator.MatchPassword('newPassword', 'confirmPassword') }
    );
  }
}
