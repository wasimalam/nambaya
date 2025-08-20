import { PharmacyService } from '@app/user/Views/pharmacy/pharmacy.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation, ElementRef } from '@angular/core';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-comleted-studies-list',
  templateUrl: './completed-studies-list.component.html',
  styleUrls: ['./completed-studies-list.component.scss']
})
export class CompletedStudesListComponent implements OnInit, AfterViewInit, OnDestroy {
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
  order: string;
  columnName: string;
  pharmacies: any[];
  gettingPharmacies = false;
  filterType: string;
  toggleBtn = false;
  deviceId: any;
  urlPrefix = '';
  cookieKeyPerPage: string;
  orderBy: string;
  pageTitle = 'Completed Case Studies';
  caseStatuses: any;

  public presetDispatchedCasesFilter = {
    Property: 'StatusID',
    Operation: 'Greater',
    Value: 658,
    Description: 'Status',
    isHidden: true
  };

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('grid1', { static: true }) public grid1: IgxGridComponent;
  // @ts-ignore
  @ViewChild('caseIDString') caseIDString: ElementRef;
  public filterParam: string;
  public filterObject = [];

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private remoteService: PatientsListService,
    private pharmacyService: PharmacyService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private cookieService: CookieService,
    private activatedRoute: ActivatedRoute
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'centerCompletedCases_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
    const listType = this.activatedRoute.snapshot.url[1];
    if (listType) {
      this.filterParam = listType.path;
      if (this.filterParam === 'dispatched-cases') {
        this.pageTitle = 'dispatched_cases';
        this.filterObject.push(this.presetDispatchedCasesFilter);
      }
    }
    this.caseStatuses = {
      '659': 'Report Dispatch Failed',
      '660': 'Report Dispatching',
      '661': 'Report Dispatched'
    };
  }

  perPageChangeFn(numberOfRecords: number) {
    this.grid1.height = null;
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

  goBack(): void {
    this.location.back();
  }
  public getAllPharmacies() {
    this.pharmacyService.getAllPharmacies().subscribe(data => {
      this.pharmacies = data.data;
      this.gettingPharmacies = false;
    });
  }
  public ngOnInit() {
    this.createSearchForm();
    this.getAllPharmacies();
    this.data = this.remoteService.remoteData.asObservable();
    this.dataLength = this.remoteService.dataLength.asObservable();
    this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.grid1.isLoading = false;
    });
  }

  handleRowClick($event: any) {
    this.router.navigate(['/center/sendCompletedStudy', $event.cell.row.rowData.caseID]);
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

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.grid1.isLoading = true;
    this.applyFilters();
    // this.remoteService.getCompletedCases(0, this.perPage);
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
    this.filterObject = [];
    if (this.filterParam && this.filterParam === 'dispatched-cases') {
      this.filterObject.push(this.presetDispatchedCasesFilter);
    }

    let description: string;
    this.filterType = null;

    Object.keys(this.patientSearchForm.controls).forEach(field => {
      const control = this.patientSearchForm.get(field);
      const value = control.value;
      if (this[field].nativeElement === undefined) {
        description = this[field].elementRef.nativeElement.getAttribute('data-description');
      } else {
        description = this[field].nativeElement.getAttribute('data-description');
      }

      if (control.value) {
        this.filterObject.push({
          Property: field,
          Operation: 'Equal',
          Value: value,
          Description: description,
          isHidden: false
        });
      }
    });

    /*    Object.keys(this.patientSearchForm.controls).forEach(field => {
      const control = this.patientSearchForm.get(field);
      if (this[field].nativeElement === undefined) {
        description = this[field].elementRef.nativeElement.getAttribute('data-description');
        operation = 'Equal';
      } else {
        description = this[field].nativeElement.getAttribute('data-description');
        operation = 'Like';
      }
      if (field === 'pharmacyId' && control.value !== undefined && control.value !== null) {
        this.filterType = 'pharmacy';
        const filteredPharmacies = this.pharmacies.filter(ph => ph.name === control.value);
        if (filteredPharmacies.length > 0) {
          const pharmacyId = filteredPharmacies[0].id;
          this.filterObject.push({
            Property: field,
            Operation: 'Equal',
            Value: parseInt(pharmacyId),
            Description: description,
            type: 'pharmacy'
          });
        } else {
          /!*this.translateService.get("plese select from drop down and then filter on pharmacy").subscribe(text => {
            this.toastr.error(text);
          });*!/
        }
      }

      if (field !== 'pharmacyId' && control.value) {
        this.filterObject.push({
          Property: field,
          Operation: operation,
          Value: control.value,
          Description: description,
          type: ''
        });
      }
    });*/
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.filterTags = [];
    if (this.filterParam === 'dispatched-cases') {
      this.remoteService.getAllPatients(skip, top, this.filterObject, this.orderBy);
    } else {
      this.remoteService.getCompletedCases(skip, top, this.filterObject, this.orderBy);
    }
    this.filterObject.forEach(filter => {
      if (filter.Property !== 'StatusID') {
        this.filterTags.push(filter);
      }
    });
  }

  navigateToSendReport(index) {
    const caseId = this.grid1.data[index].caseID;
    this.router.navigate(['/center/sendCompletedStudy', caseId]);
  }

  private createSearchForm() {
    this.patientSearchForm = this.formBuilder.group({
      caseIDString: ['']
    });
  }
}
