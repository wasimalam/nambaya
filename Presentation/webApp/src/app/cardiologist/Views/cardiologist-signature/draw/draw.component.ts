import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {CustomValidatorService} from '@app/shared/services/custom-validator.service';
import {PatientsService} from '@app/pharmacist/Models/patients.service';
import {Location} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import { SignaturePad} from 'angular2-signaturepad';
import {CardioSigStringContext, FileUploadService} from '@app/shared/fileUpload.service';
import {IgxDialogComponent} from 'igniteui-angular';
import {OAuthService} from '@app/shared/OAuth.Service';
import {CardiologistAccountsService} from '@app/cardiologist/Models/cardiologist-accounts.service';

@Component({
  selector: 'app-draw',
  templateUrl: './draw.component.html',
  styleUrls: ['./draw.component.scss']
})
export class DrawComponent implements OnInit {

  isLoading = false;
  public uploading = false;
  public saving = false;
  public disableButtons = false;
  public emptySignature = false;
  otpToken = '';
  uploadSigStringForm!: FormGroup;
  public cardioSigStringContext: CardioSigStringContext = {
    otp: '',
    token: '',
    email: '',
    ImageData: '',
  };

  @ViewChild('SignaturePad', { static: true }) public signaturePad: SignaturePad;
  @ViewChild('uploadSigStringDialog', {static: true}) public uploadSigImgDialog: IgxDialogComponent;

  signaturePadOptions = {
    'minWidth': 1,
    'canvasWidth': 537,
    'canvasHeight': 300
  };

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService,
    public fileUploadService: FileUploadService,
    public oAuthService: OAuthService,
    private cardiologistService: CardiologistAccountsService,
  ) { }

  ngOnInit() {
    this.createPhoneVerificationForm();
    this.isLoading = true;
    this.cardiologistService.getSignatureData().subscribe(
      response => {
        this.isLoading = false;
        if (response.status === 200) {
          // @ts-ignore
          this.signaturePad.fromDataURL('data:image/png;base64,'+response.body.fileDataString);
        }
      },
      error => {
        this.isLoading = false;
        if (error.status === 404) {
          return;
        }
      }
    );
  }

  goBack(): void {
    this.location.back();
  }

  drawStart() {
    this.emptySignature = false;
  }

  public generateSigStringUploadVerificationOtp() {
    if(this.signaturePad.isEmpty()) {
      this.emptySignature = true;
      return;
    }
    this.isLoading = true;
    this.fileUploadService.generateSigImgUploadVerificationOtp().subscribe(
      response => {
        if (response.status === 200) {
          this.otpToken = response.body.token;
          this.uploadSigStringForm.reset();
          this.uploadSigImgDialog.open();
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

  uploadSigString() {
    if (this.uploadSigStringForm.invalid) {
      this.validateAllFormFields(this.uploadSigStringForm);
    } else {
      this.uploading = true;
      this.saving = true;
      this.uploadSigImgDialog.close();
      this.disableButtons = true;
      this.cardioSigStringContext.token = this.otpToken;
      this.cardioSigStringContext.email = this.oAuthService.userData.email;
      this.cardioSigStringContext.otp = this.uploadSigStringForm.value.otp.trim();
      this.cardioSigStringContext.ImageData = this.signaturePad.toDataURL()
        .replace('data:image/png;base64,', '');

      this.fileUploadService.postSigString(this.cardioSigStringContext).subscribe(
        data => {
            this.translateService.get('signature_successfully_saved').subscribe(text => {
              this.toastr.success(text);
            });
            this.uploading = false;
            this.saving = false;
            this.disableButtons = false;
            this.router.navigate(['/cardiologist/signature/preview']);
        },
        error => {
          this.isLoading = false;
          this.uploading = false;
          this.disableButtons = false;
          this.saving = false;
          this.uploadSigStringForm.reset();
          this.uploadSigImgDialog.open();
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

  clearCanvas() {
    this.signaturePad.clear();
  }

  undo() {
    const data = this.signaturePad.toData();
    if (data) {
      data.pop();
      this.signaturePad.fromData(data);
    }
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createPhoneVerificationForm() {
    this.uploadSigStringForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }

}
