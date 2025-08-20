import { PharmacyService } from '@app/user/Views/pharmacy/pharmacy.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation, ElementRef } from '@angular/core';
import { IgxDialogComponent, IgxGridComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PatientsService } from '@app/pharmacist/Models/patients.service';
import { CookieService } from 'ngx-cookie-service';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-deactivated-patients',
  templateUrl: './deactivated-patients.component.html',
  styleUrls: ['./deactivated-patients.component.scss']
})
export class DeactivatedPatientsComponent implements OnInit, AfterViewInit, OnDestroy {
  public page = 0;
  public totalCount: any;
  public pages = [];
  public data: Observable<any[]>;
  public dataLength: Observable<number> = new BehaviorSubject(0);
  public selectOptions = [5, 10, 15, 25, 50];
  public patientCaseId: number = 0;
  public patientId: number = 0;
  patientSearchForm!: FormGroup;
  filterTags = [];
  order: string;
  columnName: string;
  pharmacies: any[];
  filterType: string;
  toggleBtn: boolean = false;
  deviceId: any;
  deactivatedFilterObject: any[];
  deletePatientId: number;
  isLoading = false;
  isToggled = false;
  urlPrefix = '';
  cookieKeyPerPage: string;
  orderBy: string;

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('grid1', { static: true }) public grid1: IgxGridComponent;
  // @ts-ignore
  @ViewChild('PharmacyPatientID') PharmacyPatientID: ElementRef;
  // @ts-ignore
  @ViewChild('email') email: ElementRef;
  // @ts-ignore
  @ViewChild('phone') phone: ElementRef;
  // @ts-ignore
  @ViewChild('firstName') firstName: ElementRef;
  // @ts-ignore
  @ViewChild('lastName') lastName: ElementRef;

  @ViewChild('deleteConfirmDialog', { static: true }) public deleteConfirmDialog: IgxDialogComponent;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private remoteService: PatientsListService,
    private patientsService: PatientsService,
    private pharmacyService: PharmacyService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private translateService: TranslateService,
    private cookieService: CookieService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'centerDeactivatedPatients_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
    this.deactivatedFilterObject = [];
    this.deactivatedFilterObject.push({
      Property: 'IsActive',
      Operation: 'Equal',
      Value: false,
      Description: '',
      type: ''
    });
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
    if (!this.isToggled) {
      // tslint:disable-next-line:max-line-length
      this.router.navigate([
        this.urlPrefix + '/patient/edit/' + $event.cell.row.rowData.id + '/' + $event.cell.row.rowData.caseID
      ]);
    }
  }

  onDialogClose() {
    this.isToggled = false;
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
    this.remoteService.getDeactivatedPatients(0, this.perPage, this.deactivatedFilterObject);
  }

  confirmDelete(id: number) {
    this.isToggled = true;
    this.deletePatientId = id;
    this.deleteConfirmDialog.open();
  }

  deleteDeactivatedPatient() {
    this.isLoading = true;
    this.deleteConfirmDialog.close();
    this.patientsService.deleteDeactivatedPatient(this.deletePatientId).subscribe(
      response => {
        this.translateService.get('patient_delete_success').subscribe(text => {
          this.toastr.success(text);
          this.isLoading = false;
          this.applyFilters();
        });
      },
      error => {
        this.isLoading = false;
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
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
    const filterObject = [];
    filterObject.push({
      Property: 'IsActive',
      Operation: 'Equal',
      Value: false,
      Description: '',
      type: ''
    });
    this.grid1.isLoading = true;
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
        if (description === 'Patient ID') {
          operation = 'Like';
        } else {
          operation = 'Equal';
        }
      }

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
    this.remoteService.getDeactivatedPatients(skip, top, filterObject, this.orderBy);

    filterObject.splice(0, 1);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.patientSearchForm = this.formBuilder.group({
      PharmacyPatientID: [''],
      firstName: [''],
      lastName: [''],
      email: [''],
      phone: ['']
    });
  }
}
