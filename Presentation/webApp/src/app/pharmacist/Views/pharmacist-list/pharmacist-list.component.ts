import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Location } from '@angular/common';
import { PharmacistService } from '@app/pharmacist/Models/pharmacist.service';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  providers: [PharmacistService],
  selector: 'app-pharmacist-list',
  templateUrl: './pharmacist-list.component.html',
  styleUrls: ['./pharmacist-list.component.scss']
})
export class PharmacistListComponent implements OnInit, AfterViewInit, OnDestroy {
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

  pharmacistSearchForm!: FormGroup;
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
  pharmacyId: number;
  cookieKeyPerPage: string;
  orderBy: string;
  urlPrefix = '';

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('pharmacistGrid', { static: true }) public pharmacistGrid: IgxGridComponent;
  @ViewChild('button', { static: true }) public igxButton: ElementRef;
  // @ts-ignore
  @ViewChild('Name') Name: ElementRef;
  // @ts-ignore
  @ViewChild('Email') Email: ElementRef;
  // @ts-ignore
  @ViewChild('ZipCode') ZipCode: ElementRef;
  // @ts-ignore
  @ViewChild('Phone') Phone: ElementRef;
  // @ts-ignore
  @ViewChild('isActive') isActive: ElementRef;
  // @ts-ignore
  @ViewChild('isLocked') isLocked: ElementRef;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private pharmacistService: PharmacistService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private customValidatorService: CustomValidatorService,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'pharmaList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  goBack(): void {
    this.location.back();
  }

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.pharmacistGrid.height = null;
  }

  onClickedOutside(e: Event) {
    const idAttr = (event.target as Element).id;
    if (idAttr !== 'chev' && !idAttr.includes('igx-drop-down-item') && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  public ngOnInit() {
    this.data = this.pharmacistService.remoteData.asObservable();
    this.dataLength = this.pharmacistService.dataLength.asObservable();
    this._dataLengthSubscriber = this.pharmacistService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.pharmacistGrid.isLoading = false;
    });
    this.createSearchForm();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.pharmacistGrid.isLoading = true;
    this.pharmacyId = parseInt(localStorage.getItem('appUserId'));
    this.applyPharmacistFilters();
  }

  handleRowClick($event: any) {
    this.router.navigate(['/pharmacist/pharmacist-edit/' + $event.cell.row.rowData.id]);
  }

  public sort(columnName: string) {
    if (this.order === 'ASC') {
      this.order = 'DESC';
    } else {
      this.order = 'ASC';
    }
    this.columnName = columnName;
    this.orderBy = columnName + ' ' + this.order;
    this.applyPharmacistFilters();
  }

  public paginate(page: number) {
    this.page = page;
    this.applyPharmacistFilters();
  }

  removeFilter(property: string) {
    this.pharmacistSearchForm.controls[property].setValue('');
    this.applyPharmacistFilters(true);
  }

  resetPharmacistFilters() {
    this.pharmacistSearchForm.reset();
    this.applyPharmacistFilters(true);
  }

  applyPharmacistFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyPharmacistFilters();
      return;
    }
    this.pharmacistGrid.isLoading = true;
    const filterObject = [];
    Object.keys(this.pharmacistSearchForm.controls).forEach(field => {
      const control = this.pharmacistSearchForm.get(field);
      let description: string;
      let operation: string;
      if (control.value !== null && control.value !== '') {
        if (field === 'isActive' || field === 'isLocked') {
          operation = 'Equal';
          description = field === 'isActive' ? 'filter_active_label' : 'filter_locked_label';
        } else {
          description = this[field].nativeElement.getAttribute('data-description');
          operation = 'Like';
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
    this.pharmacistService.getData(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.pharmacistSearchForm = this.formBuilder.group({
      Name: [''],
      Email: [''],
      ZipCode: ['', Validators.compose([this.customValidatorService.numberValidator()])],
      Phone: [''],
      isActive: [''],
      isLocked: ['']
    });
  }
}
