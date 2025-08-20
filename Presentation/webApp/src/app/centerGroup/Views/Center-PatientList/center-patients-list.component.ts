import { PharmacyService } from '@app/user/Views/pharmacy/pharmacy.service';
import { ToastrService } from 'ngx-toastr';
import { FileUploadService } from '@app/shared/fileUpload.service';
import { Location } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation, ElementRef } from '@angular/core';
import {
  IgxGridComponent,
  ConnectedPositioningStrategy,
  IgxDropDownComponent,
  AbsoluteScrollStrategy,
  IgxPaginatorComponent
} from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { IgxToggleDirective } from 'igniteui-angular';
import { FormGroup, FormBuilder } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-center-patients-list',
  templateUrl: './center-patients-list.component.html',
  styleUrls: ['./center-patients-list.component.scss']
})
export class CenterPatientsListComponent implements OnInit, AfterViewInit, OnDestroy {
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
  public patientStatusId = 0;
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
  order: string;
  columnName: string;
  isDownloadingFile = false;
  pharmacies: any[];
  gettingPharmacies = false;
  pharmacySelected: any;
  filterType: string;
  urlPrefix = '';
  caseStatuses: Object;
  isToggled = false;
  cookieKeyPerPage: string;
  orderBy: string;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('grid1', { static: true }) public grid1: IgxGridComponent;
  @ViewChild(IgxToggleDirective, { static: true }) public igxTogglettt: IgxToggleDirective;
  @ViewChild('button', { static: true }) public igxButton: ElementRef;
  // @ts-ignore
  @ViewChild('firstName') firstName: ElementRef;
  // @ts-ignore
  @ViewChild('lastName') lastName: ElementRef;
  // @ts-ignore
  @ViewChild('PharmacyPatientID') PharmacyPatientID: ElementRef;

  // @ts-ignore
  @ViewChild('statusID') statusID: ElementRef;
  // @ts-ignore
  @ViewChild('pharmacyId') pharmacyId: ElementRef;
  @ViewChild(IgxDropDownComponent, { static: true }) public igxDropDown: IgxDropDownComponent;

  public _positionSettings = {
    horizontalStartPoint: 0,
    verticalStartPoint: -8.5
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
    private translateService: TranslateService,
    private location: Location,
    private toastr: ToastrService,
    private remoteService: PatientsListService,
    private pharmacyService: PharmacyService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private fileService: FileUploadService,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
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
    this.cookieKeyPerPage = 'centerPatients_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  onClickedOutside(e: any) {
    let ignoreelement = e.path.find((item: any) => {
      if (item.id) {
        return item.id.includes('igx-drop-down-item');
      } else {
        return false;
      }
    });

    const idAttr = (e.target as Element).id;
    if (idAttr !== 'chev' && !ignoreelement && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  perPageChangeFn(numberOfRecords: number) {
    this.grid1.height = null;
  }

  public getGenders() {
    this.remoteService.getGendersList().subscribe(data => {
      this.genders = data;
    });
  }

  goBack(): void {
    this.location.back();
  }

  itemselectedEvent(event) {}

  public getEvaluationStatues() {
    this.remoteService.getEvaluationStatusList().subscribe(data => {
      this.evaluationStatuses = data;
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

  public getAllPharmacies() {
    this.pharmacyService.getAllPharmacies().subscribe(data => {
      this.pharmacies = data.data;
      this.gettingPharmacies = false;
    });
  }

  public ngOnInit() {
    this.createSearchForm();
    this.getGenders();
    this.getAllPharmacies();
    this.getEvaluationStatues();
    this.data = this.remoteService.remoteData.asObservable();
    this.dataLength = this.remoteService.dataLength.asObservable();
    this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.grid1.isLoading = false;
    });
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
  public toggle(id, index) {
    this.isToggled = true;
    this.deviceId = this.grid1.data[index].deviceID;
    this.patientCaseId = id;
    this.patientId = this.grid1.data[index].id;
    this.patientStatusId = this.grid1.data[index].statusID;

    this.igxButton.nativeElement = document.getElementById(id);
    this._overlaySettings.positionStrategy.settings.target = this.igxButton.nativeElement;
    this.igxTogglettt.toggle(this._overlaySettings);
  }

  public ngAfterViewInit() {
    this.grid1.isLoading = true;
    this.remoteService.getAllPatients(0, this.perPage);
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
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

  applyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyFilters();
      return;
    }
    this.grid1.isLoading = true;
    const filterObject = [];
    let operation: string;
    let description: string;
    this.filterType = null;

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
      if (field === 'pharmacyId' && control.value !== undefined && control.value !== null) {
        this.filterType = 'pharmacy';
        const filteredPharmacies = this.pharmacies.filter(ph => ph.name === control.value);
        if (filteredPharmacies.length > 0) {
          const pharmacyId = filteredPharmacies[0].id;
          filterObject.push({
            Property: field,
            Operation: 'Equal',
            Value: parseInt(pharmacyId),
            Description: description,
            type: 'pharmacy'
          });
        } else {
          /*this.translateService.get("plese select from drop down and then filter on pharmacy").subscribe(text => {
            this.toastr.error(text);
          });*/
        }
      }

      if (field !== 'pharmacyId' && control.value) {
        filterObject.push({
          Property: field,
          Operation: operation,
          Value: control.value,
          Description: description,
          type: ''
        });
      }
    });
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.remoteService.getAllPatients(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  public dateFormatter = (date: Date) => {
    const dateFormat = localStorage.getItem('dateFormat');
    const timeZone = localStorage.getItem('timeZone');
    return date
      ? `${moment
          .utc(date)
          .tz(timeZone)
          .format(dateFormat)}`
      : ``;
  };

  private createSearchForm() {
    this.patientSearchForm = this.formBuilder.group({
      PharmacyPatientID: [''],
      statusID: [''],
      pharmacyId: [''],
      firstName: [''],
      lastName: ['']
    });
  }
}
