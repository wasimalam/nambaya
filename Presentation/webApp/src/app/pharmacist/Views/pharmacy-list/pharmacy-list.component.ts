import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild, ElementRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { PharmacyService } from '@app/pharmacist/Models/pharmacy.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  providers: [PharmacyService],
  selector: 'app-pharmacy-list',
  templateUrl: './pharmacy-list.component.html',
  styleUrls: ['./pharmacy-list.component.scss']
})
export class PharmacyListComponent implements OnInit, AfterViewInit, OnDestroy {
  pharmacySearchForm!: FormGroup;
  public page = 0;
  public totalCount: any;
  public pages = [];
  public data: Observable<any[]>;
  public dataLength: Observable<number> = new BehaviorSubject(0);
  public selectOptions = [5, 10, 15, 25, 50];
  toggleBtn = false;
  filterTags = [];
  order: string;
  columnName: string;
  urlPrefix = '';
  public isCenterUser = false;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('pharmacyGrid', { static: true }) public pharmacyGrid: IgxGridComponent;
  @ViewChild('button', { static: true }) public igxButton: ElementRef;
  // @ts-ignore
  @ViewChild('Name') Name: ElementRef;
  // @ts-ignore
  @ViewChild('Email') Email: ElementRef;
  // @ts-ignore
  @ViewChild('ZipCode') ZipCode: ElementRef;
  // @ts-ignore
  @ViewChild('identification') identification: ElementRef;
  // @ts-ignore
  @ViewChild('contact') contact: ElementRef;
  // @ts-ignore
  @ViewChild('Phone') Phone: ElementRef;
  // @ts-ignore
  @ViewChild('isActive') isActive: ElementRef;
  // @ts-ignore
  @ViewChild('isLocked') isLocked: ElementRef;

  cookieKeyPerPage: string;
  orderBy: string;
  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private pharmacyService: PharmacyService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private customValidatorService: CustomValidatorService,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'pharmacyList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.pharmacyGrid.height = null;
  }

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

  goBack(): void {
    this.location.back();
  }

  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  onClickedOutside(e: Event) {
    const idAttr = (event.target as Element).id;
    if (idAttr !== 'chev' && !idAttr.includes('igx-drop-down-item') && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  handleRowClick($event: any) {
    this.router.navigate([this.urlPrefix + '/pharmacy/edit/' + $event.cell.row.rowData.id]);
  }

  public sort(columnName: string) {
    if (this.order === 'ASC') {
      this.order = 'DESC';
    } else {
      this.order = 'ASC';
    }
    this.columnName = columnName;
    this.orderBy = columnName + ' ' + this.order;
    this.applyPharmacyFilters();
  }

  public ngOnInit() {
    this.data = this.pharmacyService.remoteData.asObservable();
    this.dataLength = this.pharmacyService.dataLength.asObservable();
    this._dataLengthSubscriber = this.pharmacyService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.pharmacyGrid.isLoading = false;
    });
    this.createSearchForm();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    const applicationId = localStorage.getItem('applicationId');
    if (applicationId === 'centralgroupuser') {
      this.isCenterUser = true;
    }
    this.pharmacyGrid.isLoading = true;
    this.applyPharmacyFilters();
  }

  public paginate(page: number) {
    this.page = page;
    this.applyPharmacyFilters();
  }

  removeFilter(property: string) {
    this.pharmacySearchForm.controls[property].setValue('');
    this.applyPharmacyFilters(true);
  }

  resetPharmacyFilters() {
    this.pharmacySearchForm.reset();
    this.applyPharmacyFilters(true);
  }

  applyPharmacyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyPharmacyFilters();
      return;
    }
    this.pharmacyGrid.isLoading = true;
    const filterObject = [];
    Object.keys(this.pharmacySearchForm.controls).forEach(field => {
      const control = this.pharmacySearchForm.get(field);
      if (control.value !== null && control.value !== '') {
        let operation = '';
        let description = '';
        if (field === 'isActive' || field === 'isLocked') {
          operation = 'Equal';
          description = field === 'isActive' ? 'filter_active_label' : 'filter_locked_label';
        } else {
          operation = 'Like';
          description = this[field].nativeElement.getAttribute('data-description');
        }
        filterObject.push({
          Property: field,
          Operation: operation,
          Value: control.value,
          Description: description
        });
      }
    });
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.pharmacyService.getData(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.pharmacySearchForm = this.formBuilder.group({
      Name: [''],
      Email: [''],
      ZipCode: ['', Validators.compose([this.customValidatorService.numberValidator()])],
      Phone: [''],
      identification: [''],
      contact: [''],
      isActive: [''],
      isLocked: ['']
    });
  }
}
