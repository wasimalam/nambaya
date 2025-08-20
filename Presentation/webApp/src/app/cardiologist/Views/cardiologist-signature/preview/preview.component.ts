import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {CustomValidatorService} from '@app/shared/services/custom-validator.service';
import {PatientsService} from '@app/pharmacist/Models/patients.service';
import {Location} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {
  CardioDeleteSignatureContext,
  CardiologistAccountsService
} from '@app/cardiologist/Models/cardiologist-accounts.service';
import {IgxDialogComponent} from 'igniteui-angular';
import {OAuthService} from '@app/shared/OAuth.Service';

@Component({
  selector: 'app-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements OnInit {

  isLoading = false;
  fileDataString = '';
  deleteSignatureForm!: FormGroup;
  otpToken = '';
  public cardiodeleteSignatureContext: CardioDeleteSignatureContext = {
    otp: '',
    token: '',
    email: ''
  };

  @ViewChild('deleteSignatureDialog', {static: true}) public deleteSignatureDialog: IgxDialogComponent;


  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService,
    private cardiologistService: CardiologistAccountsService,
    private oAuthService: OAuthService,
  ) { }

  ngOnInit() {
    this.createPhoneVerificationForm();
    this.isLoading = true;
    this.cardiologistService.getSignatureData().subscribe(
      response => {
        this.isLoading = false;
        if (response.status === 200) {
          // @ts-ignore
          this.fileDataString = response.body.fileDataString;
        }
      },
      error => {
        this.isLoading = false;
        if (error.status === 404) {
          this.translateService.get(error.error).subscribe(text => {});
          return;
        }

        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
  }

  public generateDeleteSignatureOtp() {
    this.isLoading = true;
    this.cardiologistService.generateDeleteSignatureOtp().subscribe(
      response => {
        if (response.status === 200) {
          this.otpToken = response.body.token;
          this.deleteSignatureForm.reset();
          this.deleteSignatureDialog.open();
        }
        this.isLoading = false;
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoading = false;
      }
    );
  }

  deleteSignature() {
    if (this.deleteSignatureForm.invalid) {
      this.validateAllFormFields(this.deleteSignatureForm);
    } else {
      this.isLoading = true;
      this.deleteSignatureDialog.close();
      this.cardiodeleteSignatureContext.token = this.otpToken;
      this.cardiodeleteSignatureContext.email = this.oAuthService.userData.email;
      this.cardiodeleteSignatureContext.otp = this.deleteSignatureForm.value.otp.trim();

      this.cardiologistService.deleteSignature(this.cardiodeleteSignatureContext).subscribe(
        data => {
          this.translateService.get('signature_delete_success').subscribe(text => {
            this.toastr.success(text);
            this.fileDataString = '';
          });
          this.isLoading = false;
        },
        error => {
          this.isLoading = false;
          this.deleteSignatureForm.reset();
          this.deleteSignatureDialog.open();
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

  private createPhoneVerificationForm() {
    this.deleteSignatureForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }

}
