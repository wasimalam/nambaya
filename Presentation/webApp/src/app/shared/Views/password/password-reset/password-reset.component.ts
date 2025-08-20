import { AfterViewInit, Component, OnInit} from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import {CustomValidatorService} from '@app/shared/services/custom-validator.service';
import {PhoneVerificationService, PasswordChangeContext} from '@app/shared/services/phone-verification.service';

@Component({
  providers: [PhoneVerificationService],
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss']
})
export class PasswordResetComponent implements OnInit, AfterViewInit {
  passwordResetForm!: FormGroup;

  isLoading = false;
  public applicationId: any = 'cardiologist';
  public userId: any;
  languages: any;
  roleCode = '';
  public passwordFieldType = 'password';
  passwordChangeContext: PasswordChangeContext;

  constructor(
    private formBuilder: FormBuilder,
    private cardiologistAccountsService: CardiologistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private customValidator: CustomValidatorService,
    private phoneVerificationService: PhoneVerificationService,
  ) {
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
    }
  }

  ngOnInit() {
    this.createPasswordResetForm();
  }

  ngAfterViewInit() {}

  goBack(): void {
    this.location.back();
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  public showPassword() {
    if (this.passwordFieldType == 'password') {
      this.passwordFieldType = 'text';
    } else {
      this.passwordFieldType = 'password';
    }
  }

  updatePassword() {
    if (this.passwordResetForm.invalid) {
      this.validateAllFormFields(this.passwordResetForm);
      this.scrollToError();
    } else {
      this.passwordChangeContext = this.passwordResetForm.value;
      if(this.passwordChangeContext.OldPassword === this.passwordChangeContext.NewPassword) {
        this.translateService.get('new_password_should_be_different').subscribe(text => {
          this.toastr.info(text);
        });
        return;
      }

      this.isLoading = true;
      this.phoneVerificationService.changePassword(this.passwordChangeContext).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('password_successfully_saved').subscribe(text => {
              this.toastr.success(text);
            });
          }
          this.isLoading = false;
        },
        error => {
          this.isLoading = false;
          if (error.status === 400 && error.error === 'OLD_PASSWORD_DOES_NOT_MATCH') {
            this.translateService.get(error.error).subscribe(text => {
              this.toastr.error(text);
            });
            return;
          }

          this.translateService.get('Something went wrong.').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createPasswordResetForm() {
    this.passwordResetForm = this.formBuilder.group({
      OldPassword: ['', Validators.required],
      NewPassword: ['',  [Validators.required, Validators.compose([this.customValidator.patternValidator()])]],
      confirmPassword: ['']
    },{
      validator: this.customValidator.MatchPassword('NewPassword', 'confirmPassword')
    });
  }
}
