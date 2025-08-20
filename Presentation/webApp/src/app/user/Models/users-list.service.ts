import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UsersListService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  private userServiceUrl = new APIUrls().NambayaUserServiceBaseUrl;
  private userManagementServiceUrl = new APIUrls().UserManagementServiceBaseUrl;

  private URLS = {
    getUserList: `${this.userServiceUrl}/api/v1/nambayauser`,
    getUserRoles: `${this.userManagementServiceUrl}/api/v1/role/user`,
  }

  constructor(private http: HttpClient) {
    this.remoteData = new BehaviorSubject([]);
  }

  public getUserRoles() {
    return this.http.get(this.URLS.getUserRoles, { observe: 'response' });
  }

  public getData(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
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
      .get(`${this.URLS.getUserList + qS}`)
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
}
