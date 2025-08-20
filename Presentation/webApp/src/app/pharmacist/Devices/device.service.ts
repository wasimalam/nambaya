import { APIUrls } from '@app/shared/Addresses';
import { DeviceAssignModel } from '@app/pharmacist/Devices/Device-Assign/deviceassign.model';
import { DeviceModel } from '@app/pharmacist/Devices/device.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class DeviceService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;
  private urls = {
    saveUrl: `${this.pharmacistServiceUrl}/api/v1/Device`,
    assignUrl: `${this.pharmacistServiceUrl}/api/v1/Device/AssignDevice`,
    updateUrl: `${this.pharmacistServiceUrl}/api/v1/Device`,
    getOneUrl: `${this.pharmacistServiceUrl}/api/v1/Device/`,
    getDeviceAssignmentUrl: `${this.pharmacistServiceUrl}/api/v1/device/GetDeviceAssignment`,
    getStatusesUrl: `${this.pharmacistServiceUrl}/api/v1/lookups/items?code=device-status`,
    getDevicesByPharmacyUrl: `${this.pharmacistServiceUrl}/api/v1/Device/pharmacy/`
  };

  constructor(private http: HttpClient, private toastr: ToastrService) {
    this.remoteData = new BehaviorSubject([]);
  }

  save(device: DeviceModel, pharmacyId: number) {
    device.pharmacyId = pharmacyId;
    device.statusID = 451;

    return this.http.post(`${this.urls.saveUrl}`, device, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  assign(deviceAssign: DeviceAssignModel) {
    return this.http
      .post(`${this.urls.assignUrl}`, deviceAssign, { observe: 'response' })
      .pipe(catchError(this.handleError));
  }

  update(device: DeviceModel): any {
    return this.http.put(`${this.urls.updateUrl}`, device, { observe: 'response' }).pipe(catchError(this.handleError));
  }

  getOne(deviceId: number): Observable<any> {
    return this.http.get(`${this.urls.getOneUrl}` + deviceId).pipe(catchError(this.handleError));
  }

  getDeviceAssignment(caseId?: number): any {
    const url = `${this.urls.getDeviceAssignmentUrl}/${caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  getStatuses(): any {
    return this.http.get(`${this.urls.getStatusesUrl}`).pipe(catchError(this.handleError));
  }

  public getDevicesofOnePharmacy(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
    let qS = '';
    if (perPage) {
      qS = `?offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    if (orderBy) {
      qS += `&orderby=` + orderBy;
    }
    this.http
      .get(`${this.urls.getDevicesByPharmacyUrl}` + qS)
      .pipe(
        map((data: any) => {
          return data;
        })
      )
      .subscribe(data => {
        this.remoteData.next(data.data);
        this.dataLength.next(data.totalCount);
      });
  }

  public getAllDevicesofOnePharmacy(index?: number, perPage?: number, filterObject?: object): any {
    let qS = '';
    if (perPage || filterObject) {
      qS += '?';
    }
    if (perPage) {
      qS = `offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    this.http
      .get(`${this.urls.getDevicesByPharmacyUrl}` + qS)
      .pipe(
        map((data: any) => {
          return data;
        })
      )
      .subscribe(data => {
        this.remoteData.next(data.data);
        this.dataLength.next(data.totalCount);
      });
  }

  public getAvailableDevicesofOnePharmacy(index?: number, perPage?: number, filterObject?: object): any {
    let qS = '';
    if (perPage || filterObject) {
      qS += '?';
    }
    if (perPage) {
      qS = `offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    return this.http.get(`${this.urls.getDevicesByPharmacyUrl}` + qS);
  }

  handleError(error) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }
    return throwError(error || 'Node.js server error');
  }
}
