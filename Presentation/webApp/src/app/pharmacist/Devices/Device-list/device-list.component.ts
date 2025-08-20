import { Router, ActivatedRoute } from '@angular/router';
import { DeviceService } from '@app/pharmacist/Devices/device.service';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation, ElementRef } from '@angular/core';
import {
  IgxGridComponent,
  HorizontalAlignment,
  VerticalAlignment,
  ConnectedPositioningStrategy,
  CloseScrollStrategy,
  IgxPaginatorComponent
} from 'igniteui-angular';
import { BehaviorSubject, Observable } from 'rxjs';

import { OAuthService } from '@app/shared/OAuth.Service';
import { IgxToggleDirective } from 'igniteui-angular';
import { PharmacistAccountsService } from '@app/pharmacist/Models/pharmacist-accounts.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Location } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.scss']
})
export class DeviceListComponent implements OnInit, AfterViewInit, OnDestroy {
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
  public totalCount = 0;
  public pages = [];
  public data: Observable<any[]>;
  public dataLength: Observable<number> = new BehaviorSubject(0);
  public selectOptions = [5, 10, 15, 25, 50];
  deviceSearchForm!: FormGroup;
  filterTags = [];
  statuses: any[];
  order: string;
  columnName: string;

  public filterParam: string;
  public filterObject = [];
  public presetFilter = { Property: 'StatusID', Operation: 'Equal', Value: 451, Description: 'Status', isHidden: true };

  @ViewChild('paginator', { read: IgxPaginatorComponent, static: false }) public paginator: IgxPaginatorComponent;
  @ViewChild('grid1', { static: true }) public grid1: IgxGridComponent;
  @ViewChild(IgxToggleDirective, { static: true }) public igxTogglettt: IgxToggleDirective;
  @ViewChild('button', { static: true }) public igxButton: ElementRef;
  // @ts-ignore
  @ViewChild('serialNumber') serialNumber: ElementRef;
  // @ts-ignore
  @ViewChild('name') name: ElementRef;
  // @ts-ignore
  @ViewChild('manufacturer') manufacturer: ElementRef;
  // @ts-ignore
  @ViewChild('StatusID') StatusID: ElementRef;

  public _positionSettings = {
    horizontalStartPoint: HorizontalAlignment.Left,
    verticalStartPoint: VerticalAlignment.Bottom
  };
  public _overlaySettings = {
    closeOnOutsideClick: false,
    modal: false,
    positionStrategy: new ConnectedPositioningStrategy(this._positionSettings),
    scrollStrategy: new CloseScrollStrategy()
  };

  toggleBtn = false;
  loggedPharmacist: any;
  pharmacyId: any;
  editingDeviceId: any;
  gettingStatuses: boolean;
  cookieKeyPerPage: string;
  orderBy: string;
  urlPrefix = '';

  private _perPage;
  private _dataLengthSubscriber;

  constructor(
    private remoteService: DeviceService,
    public oAuthService: OAuthService,
    public deviceService: DeviceService,
    public pharmacistService: PharmacistAccountsService,
    private formBuilder: FormBuilder,
    private location: Location,
    private activatedRouter: ActivatedRoute,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.filterParam = this.activatedRouter.snapshot.params.filter;
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
    if (this.filterParam) {
      this.filterObject.push(this.presetFilter);
    }
    this.cookieKeyPerPage = 'devicesList_' + localStorage.getItem('userId');
    // tslint:disable-next-line:max-line-length
    this._perPage = this.cookieService.check(this.cookieKeyPerPage)
      ? this.cookieService.get(this.cookieKeyPerPage)
      : 10;
  }

  // tslint:disable-next-line:typedef
  clickEvent($event) {
    this.toggleBtn = !this.toggleBtn;
  }

  onClickedOutside(e: any) {
    const idAttr = (event.target as Element).id;
    let ignoreelement = e.path.find((item: any) => {
      if (item.id) {
        return item.id.includes('igx-drop-down-item');
      } else {
        return false;
      }
    });
    if (idAttr !== 'chev' && !ignoreelement && this.toggleBtn) {
      this.toggleBtn = false;
    }
  }

  goBack(): void {
    this.location.back();
  }

  handleRowClick($event: any) {
    this.router.navigate(['/pharmacist/device/edit/' + $event.cell.row.rowData.id]);
  }

