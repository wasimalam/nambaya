import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class CardiologistService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  private CardiologistServiceUrl = new APIUrls().CardiologistServiceBaseUrl;
  private urls = {
    getDataUrl: `${this.CardiologistServiceUrl}/api/v1/cardiologist`,
    getNurseData: `${this.CardiologistServiceUrl}/api/v1/nurse`
  };

  constructor(private http: HttpClient) {
    this.remoteData = new BehaviorSubject([]);
    this.dataLength = new BehaviorSubject(0);
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
      qS += `&orderBy=` + orderBy;
    }

    this.http
      .get(`${this.urls.getDataUrl + qS}`)
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

  public getNurseData(index?: number, perPage?: number, filterObject?: object, orderBy?: string): any {
    let qS = '';
    if (perPage) {
      qS = `?offset=${index}&limit=${perPage}`;
    }
    if (filterObject) {
      qS += `&filter=` + JSON.stringify(filterObject);
    }
    if (orderBy) {
      qS += `&orderBy=` + orderBy;
    }

    this.http
      .get(`${this.urls.getNurseData + qS}`)
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
