import { error } from 'util';
import { ActivatedRoute, Router } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { OAuthService } from '@app/shared/OAuth.Service';
import { NgxFileDropEntry, FileSystemFileEntry } from 'ngx-file-drop';
import { ToastrService } from 'ngx-toastr';
import { FileUploadService } from '@app/shared/fileUpload.service';
import { PatientsService } from '@app/pharmacist/Models/patients.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

@Component({
  providers: [FileUploadService],
  selector: 'app-import-patient',
  templateUrl: './import-patient.component.html',
  styleUrls: ['./import-patient.component.scss']
})
export class ImportPatientComponent implements OnInit, AfterViewInit, OnDestroy {
  importPatientForm!: FormGroup;
  public fileToUpload: File = null;
  public uploading = false;
  public progress = 0;
  public pharmacyId: number;
  public pharmacyIdPrefix: string;
  public isPharmacist = false;

  public files: NgxFileDropEntry[] = [];
  wizardDataContext = {
    currentStep: 521,
    step521URL: './',
    step522URL: './',
    step523URL: './',
    step524URL: './',
    step525URL: './',
    step526URL: './',
    step527URL: './',
    prevStepURL: '',
    patientId: '',
    pharmacyPatientId: '',
    patientName: ''
  };

  constructor(
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    public oAuthService: OAuthService,
    public router: Router,
    public activatedRoute: ActivatedRoute,
    public patientsService: PatientsService,
    private location: Location,
    private translateService: TranslateService
  ) {}
  public handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
  }
  goBack(): void {
    this.location.back();
  }
  uploadPatientFile() {
    if (this.importPatientForm.controls.PharmacyPatientID.value === '') {
      this.importPatientForm.controls.PharmacyPatientID.markAsTouched({ onlySelf: true });
      return;
    }
    this.uploading = true;
    const pharmacyPatientID = this.pharmacyIdPrefix + '-' + this.importPatientForm.controls.PharmacyPatientID.value;
    this.patientsService.importPatient(this.fileToUpload, pharmacyPatientID).subscribe(
      data => {
        this.progress = data.message;
        if (data.id > 0) {
          this.progress = 100;
          this.translateService.get('Patient Successfully Imported').subscribe(text => {
            this.toastr.success(text);
          });
          if (typeof data.id !== 'undefined' && data.id !== null) {
            if (this.isPharmacist) {
              sessionStorage.setItem('wiz_patientCaseID', data.caseID);
              sessionStorage.setItem('wiz_patientID', data.id);
              this.router.navigate(['/pharmacist/patient/edit/' + data.id + '/' + data.caseID]);
            } else {
              this.goBack();
            }
          }
          this.progress = data.message;
        }
      },
      error => {
        this.uploading = false;
        this.translateService.get(error.error).subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
  }

  public getPharmacyIdPrefix() {
    this.patientsService.getPharmacyIdPrefix().subscribe(result => {
      this.pharmacyIdPrefix = result;
    });
  }

  public ngOnInit() {
    this.getPharmacyIdPrefix();
    this.createImportPatientForm();
    this.oAuthService.checkIfAuthenticated();
  }

  public ngOnDestroy() {}

  public ngAfterViewInit() {
    const applicationId = localStorage.getItem('applicationId');
    if (applicationId === 'pharmacist' || applicationId === 'Pharmacy') {
      this.isPharmacist = true;
    }
  }

  public cancelUpload() {
    this.fileToUpload = null;
  }
  public dropped(files: NgxFileDropEntry[]) {
    this.files = files;
    for (const droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if (file.type.toLowerCase() === 'text/xml' || file.type.toLowerCase() === 'application/x-zip-compressed') {
            this.fileToUpload = file;
          } else {
            this.translateService.get('Only XML file is allowed').subscribe(text => {
              this.toastr.error(text);
            });
          }
        });
      }
    }
  }

  // tslint:disable-next-line:typedef
  public fileOver(event) {}

  // tslint:disable-next-line:typedef
  public fileLeave(event) {}

  private createImportPatientForm() {
    this.importPatientForm = this.formBuilder.group({
      PharmacyPatientID: ['', Validators.required]
    });
  }
}