  getStatuses() {
    this.deviceService.getStatuses().subscribe(data => {
      this.statuses = data;
      this.gettingStatuses = false;
    });
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

  // tslint:disable-next-line:typedef
  public navigateToAssignedPage(rowIndex) {
    const patientCaseId = this.grid1.data[rowIndex].patientCaseID;
    const pharmacyId = this.grid1.data[rowIndex].pharmacyID;
    this.router.navigate(['/assigndevice', patientCaseId, pharmacyId]);
  }

  public ngOnInit() {
    this.createSearchForm();
    this.oAuthService.observableUser.subscribe(user => {
      this.getStatuses();
      // tslint:disable-next-line:triple-equals
      if (user != undefined && user.appuserid > 0) {
        if (user.rolecode === 'Pharmacist') {
          this.getOnePharmacist(user.appuserid);
        } else if (user.rolecode === 'Pharmacy') {
          this.pharmacyId = user.appuserid;

          this.remoteService.getDevicesofOnePharmacy(0, this.perPage, this.filterObject);
          this.data = this.remoteService.remoteData.asObservable();
          this.dataLength = this.remoteService.dataLength.asObservable();
          this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
            this.totalCount = data;
            this.grid1.isLoading = false;
          });
        }
      }
    });
  }

  public toggle(id) {
    this.editingDeviceId = id;
    this.igxButton.nativeElement = document.getElementById(id);
    this._overlaySettings.positionStrategy.settings.target = this.igxButton.nativeElement;
    this.igxTogglettt.toggle(this._overlaySettings);
  }

  public ngOnDestroy() {
    if (this._dataLengthSubscriber) {
      this._dataLengthSubscriber.unsubscribe();
    }
  }

  public ngAfterViewInit() {
    this.grid1.isLoading = true;
  }

  public paginate(page: number) {
    this.page = page;
    this.applyFilters();
  }

  removeFilter(property: string) {
    this.deviceSearchForm.controls[property].setValue('');
    this.applyFilters(true);
  }

  resetFilters() {
    this.deviceSearchForm.reset();
    this.applyFilters(true);
  }

  applyFilters(reset: boolean = false) {
    if (reset) {
      this.paginator ? (this.paginator.page = 0) : this.applyFilters();
      return;
    }
    this.grid1.isLoading = true;
    this.filterObject = [];
    if (this.filterParam) {
      this.filterObject.push(this.presetFilter);
    }
    let description: string;
    let operation: string;
    Object.keys(this.deviceSearchForm.controls).forEach(field => {
      const control = this.deviceSearchForm.get(field);
      if (this[field].nativeElement === undefined) {
        description = this[field].elementRef.nativeElement.getAttribute('data-description');
        operation = 'Equal';
      } else {
        description = this[field].nativeElement.getAttribute('data-description');
        operation = 'Like';
      }
      if (control.value) {
        this.filterObject.push({
          Property: field,
          Operation: operation,
          Value: control.value,
          Description: description,
          isHidden: false
        });
      }
    });
    const skip = this.page * this.perPage;
    const top = this.perPage;
    this.toggleBtn = false;
    this.remoteService.getDevicesofOnePharmacy(skip, top, this.filterObject, this.orderBy);
    const _this = this;
    _this.filterTags = [];
    this.filterObject.forEach(filter => {
      if (!filter.isHidden) {
        _this.filterTags.push(filter);
      }
    });
  }

  perPageChangeFn(numberOfRecords: number) {
    this.grid1.height = null;
  }

  private getOnePharmacist(id: number) {
    this.pharmacistService.getOne(id).subscribe(data => {
      this.loggedPharmacist = data;
      this.pharmacyId = this.loggedPharmacist.pharmacyID;
      this.remoteService.getDevicesofOnePharmacy(0, this.perPage, this.filterObject);
      this.data = this.remoteService.remoteData.asObservable();
      this.dataLength = this.remoteService.dataLength.asObservable();
      this._dataLengthSubscriber = this.remoteService.dataLength.subscribe(data => {
        this.totalCount = data;
        this.grid1.isLoading = false;
      });
    });
  }

  private createSearchForm() {
    this.deviceSearchForm = this.formBuilder.group({
      serialNumber: [''],
      name: [''],
      manufacturer: [''],
      StatusID: ['']
    });
  }
}
