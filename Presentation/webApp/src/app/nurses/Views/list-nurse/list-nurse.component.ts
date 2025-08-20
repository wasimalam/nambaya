import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild, ElementRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IgxPaginatorComponent, IgxGridComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Location } from '@angular/common';
import { CardiologistService } from '@app/cardiologist/Models/cardiologist.service';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  providers: [CardiologistService],
  selector: 'app-list-nurse',
  templateUrl: './list-nurse.component.html',
  styleUrls: ['./list-nurse.component.scss']
})
export class ListNurseComponent implements OnInit, AfterViewInit, OnDestroy {
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

  nurseSearchForm!: FormGroup;
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
  cookieKeyPerPage: string;
  orderBy: string;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('nurseGrid', { static: true }) public nurseGrid: IgxGridComponent;
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
  @ViewChild('ClinicNo') ClinicNo: ElementRef;
  // @ts-ignore
  @ViewChild('isActive') isActive: ElementRef;
  // @ts-ignore
  @ViewChild('isLocked') isLocked: ElementRef;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private cardiologistService: CardiologistService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private customValidatorService: CustomValidatorService,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'cardioList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.nurseGrid.height = null;
  }

  goBack(): void {
    this.location.back();
  }

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  onClickedOutside(e: Event) {
    const idAttr = (event.target as Element).id;
    if (idAttr !== 'chev' && !idAttr.includes('igx-drop-down-item') && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  public ngOnInit() {
    this.data = this.cardiologistService.remoteData.asObservable();
    this.dataLength = this.cardiologistService.dataLength.asObservable();
    this._dataLengthSubscriber = this.cardiologistService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.nurseGrid.isLoading = false;
    });
    this.createSearchForm();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.nurseGrid.isLoading = true;
    this.applyNurseFilters();
  }

  handleRowClick($event: any) {
    this.router.navigate(['/cardiologist/nurse/edit/' + $event.cell.row.rowData.id]);
  }

  public sort(columnName: string) {
    if (this.order === 'ASC') {
      this.order = 'DESC';
    } else {
      this.order = 'ASC';
    }
    this.columnName = columnName;
    this.orderBy = columnName + ' ' + this.order;
    this.applyNurseFilters();
  }

  public paginate(page: number) {
    this.page = page;
    this.applyNurseFilters();
  }

  removeFilter(property: string) {
    this.nurseSearchForm.controls[property].setValue('');
    this.applyNurseFilters(true);
  }

  resetNurseFilters() {
    this.nurseSearchForm.reset();
    this.applyNurseFilters(true);
  }

  applyNurseFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyNurseFilters();
      return;
    }
    this.nurseGrid.isLoading = true;
    const filterObject = [];
    Object.keys(this.nurseSearchForm.controls).forEach(field => {
      const control = this.nurseSearchForm.get(field);
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
        if(field === 'ClinicNo') {
          field = 'CompanyID';
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
    this.cardiologistService.getNurseData(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.nurseSearchForm = this.formBuilder.group({
      Name: [''],
      Email: [''],
      ZipCode: ['', Validators.compose([this.customValidatorService.numberValidator()])],
      Phone: [''],
      ClinicNo: [''],
      isActive: [''],
      isLocked: ['']
    });
  }
}
