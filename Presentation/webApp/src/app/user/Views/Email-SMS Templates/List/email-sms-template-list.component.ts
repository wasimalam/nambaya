import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { Location } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation, ElementRef } from '@angular/core';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { OAuthService } from '@app/shared/OAuth.Service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-email-sms-templates-list',
  templateUrl: './email-sms-template-list.component.html',
  styleUrls: ['./email-sms-template-list.component.scss']
})
export class EmailSmsTemplateListComponent implements OnInit, AfterViewInit, OnDestroy {
  public page = 0;
  public totalCount: any;
  public pages = [];
  public data: Observable<any[]>;
  public dataLength: Observable<number> = new BehaviorSubject(0);
  public selectOptions = [5, 10, 15, 25, 50];
  templateSearchForm!: FormGroup;
  filterTags = [];
  order: string;
  columnName: string;
  filterType: string;
  toggleBtn = false;
  cookieKeyPerPage: string;
  orderBy: string;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('grid1', { static: true }) public grid1: IgxGridComponent;
  // @ts-ignore
  @ViewChild('code') code: ElementRef;

  // @ts-ignore
  @ViewChild('subject') subject: ElementRef;
  // @ts-ignore
  @ViewChild('message') message: ElementRef;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private location: Location,
    private remoteService: UserAccountsService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.cookieKeyPerPage = 'templateList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.grid1.height = null;
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

  onClickedOutside(e: Event) {
    const idAttr = (event.target as Element).id;
    if (idAttr !== 'chev' && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  goBack(): void {
    this.location.back();
  }

  public ngOnInit() {
    this.createSearchForm();
    this.data = this.remoteService.remoteData.asObservable();
    this.dataLength = this.remoteService.dataLength.asObservable();
    this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.grid1.isLoading = false;
    });
  }

  handleRowClick($event: any) {
    this.router.navigate(['/user/template/edit/' + $event.cell.row.rowData.id]);
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
  }

  public paginate(page: number) {
    this.page = page;
    this.applyFilters();
  }

  removeFilter(property: string) {
    this.templateSearchForm.controls[property].setValue('');
    this.applyFilters(true);
  }

  resetFilters() {
    this.templateSearchForm.reset();
    this.applyFilters(true);
  }

  applyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyFilters();
      return;
    }
    const filterObject = [];
    let operation: string;
    let description: string;
    this.filterType = null;

    Object.keys(this.templateSearchForm.controls).forEach(field => {
      const control = this.templateSearchForm.get(field);
      if (this[field].nativeElement === undefined) {
        description = this[field].elementRef.nativeElement.getAttribute('data-description');
        operation = 'Equal';
      } else {
        description = this[field].nativeElement.getAttribute('data-description');
        operation = 'Like';
      }

      if (control.value) {
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
    this.remoteService.getAllTemplates(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.templateSearchForm = this.formBuilder.group({
      code: [''],
      subject: [''],
      message: ['']
    });
  }
}
