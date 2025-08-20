import { Router } from '@angular/router';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { UsersListService } from '@app/user/Models/users-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Location } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, AfterViewInit, OnDestroy {
  public page = 0;
  public totalCount = 0;
  public pages = [];
  public data: Observable<any[]>;
  public dataLength: Observable<number> = new BehaviorSubject(0);
  public selectOptions = [5, 10, 15, 25, 50];
  userSearchForm!: FormGroup;
  filterTags = [];
  statuses: any[];
  order: string;
  columnName: string;
  cookieKeyPerPage: string;
  orderBy: string;
  toggleBtn = false;
  userRoles = null;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('userGrid', { static: true }) public userGrid: IgxGridComponent;
  // @ts-ignore
  @ViewChild('name') name: ElementRef;
  // @ts-ignore
  @ViewChild('email') email: ElementRef;
  // @ts-ignore
  @ViewChild('isActive') isActive: ElementRef;
  // @ts-ignore
  @ViewChild('isLocked') isLocked: ElementRef;
  // @ts-ignore
  @ViewChild('roleCode') roleCode: ElementRef;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private remoteService: UsersListService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.cookieKeyPerPage = 'userList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  public get perPage(): number {
    if (this.cookieService.check(this.cookieKeyPerPage)) {
      return Number(this.cookieService.get(this.cookieKeyPerPage));
    }

    return this._perPage;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.userGrid.height = null;
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

  public getUserRoles() {
    this.remoteService.getUserRoles().subscribe(response => {
      this.userRoles = response.body;
    });
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

  handleRowClick($event: any) {
    this.router.navigate(['/user/user-edit/' + $event.cell.row.rowData.id]);
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

  public paginate(page: number) {
    this.page = page;
    this.userGrid.isLoading = true;
    this.applyFilters();
  }

  removeFilter(property: string) {
    this.userSearchForm.controls[property].setValue('');
    this.applyFilters(true);
  }

  resetFilters() {
    this.userSearchForm.reset();
    this.toggleBtn = false;
    this.applyFilters(true);
  }

  applyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyFilters();
      return;
    }
    this.userGrid.isLoading = true;
    const filterObject = [];
    let description: string;
    let operation: string;
    Object.keys(this.userSearchForm.controls).forEach(field => {
      const control = this.userSearchForm.get(field);
      if (control.value !== null && control.value !== '') {
        if (field === 'isActive' || field === 'isLocked' || field === 'roleCode') {
          operation = 'Equal';
          if(field === 'isActive') {
            description = 'filter_active_label';
          } else if(field === 'isLocked') {
            description = 'filter_locked_label';
          } else {
            description = 'Role';
          }
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
    this.remoteService.getData(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  public ngOnInit() {
    this.createSearchForm();
    this.data = this.remoteService.remoteData.asObservable();
    this.dataLength = this.remoteService.dataLength.asObservable();
    this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.userGrid.isLoading = false;
    });
    this.getUserRoles();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.userGrid.isLoading = true;
    this.applyFilters();
  }
  private createSearchForm() {
    this.userSearchForm = this.formBuilder.group({
      name: [''],
      email: [''],
      isActive: [''],
      isLocked: [''],
      roleCode: ['']
    });
  }
}
