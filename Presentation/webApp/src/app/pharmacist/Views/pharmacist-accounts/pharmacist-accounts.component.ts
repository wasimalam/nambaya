import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { Location } from '@angular/common';
import { PharmacistAccountsService } from '@app/pharmacist/Models/pharmacist-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { OAuthService } from '@app/shared/OAuth.Service';
import { PharmacistAccountsContext } from '@app/pharmacist/Models/pharmacist-accounts.service';
import { forkJoin } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { PhoneVerificationContext, PhoneVerificationService } from '@app/shared/services/phone-verification.service';
import { IgxDialogComponent } from 'igniteui-angular';
import { LoggedUserInfoService } from '@app/shared/services/logged-user-info.service';
import {SearchCountryField, CountryISO, NgxIntlTelInputComponent} from "ngx-intl-tel-input";

@Component({
  providers: [PhoneVerificationService],
  selector: 'app-pharmacist-accounts',
  templateUrl: './pharmacist-accounts.component.html',
  styleUrls: ['./pharmacist-accounts.component.scss']
})
export class PharmacistAccountsComponent implements OnInit, OnDestroy, AfterViewInit {
  pharmacistAccountsForm!: FormGroup;
  phoneVerificationForm!: FormGroup;
  pharmacistAccountsData: any;
  pharmacistAccountsContext: PharmacistAccountsContext;
  phoneVerificationContext: PhoneVerificationContext;
  otpToken = '';

  error: string | undefined;
  isLoading = false;
  isDisplayVerifyLink = false;
  public applicationId: any = 'pharmacist';
  public userId: any = 0;
  reformattedPhone = false;
  roleCode: string;
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];

  // @ts-ignore
  @ViewChild('phoneVerificationDialog', { static: true }) public phoneVerificationDialog: IgxDialogComponent;
  @ViewChild('phoneControl', { static: true }) public phoneControl: NgxIntlTelInputComponent;

  constructor(
    private formBuilder: FormBuilder,
    private loggedUserService: LoggedUserInfoService,
    private customValidator: CustomValidatorService,
    private pharmacistAccountsService: PharmacistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private oAutheService: OAuthService,
    private translateService: TranslateService,
    private phoneVerificationService: PhoneVerificationService
  ) {}

  ngOnInit() {
    this.isLoading = true;
    if (localStorage.getItem('applicationId') && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
      this.roleCode = this.oAutheService.userData.rolecode;
      if (this.roleCode === 'Pharmacy') {
        this.applicationId = 'Pharmacy';
      }
    }
    if (localStorage.getItem('appUserId') !== 'undefined') {
      this.userId = localStorage.getItem('appUserId');
    }
    this.createPharmacistSettingsForm();
    this.createPhoneVerificationForm();

    const pharmacistRequest = this.pharmacistAccountsService.getData(this.userId, this.applicationId);

    forkJoin([pharmacistRequest]).subscribe(results => {
      this.pharmacistAccountsData = results[0];
      this.pharmacistAccountsForm.patchValue(this.pharmacistAccountsData);
      this.pharmacistAccountsForm.markAsUntouched();
      this.isLoading = false;
      this.pharmacistAccountsForm.get('phone').valueChanges.subscribe(value => {
        this.isDisplayVerifyLink = false;
        if(this.pharmacistAccountsData.phone && !this.reformattedPhone && value) {
          this.reformattedPhone = true;
          this.pharmacistAccountsForm.patchValue({phone: value.number});
        }

        if(this.pharmacistAccountsData.phoneVerified) {
          if(
            value
            && this.pharmacistAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.pharmacistAccountsData.phone
          ) {
            this.isDisplayVerifyLink = true;
          }
        } else {
          if(
            (value
            && this.pharmacistAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.pharmacistAccountsData.phone) ||
            (value
              && this.pharmacistAccountsForm.controls.phone.status === 'VALID'
              && !this.pharmacistAccountsData.phoneVerified
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

  goBack(): void {
    this.location.back();
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  getPhoneToSave() {
    if(this.pharmacistAccountsForm.value.phone && this.pharmacistAccountsForm.value.phone.e164Number) {
      return this.pharmacistAccountsForm.value.phone.e164Number;
    } else {
      return this.pharmacistAccountsForm.value.phone;
    }
  }

  getPhoneToDisplay() {
    return this.phoneControl.value;
  }

  updatePharmacistSettings() {
    if (this.pharmacistAccountsForm.invalid) {
      this.validateAllFormFields(this.pharmacistAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;

      this.pharmacistAccountsContext = this.pharmacistAccountsForm.value;
      this.pharmacistAccountsContext.phone = this.getPhoneToSave();
      if (this.oAutheService.userData.rolecode !== 'Pharmacy') {
        this.pharmacistAccountsContext.pharmacyID = this.pharmacistAccountsData.pharmacyID;
      }
      this.pharmacistAccountsContext.id = Number(this.userId);
      this.pharmacistAccountsContext.isActive = true;
      this.pharmacistAccountsService.updatePharmacistSettings(this.pharmacistAccountsContext).subscribe(
        response => {
          this.pharmacistAccountsForm.patchValue({
            phone: this.getPhoneToDisplay()
          });
          if (response.status === 200) {
            this.translateService.get('Settings successfully saved').subscribe(text => {
              this.toastr.success(text);
            });
          }
          this.pharmacistAccountsData = response.body;
          this.loggedUserService.updateLoggedUserName(this.pharmacistAccountsData.name);
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
          this.pharmacistAccountsForm.patchValue({
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
            this.pharmacistAccountsData = response.body;
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

  private createPharmacistSettingsForm() {
    let controlObj = {
      email: ['', Validators.email],
      phone: [''],
      street: [''],
      address: [''],
      contact: [''],
      zipCode: ['', Validators.compose([this.customValidator.numberValidator()])]
    };
    if (this.roleCode === 'Pharmacy') {
      controlObj = { ...controlObj, ...{ name: ['', Validators.required] } };
      controlObj = {
        ...controlObj,
        ...{ identification: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]] }
      };
    } else {
      controlObj = {
        ...controlObj,
        ...{ firstName: ['', Validators.required] },
        ...{ lastName: ['', Validators.required] }
      };
    }
    this.pharmacistAccountsForm = this.formBuilder.group(controlObj);
  }

  private createPhoneVerificationForm() {
    this.phoneVerificationForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }
}
