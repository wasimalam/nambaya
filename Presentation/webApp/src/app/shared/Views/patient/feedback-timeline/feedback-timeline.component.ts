import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { FeedbackTimelineService, FeedbackTimelineContext } from '@app/shared/services/feedbackTimeline.service';
import { TranslateService } from '@ngx-translate/core';
import * as moment from 'moment';
import 'moment-timezone/index';

@Component({
  providers: [FeedbackTimelineService],
  selector: 'app-feedback-timeline',
  templateUrl: './feedback-timeline.component.html',
  styleUrls: ['./feedback-timeline.component.scss']
})
export class FeedbackTimelineComponent implements OnInit, OnDestroy, AfterViewInit {
  feedbackTimelineForm!: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  patientCaseId: number;
  timeLineData: any = null;
  isCardiologist = false;

  private feedbackTimelineContext: FeedbackTimelineContext;

  constructor(
    private formBuilder: FormBuilder,
    private feedbackTimelineService: FeedbackTimelineService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private customValidator: CustomValidatorService,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;
    this.createfeedbackTimelineForm();
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;
    this.getTimeLineData();
  }

  getTimeLineData() {
    this.feedbackTimelineService.getFeedbackTimelineData(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          const timelineData = response.body;
          const dateFormat = localStorage.getItem('dateFormat');
          const timeFormat = localStorage.getItem('timeFormat');
          const timeZone = localStorage.getItem('timeZone');
          this.timeLineData = timelineData.map(data => {
            data.createdOn = `${moment
              .utc(data.createdOn)
              .tz(timeZone)
              .format(dateFormat + ' ' + timeFormat)}`.toString();
            return data;
          });
        }
      },
      error => {}
    );
  }

  ngOnDestroy() {}
  ngAfterViewInit() {
    const applicationId = localStorage.getItem('applicationId');
    if (applicationId === 'cardiologist') {
      this.isCardiologist = true;
      this.feedbackTimelineForm.disable();
    }
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateFeedbackTimeline() {
    if (this.feedbackTimelineForm.invalid) {
      this.validateAllFormFields(this.feedbackTimelineForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.feedbackTimelineContext = this.feedbackTimelineForm.value;
      this.feedbackTimelineContext.patientCaseID = Number(this.patientCaseId);

      this.feedbackTimelineService.updateFeedbackTimeline(this.feedbackTimelineContext).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Feedback added to the timeline').subscribe(text => {
              this.toastr.success(text);
            });
            this.isLoading = false;
            this.feedbackTimelineForm.reset();
            this.getTimeLineData();
          }
        },
        error => {
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
          this.isLoading = false;
        }
      );
    }
  }

  goBack(): void {
    this.location.back();
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createfeedbackTimelineForm() {
    this.feedbackTimelineForm = this.formBuilder.group({
      notes: ['', [Validators.required, Validators.maxLength(500)]]
    });
  }
}
