import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class PharmacyService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  private PharmacistBaseUrl = new APIUrls().PharmacistServiceBaseUrl;
  private Urls = {
    getDataUrl: `${this.PharmacistBaseUrl}/api/v1/pharmacy`
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
      .get(`${this.Urls.getDataUrl + qS}`)
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
