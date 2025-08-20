import { MessageReceived, SignalRService } from '@app/shared/services/signal-r.service';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { IgxDialogComponent, IgxGridComponent, IgxRadioComponent, IRowSelectionEventArgs } from 'igniteui-angular';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { QuickEvaluationContext, QuickEvaluationService } from '@app/shared/services/quickEvaluation.service';
import { FileUploadService } from '@app/shared/fileUpload.service';
import { DomSanitizer } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { OAuthService } from '@app/shared/OAuth.Service';
import { CardioWizardComponent } from '@app/shared/Views/wizard/cardio-wizard/cardio-wizard.component';
import { CaseStatuses, PatientWizardSteps } from '@app/objects/configurations';

@Component({
  providers: [QuickEvaluationService],
  selector: 'app-quick-evaluation',
  templateUrl: './quick-evaluation.component.html',
  styleUrls: ['./quick-evaluation.component.scss']
})
export class QuickEvaluationComponent implements OnInit, OnDestroy, AfterViewInit {
  quickEvaluationForm!: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  patientCaseId: number;
  patientId: number;
  pharmacyId: number;
  quickEvaluationData: any = null;
  quickEvaluationDataFromEdf: any = null;
  quickEvaluationImages: any;
  quickOptionsData: any = null;
  isFirstSave: boolean;
  isCardiologist = false;
  isPharmacist = false;
  drugRadioDisabled = false;
  caseFinalizedByPharmacist: boolean;
  images = [];
  public gettingResult = true;
  public gettingImages = true;
  public executingQE = false;
  public progress = 0;
  public gettingImagesLabel = 'loading_please_wait';
  public patientEDFfile: any;
  public quickResultId: any = null;
  public urlPrefix = '';
  public parentData: any = {
    stepNumber: 0,
    cardioWizardCurrentStep: 2
  };

  QEStatuses = {
    '511': 'Green',
    '512': 'Yellow',
    '513': 'Orange',
    '514': 'Red',
    '515': 'Red Red'
  };

  qeTableData = [
    {
      id: 515,
      result: 'Red Red',
      range: '> 500ms',
      proposition: 'quick_eval_table.red_red_proposition',
      implications: 'quick_eval_table.red_red_implications'
    },
    {
      id: 514,
      result: 'Red',
      range: '440 ~ 500ms',
      proposition: 'quick_eval_table.red_proposition',
      implications: 'quick_eval_table.red_implications'
    },
    {
      id: 513,
      result: 'Orange',
      range: '400 ~ 440ms',
      proposition: 'quick_eval_table.orange_proposition',
      implications: 'quick_eval_table.orange_implications'
    },
    {
      id: 511,
      result: 'Green',
      range: '330 ~ 400ms',
      proposition: 'quick_eval_table.green_proposition',
      implications: ''
    },
    {
      id: 512,
      result: 'Yellow',
      range: '< 330ms',
      proposition: 'quick_eval_table.yellow_proposition',
      implications: 'quick_eval_table.yellow_implications'
    }
  ];

  // @ts-ignore
  @ViewChild(CardioWizardComponent) cardioWizard: CardioWizardComponent;

  @ViewChild('drugRadio', { static: true }) public drugRadio: IgxRadioComponent;
  @ViewChild('caseConfirmDialog', { static: true }) public caseConfirmDialog: IgxDialogComponent;
  @ViewChild('qeTableGrid', { static: true }) public qeTableGrid: IgxGridComponent;

  public disabledCollection: any[] = [];
  public caseObject: any = null;
  public caseStatuses = CaseStatuses;
  private quickEvaluationContext: QuickEvaluationContext;
  private remainingTime: any = '';

