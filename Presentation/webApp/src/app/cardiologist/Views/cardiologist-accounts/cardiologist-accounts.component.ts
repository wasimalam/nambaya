import { TranslateService } from '@ngx-translate/core';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { Location } from '@angular/common';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { CardiologistAccountsContext } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { PhoneVerificationContext, PhoneVerificationService } from '@app/shared/services/phone-verification.service';
import { IgxDialogComponent } from 'igniteui-angular';
import { LoggedUserInfoService } from '@app/shared/services/logged-user-info.service';
import {SearchCountryField, CountryISO, NgxIntlTelInputComponent} from "ngx-intl-tel-input";

@Component({
  providers: [PhoneVerificationService],
  selector: 'app-cardiologist-accounts',
  templateUrl: './cardiologist-accounts.component.html',
  styleUrls: ['./cardiologist-accounts.component.scss']
})
export class CardiologistAccountsComponent implements OnInit, OnDestroy, AfterViewInit {
  cardiologistAccountsForm!: FormGroup;
  phoneVerificationForm!: FormGroup;
  cardiologistAccountsData: any;
  cardiologistAccountsContext: CardiologistAccountsContext;
  phoneVerificationContext: PhoneVerificationContext;
  otpToken = '';
  isLoading = false;
  isDisplayVerifyLink = false;
  reformattedPhone = false;
  public applicationId: any = 'cardiologist';
  public roleCode: any = '';
  public userId: any;

  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];

  @ViewChild('phoneVerificationDialog', { static: true }) public phoneVerificationDialog: IgxDialogComponent;
  @ViewChild('phoneControl', { static: true }) public phoneControl: NgxIntlTelInputComponent;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private cardiologistAccountsService: CardiologistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private phoneVerificationService: PhoneVerificationService,
    private loggedUserService: LoggedUserInfoService
  ) {}

  ngOnInit() {
    this.isLoading = true;
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
      this.roleCode = localStorage.getItem('roleCode');
      if (this.roleCode === 'Nurse') {
        this.applicationId = 'nurse';
      }
    }
    if (localStorage.getItem('appUserId') !== 'undefined') {
      this.userId = localStorage.getItem('appUserId');
    }

    this.createCardiologistSettingsForm();
    this.createPhoneVerificationForm();

    const cardiologistRequest = this.cardiologistAccountsService.getData(this.userId, this.applicationId);

    forkJoin([cardiologistRequest]).subscribe(results => {
      this.cardiologistAccountsData = results[0];
      this.cardiologistAccountsForm.patchValue(this.cardiologistAccountsData);
      this.cardiologistAccountsForm.markAsUntouched();
      this.isLoading = false;

      this.cardiologistAccountsForm.get('phone').valueChanges.subscribe(value => {
        this.isDisplayVerifyLink = false;
        if(this.cardiologistAccountsData.phone && !this.reformattedPhone && value) {
          this.reformattedPhone = true;
          this.cardiologistAccountsForm.patchValue({
            phone: value.number
          })

        }

        if(this.cardiologistAccountsData.phoneVerified) {
          if(
            value
            && this.cardiologistAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.cardiologistAccountsData.phone
          ) {
            this.isDisplayVerifyLink = true;
          }
        } else {
          if(
            (value
            && this.cardiologistAccountsForm.controls.phone.status === 'VALID'
            && value.e164Number !== this.cardiologistAccountsData.phone)  ||
            (value
              && this.cardiologistAccountsForm.controls.phone.status === 'VALID'
              && !this.cardiologistAccountsData.phoneVerified
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
    if(this.cardiologistAccountsForm.value.phone && this.cardiologistAccountsForm.value.phone.e164Number) {
      return this.cardiologistAccountsForm.value.phone.e164Number;
    } else {
      return this.cardiologistAccountsForm.value.phone;
    }
  }

  getPhoneToDisplay() {
    return this.phoneControl.value;
  }

  updateCardiologistSettings() {
    if (this.cardiologistAccountsForm.invalid) {
      this.validateAllFormFields(this.cardiologistAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.cardiologistAccountsContext = this.cardiologistAccountsForm.value;
      this.cardiologistAccountsContext.phone = this.getPhoneToSave();
      this.cardiologistAccountsContext.id = Number(this.userId);
      this.cardiologistAccountsContext.isActive = true;
      this.cardiologistAccountsContext.role = 'Cardiologist';
      this.cardiologistAccountsContext.companyID = this.cardiologistAccountsData.companyID;
      if (this.roleCode === 'Nurse') {
        this.cardiologistAccountsContext.role = 'Nurse';
        this.cardiologistAccountsContext.cardiologistID = this.cardiologistAccountsData.cardiologistID;
      }

      this.cardiologistAccountsService.updateCardiologistSettings(this.cardiologistAccountsContext).subscribe(
        response => {
          if (response.status === 200) {
            this.cardiologistAccountsForm.patchValue({
              phone: this.getPhoneToDisplay()
            });
            this.translateService.get('Settings successfully saved').subscribe(text => {
              this.toastr.success(text);
            });
          }
          this.cardiologistAccountsData = response.body;
          let loggedUserName = this.cardiologistAccountsData.lastName
            ? this.cardiologistAccountsData.firstName + ' ' + this.cardiologistAccountsData.lastName
            : this.cardiologistAccountsData.firstName;
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
          this.cardiologistAccountsForm.patchValue({
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
            this.cardiologistAccountsData = response.body;
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

  private createCardiologistSettingsForm() {
    let controlObj = {
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
    };

    if (this.roleCode === 'Cardiologist') {
      controlObj = {
        ...controlObj,
        ...{
          doctorID: [
            '',
            [
              Validators.required,
              Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(12)])
            ]
          ],
          companyID: ['', Validators.required]
        }
      };
    } else if (this.roleCode === 'Nurse') {
      controlObj = {
        ...controlObj,
        ...{ companyID: ['', Validators.required] }
      };
    }

    this.cardiologistAccountsForm = this.formBuilder.group(controlObj);
  }

  private createPhoneVerificationForm() {
    this.phoneVerificationForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }
}
