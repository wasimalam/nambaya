import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PharmacyService } from '@app/user/Views/pharmacy/pharmacy.service';
import { ToastrService } from 'ngx-toastr';
import { FileUploadService } from '@app/shared/fileUpload.service';
import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
  ViewEncapsulation,
  ElementRef
} from '@angular/core';
import {
  IgxGridComponent,
  VerticalAlignment,
  ConnectedPositioningStrategy,
  AbsoluteScrollStrategy,
  IgxPaginatorComponent
} from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { IgxToggleDirective } from 'igniteui-angular';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Location } from '@angular/common';
import * as moment from 'moment';
import { CookieService } from 'ngx-cookie-service';

@Component({
  encapsulation: ViewEncapsulation.None,
  providers: [PatientsListService],
  selector: 'app-cardiologist-patients-list',
  templateUrl: './cardiologist-patients-list.component.html',
  styleUrls: ['./cardiologist-patients-list.component.scss']
})
export class CardiologistPatientsListComponent implements OnInit, AfterViewInit, OnDestroy {
  public caseStatuses;
  public QEStatuses;
  public genderStatuses;

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
  public patientCaseId = 0;
  public patientId = 0;
  patientSearchForm!: FormGroup;
  filterTags = [];
  genders: any[];
  evaluationStatuses: any[];
  qeStatuses: any[];
  order: string;
  pageTitle: string = 'Patient Cases';
  columnName: string;
  isAssigningCase = false;
  pharmacies: any[];
  public patientStatusId = 0;
  public filterParam: string;
  public filterObject = [];
  cookieKeyPerPage: string;
  orderBy: string;
  isDownloadingFile = false;