  constructor(
    private formBuilder: FormBuilder,
    private quickEvaluationService: QuickEvaluationService,
    private patientWizardService: PatientWizardService,
    private fileUploadServie: FileUploadService,
    private location: Location,
    private router: Router,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private customValidator: CustomValidatorService,
    private translateService: TranslateService,
    private _sanitizer: DomSanitizer,
    private oAuthService: OAuthService,
    public signalRService: SignalRService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  public handleRowSelection(event: IRowSelectionEventArgs) {
    if (this.caseFinalizedByPharmacist) {
      event.newSelection = event.oldSelection;
    }

    this.quickResultId = null;
    if (event.added.length) {
      this.quickResultId = event.added[0];
    }
  }

  listenToSignalR() {
    this.executingQE = true;
    this.gettingImagesLabel = 'quick_evaluation_in_queue';
    this.progress = 0;
    this.signalRService.signalReceived.subscribe((patientData: MessageReceived) => {
      if (patientData.step === 'EVALUATION_STARTED') {
        this.gettingImagesLabel = 'evaluation_started';
      }

      if (patientData.step === 'EDF_FILE_DOWNLOADING') {
        this.gettingImagesLabel = 'file_download_started';
      }

      if (patientData.step === 'EDF_FILE_DOWNLOADED') {
        this.gettingImagesLabel = 'file_download_completed';
        setTimeout(() => {
          this.gettingImagesLabel = 'execute_quick_evaluation';
        }, 1500);
        if (patientData.remainingTime > 0 && !this.remainingTime) {
          this.remainingTime = patientData.remainingTime;
          const progressStep = 100 / this.remainingTime;

          for (let i = 1; i <= this.remainingTime; i++) {
            setTimeout(() => {
              this.progress = Number(this.progress) + Number(progressStep);
            }, 1000 * i);
          }
        }
      }
      if (patientData.step === 'EVALUATION_TIMESTAMP' && !this.remainingTime) {
        this.gettingImagesLabel = 'execute_quick_evaluation';
        if (patientData.remainingTime > 0) {
          this.remainingTime = patientData.remainingTime;
          const progressStep = 100 / this.remainingTime;
          for (let i = 1; i <= this.remainingTime; i++) {
            setTimeout(() => {
              this.progress = Number(this.progress) + Number(progressStep);
            }, 1000 * i);
          }
        }
      }
      if (patientData.step === 'EVALUATION_FINISHED') {
        this.progress = 100;
        clearTimeout();
        this.gettingImagesLabel = 'evaluation_completed';
        setTimeout(() => {
          this.executingQE = false;
        }, 1000);
        this.getQuickEvaluationImages();
      }
    });
    this.signalRService.addQEPrgoressListener();
  }

  getQuickEvaluationImages() {
    if (this.progress === 100) {
      this.gettingImagesLabel = 'getting_images';
    }
    this.quickEvaluationService.getQuickEvaluationImages(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.quickEvaluationImages = response.body;
          this.images = this.quickEvaluationImages.data;
          if (this.images.length > 0) {
            this.gettingImages = false;
          } else {
            this.listenToSignalR();
          }
        }
      },
      error => {
        if (error.status === 404) {
          this.listenToSignalR();
          this.isFirstSave = true;
        } else {
          this.isFirstSave = false;
        }
      }
    );
  }

  ngOnInit() {
    const applicationId = localStorage.getItem('applicationId');
    this.parentData.stepNumber = PatientWizardSteps.QUICK_EVALUATION;
    if (applicationId === 'pharmacist' || applicationId === 'Pharmacy') {
      this.isPharmacist = true;
    }

    if (applicationId === 'cardiologist') {
      this.isCardiologist = true;
    }
    this.isLoading = true;
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;
    this.signalRService.startConnection(this.patientCaseId);
    this.createQuickEvaluationForm();
    this.patientId = Number(sessionStorage.getItem('wiz_patientID'));
    this.pharmacyId = Number(localStorage.getItem('pharmacyId'));
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;

    this.quickEvaluationService.getQuickEvaluation(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.quickEvaluationData = response.body;
          this.quickEvaluationData.hours = response.body.measurementTime.split(':')[0];
          this.quickEvaluationData.minutes = response.body.measurementTime.split(':')[1];
          this.quickResultId = response.body.quickResultID;
          this.qeTableGrid.selectRows([this.quickResultId], true);
          this.qeTableGrid.cdr.detectChanges();
          this.quickEvaluationForm.patchValue(this.quickEvaluationData);
          this.gettingResult = false;
          this.isLoading = false;
        }
      },
      error => {
        if (error.status === 404) {
          this.isFirstSave = true;
          this.fileUploadServie.getPatientEDFFile(this.patientCaseId).subscribe(response => {
            if (response.status === 200) {
              this.patientEDFfile = response.body;
              if (this.patientEDFfile.duration) {
                this.quickEvaluationDataFromEdf = this.patientEDFfile;
                this.quickEvaluationDataFromEdf.hours = Math.floor(this.patientEDFfile.duration / 3600);
                this.quickEvaluationDataFromEdf.minutes = Math.floor((this.patientEDFfile.duration % 3600) / 60);
                this.quickEvaluationForm.patchValue(this.quickEvaluationDataFromEdf);
                this.quickEvaluationForm.controls.hours.disable();
                this.quickEvaluationForm.controls.minutes.disable();
              }
            }
          });
        } else {
          this.isFirstSave = false;
        }
        this.gettingResult = false;
        this.isLoading = false;
      }
    );

    this.patientWizardService.getCase(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.caseObject = response.body;
          if (this.caseObject.statusID >= 655) {
            this.caseFinalizedByPharmacist = true;
            this.quickEvaluationForm.disable();
            this.drugRadioDisabled = true;
            this.qeTableData.map(item => {
              this.disabledCollection.push(item.id);
            });
          }

          if (this.caseObject.statusID < 655) {
            this.oAuthService.observableUser.subscribe(user => {
              if (user) {
                if (this.oAuthService.userData.rolecode === 'CentralGroupUser') {
                  this.location.back();
                }
              }
            });
          }
          this.getQuickEvaluationImages();
        }
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );

    this.quickEvaluationService.getQuickOptions().subscribe(
      response => {
        this.quickOptionsData = response;
      },
      error => {
        this.isFirstSave = error.status === 404 ? true : false;
      }
    );
  }

  ngOnDestroy() {
    this.signalRService.stopConnection();
  }

  // tslint:disable-next-line:typedef
  public openImageInNewTab(src) {
    const newTab = window.open();
    newTab.document.body.innerHTML = `<img src="data:image/jpg;base64,${src}">`;
  }

  ngAfterViewInit() {
    if (this.isCardiologist) {
      this.quickEvaluationForm.disable();
    }
  }

  openAssignCaseDialog() {
    this.cardioWizard.openAssignCaseDialog();
  }

  openCaseConfirmDialog() {
    if (!this.quickEvaluationForm.invalid) {
      if (!this.quickResultId) {
        const firstElementWithError = document.querySelector('.qeOption');
        firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
        return;
      }

      this.caseConfirmDialog.open();
    } else {
      this.validateAllFormFields(this.quickEvaluationForm);
      this.scrollToError();
    }
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateQuickEvaluation() {
    if (this.quickEvaluationForm.invalid) {
      this.validateAllFormFields(this.quickEvaluationForm);
      this.scrollToError();
    } else {
      const hours = this.quickEvaluationDataFromEdf
        ? this.quickEvaluationDataFromEdf.hours
        : this.quickEvaluationForm.value.hours;
      const minutes = this.quickEvaluationDataFromEdf
        ? this.quickEvaluationDataFromEdf.minutes
        : this.quickEvaluationForm.value.minutes;
      this.quickEvaluationContext = this.quickEvaluationForm.value;
      this.quickEvaluationContext.quickResultID = this.quickResultId;
      this.quickEvaluationContext.patientCaseID = Number(this.patientCaseId);
      this.quickEvaluationContext.measurementTime = hours + ':' + minutes;
      if (this.quickEvaluationData !== null) {
        this.quickEvaluationContext.id = this.quickEvaluationData.id;
      }

      this.quickEvaluationService.updateQuickEvaluation(this.quickEvaluationContext, this.isFirstSave).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('notifications.quickEvaluations.save_success').subscribe(text => {
              this.toastr.success(text);
            });
            if (this.isPharmacist) {
              this.completeWizard();
            } else {
              this.router.navigate(['pharmacist/dashboard']);
            }
          }
        },
        error => {
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  public completeWizard() {
    this.patientWizardService.updateCaseStep(this.patientCaseId, 527).subscribe(result => {
      this.isLoading = false;
      this.router.navigate(['pharmacist/dashboard']);
    });
  }

  goBack(): void {
    this.router.navigate(['pharmacist/patients']);
  }

  goToPatients(): void {
    if (this.isPharmacist) {
      this.router.navigate(['pharmacist/patients']);
    } else {
      this.location.back();
    }
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createQuickEvaluationForm() {
    this.quickEvaluationForm = this.formBuilder.group({
      hours: ['', [Validators.required, Validators.min(1), Validators.max(48)]],
      minutes: ['', [Validators.required, Validators.min(0), Validators.max(60)]],
      quickResultID: [''],
      notes: ['']
    });
  }
}
