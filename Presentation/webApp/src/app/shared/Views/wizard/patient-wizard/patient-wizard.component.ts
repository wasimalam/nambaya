import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { PatientWizardService, WizardDataContext } from '@app/shared/services/patient-wizard.service';
import { IgxDialogComponent } from 'igniteui-angular';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { CaseStatuses, PatientWizardSteps } from '@app/objects/configurations';
import { OAuthService } from '@app/shared/OAuth.Service';

@Component({
  selector: 'app-patient-wizard',
  templateUrl: './patient-wizard.component.html',
  styleUrls: ['./patient-wizard.component.scss']
})
export class PatientWizardComponent implements OnInit, OnChanges {
  message: string = 'From Child';
  isLoadinginWizard = false;
  caseStatuses = CaseStatuses;
  patientWizardSteps = PatientWizardSteps;

  wizardDataContext: WizardDataContext = {
    currentStep: 522,
    step522URL: './',
    step523URL: './',
    step525URL: './',
    step526URL: './',
    step527URL: './',
    editPatientStepURL: './',
    medicationStepURL: './',
    deviceStepURL: './',
    edfStepURL: './',
    qeStepURL: './',
    prevStepURL: './',
    skipStepURL: '',
    patientId: 0,
    pharmacyPatientId: '',
    patientName: '',
    caseStatusId: 0,
    caseId: 0
  };

  @Input() caseObject: any;
  @Input() parentData: any;

  @Output() messageEvent = new EventEmitter<string>();
  @ViewChild('deactivateConfirmDialog', { static: true }) public deactivateConfirmDialog: IgxDialogComponent;
  private urlPrefix: string;

  constructor(
    private patientWizardService: PatientWizardService,
    private translateService: TranslateService,
    private toastr: ToastrService,
    private router: Router,
    private oAuthService: OAuthService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  sendMessage() {
    this.messageEvent.emit(this.message);
  }

  ngOnInit(): void {}

  ngOnChanges() {
    if (this.caseObject) {
      this.wizardDataContext.patientId = this.caseObject.id;
      this.wizardDataContext.pharmacyPatientId = this.caseObject.pharmacyPatientID;
      this.wizardDataContext.patientName = this.caseObject.firstName + ' ' + this.caseObject.lastName;
      this.wizardDataContext.caseStatusId = this.caseObject.statusID;
      this.wizardDataContext.caseId = this.caseObject.caseID;

      this.wizardDataContext.currentStep = this.caseObject.stepID;
      this.wizardDataContext.editPatientStepURL =
        this.urlPrefix + '/patient/edit/' + this.caseObject.id + '/' + this.caseObject.caseID;
      this.wizardDataContext.medicationStepURL =
        this.urlPrefix + '/patient/additional-medication/' + this.caseObject.caseID + '/' + this.caseObject.id;
      this.wizardDataContext.deviceStepURL =
        this.urlPrefix + '/assigndevice/' + this.caseObject.caseID + '/' + this.oAuthService.userData.pharmacyid;
      if (this.caseObject.statusID >= CaseStatuses.DEVICE_ALLOCATED) {
        this.wizardDataContext.edfStepURL = this.urlPrefix + '/edfupload/' + this.caseObject.caseID;
      }
      if (this.caseObject.statusID >= CaseStatuses.DEVICE_RETURNED) {
        this.wizardDataContext.qeStepURL = this.urlPrefix + '/patient/quick-evaluation/' + this.caseObject.caseID;
      }
    }
  }

  openDeactivatePatientDialog() {
    this.deactivateConfirmDialog.open();
  }

  deActivatePatient() {
    this.isLoadinginWizard = true;
    this.patientWizardService.deActivatePatient(this.wizardDataContext.patientId).subscribe(
      response => {
        this.translateService.get('patient_deactivate_success').subscribe(text => {
          this.toastr.success(text);
          this.isLoadinginWizard = false;
          this.router.navigate(['/pharmacist/patients']);
        });
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoadinginWizard = false;
      }
    );
  }
}
