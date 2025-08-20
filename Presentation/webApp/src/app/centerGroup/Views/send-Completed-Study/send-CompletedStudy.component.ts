import { ReportContext, PatientsService } from '@app/pharmacist/Models/patients.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { FileUploadService } from '@app/shared/fileUpload.service';
import { TranslateService } from '@ngx-translate/core';
import * as moment from 'moment';
import { IgxDatePickerComponent, IgxDialogComponent } from 'igniteui-angular';

@Component({
  selector: 'app-send-completedStudy',
  templateUrl: './send-CompletedStudy.component.html',
  styleUrls: ['./send-CompletedStudy.component.scss']
})
export class SendCompletedStudyComponent implements OnInit, OnDestroy, AfterViewInit {
  sendCompletedStudyForm!: FormGroup;
  isLoading = false;
  patientCaseId: number;
  isDownloadingdetailedFile: boolean;
  isDownloadingMedicationFile: boolean;
  caseDispatchData: any = null;
  medicationPlan: any = null;
  public date: Date = new Date(Date.now());
  private reportContext: ReportContext;

  @ViewChild(IgxDatePickerComponent, { static: true }) public datePicker: IgxDatePickerComponent;
  @ViewChild('caseDispatchDialog', { static: true }) public caseDispatchDialog: IgxDialogComponent;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private patientService: PatientsService,
    private fileService: FileUploadService,
    private router: Router,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.patientCaseId = Number(this.activatedRoute.snapshot.params.caseId);
    this.createForm();

    this.isLoading = true;
    this.patientService.getDispatchDetails(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.caseDispatchData = response.body;
          this.sendCompletedStudyForm.patchValue(this.caseDispatchData);
        }
        this.isLoading = false;
      },
      error => {
        /*this.isFirstSave = error.status === 404 ? true : false;*/
        /*Setting default Form values*/
        this.sendCompletedStudyForm.patchValue({
          isMedicationPlanAttached: false,
          isDetailEvaluationAttached: false
        });
        this.isLoading = false;
      }
    );

    this.fileService.getMedicationFile(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.medicationPlan = response.body;
        }
      },
      error => {}
    );
  }

  ngOnDestroy() {}

  ngAfterViewInit() {}

  goBack(): void {
    this.location.back();
  }

  public dispatchCaseDetails() {
    this.reportContext = this.sendCompletedStudyForm.value;
    this.reportContext.patientCaseID = this.patientCaseId;
    this.reportContext.isDetailEvaluationAttached = true;
    this.reportContext.dispatchDate = this.datePicker.value.toISOString();
    this.isLoading = true;
    this.patientService.sendCompletedStudy(this.reportContext).subscribe(
      response => {
        if (response.status === 200) {
          this.translateService.get('Saved Successfully').subscribe(text => {
            this.toastr.success(text);
            this.router.navigate(['center/dispatched-cases']);
          });
        }
        this.isLoading = false;
        this.caseDispatchDialog.close();
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
        this.caseDispatchDialog.close();
        this.isLoading = false;
      }
    );
  }

  public downloadDetailedEvaluationReport() {
    this.isDownloadingdetailedFile = true;

    this.fileService.downloadEcgFile(this.patientCaseId).subscribe(
      (response: any) => {
        const headers = response.headers;
        const contentDisposition = headers.get('Content-Disposition').replace(/\s/g, '');
        // tslint:disable-next-line:max-line-length
        const filename = contentDisposition
          .split(';')[1]
          .split('filename')[1]
          .split('=')[1]
          .trim()
          .replace(/['"]+/g, '');
        const dataType = response.body.type;
        const binaryData = [];
        binaryData.push(response.body);
        const downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));
        downloadLink.setAttribute('download', filename);
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.isDownloadingdetailedFile = false;
      },
      error => {
        this.translateService.get(error.statusText).subscribe(text => {
          this.toastr.error(text);
        });
        this.isDownloadingdetailedFile = false;
      }
    );
  }

  public downloadMedicationFile() {
    this.isDownloadingMedicationFile = true;
    this.fileService.downloadMedicationFile(this.patientCaseId).subscribe(
      (response: any) => {
        const headers = response.headers;
        const contentDisposition = headers.get('Content-Disposition').replace(/\s/g, '');
        // tslint:disable-next-line:max-line-length
        const filename = contentDisposition
          .split(';')[1]
          .split('filename')[1]
          .split('=')[1]
          .trim()
          .replace(/['"]+/g, '');
        const dataType = response.body.type;
        const binaryData = [];
        binaryData.push(response.body);
        const downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));
        downloadLink.setAttribute('download', filename);
        document.body.appendChild(downloadLink);
        downloadLink.click();
        this.isDownloadingMedicationFile = false;
      },
      error => {
        this.translateService.get(error.statusText).subscribe(text => {
          this.toastr.error(text);
        });
        this.isDownloadingMedicationFile = false;
      }
    );
  }

  public formatter = (date: Date) => {
    const dateFormat = localStorage.getItem('dateFormat');
    const timeZone = localStorage.getItem('timeZone');

    return `${moment
      .utc(date)
      .tz(timeZone)
      .format(dateFormat)}`;
  };

  private createForm() {
    this.sendCompletedStudyForm = this.formBuilder.group({
      isMedicationPlanAttached: ['']
    });
  }
}
