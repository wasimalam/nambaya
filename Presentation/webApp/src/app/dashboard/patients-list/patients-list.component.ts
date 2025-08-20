import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation, ElementRef } from '@angular/core';
import {
  IgxGridComponent,
  ConnectedPositioningStrategy,
  AbsoluteScrollStrategy,
  IgxPaginatorComponent,
  IgxDialogComponent
} from 'igniteui-angular';

import { BehaviorSubject, Observable } from 'rxjs';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { IgxToggleDirective } from 'igniteui-angular';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { CookieService } from 'ngx-cookie-service';
import { DeactivatePatientContext, PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { CaseStatuses } from '@app/objects/configurations';

@Component({
  encapsulation: ViewEncapsulation.None,
  providers: [PatientsListService],
  selector: 'app-patients-list',
  templateUrl: './patients-list.component.html',
  styleUrls: ['./patients-list.component.scss']
})
export class PatientsListComponent implements OnInit, AfterViewInit, OnDestroy {
  public get perPage(): number {
    if (this.cookieService.check(this.cookieKeyPerPage)) {
      return Number(this.cookieService.get(this.cookieKeyPerPage));
    }

    return this._perPage;
  }

  public set perPage(val: number) {
    if (
      this._perPage === val ||
      // tslint:disable-next-line:max-line-length
      (this.cookieService.check(this.cookieKeyPerPage) && Number(this.cookieService.get(this.cookieKeyPerPage)) === val)
    ) {
      return;
    }
    if (this._perPage === val) {
      return;
    }
    if (
      !this.cookieService.check(this.cookieKeyPerPage) ||
      (this.cookieService.check(this.cookieKeyPerPage) && val !== Number(this.cookieService.get(this.cookieKeyPerPage)))
    ) {
      this.cookieService.set(this.cookieKeyPerPage, val.toString(), 3600, '/', '', false, 'Lax');
    }
    this._perPage = val;
    this.paginate(0);
  }
  public page = 0;
  public totalCount: any;
  public pages = [];
  public data: Observable<any[]>;
  public dataLength: Observable<number> = new BehaviorSubject(0);
  public selectOptions = [5, 10, 15, 25, 50];
  public patientIddd = 0; // TODO: Mateen please change this to PatientCaseId [Irfan]
  public patientId = 0;
  public pharmacyId = 0;
  public pharmacyPatientId = 0;
  patientSearchForm!: FormGroup;
  deactivatePatientForm!: FormGroup;
  public filterTags = [];
  genders: any[];
  order: string;
  columnName: string;
  caseStatuses: Object;
  public caseId: number;
  public patientStatusId = 0;
  public filterParam: string;
  public filterObject = [];
  public preSetFilter = { Property: 'StatusID', Operation: '', Value: 0, Description: 'Status', isHidden: true };
  public preSetFilter1 = { Property: 'StatusID', Operation: '', Value: 0, Description: 'Status', isHidden: true };
  isToggled = false;
  urlPrefix = '';
  cookieKeyPerPage: string;
  orderBy: string;
  isLoading = false;
  otpToken = '';

  public deactivatePatientContext: DeactivatePatientContext = {
    patientId: 0,
    otp: '',
    token: '',
    email: ''
  };
  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('grid1', { static: true }) public grid1: IgxGridComponent;
  @ViewChild(IgxToggleDirective, { static: true }) public igxTogglettt: IgxToggleDirective;
  @ViewChild('button', { static: true }) public igxButton: ElementRef;
  // @ts-ignore
  @ViewChild('firstName') firstName: ElementRef;
  // @ts-ignore
  @ViewChild('lastName') lastName: ElementRef;
  // @ts-ignore
  @ViewChild('insuranceNumber') insuranceNumber: ElementRef;
  // @ts-ignore
  @ViewChild('PharmacyPatientID') PharmacyPatientID: ElementRef;
  // @ts-ignore
  @ViewChild('genderID') genderID: ElementRef;
  @ViewChild('deactivateConfirmDialog', { static: true }) public deactivateConfirmDialog: IgxDialogComponent;
  @ViewChild('deactivateOTPDialog', { static: true }) public deactivateOTPDialog: IgxDialogComponent;

  public _positionSettings = {
    horizontalStartPoint: 0,
    verticalStartPoint: -11
  };

  public _overlaySettings = {
    closeOnOutsideClick: true,
    modal: false,
    positionStrategy: new ConnectedPositioningStrategy(this._positionSettings),
    scrollStrategy: new AbsoluteScrollStrategy()
  };

  toggleBtn = false;
  deviceId: any;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private remoteService: PatientsListService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private cookieService: CookieService,
    private patientWizardService: PatientWizardService,
    private translateService: TranslateService,
    private toastr: ToastrService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'pharmaPatientsList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
    this.caseStatuses = {
      '651': 'Case Started',
      '652': 'Device Allocated',
      '653': 'Device Returned',
      '654': 'Quick Evaluation In Queue',
      '655': 'Quick Evaluation Completed',
      '656': 'Quick Evaluation Completed',
      '657': 'E-Sign Pending',
      '658': 'Detail Evaluation Completed',
      '659': 'Report Dispatch Failed',
      '660': 'Report Dispatching',
      '661': 'Report Dispatched'
    };
    this.filterParam = this.activatedRoute.snapshot.params.filter;
    if (this.filterParam) {
      if (this.filterParam === 'casestarted') {
        this.preSetFilter.Value = 651;
        this.preSetFilter.Operation = 'Equal';
        this.filterObject.push(this.preSetFilter);
      } else if (this.filterParam === 'ecgpending') {
        this.preSetFilter.Value = 652;
        this.preSetFilter.Operation = 'GreaterOrEqual';
        this.preSetFilter1.Value = 654;
        this.preSetFilter1.Operation = 'LessOrEqual';
        this.filterObject.push(this.preSetFilter);
        this.filterObject.push(this.preSetFilter1);
      } else if (this.filterParam === 'ecgtransmitted') {
        this.preSetFilter.Value = 655;
        this.preSetFilter.Operation = 'GreaterOrEqual';
        this.preSetFilter1.Value = 656;
        this.preSetFilter1.Operation = 'LessOrEqual';
        this.filterObject.push(this.preSetFilter);
        this.filterObject.push(this.preSetFilter1);
      } else if (this.filterParam === 'ecgaccessed') {
        this.preSetFilter.Value = 657;
        this.preSetFilter.Operation = 'GreaterOrEqual';
        this.filterObject.push(this.preSetFilter);
      }
    }
  }

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  onClickedOutside(e: Event) {
    const idAttr = (event.target as Element).id;
    if (idAttr !== 'chev' && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  public getGenders() {
    this.remoteService.getGendersList().subscribe(data => {
      this.genders = data;
    });
  }

  public ngOnInit() {
    this.createSearchForm();
    this.createOTPVerificationForm();
    this.getGenders();
    this.oAuthService.observableUser.subscribe(user => {
      // tslint:disable-next-line:triple-equals
      if (user != undefined && user.appuserid > 0) {
        this.pharmacyId = user.pharmacyid;
        this.remoteService.getAllPatients(0, this.perPage, this.filterObject);
        this.data = this.remoteService.remoteData.asObservable();
        this.dataLength = this.remoteService.dataLength.asObservable();
        this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
          this.totalCount = data;
          this.grid1.isLoading = false;
        });
      }
    });
  }

  perPageChangeFn(numberOfRecords: number) {
    this.grid1.height = null;
  }

  onToggleClose() {
    this.isToggled = false;
  }

  handleRowClick($event: any) {
    if (!this.isToggled) {
      // tslint:disable-next-line:max-line-length
      this.router.navigate([
        this.urlPrefix + '/patient/edit/' + $event.cell.row.rowData.id + '/' + $event.cell.row.rowData.caseID
      ]);
    }
  }

  public sort(columnName: string) {
    if (this.order === 'ASC') {
      this.order = 'DESC';
    } else {
      this.order = 'ASC';
    }
    this.columnName = columnName;
    this.orderBy = columnName + ' ' + this.order;
    this.applyFilters();
  }

  // tslint:disable-next-line:typedef
  resumePatient(id, index) {
    this.caseId = this.grid1.data[index].caseID;
    this.deviceId = this.grid1.data[index].deviceID;

    if (this.grid1.data[index].statusID <= CaseStatuses.DEVICE_ALLOCATED) {
      this.router.navigate(['./pharmacist/assigndevice/' + this.caseId + '/' + this.pharmacyId]);
    }

    if (this.grid1.data[index].statusID === CaseStatuses.DEVICE_ALLOCATED) {
      this.router.navigate(['./pharmacist/edfupload/' + this.caseId + '/' + this.deviceId]);
    }

    if (this.grid1.data[index].statusID >= CaseStatuses.QE_IN_QUEUE) {
      this.router.navigate(['./pharmacist/patient/quick-evaluation/' + this.caseId]);
    }
  }

  public toggle(id, index) {
    this.isToggled = true;
    this.patientStatusId = this.grid1.data[index].statusID;
    this.deviceId = this.grid1.data[index].deviceID;

    this.patientIddd = id;
    this.patientId = this.grid1.data[index].id;
    this.pharmacyPatientId = this.grid1.data[index].pharmacyPatientID;
    this.igxButton.nativeElement = document.getElementById(id);
    this._overlaySettings.positionStrategy.settings.target = this.igxButton.nativeElement;
    this.caseId = this.grid1.data[index].caseID;

    this.igxTogglettt.toggle(this._overlaySettings);
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }
  goBack(): void {
    this.location.back();
  }
  public ngAfterViewInit() {
    this.grid1.isLoading = true;
  }

  public dateFormatter = (date: Date) => {
    const dateFormat = localStorage.getItem('dateFormat');
    const timeZone = localStorage.getItem('timeZone');
    return `${moment
      .utc(date)
      .tz(timeZone)
      .format(dateFormat)}`;
  };
  public paginate(page: number) {
    this.page = page;
    this.applyFilters();
  }

  removeFilter(property: string) {
    this.patientSearchForm.controls[property].setValue('');
    this.applyFilters(true);
  }

  resetFilters() {
    this.patientSearchForm.reset();
    this.applyFilters(true);
  }

  applyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyFilters();
      return;
    }
    this.grid1.isLoading = true;
    this.grid1.height = 'auto';
    this.filterObject = [];
    if (this.filterParam) {
      if (this.filterParam === 'casestarted') {
        this.filterObject.push(this.preSetFilter);
      } else if (this.filterParam === 'ecgpending') {
        this.filterObject.push(this.preSetFilter);
        this.filterObject.push(this.preSetFilter1);
      } else if (this.filterParam === 'ecgtransmitted') {
        this.filterObject.push(this.preSetFilter);
        this.filterObject.push(this.preSetFilter1);
      } else if (this.filterParam === 'ecgaccessed') {
        this.filterObject.push(this.preSetFilter);
      }
    }
    let operation: string;
    let description: string;
    Object.keys(this.patientSearchForm.controls).forEach(field => {
      const control = this.patientSearchForm.get(field);
      if (this[field].nativeElement === undefined) {
        description = this[field].elementRef.nativeElement.getAttribute('data-description');
        operation = 'Equal';
      } else {
        description = this[field].nativeElement.getAttribute('data-description');
        if (description === 'Pharmacy Patient ID') {
          operation = 'Like';
        } else {
          operation = 'Equal';
        }
      }

      if (control.value) {
        this.filterObject.push({
          Property: field,
          Operation: operation,
          Value: control.value,
          Description: description,
          isHidden: false
        });
      }
    });
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    const _this = this;
    this.remoteService.getAllPatients(skip, top, this.filterObject, this.orderBy);
    _this.filterTags = [];
    this.filterObject.forEach(filter => {
      if (!filter.isHidden) {
        _this.filterTags.push(filter);
      }
    });
  }

  openDeactivatePatientDialog() {
    this.deactivateConfirmDialog.open();
    this.igxTogglettt.toggle();
  }

  public generateECGUploadVerificationOtp() {
    this.isLoading = true;
    this.patientWizardService.generateDeactivatePatientVerificationOtp(this.patientId).subscribe(
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
    this.deactivateOTPDialog.open();
    this.deactivateConfirmDialog.close();
  }

  deActivatePatient() {
    if (this.deactivatePatientForm.invalid) {
      this.validateAllFormFields(this.deactivatePatientForm);
    } else {
      this.isLoading = true;
      this.deactivateOTPDialog.close();
      this.toggleBtn = false;
      this.deactivatePatientContext.patientId = Number(this.patientId);
      this.deactivatePatientContext.email = this.oAuthService.userData.email;
      this.deactivatePatientContext.token = this.otpToken;
      this.deactivatePatientContext.otp = this.deactivatePatientForm.value.otp.trim();

      this.patientWizardService.deActivatePatientWithOTP(this.deactivatePatientContext).subscribe(
        response => {
          this.translateService.get('patient_deactivate_success').subscribe(text => {
            this.deactivateOTPDialog.close();
            this.toastr.success(text);
          });
          this.grid1.isLoading = false;
          this.isLoading = false;
          this.applyFilters();
        },
        error => {
          this.deactivateOTPDialog.open();
          this.grid1.isLoading = false;
          this.isLoading = false;
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

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createSearchForm() {
    this.patientSearchForm = this.formBuilder.group({
      PharmacyPatientID: [''],
      firstName: [''],
      lastName: [''],
      insuranceNumber: [''],
      genderID: ['']
    });
  }

  private createOTPVerificationForm() {
    this.deactivatePatientForm = this.formBuilder.group({
      otp: ['', Validators.required]
    });
  }
}
