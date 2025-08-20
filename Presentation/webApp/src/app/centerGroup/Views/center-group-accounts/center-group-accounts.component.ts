import {AfterViewInit, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {Location} from '@angular/common';
import {CustomValidatorService} from '@app/shared/services/custom-validator.service';
import {forkJoin} from 'rxjs';
import {CenterGroupAccountsService} from '@app/centerGroup/Models/center-group-accounts.service';
import {ToastrService} from 'ngx-toastr';
import {CenterGroupAccountsContext} from '@app/centerGroup/Models/center-group-accounts.service';
import {TranslateService} from '@ngx-translate/core';
import {PhoneVerificationContext, PhoneVerificationService} from '@app/shared/services/phone-verification.service';
import {IgxDialogComponent} from 'igniteui-angular';
import {OAuthService} from '@app/shared/OAuth.Service';
import {LoggedUserInfoService} from '@app/shared/services/logged-user-info.service';
import {SearchCountryField, CountryISO, NgxIntlTelInputComponent} from "ngx-intl-tel-input";

@Component({
  providers: [PhoneVerificationService],
  selector: 'app-physician-accounts',
  templateUrl: './center-group-accounts.component.html',
  styleUrls: ['./center-group-accounts.component.scss']
})
export class CenterGroupAccountsComponent implements OnInit, OnDestroy, AfterViewInit {
  centerGroupAccountsForm!: FormGroup;
  phoneVerificationForm!: FormGroup;
  centerGroupAccountsData: any;
  centerGroupAccountsContext: CenterGroupAccountsContext;
  phoneVerificationContext: PhoneVerificationContext;

  otpToken = '';
  error: string | undefined;
  isLoading = false;
  isDisplayVerifyLink = false;
  reformattedPhone = false;
  public applicationId: any = 'center';
  public userId: any;
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];
  @ViewChild('phoneVerificationDialog', {static: true}) public phoneVerificationDialog: IgxDialogComponent;
  @ViewChild('phoneControl', { static: true }) public phoneControl: NgxIntlTelInputComponent;

  constructor(
    private loggedUserService: LoggedUserInfoService,
    private oAutheService: OAuthService,
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private centerGroupAccountsService: CenterGroupAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private phoneVerificationService: PhoneVerificationService
  ) {
  }

  ngOnInit() {
    this.isLoading = true;
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
    }
    if (localStorage.getItem('appUserId') !== 'undefined') {
      this.userId = localStorage.getItem('appUserId');
    }

    this.createCenterGroupSettingsForm();
    this.createPhoneVerificationForm();

    const centerGroupRequest = this.centerGroupAccountsService.getData(this.userId, this.applicationId);

    forkJoin([centerGroupRequest]).subscribe(results => {
      this.centerGroupAccountsData = results[0];
      this.centerGroupAccountsForm.patchValue(this.centerGroupAccountsData);
      this.centerGroupAccountsForm.markAsUntouched();
      this.isLoading = false;
      this.centerGroupAccountsForm.get('phone').valueChanges.subscribe(value => {
        this.isDisplayVerifyLink = false;
        if (this.centerGroupAccountsData.phone && !this.reformattedPhone && value) {
          this.reformattedPhone = true;
          this.centerGroupAccountsForm.patchValue({
            phone: value.number
          })
        }

        if (this.centerGroupAccountsData.phoneVerified) {
          if (
            value
            && this.centerGroupAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.centerGroupAccountsData.phone
          ) {
            this.isDisplayVerifyLink = true;
          }
        } else {
          if (
            (value
            && this.centerGroupAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.centerGroupAccountsData.phone) ||
            (value
              && this.centerGroupAccountsForm.controls.phone.status === 'VALID'
              && !this.centerGroupAccountsData.phoneVerified
            )
          ) {
            this.isDisplayVerifyLink = true;
          }
        }
      });
    });
  }

  ngOnDestroy() {
  }

  ngAfterViewInit() {
  }

  goBack(): void {
    this.location.back();
  }

  getPhoneToSave() {
    if(this.centerGroupAccountsForm.value.phone && this.centerGroupAccountsForm.value.phone.e164Number) {
      return this.centerGroupAccountsForm.value.phone.e164Number;
    } else {
      return this.centerGroupAccountsForm.value.phone;
    }
  }

  getPhoneToDisplay() {
    return this.phoneControl.value;
  }

  updateCenterGroupSettings() {
    if (this.centerGroupAccountsForm.invalid) {
      this.validateAllFormFields(this.centerGroupAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.centerGroupAccountsContext = this.centerGroupAccountsForm.value;
      this.centerGroupAccountsContext.phone = this.getPhoneToSave();
      this.centerGroupAccountsContext.id = Number(this.userId);
      this.centerGroupAccountsContext.isActive = true;

      this.centerGroupAccountsService.updateCenterGroupSettings(this.centerGroupAccountsContext).subscribe(
        response => {
          this.centerGroupAccountsForm.patchValue({
            phone: this.getPhoneToDisplay()
          });
          if (response.status === 200) {
            this.translateService.get('Settings successfully saved').subscribe(text => {
              this.toastr.success(text);
            });
          }
          this.centerGroupAccountsData = response.body;
          let loggedUserName = this.centerGroupAccountsData.lastName
            ? this.centerGroupAccountsData.firstName + ' ' + this.centerGroupAccountsData.lastName
            : this.centerGroupAccountsData.firstName;
          this.loggedUserService.updateLoggedUserName(loggedUserName);
          this.isLoading = false;
        },
        error => {
          let message = 'Something went wrong';
          if (error.status === 400) {
            message = error.error;
          }
          this.translateService.get(message).subscribe(text => {
            this.toastr.error(text);
          });
          this.isLoading = false;
        }
      );
    }
  }

  public generatePhoneVerificationOtp() {
    this.isLoading = true;
    this.phoneVerificationService.generatePhoneVerificationOtp(this.getPhoneToSave()).subscribe(
      response => {
        if (response.status === 200) {
          this.centerGroupAccountsForm.patchValue({
            phone: this.getPhoneToDisplay()
          });
          this.otpToken = response.body.token;
          this.isLoading = false;
          this.openOTPDialog();
        }
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoading = false;
      }
    );
  }

  public openOTPDialog() {
    this.phoneVerificationDialog.open();
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({behavior: 'smooth', block: 'center'});
  }

  public verifyPhone() {
    if (this.phoneVerificationForm.invalid) {
      this.validateAllFormFields(this.phoneVerificationForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.phoneVerificationContext = this.phoneVerificationForm.value;
      this.phoneVerificationContext.token = this.otpToken;
      this.phoneVerificationContext.PhoneNumber = this.getPhoneToSave();
      this.phoneVerificationService.verifyOtp(this.phoneVerificationContext).subscribe(
        response => {
          this.isLoading = false;
          if (response.status === 200) {
            this.translateService.get('phone_successfully_verified').subscribe(text => {
              this.toastr.success(text);
            });
            this.isDisplayVerifyLink = false;
            this.centerGroupAccountsData = response.body;
            this.phoneVerificationDialog.close();
          }
        },
        error => {
          this.isLoading = false;
          if (error.status === 400 && error.error === 'OTP_VERIFICATION_FAILED') {
            this.translateService.get(error.error).subscribe(text => {
              this.toastr.error(text);
            });
            return;
          }
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({onlySelf: true});
    });
  }

  private createCenterGroupSettingsForm() {
    this.centerGroupAccountsForm = this.formBuilder.group(
      {
        firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(45)])]],
        lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(45)])]],
        email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
        phone: [''],
        street: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
        address: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
        zipCode: [
          '',
          Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(5)])
        ]
      }
    );
  }

  private createPhoneVerificationForm() {
    this.phoneVerificationForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }
}
