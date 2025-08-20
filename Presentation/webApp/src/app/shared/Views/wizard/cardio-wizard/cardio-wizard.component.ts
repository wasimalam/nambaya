import { Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild } from '@angular/core';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { IgxDialogComponent } from 'igniteui-angular';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import {
  CardiologistAccountsService,
  CardioWizardDataContext
} from '@app/cardiologist/Models/cardiologist-accounts.service';
import { CaseStatuses } from '@app/objects/configurations';

@Component({
  selector: 'app-cardio-wizard',
  templateUrl: './cardio-wizard.component.html',
  styleUrls: ['./cardio-wizard.component.scss']
})
export class CardioWizardComponent implements OnInit, OnChanges {
  message: string = 'From Child';
  isLoadinginWizard = false;
  caseStatuses = CaseStatuses;

  cardioWizardDataContext: CardioWizardDataContext = {
    currentStep: 1,
    caseId: 0,
    caseStatusId: 0,
    ecgStepURL: './',
    medicationStepURL: '',
    patientId: 0,
    patientName: '',
    pharmacyPatientId: '',
    qeStepURL: '',
    navigatorCaseId: ''
  };

  @Input() caseObject: any;
  @Input() parentData: any;

  @Output() messageEvent = new EventEmitter<any>();
  @ViewChild('assignCaseConfirmDialog', { static: true }) public assignCaseConfirmDialog: IgxDialogComponent;
  private urlPrefix: string;

  constructor(
    private patientWizardService: PatientWizardService,
    private translateService: TranslateService,
    private toastr: ToastrService,
    private router: Router,
    private remoteService: PatientsListService,
    private cardioService: CardiologistAccountsService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  /*sendMessage() {
    this.messageEvent.emit(this.caseObject);
  }*/

  ngOnInit(): void {}

  ngOnChanges() {
    if (this.caseObject) {
      this.cardioWizardDataContext.patientId = this.caseObject.id;
      this.cardioWizardDataContext.navigatorCaseId = this.caseObject.caseIDString;
      this.cardioWizardDataContext.pharmacyPatientId = this.caseObject.pharmacyPatientID;
      this.cardioWizardDataContext.patientName = this.caseObject.firstName + ' ' + this.caseObject.lastName;
      this.cardioWizardDataContext.caseStatusId = this.caseObject.statusID;

      this.cardioWizardDataContext.currentStep = this.caseObject.stepID;
      this.cardioWizardDataContext.medicationStepURL =
        this.urlPrefix + '/patient/additional-medication/' + this.caseObject.caseID + '/' + this.caseObject.id;
      this.cardioWizardDataContext.qeStepURL = this.urlPrefix + '/patient/quick-evaluation/' + this.caseObject.caseID;
      this.cardioWizardDataContext.ecgStepURL = this.urlPrefix + '/ecgupload/' + this.caseObject.caseID;
    }
  }

  openAssignCaseDialog() {
    this.assignCaseConfirmDialog.open();
  }

  assignCase() {
    this.assignCaseConfirmDialog.close();
    this.isLoadinginWizard = true;
    this.remoteService.assignCaseToCardiologist(this.caseObject.caseID).subscribe(
      response => {
        if (response.status === 200) {
          this.translateService.get('case_successfully_assigned').subscribe(text => {
            this.toastr.success(text);
          });
          this.caseObject.statusID = CaseStatuses.DE_LOCKED;
          this.isLoadinginWizard = false;
          this.messageEvent.emit(this.caseObject);
          this.getSignatureData();
        }
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoadinginWizard = false;
      }
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

  getSignatureData() {
    this.cardioService.getSignatureData().subscribe(
      response => {},
      error => {
        if (error.status === 404) {
          this.displaySignatureRequiredMessage();
        }
      }
    );
  }
}
