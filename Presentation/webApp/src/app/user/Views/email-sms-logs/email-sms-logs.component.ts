import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import * as moment from 'moment';
import 'moment-timezone/index';
import { I18nService } from '@app/core';

@Component({
  providers: [UserAccountsService],
  selector: 'app-email-sms-logs',
  templateUrl: './email-sms-logs.component.html',
  styleUrls: ['./email-sms-logs.component.scss']
})
export class EmailSmsLogsComponent implements OnInit, AfterViewInit, OnDestroy {
  emailSMSSearchForm!: FormGroup;
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
  deleteDoctorId: number;
  isLoading = false;
  isToggled = false;
  urlPrefix = '';
  cookieKeyPerPage: string;
  orderBy: string;
  invalidDate = false;
  public currentLanguage = 'de-DE';
  public date: Date = new Date(Date.now());

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('emailSMSGrid', { static: true }) public emailSMSGrid: IgxGridComponent;
  @ViewChild('button', { static: true }) public igxButton: ElementRef;
  // @ts-ignore
  @ViewChild('Subject') Subject: ElementRef;
  // @ts-ignore
  @ViewChild('Recipient') Recipient: ElementRef;
  // @ts-ignore
  @ViewChild('Message') Message: ElementRef;
  // @ts-ignore
  @ViewChild('SentTime') SentTime: ElementRef;
  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private customValidatorService: CustomValidatorService,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private router: Router,
    private cookieService: CookieService,
    private userService: UserAccountsService,
    private i18nService: I18nService
  ) {
    this.currentLanguage = this.i18nService.language;
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'doctorList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.emailSMSGrid.height = null;
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

  public formatter = (date: Date) => {
    if (date) {
      const dateFormat = localStorage.getItem('dateFormat');
      const timeZone = localStorage.getItem('timeZone');
      return `${moment
        .utc(date)
        .tz(timeZone)
        .format(dateFormat)}`;
    }
  };

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  onClickedOutside(e: Event) {
    const idAttr = (e.target as Element).id;
    if (idAttr !== 'chev' && idAttr !== '' && this.toggleBtn && idAttr !== 'igx-icon-15') {
      this.toggleBtn = false;
    }
  }

  public ngOnInit() {
    this.data = this.userService.remoteData.asObservable();
    this.dataLength = this.userService.dataLength.asObservable();
    this._dataLengthSubscriber = this.userService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.emailSMSGrid.isLoading = false;
    });
    this.createSearchForm();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.emailSMSGrid.isLoading = true;
    this.order = 'ASC';
    this.sort('timeStamp');
  }

  public sort(columnName: string) {
    if (this.order === 'ASC') {
      this.order = 'DESC';
    } else {
      this.order = 'ASC';
    }
    this.columnName = columnName;
    this.orderBy = columnName + ' ' + this.order;
    this.applyLogFilters();
  }

  public paginate(page: number) {
    this.page = page;
    this.applyLogFilters();
  }

  removeFilter(property: string) {
    if (property === 'TimeStamp') {
      this.emailSMSSearchForm.controls.SentTime.setValue('');
    } else {
      this.emailSMSSearchForm.controls[property].setValue('');
    }
    this.applyLogFilters(true);
  }

  resetPharmacistFilters() {
    this.emailSMSSearchForm.reset();
    this.applyLogFilters(true);
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

  formatHTMLString(value: string) {
    return value.replace(/<[^>]*>?/gm, '');
  }

  // tslint:disable-next-line:typedef
  validateDate(date) {
    this.invalidDate = true;
    if (date !== null) {
      const currentDate = new Date();
      const selectedDate = new Date(date);
      if (currentDate > selectedDate) {
        this.invalidDate = false;
      }
    }
  }

  applyLogFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyLogFilters();
      return;
    }
    this.emailSMSGrid.isLoading = true;
    const filterObject = [];
    Object.keys(this.emailSMSSearchForm.controls).forEach(field => {
      const control = this.emailSMSSearchForm.get(field);
      let description: string;
      let value: string;
      let operation: string;
      if (control.valid && control.value) {
        if (this[field].nativeElement === undefined) {
          description = this[field].element.nativeElement.getAttribute('data-description');
          field = 'TimeStamp';
          let greaterEqualDate = moment(control.value).format('YYYY-MM-DD');
          let lessEqualDate = moment(control.value)
            .add(1, 'days')
            .format('YYYY-MM-DD');
          greaterEqualDate = moment.utc(greaterEqualDate).toISOString();
          lessEqualDate = moment.utc(lessEqualDate).toISOString();
          operation = 'GreaterOrEqual';
          value = greaterEqualDate;
          filterObject.push({
            Property: 'TimeStamp',
            Operation: 'Less',
            Value: lessEqualDate,
            Description: description
          });
        } else {
          description = this[field].nativeElement.getAttribute('data-description');
          value = control.value;
          operation = 'Like';
        }

        filterObject.push({
          Property: field,
          Operation: operation,
          Value: value,
          Description: description
        });
      }
    });

    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.userService.getAllEmailSMSLogs(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject.filter(item => item.Operation !== 'Less');
  }

  private createSearchForm() {
    this.emailSMSSearchForm = this.formBuilder.group({
      Subject: [''],
      Recipient: [''],
      Message: [''],
      SentTime: ['']
    });
  }
}
