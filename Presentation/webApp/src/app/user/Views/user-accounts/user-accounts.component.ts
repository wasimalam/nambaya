import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { forkJoin } from 'rxjs';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { UserAccountsContext } from '@app/user/Models/user-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { PhoneVerificationContext, PhoneVerificationService } from '@app/shared/services/phone-verification.service';
import { IgxDialogComponent } from 'igniteui-angular';
import { LoggedUserInfoService } from '@app/shared/services/logged-user-info.service';
import {SearchCountryField, CountryISO, NgxIntlTelInputComponent} from "ngx-intl-tel-input";

@Component({
  providers: [PhoneVerificationService],
  selector: 'app-accounts',
  templateUrl: './user-accounts.component.html',
  styleUrls: ['./user-accounts.component.scss']
})
export class UserAccountsComponent implements OnInit, OnDestroy, AfterViewInit {
  userAccountsForm!: FormGroup;
  phoneVerificationForm!: FormGroup;
  userAccountsData: any;
  userAccountsContext: UserAccountsContext;
  phoneVerificationContext: PhoneVerificationContext;
  otpToken = '';

  isLoading = false;
  isDisplayVerifyLink = false;
  reformattedPhone = false;
  public applicationId: any = 'nambayauser';
  public userId: any;
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];
  @ViewChild('phoneVerificationDialog', { static: true }) public phoneVerificationDialog: IgxDialogComponent;
  @ViewChild('phoneControl', { static: true }) public phoneControl: NgxIntlTelInputComponent;

  constructor(
    private loggedUserService: LoggedUserInfoService,
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private userAccountsService: UserAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private phoneVerificationService: PhoneVerificationService
  ) {}

  ngOnInit() {
    this.createUserSettingsForm();
    this.createPhoneVerificationForm();
    this.isLoading = true;

    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
    }
    if (localStorage.getItem('appUserId') !== 'undefined') {
      this.userId = localStorage.getItem('appUserId');
    }

    const userRequest = this.userAccountsService.getData(this.userId, this.applicationId);

    forkJoin([userRequest]).subscribe(results => {
      this.userAccountsData = results[0];
      this.userAccountsForm.patchValue(this.userAccountsData);

      this.userAccountsForm.markAsUntouched();
      this.isLoading = false;

      this.userAccountsForm.get('phone').valueChanges.subscribe(value => {
        this.isDisplayVerifyLink = false;
        if(this.userAccountsData.phone && !this.reformattedPhone && value) {
          this.reformattedPhone = true;
          this.userAccountsForm.patchValue({
            phone: value.number
          })
        }

        if(this.userAccountsData.phoneVerified) {
          if(
            value
            && this.userAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.userAccountsData.phone
          ) {
            this.isDisplayVerifyLink = true;
          }
        } else {
          if(
            (value
            && this.userAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.userAccountsData.phone)  ||
            (value
              && this.userAccountsForm.controls.phone.status === 'VALID'
              && !this.userAccountsData.phoneVerified
            )
          ) {
            this.isDisplayVerifyLink = true;
          }
        }
      });
    });
  }
  ngOnDestroy() {}
  ngAfterViewInit() {}

  getPhoneToSave() {
    if(this.userAccountsForm.value.phone && this.userAccountsForm.value.phone.e164Number) {
      return this.userAccountsForm.value.phone.e164Number;
    } else {
      return this.userAccountsForm.value.phone;
    }
  }

  getPhoneToDisplay() {
    return this.phoneControl.value;
  }

  updateUserSettings() {
    if (this.userAccountsForm.invalid) {
      this.validateAllFormFields(this.userAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.userAccountsContext = this.userAccountsForm.value;
      this.userAccountsContext.id = Number(this.userId);
      this.userAccountsContext.isActive = true;
      this.userAccountsContext.phone = this.getPhoneToSave();

      this.userAccountsService.updateUserSettings(this.userAccountsContext).subscribe(
        response => {
          this.userAccountsForm.patchValue({
            phone: this.getPhoneToDisplay()
          });
          if (response.status === 200) {
            this.translateService.get('Settings successfully saved').subscribe(text => {
              this.toastr.success(text);
            });
          }
          this.userAccountsData = response.body;

          let loggedUserName = this.userAccountsData.lastName
            ? this.userAccountsData.firstName + ' ' + this.userAccountsData.lastName
            : this.userAccountsData.firstName;
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
          this.userAccountsForm.patchValue({
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
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
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
            this.userAccountsData = response.body;

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

  goBack(): void {
    this.location.back();
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createUserSettingsForm() {
    this.userAccountsForm = this.formBuilder.group(
      {
        firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
        lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
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
