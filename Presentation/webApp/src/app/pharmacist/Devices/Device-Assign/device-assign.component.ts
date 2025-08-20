import { DeviceAssignModel } from '../deviceassign.model';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from './../device.service';
import { Component, OnInit, OnDestroy, AfterViewInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { IComboSelectionChangeEventArgs, IgxComboComponent, IgxDialogComponent } from 'igniteui-angular';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { forkJoin } from 'rxjs';
import { PatientWizardSteps } from '@app/objects/configurations';
import * as moment from 'moment';

@Component({
  selector: 'app-device-assign',
  templateUrl: './device-assign.component.html'
})
export class DeviceAssignComponent implements OnInit, OnDestroy, AfterViewInit {
  deviceAssignForm: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = true;
  patientId: number;
  pharmacyId: number;
  patientCaseId: number;
  deviceId: any;
  dateselected: Date;
  filterObject = [];
  public devices: any[];
  public deviceData: any;
  public availableDevices: any[];
  public deviceAssignmentData: any;
  deviceSelected: any;
  deviceName: any;
  urlPrefix = '';
  enableSkipButton: boolean;
  isPharmacist = false;
  caseFinalizedByPharmacist: boolean;
  deviceAssign: DeviceAssignModel = new DeviceAssignModel();
  // @ts-ignore
  @ViewChild('withValueKey', { read: IgxComboComponent }) public comboValueKey: IgxComboComponent;
  @ViewChild('reassignConfirmDialog', { static: true }) public reassignConfirmDialog: IgxDialogComponent;

  public caseObject: any = null;
  public parentData: any = {
    stepNumber: 0
  };

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private deviceService: DeviceService,
    private patientWizardService: PatientWizardService,
    private location: Location,
    private router: Router,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.pharmacyId = this.activatedRoute.snapshot.params.pharmacyId;
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;
    this.patientId = Number(sessionStorage.getItem('wiz_patientID'));
  }

  ngOnInit() {
    this.parentData.stepNumber = PatientWizardSteps.DEVICE_ASSIGNMENT;
    this.isLoading = true;
    this.createdeviceAssignForm();
    this.filterObject.push({
      Property: 'StatusID',
      Operation: 'Equal',
      Value: '451',
      Description: ''
    });
    this.enableSkipButton = false;
    const caseRequest = this.patientWizardService.getCase(this.patientCaseId);
    const availableDevicesofOnePharmacyRequest = this.deviceService.getAvailableDevicesofOnePharmacy(0, 0);

    forkJoin([caseRequest, availableDevicesofOnePharmacyRequest]).subscribe(
      results => {
        /*Start handling case data*/
        // @ts-ignore
        this.caseObject = results[0].body;
        if (this.caseObject.stepID === 527) {
          this.caseFinalizedByPharmacist = true;
          this.deviceAssignForm.disable();
        }
        /*End handling case data*/

        // @ts-ignore
        this.availableDevices = results[1].data;

        this.deviceService.getDeviceAssignment(this.patientCaseId).subscribe(
          response => {
            this.deviceAssignmentData = response.body;
            this.deviceName = this.availableDevices.filter(
              item => item.id === this.deviceAssignmentData.deviceID
            )[0].serialNumber;

            this.dateselected = this.deviceAssignmentData.assignmentDate;
            this.deviceAssignForm.patchValue({
              assignedDevice: this.deviceName
            });
            this.isLoading = false;
          },
          error => {
            this.deviceAssignmentData = null;
            this.isLoading = false;
          }
        );

        if (this.caseObject.statusID === 652 || this.caseObject.statusID >= 654) {
          this.enableSkipButton = true;
        }
      },
      error => {
        this.isLoading = false;
      }
    );

    this.deviceService.getAllDevicesofOnePharmacy(0, 0, this.filterObject);
    this.deviceService.remoteData.subscribe(data => {
      this.devices = data;
    });
  }
  public dateFormatter = (date: Date) => {
    if (date) {
      const dateFormat = localStorage.getItem('dateFormat');
      const timeZone = localStorage.getItem('timeZone');
      const timeFormat = localStorage.getItem('timeFormat');

      return `${moment
        .utc(date)
        .tz(timeZone)
        .format(dateFormat + ' ' + timeFormat)}`;
    } else {
      return ``;
    }
  };
  ngOnDestroy() {}

  ngAfterViewInit() {
    const applicationId = localStorage.getItem('applicationId');

    if (applicationId === 'pharmacist' || applicationId === 'Pharmacy') {
      this.isPharmacist = true;
    }
  }

  getAllDevicesofOnePharmacy() {
    this.deviceService.getAllDevicesofOnePharmacy(0, 0, this.filterObject);
  }

  openReassignDeviceDialog() {
    this.reassignConfirmDialog.open();
  }

  unAssignDevice() {
    this.reassignConfirmDialog.close();
    this.isLoading = true;
    this.deviceAssign.deviceId = this.caseObject.deviceID;
    this.deviceAssign.PatientCaseID = Number(this.patientCaseId);
    this.deviceAssign.IsAssigned = false;
    this.deviceAssign.DeviceStatusID = 451;
    this.deviceService.assign(this.deviceAssign).subscribe(
      response => {
        if (response.status === 200) {
          this.caseObject.statusID = 653; // 653 => Device returned Status
          this.enableSkipButton = false;
          this.translateService.get('Device UnAssigned Successfully').subscribe(text => {
            this.toastr.success(text);
          });
          this.getAllDevicesofOnePharmacy();
        }
        this.isLoading = false;
      },
      error => {
        if (error.status === 400 && error.error === 'INVALID_DEVICE_ASSIGNMENT_STATUS') {
        }
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoading = false;
      }
    );
  }

  public singleSelection(event: IComboSelectionChangeEventArgs) {
    if (event.added.length) {
      event.newSelection = event.added;
      this.deviceSelected = event.added[0];
      this.comboValueKey.close();
    }
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  assignDevice() {
    if (this.caseObject && (this.caseObject.statusID === 652 || this.caseObject.statusID >= 654)) {
      this.skipDeviceAssignmentStep(this.deviceAssignmentData.deviceID);
      return;
    }

    if (this.deviceAssignForm.invalid) {
      this.validateAllFormFields(this.deviceAssignForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.deviceAssign.deviceId = Number(this.deviceSelected);
      this.deviceAssign.PatientCaseID = Number(this.patientCaseId);
      this.deviceAssign.AssignmentDate = this.dateselected;
      this.deviceAssign.IsAssigned = true;
      this.deviceAssign.DeviceStatusID = 452;
      this.deviceService.assign(this.deviceAssign).subscribe(
        response => {
          if (response.status === 200) {
            this.deviceData = response.body;
            this.translateService.get('Device Assigned Successfully').subscribe(text => {
              this.toastr.success(text);
              this.deviceAssignForm.reset();
              if (this.isPharmacist) {
                this.skipDeviceAssignmentStep(this.deviceData.id);
              } else {
                this.goBack();
              }
            });
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
  }

  public skipDeviceAssignmentStep(deviceId: number) {
    if (this.caseFinalizedByPharmacist) {
      this.router.navigate([this.urlPrefix + '/dashboard']);
    } else {
      this.isLoading = true;
      this.patientWizardService.updateCaseStep(this.patientCaseId, 525).subscribe(result => {
        this.isLoading = false;
        this.router.navigate([this.urlPrefix + '/dashboard']);
      });
    }
  }

  goBack(): void {
    this.location.back();
  }

  goToPatients(): void {
    this.router.navigate(['pharmacist/patients']);
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createdeviceAssignForm() {
    this.deviceAssignForm = this.formBuilder.group({
      device: ['', Validators.required],
      assignedDevice: [''],
      measurementStart: ['', Validators.required]
    });
  }
}
