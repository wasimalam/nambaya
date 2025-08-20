import { TranslateService } from '@ngx-translate/core';
import { FileUploadService, CardioECGFileContext } from '@app/shared/fileUpload.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { OAuthService } from '@app/shared/OAuth.Service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxFileDropEntry, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { IgxSelectComponent } from 'igniteui-angular';
import { IgxDialogComponent } from 'igniteui-angular';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { CardioWizardComponent } from '@app/shared/Views/wizard/cardio-wizard/cardio-wizard.component';
import { CaseStatuses } from '@app/objects/configurations';

@Component({
  providers: [FileUploadService],
  selector: 'app-fileUpload',
  templateUrl: './uploadEcg.component.html',
  styleUrls: ['./uploadEcg.component.scss']
})
export class EcgUploadComponent implements OnInit, AfterViewInit, OnDestroy {
  public cardioECGFileContext: CardioECGFileContext = {
    patientCaseId: 0,
    notesTypeId: 0,
    notes: '',
    otp: '',
    token: '',
    email: ''
  };
  public caseObject: any = null;
  public patientCaseId: number;
  public fileToUpload: File = null;
  public uploading = false;
  public saving = false;
  public progress = 0;
  public disableButtons = false;
  public files: NgxFileDropEntry[] = [];
  public remarks = '';
  public selectedNotes = '';
  public noteTypes: any = '';
  public notesTypeArray = {};
  isLoading = false;
  signatureExists = false;
  isDownloadingFile = false;
  ecgReportData = null;
  caseStatuses = CaseStatuses;
  roleCode = '';

  uploadEcgForm!: FormGroup;
  otpToken = '';
  public urlPrefix = '';
  public parentData: any = {
    stepNumber: 0,
    cardioWizardCurrentStep: 3
  };

  @ViewChild('uploadEcgDialog', { static: true }) public uploadEcgDialog: IgxDialogComponent;
  @ViewChild(IgxSelectComponent, { static: false }) public noteTypeSelect: IgxSelectComponent;
  // @ts-ignore
  @ViewChild(CardioWizardComponent) cardioWizard: CardioWizardComponent;

  constructor(
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    public oAuthService: OAuthService,
    public router: Router,
    public activatedRoute: ActivatedRoute,
    public fileUploadService: FileUploadService,
    private location: Location,
    private translateService: TranslateService,
    private cardioService: CardiologistAccountsService,
    private patientWizardService: PatientWizardService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  public handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
  }

  openAssignCaseDialog() {
    this.cardioWizard.openAssignCaseDialog();
  }