  public presetActiveCasesFilter = {
    Property: 'StatusID',
    Operation: 'Equal',
    Value: 655,
    Description: 'Status',
    isHidden: true
  };
  public presetAssignedCasesFilter = {
    Property: 'StatusID',
    Operation: 'Greater',
    Value: 655,
    Description: 'Status',
    isHidden: true
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
  @ViewChild('caseIDString') caseIDString: ElementRef;
  // @ts-ignore
  @ViewChild('PharmacyPatientID') PharmacyPatientID: ElementRef;
  // @ts-ignore
  @ViewChild('genderID') genderID: ElementRef;
  // @ts-ignore
  @ViewChild('statusID') statusID: ElementRef;
  // @ts-ignore
  @ViewChild('quickResultID') quickResultID: ElementRef;

  public _positionSettings = {
    horizontalStartPoint: 0,
    verticalStartPoint: -7.5
  };
  public _overlaySettings = {
    closeOnOutsideClick: true,
    modal: false,
    positionStrategy: new ConnectedPositioningStrategy(this._positionSettings),
    scrollStrategy: new AbsoluteScrollStrategy()
  };

  toggleBtn = false;
  isToggled = false;
  deviceId: any;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private location: Location,
    private toastr: ToastrService,
    private remoteService: PatientsListService,
    private pharmacyService: PharmacyService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private router: Router,
    private fileService: FileUploadService,
    private cookieService: CookieService
  ) {
    this.cookieKeyPerPage = 'cardioPatientList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
    const listType = this.activatedRoute.snapshot.url[3];

    if (listType) {
      this.filterParam = listType.path;
      if (this.filterParam === 'openCases') {
        this.filterObject.push(this.presetActiveCasesFilter);
        this.pageTitle = 'open_cases';
      } else if (this.filterParam === 'assignedCases') {
        this.pageTitle = 'my_cases';
        this.filterObject.push(this.presetAssignedCasesFilter);
      }
    }
    this.caseStatuses = {
      '651': 'Case Started',
      '652': 'Device Allocated',
      '653': 'Device Returned',
      '654': 'Quick Eval In Queue',
      '655': 'Quick Eval Completed',
      '656': 'Detailed Eval Locked',
      '657': 'E-Sign Pending',
      '658': 'Detailed Eval Completed',
      '659': 'Report Dispatch Failed',
      '660': 'Report Dispatching',
      '661': 'Report Dispatched'
    };

    this.QEStatuses = {
      '511': 'Green',
      '512': 'Yellow',
      '513': 'Orange',
      '514': 'Red',
      '515': 'Red Red'
    };

    this.genderStatuses = {
      '401': 'Male',
      '402': 'Female',
      '403': 'Unknown',
      '404': 'Diverse'
    };
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

  onToggleClose() {
    this.isToggled = false;
  }

  handleRowClick($event: any) {
    if (!this.isToggled) {
      this.router.navigate(['/cardiologist/patient/quick-evaluation/' + $event.cell.row.rowData.caseID]);
    }
  }
  public dateFormatter = (date: Date) => {
    if (date) {
      const dateFormat = localStorage.getItem('dateFormat');
      const timeZone = localStorage.getItem('timeZone');

      return `${moment
        .utc(date)
        .tz(timeZone)
        .format(dateFormat)}`;
    } else {
      return ``;
    }
  };

  public getEvaluationStatues() {
    const statuses = [];

    this.remoteService.getcaseStatuses().subscribe(data => {
      if (data.length > 0) {
        data.forEach(status => {
          if (status.id > 655) {
            statuses.push(status);
          }
        });
      }
      this.evaluationStatuses = statuses;
    });
  }

  public getQEStatues() {
    this.remoteService.getQEStatuses().subscribe(data => {
      this.qeStatuses = data;
    });
  }

  public ngOnInit() {
    this.createSearchForm();
    this.getGenders();
    this.getEvaluationStatues();
    this.getQEStatues();

    this.data = this.remoteService.remoteData.asObservable();
    this.dataLength = this.remoteService.dataLength.asObservable();
    this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(
      data => {
        this.totalCount = data;
        this.grid1.isLoading = false;
      },
      error => {
        this.grid1.isLoading = false;
      }
    );
  }

  assignCaseToCardiologist(patientCaseId: number) {
    this.isAssigningCase = true;
    this.remoteService.assignCaseToCardiologist(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          const caseObject = response.body;
          this.isAssigningCase = false;
          this.applyFilters();
          this.igxTogglettt.close();
        }
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.isAssigningCase = false;
        this.applyFilters();
        this.igxTogglettt.close();
      }
    );
  }

  public sort(columnName: string) {
    if (columnName === 'caseIDString') {
      columnName = 'caseID';
    }
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
  public toggle(id, index) {
    this.isToggled = true;
    this.deviceId = this.grid1.data[index].deviceID;

    this.patientStatusId = this.grid1.data[index].statusID;
    this.patientCaseId = id;
    this.patientId = this.grid1.data[index].id;
    this.igxButton.nativeElement = document.getElementById(id);
    this._overlaySettings.positionStrategy.settings.target = this.igxButton.nativeElement;
    this.igxTogglettt.toggle(this._overlaySettings);
    sessionStorage.setItem('wiz_patientCaseID', this.grid1.data[index].caseID.toString());
    sessionStorage.setItem('wiz_patientID', this.patientId.toString());
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.grid1.isLoading = true;
    this.applyFilters();
  }

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

  goBack(): void {
    this.location.back();
  }

  perPageChangeFn(numberOfRecords: number) {
    this.grid1.height = null;
  }

  applyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyFilters();
      return;
    }
    this.grid1.isLoading = true;
    this.filterObject = [];
    if (this.filterParam) {
      if (this.filterParam === 'openCases') {
        this.filterObject.push(this.presetActiveCasesFilter);
        if (!this.orderBy) {
          this.orderBy = 'quickResultID DESC';
        }
      } else if (this.filterParam === 'assignedCases') {
        this.filterObject.push(this.presetAssignedCasesFilter);
        if (!this.orderBy) {
          this.orderBy = 'statusID ASC';
        }
      }
    }
    let operation: string;
    let description: string;
    Object.keys(this.patientSearchForm.controls).forEach(field => {
      if (field === 'statusID' && this.filterParam === 'openCases') {
        return;
      }
      const control = this.patientSearchForm.get(field);
      let value = control.value;
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
          Value: value,
          Description: description,
          isHidden: false
        });
      }
    });
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.remoteService.getAllPatients(skip, top, this.filterObject, this.orderBy);
    const _this = this;
    _this.filterTags = [];
    this.filterObject.forEach(filter => {
      if (!filter.isHidden) {
        _this.filterTags.push(filter);
      }
    });
  }

  private createSearchForm() {
    this.patientSearchForm = this.formBuilder.group({
      PharmacyPatientID: [''],
      genderID: [''],
      statusID: [''],
      quickResultID: [''],
      firstName: [''],
      lastName: [''],
      caseIDString: ['']
    });
  }

  public downloadFile() {
    this.isDownloadingFile = true;
    this.fileService.downloadEcgFile(this.patientCaseId).subscribe(
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
}
