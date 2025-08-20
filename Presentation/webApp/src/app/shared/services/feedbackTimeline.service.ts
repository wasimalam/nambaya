import { APIUrls } from '@app/shared/Addresses';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

export interface FeedbackTimelineContext {
  patientCaseID: number;
  notes: string;
}
@Injectable()
export class FeedbackTimelineService {
  private patientServiceUrl = new APIUrls().PatientServiceBaseUrl;

  private URLs = {
    caseNotes: `${this.patientServiceUrl}/api/v1/patient/caseNotes`
  };

  constructor(private http: HttpClient) {}

  getFeedbackTimelineData(caseId?: number): any {
    const url = `${this.URLs.caseNotes}/${caseId}`;

    return this.http.get(url, { observe: 'response' });
  }

  updateFeedbackTimeline(feedbackTimelineContext: FeedbackTimelineContext): any {
    return this.http.post(this.URLs.caseNotes, feedbackTimelineContext, { observe: 'response' });
  }
}