  public ngOnInit() {
    this.roleCode = localStorage.getItem('roleCode');
    this.patientCaseId = this.activatedRoute.snapshot.params.patientcaseId;
    this.patientWizardService.getCase(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.caseObject = response.body;
          if (this.caseObject.statusID >= this.caseStatuses.DE_SIGN_PENDING) {
            this.getECGReportData();
          }
          if (this.caseObject.statusID <= this.caseStatuses.DE_SIGN_PENDING) {
            // Case is assigned and check for the signature
            if (this.roleCode === 'Cardiologist') {
              this.getSignatureData();
            }
          }
        }
      },
      error => {}
    );

    this.fileUploadService.getNoteTypes().subscribe(results => {
      this.noteTypes = results;
      this.noteTypes.forEach(element => {
        this.notesTypeArray['' + element.id] = element.description;
      });
    });

    this.createPhoneVerificationForm();
  }

  getSignatureData() {
    this.cardioService.getSignatureData().subscribe(
      response => {
        if (response.status === 200) {
          this.signatureExists = true;
        }
      },
      error => {
        if (error.status === 404) {
          this.displaySignatureRequiredMessage();
        }
      }
    );
  }

  getECGReportData() {
    this.patientWizardService.getECGReportData(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.ecgReportData = response.body;
          this.remarks = this.ecgReportData.notes;
          this.selectedNotes = this.ecgReportData.notesTypeID;
        }
      },
      error => {}
    );
  }

  displaySignatureRequiredMessage(): void {
    this.translateService.get('signature_for_file_upload_required').subscribe(text => {
      this.toastr.info(text, '', {
        closeButton: true,
        positionClass: 'toast-top-center',
        timeOut: 20000,
        extendedTimeOut: 20000,
        toastClass: 'toastr-custom-width nambaya-blue-bg ngx-toastr'
      });
    });
  }

  goBack(): void {
    this.location.back();
  }

  goToPatients(): void {
    this.router.navigate(['cardiologist/patients/list']);
  }

  public generateECGUploadVerificationOtp() {
    if (!this.signatureExists) {
      this.displaySignatureRequiredMessage();
      return;
    }
    this.isLoading = true;
    this.fileUploadService.generateECGUploadVerificationOtp(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.otpToken = response.body.token;
          this.openOTPDialog();
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

  public openOTPDialog() {
    this.uploadEcgDialog.open();
  }

  uploadFileToActivity() {
    if (this.uploadEcgForm.invalid) {
      this.validateAllFormFields(this.uploadEcgForm);
    } else {
      if (this.remarks.length > 500) {
        this.toastr.error('remarks_limit');
        return false;
      }

      this.uploading = true;
      this.uploadEcgDialog.close();
      this.disableButtons = true;

      this.cardioECGFileContext.patientCaseId = Number(this.patientCaseId);
      this.cardioECGFileContext.notes = this.remarks;
      this.cardioECGFileContext.notesTypeId = Number(this.noteTypeSelect.value);
      this.cardioECGFileContext.email = this.oAuthService.userData.email;
      this.cardioECGFileContext.token = this.otpToken;
      this.cardioECGFileContext.otp = this.uploadEcgForm.value.otp.trim();

      let ecgUploadRequest: any;
      if (this.fileToUpload) {
        ecgUploadRequest = this.fileUploadService.postEcgFile(this.fileToUpload, this.cardioECGFileContext);
      } else {
        ecgUploadRequest = this.fileUploadService.signECGReport(this.cardioECGFileContext);
      }
      ecgUploadRequest.subscribe(
        data => {
          if (data && data.message === 100) {
            for (let i = this.progress; i <= 100; i++) {
              this.progress = this.progress + 1;
            }
            this.uploading = false;
            this.saving = true;
          }
          if (data == null) {
            this.progress = 100;
            this.translateService.get('detailed_report_successfully_signed').subscribe(text => {
              this.toastr.success(text);
              this.caseObject.statusID = this.caseStatuses.DE_COMPLETED;
              this.getECGReportData();
              this.saving = false;
              this.disableButtons = false;
            });
          } else {
            this.progress = data.message;
          }
        },
        error => {
          this.isLoading = false;
          this.uploading = false;
          this.disableButtons = false;
          this.saving = false;
          this.uploadEcgDialog.open();
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

  uploadUnsignedFile() {
    if (this.remarks.length > 500) {
      this.toastr.error('remarks_limit');
      return false;
    }

    this.uploading = true;
    this.uploadEcgDialog.close();
    this.disableButtons = true;

    this.cardioECGFileContext.patientCaseId = Number(this.patientCaseId);
    this.cardioECGFileContext.notes = this.remarks;
    this.cardioECGFileContext.notesTypeId = Number(this.noteTypeSelect.value);

    let ecgUploadRequest: any;
    if (this.fileToUpload) {
      ecgUploadRequest = this.fileUploadService.postEcgFile(this.fileToUpload, this.cardioECGFileContext);
    } else {
      ecgUploadRequest = this.fileUploadService.updateECGReportData(this.cardioECGFileContext);
    }

    ecgUploadRequest.subscribe(
      data => {
        if (data && data.message === 100) {
          for (let i = this.progress; i <= 100; i++) {
            this.progress = this.progress + 1;
          }
          this.uploading = false;
          this.saving = true;
        }
        if (data == null) {
          this.progress = 100;
          let successMsg = 'file uploaded successfully';
          if (!this.fileToUpload) {
            successMsg = 'data_saved_successfully';
          }
          this.translateService.get(successMsg).subscribe(text => {
            this.toastr.success(text);
            this.caseObject.statusID = this.caseStatuses.DE_SIGN_PENDING;
            this.getECGReportData();
            this.saving = false;
            this.disableButtons = false;
          });
        } else {
          this.progress = data.message;
        }
      },
      error => {
        this.isLoading = false;
        this.uploading = false;
        this.disableButtons = false;
        this.saving = false;

        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
  }

  public downloadFile() {
    this.isDownloadingFile = true;
    this.fileUploadService.downloadEcgFile(this.patientCaseId).subscribe(
      (response: any) => {
        const headers = response.headers;
        const contentDisposition = headers.get('Content-Disposition').replace(/\s/g, '');
        // tslint:disable-next-line:max-line-length
        const filename = contentDisposition
          .split(';')[1]
          .split('filename')[1]
          .split('=')[1]
          .trim()
          .replace(/['"]+/g, '');
        const dataType = response.body.type;
        const binaryData = [];
        binaryData.push(response.body);
        const downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));
        downloadLink.setAttribute('download', filename);
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.isDownloadingFile = false;
      },
      error => {
        this.translateService.get(error.statusText).subscribe(text => {
          this.toastr.error(text);
        });

        this.isDownloadingFile = false;
      }
    );
  }

  public ngOnDestroy() {}

  public ngAfterViewInit() {}

  public cancelUpload() {
    this.fileToUpload = null;
    this.remarks = '';
  }

  public dropped(files: NgxFileDropEntry[]) {
    this.files = files;
    for (const droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if (file.type.toLowerCase() === 'application/pdf') {
            this.fileToUpload = file;
          } else {
            this.translateService.get('only_pdf_allowed').subscribe(text => {
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
  public fileOver(event) {}

  // tslint:disable-next-line:typedef
  public fileLeave(event) {}

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createPhoneVerificationForm() {
    this.uploadEcgForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }
}
