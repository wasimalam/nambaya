import { CenterGroupAccountsService } from '@app/centerGroup/Models/center-group-accounts.service';
import { AfterViewInit, Component, OnDestroy, OnInit, TemplateRef, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-center-list',
  templateUrl: './center-list.component.html',
  styleUrls: ['./center-list.component.scss']
})
export class CenterListComponent implements OnInit, AfterViewInit, OnDestroy {
  centerSearchFrom: FormGroup;
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
  cookieKeyPerPage: string;
  orderBy: string;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('centerGrid', { static: true }) public centerGrid: IgxGridComponent;
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
    private centerService: CenterGroupAccountsService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private customValidatorService: CustomValidatorService,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.cookieKeyPerPage = 'centerList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  goBack(): void {
    this.location.back();
  }

  perPageChangeFn(numberOfRecords: number) {
    this.centerGrid.height = null;
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
    this.data = this.centerService.remoteData.asObservable();
    this.dataLength = this.centerService.dataLength.asObservable();
    this._dataLengthSubscriber = this.centerService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.centerGrid.isLoading = false;
    });
    this.createSearchForm();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.centerGrid.isLoading = true;
    this.centerService.getCenters(0, this.perPage);
  }

  handleRowClick($event: any) {
    this.router.navigate(['/user/center/edit/' + $event.cell.row.rowData.id]);
  }

  public sort(columnName: string) {
    if (this.order === 'ASC') {
      this.order = 'DESC';
    } else {
      this.order = 'ASC';
    }
    this.columnName = columnName;
    this.orderBy = columnName + ' ' + this.order;
    this.applyCenterListFilters();
  }

  public paginate(page: number) {
    this.page = page;
    this.applyCenterListFilters();
  }

  removeFilter(property: string) {
    this.centerSearchFrom.controls[property].setValue('');
    this.applyCenterListFilters(true);
  }

  resetCardiologistFilters() {
    this.centerSearchFrom.reset();
    this.applyCenterListFilters(true);
  }

  applyCenterListFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyCenterListFilters();
      return;
    }
    this.centerGrid.isLoading = true;
    const filterObject = [];
    Object.keys(this.centerSearchFrom.controls).forEach(field => {
      const control = this.centerSearchFrom.get(field);
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
    this.centerService.getCenters(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.centerSearchFrom = this.formBuilder.group({
      Name: [''],
      Email: [''],
      ZipCode: ['', Validators.compose([this.customValidatorService.numberValidator()])],
      Phone: [''],
      isActive: [''],
      isLocked: ['']
    });
  }
}
