import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {CustomValidatorService} from '@app/shared/services/custom-validator.service';
import {PatientsService} from '@app/pharmacist/Models/patients.service';
import {Location} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {FileSystemDirectoryEntry, FileSystemFileEntry, NgxFileDropEntry} from 'ngx-file-drop';
import {IgxDialogComponent} from 'igniteui-angular';
import {CardioSigImgContext, FileUploadService} from '@app/shared/fileUpload.service';
import {OAuthService} from '@app/shared/OAuth.Service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent implements OnInit {

  isLoading = false;
  public fileToUpload: File = null;
  public uploading = false;
  public saving = false;
  public progress = 0;
  public disableButtons = false;
  public files: NgxFileDropEntry[] = [];
  uploadSigImgForm!: FormGroup;
  otpToken = '';
  public cardioSigImgFileContext: CardioSigImgContext = {
    otp: '',
    token: '',
    email: '',
  };

  @ViewChild('uploadSigImgDialog', {static: true}) public uploadSigImgDialog: IgxDialogComponent;

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
  ) { }

  ngOnInit() {
    this.createPhoneVerificationForm();
  }

  goBack(): void {
    this.location.back();
  }

  public cancelUpload() {
    this.fileToUpload = null;
  }

  public generateSigImgUploadVerificationOtp() {
    this.isLoading = true;
    this.fileUploadService.generateSigImgUploadVerificationOtp().subscribe(
      response => {
        if (response.status === 200) {
          this.otpToken = response.body.token;
          this.uploadSigImgForm.reset();
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

  uploadSigImgFile() {
    if (this.uploadSigImgForm.invalid) {
      this.validateAllFormFields(this.uploadSigImgForm);
    } else {
      this.uploading = true;
      this.uploadSigImgDialog.close();
      this.disableButtons = true;
      this.cardioSigImgFileContext.token = this.otpToken;
      this.cardioSigImgFileContext.email = this.oAuthService.userData.email;
      this.cardioSigImgFileContext.otp = this.uploadSigImgForm.value.otp.trim();

      this.fileUploadService.postSigImgFile(this.fileToUpload, this.cardioSigImgFileContext).subscribe(
        data => {
          if (data && data.message === 100) {
            for (let i = this.progress; i <= 100; i++) {
              this.progress = this.progress + 1;
            }
            this.uploading = false;
            this.saving = true;
          }
          if (data && data.id) {
            this.progress = 100;
            this.translateService.get('file uploaded successfully').subscribe(text => {
              this.toastr.success(text);
            });
            this.uploading = false;
            this.saving = false;
            this.disableButtons = false;
            this.router.navigate(['/cardiologist/signature/preview']);
          } else {
            this.progress = data.message;
          }
        },
        error => {
          this.isLoading = false;
          this.uploading = false;
          this.disableButtons = false;
          this.saving = false;
          this.uploadSigImgForm.reset();
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

  public dropped(files: NgxFileDropEntry[]) {
    this.files = files;
    for (const droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if (file.type.toLowerCase() === 'image/png' || file.type.toLowerCase() === 'image/jpeg') {
            this.fileToUpload = file;
          } else {
            this.translateService.get('only_png_jpg_allowed').subscribe(text => {
              this.toastr.error(text);
            });
          }
        });
      } else {
        const fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
      }
    }
  }

  // tslint:disable-next-line:typedef
  public fileOver(event) { }

  // tslint:disable-next-line:typedef
  public fileLeave(event) { }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createPhoneVerificationForm() {
    this.uploadSigImgForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }

}
