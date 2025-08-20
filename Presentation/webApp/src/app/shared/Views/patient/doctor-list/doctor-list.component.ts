import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { IgxGridComponent, IgxDialogComponent, IgxPaginatorComponent } from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Location } from '@angular/common';
import { DoctorService } from '@app/shared/services/doctor.service';
import { PatientsService } from '@app/pharmacist/Models/patients.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  providers: [DoctorService],
  selector: 'app-doctor-list',
  templateUrl: './doctor-list.component.html',
  styleUrls: ['./doctor-list.component.scss']
})
export class DoctorListComponent implements OnInit, AfterViewInit, OnDestroy {
  doctorSearchForm!: FormGroup;
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

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('doctorGrid', { static: true }) public doctorGrid: IgxGridComponent;
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
  @ViewChild('DoctorId') DoctorId: ElementRef;
  @ViewChild('deleteConfirmDialog', { static: true }) public deleteConfirmDialog: IgxDialogComponent;

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private doctorService: DoctorService,
    public oAuthService: OAuthService,
    private formBuilder: FormBuilder,
    private location: Location,
    private customValidatorService: CustomValidatorService,
    private patientsService: PatientsService,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    this.cookieKeyPerPage = 'doctorList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  perPageChangeFn(numberOfRecords: number) {
    this.doctorGrid.height = null;
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

  public ngOnInit() {
    this.data = this.doctorService.remoteData.asObservable();
    this.dataLength = this.doctorService.dataLength.asObservable();
    this._dataLengthSubscriber = this.doctorService.dataLength.subscribe(data => {
      this.totalCount = data;
      this.doctorGrid.isLoading = false;
    });
    this.createSearchForm();
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.doctorGrid.isLoading = true;
    this.applyDoctorFilters();
  }

  handleRowClick($event: any) {
    if (!this.isToggled) {
      this.router.navigate([this.urlPrefix + '/doctor/edit/' + $event.cell.row.rowData.id]);
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
    this.applyDoctorFilters();
  }

  public paginate(page: number) {
    this.page = page;
    this.applyDoctorFilters();
  }

  removeFilter(property: string) {
    this.doctorSearchForm.controls[property].setValue('');
    this.applyDoctorFilters(true);
  }

  confirmDelete(id: number) {
    this.isToggled = true;
    this.deleteDoctorId = id;
    this.deleteConfirmDialog.open();
  }

  deleteDoctor() {
    this.isLoading = true;
    this.deleteConfirmDialog.close();
    this.patientsService.deleteDoctor(this.deleteDoctorId).subscribe(
      response => {
        if (response.status === 200) {
          this.translateService.get('doctor_delete_success').subscribe(text => {
            this.toastr.success(text);
            this.isLoading = false;
            this.applyDoctorFilters();
          });
        }
      },
      error => {
        this.isLoading = false;
        if (error.status === 400 && error.error === 'DOCTOR_IS_ASSOCIATED') {
          this.translateService.get(error.error).subscribe(text => {
            this.toastr.error(text);
          });
          return;
        }

        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
  }

  resetPharmacistFilters() {
    this.doctorSearchForm.reset();
    this.applyDoctorFilters(true);
  }

  applyDoctorFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyDoctorFilters();
      return;
    }
    this.doctorGrid.isLoading = true;
    const filterObject = [];
    Object.keys(this.doctorSearchForm.controls).forEach(field => {
      const control = this.doctorSearchForm.get(field);
      if (control.valid && control.value) {
        filterObject.push({
          Property: field,
          Operation: 'Like',
          Value: control.value,
          Description: this[field].nativeElement.getAttribute('data-description')
        });
      }
    });
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.doctorService.getData(skip, top, filterObject, this.orderBy);
    this.filterTags = filterObject;
  }

  private createSearchForm() {
    this.doctorSearchForm = this.formBuilder.group({
      Name: [''],
      Email: [''],
      ZipCode: ['', Validators.compose([this.customValidatorService.numberValidator()])],
      Phone: [''],
      DoctorId: ['']
    });
  }
}
