import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class PharmacistService {
  public remoteData: BehaviorSubject<any[]>;
  public dataLength: BehaviorSubject<number> = new BehaviorSubject(0);
  // TODO
  private pharmacistServiceUrl = new APIUrls().PharmacistServiceBaseUrl;

  private urls = {
    getData: `${this.pharmacistServiceUrl}/api/v1/pharmacist/pharmacy/`,
    pharmacyStatsUrl: `${this.pharmacistServiceUrl}/api/v1/patient/dashboard/pharmacystats`,
    goalCompletedPercentageUrl: `${this.pharmacistServiceUrl}/api/v1/patient/dashboard/goalcompletedpercent`,
    monthlyCaseStartedUrl: `${this.pharmacistServiceUrl}/api/v1/patient/dashboard/monthlycasesstarted`,
    monthlycasesCompletedUrl: `${this.pharmacistServiceUrl}/api/v1/patient/dashboard/monthlycasescompleted`
  };

  constructor(private http: HttpClient) {
    this.remoteData = new BehaviorSubject([]);
    this.dataLength = new BehaviorSubject(0);
  }

  public getPharmacyStats() {
    return this.http.get(this.urls.pharmacyStatsUrl);
  }

  public getGoalsCompleted() {
    return this.http.get(this.urls.goalCompletedPercentageUrl);
  }
  public getMonthlyCaseStarted() {
    return this.http.get(this.urls.monthlyCaseStartedUrl);
  }
  public getMonthlyCaseCompleted() {
    return this.http.get(this.urls.monthlycasesCompletedUrl);
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
      .get(`${this.urls.getData + qS}`)
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
