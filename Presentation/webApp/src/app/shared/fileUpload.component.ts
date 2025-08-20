import { FileUploadService } from './fileUpload.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { NgxFileDropEntry, FileSystemFileEntry } from 'ngx-file-drop';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { DeviceAssignModel } from '@app/pharmacist/Devices/deviceassign.model';
import { DeviceService } from '@app/pharmacist/Devices/device.service';
import { PatientWizardComponent } from '@app/shared/Views/wizard/patient-wizard/patient-wizard.component';
import { PatientWizardSteps } from '@app/objects/configurations';

@Component({
  providers: [FileUploadService],
  selector: 'app-fileUpload',
  templateUrl: './fileUpload.component.html',
  styleUrls: ['./fileUpload.component.css']
})
export class FileUploadComponent implements OnInit, AfterViewInit, OnDestroy {
  public patientId: number;
  public pharmacyId: number;
  public patientCaseId: number;
  public fileToUpload: File = null;
  public uploading: boolean = false;
  public disableButtons: boolean = false;
  public saving: boolean = false;
  public progress: number = 0;
  caseFinalizedByPharmacist: boolean;
  isEDFExists: boolean;
  deviceAssign: DeviceAssignModel = new DeviceAssignModel();
  isPharmacist = false;
  existingFileData: any;
  isLoading = false;
  urlPrefix = '';
  public parentData: any = {
    stepNumber: 0
  };
  public caseObject: any = null;

  // @ts-ignore
  @ViewChild(PatientWizardComponent) patientWizard: PatientWizardComponent;

  constructor(
    private toastr: ToastrService,
    private remoteService: PatientsListService,
    public oAuthService: OAuthService,
    public patientWizardService: PatientWizardService,
    public router: Router,
    public activatedRoute: ActivatedRoute,
    public fileUploadService: FileUploadService,
    private location: Location,
    private translateService: TranslateService,
    private deviceService: DeviceService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  public handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
  }

  goBack(): void {
    this.location.back();
  }

  goToPatients(): void {
    this.router.navigate(['pharmacist/patients']);
  }

  public cancelUpload() {
    this.fileToUpload = null;
  }

  unAssignDevice() {
    if (this.caseObject && this.caseObject.deviceID > 0) {
      this.uploading = true;
      this.deviceAssign.deviceId = Number(this.caseObject.deviceID);
      this.deviceAssign.PatientCaseID = Number(this.patientCaseId);
      this.deviceAssign.IsAssigned = false;
      this.deviceAssign.DeviceStatusID = 451;
      this.deviceService.assign(this.deviceAssign).subscribe(
        response => {
          if (response.status === 200) {
            this.uploadFileToActivity();
          }
        },
        error => {
          if (error.status === 400 && error.error === 'INVALID_DEVICE_ASSIGNMENT_STATUS') {
            this.uploadFileToActivity();
            return;
          }
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    } else {
      this.uploadFileToActivity();
    }
  }

  uploadFileToActivity() {
    this.uploading = true;
    this.disableButtons = true;
    this.fileUploadService.postFile(this.fileToUpload, this.patientCaseId).subscribe(
      data => {
        if (data && data.message === 100) {
          for (var i = this.progress; i <= 100; i++) {
            this.progress = this.progress + 1;
          }
          this.uploading = false;
          this.saving = true;
        }

        if (data == null) {
          this.progress = 100;
          this.translateService.get('file uploaded successfully').subscribe(text => {
            this.toastr.success(text);
          });

          if (this.isPharmacist) {
            this.skipEDFUploadStep();
          } else {
            this.goBack();
          }
        } else {
          this.progress = data.message;
        }
      },
      error => {
        if (error.error === 'FAILED_TO_UPLOAD') {
          this.translateService.get('invalid_edf_file').subscribe(text => {
            this.toastr.error(text);
          });
        } else {
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
        this.uploading = false;
        this.disableButtons = false;
        this.saving = false;
      }
    );
  }

  public ngOnInit() {
    this.parentData.stepNumber = PatientWizardSteps.UPLOAD_EDF;
    this.isLoading = true;
    this.oAuthService.checkIfAuthenticated();
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;
    this.patientId = Number(sessionStorage.getItem('wiz_patientID'));
    this.pharmacyId = Number(localStorage.getItem('pharmacyId'));
    this.patientWizardService.getCase(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.caseObject = response.body;
          if (this.caseObject.stepID === 527) {
            this.caseFinalizedByPharmacist = true;
          }
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
      }
    );

    this.fileUploadService.getPatientEDFFile(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.isEDFExists = true;
          this.existingFileData = response.body;
        }
      },
      error => {
        this.isEDFExists = false;
      }
    );
  }

  public ngOnDestroy() {}

  public ngAfterViewInit() {
    const applicationId = localStorage.getItem('applicationId');

    if (applicationId === 'pharmacist' || applicationId === 'Pharmacy') {
      this.isPharmacist = true;
    }
  }

  public files: NgxFileDropEntry[] = [];

  public dropped(files: NgxFileDropEntry[]) {
    this.files = files;
    for (const droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if (file.size === 0) {
            this.translateService.get('invalid_edf_file').subscribe(text => {
              this.toastr.error(text);
            });
            return;
          }

          const fileName = file.name;
          const fileExtension: string = file.name.split('.').pop();

          if (fileExtension.toLowerCase() === 'edf') {
            this.fileToUpload = file;
          } else {
            this.translateService.get('Only EDF file is allowed').subscribe(text => {
              this.toastr.error(text);
            });
          }
        });
      }
    }
  }

  openDeactivatePatientDialog() {
    this.patientWizard.openDeactivatePatientDialog();
  }

  public skipEDFUploadStep() {
    if (this.caseFinalizedByPharmacist) {
      this.router.navigate([this.urlPrefix + '/patient/quick-evaluation/' + this.patientCaseId]);
    } else {
      this.patientWizardService.updateCaseStep(this.patientCaseId, 526).subscribe(result => {
        this.router.navigate([this.urlPrefix + '/patient/quick-evaluation/' + this.patientCaseId]);
      });
    }
  }

  public fileOver(event) {}

  public fileLeave(event) {}
}
