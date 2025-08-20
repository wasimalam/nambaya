import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { IgxDialogComponent, PositionSettings } from 'igniteui-angular';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AdditionalMedicationService } from '@app/shared/services/additionalMedication.service';
import { TranslateService } from '@ngx-translate/core';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { FileUploadService } from '@app/shared/fileUpload.service';
import { CardioWizardComponent } from '@app/shared/Views/wizard/cardio-wizard/cardio-wizard.component';
import { PatientWizardSteps } from '@app/objects/configurations';

@Component({
  providers: [AdditionalMedicationService],
  selector: 'app-additional-medication',
  templateUrl: './additional-medication.component.html',
  styleUrls: ['./additional-medication.component.scss']
})
export class AdditionalMedicationComponent implements OnInit, OnDestroy, AfterViewInit {
  drugDetailForm!: FormGroup;
  isDrugDetailAddingUnderGroup = false; /*adding under a specific group*/
  error: string | undefined;
  isLoading = false;
  patientCaseId: number;
  patientId: number;
  drugGroups: any = null;
  isCardiologist = false;
  pageTitle: string = 'Medications';
  activeDrugGroupName: any = null;
  isDosageMorningText: boolean;
  isDosageNoonText: boolean;
  isDosageEveningText: boolean;
  isDosageNightText: boolean;
  caseFinalizedByPharmacist: boolean;
  public isPharmacist = false;
  public medicationPlan: null;
  isDownloadingFile = false;
  urlPrefix = '';
  public caseObject: any = null;
  message: string;
  public parentData: any = {
    stepNumber: 0,
    cardioWizardCurrentStep: 1
  };

  public positionSettings: PositionSettings = {
    minSize: { height: 100, width: 500 }
  };

  @ViewChild('drugDetailDialog', { static: true }) public drugDetailDialog: IgxDialogComponent;
  // @ts-ignore
  @ViewChild(CardioWizardComponent) cardioWizard: CardioWizardComponent;

  constructor(
    private formBuilder: FormBuilder,
    private additionalMedicationService: AdditionalMedicationService,
    private patientWizardService: PatientWizardService,
    private location: Location,
    private router: Router,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private fileUploadService: FileUploadService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.parentData.stepNumber = PatientWizardSteps.MEDICATIONS;
    this.isLoading = true;
    this.createDrugDetailForm();
    this.patientCaseId = this.activatedRoute.snapshot.params.patientCaseId;
    this.patientId = this.activatedRoute.snapshot.params.patientId;

    const drugGroupDataRequest = this.additionalMedicationService.getAdditionalMedications(this.patientCaseId);
    this.patientWizardService.getCase(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.caseObject = response.body;
          if (this.caseObject.statusID >= 655) {
            this.caseFinalizedByPharmacist = true;
          }
        }
      },
      error => {}
    );

    forkJoin([drugGroupDataRequest]).subscribe(results => {
      // @ts-ignore
      this.drugGroups = results[0].body;
      this.isLoading = false;
    });
  }

  // tslint:disable-next-line:typedef
  receiveMessage($event) {
    this.message = $event;
  }

  ngOnDestroy() {}
  ngAfterViewInit() {
    this.getMedicationPlan();
    const applicationId = localStorage.getItem('applicationId');
    if (applicationId === 'cardiologist' || applicationId === 'centralgroupuser') {
      this.pageTitle = 'Medications';
    }
    if (applicationId === 'cardiologist') {
      this.isCardiologist = true;
    }
    if (applicationId === 'pharmacist' || applicationId === 'Pharmacy') {
      this.isPharmacist = true;
    }
  }

  getMedicationPlan() {
    this.fileUploadService.getMedicationFile(this.patientCaseId).subscribe(
      response => {
        if (response.status === 200) {
          this.medicationPlan = response.body;
        }
      },
      error => {}
    );
  }
  goBack(): void {
    this.location.back();
  }

  goToPatients(): void {
    if (this.isPharmacist) {
      this.router.navigate(['pharmacist/patients']);
    } else if (this.isCardiologist) {
      this.router.navigate(['cardiologist/patients/list']);
    }
  }

  openDrugDetailDialog(drugDetail?: any, drugGroupName?: string, drugGroupID?: number) {
    this.createDrugDetailForm();
    this.isDrugDetailAddingUnderGroup = false;

    if (drugDetail) {
      if (drugDetail.drugIngredientsList.length === 0) {
        this.addDrugIngredient();
      } else {
        Object.keys(drugDetail.drugIngredientsList).forEach(field => {
          this.addDrugIngredient();
        });
      }
      this.drugDetailForm.patchValue(drugDetail);

      this.isDosageMorningText = drugDetail.isDosageMorning;
      this.isDosageNoonText = drugDetail.isDosageNoon;
      this.isDosageEveningText = drugDetail.isDosageEvening;
      this.isDosageNightText = drugDetail.isDosageNight;
    }

    this.activeDrugGroupName = drugGroupName;

    if (drugGroupID) {
      this.drugDetailForm.controls.drugGroupID.setValue(drugGroupID);
      this.addDrugIngredient();
      this.isDrugDetailAddingUnderGroup = true;
    }
    this.drugDetailDialog.open();
  }

  openAssignCaseDialog() {
    this.cardioWizard.openAssignCaseDialog();
  }

  /* Start  - Medication Plan */

  public downloadFile() {
    this.isDownloadingFile = true;
    this.fileUploadService.downloadMedicationFile(this.patientCaseId).subscribe(
      (response: any) => {
        const headers = response.headers;
        const contentDisposition = headers.get('Content-Disposition').replace(/\s/g, '');
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
        this.isDownloadingFile = false;
      },
      error => {
        this.translateService.get(error.statusText).subscribe(text => {
          this.toastr.error(text);
        });
        this.isDownloadingFile = false;
      }
    );
  }
  /* End  - Medication Plan Upload */

  addDrugIngredient() {
    if (this.drugIngredientsList.length === 3) {
      this.translateService.get('Maximum number of Ingredients reached').subscribe(text => {
        this.toastr.error(text);
      });
      return;
    }
    this.drugIngredientsList.push(this.createDrugIngredient());
  }

  createDrugIngredient(): FormGroup {
    return this.formBuilder.group({
      activeIngredients: [{ value: '', disabled: true }],
      strength: [{ value: '', disabled: true }],
      id: [{ value: 0, disabled: true }],
      drugDetailsID: [{ value: 0, disabled: true }]
    });
  }

  get drugIngredientsList(): FormArray {
    return this.drugDetailForm.get('drugIngredientsList') as FormArray;
  }

  private createDrugDetailForm() {
    this.drugDetailForm = this.formBuilder.group({
      id: [0],
      drugGroupID: [0, Validators.required],
      patientCaseID: [],
      pzn: [{ value: '', disabled: true }],
      medicineName: [{ value: '', disabled: true }],
      dosageForm: [{ value: '', disabled: true }],
      dosageFormCode: [{ value: '', disabled: true }],
      hints: [{ value: '', disabled: true }],
      treatmentReason: [{ value: '', disabled: true }],
      additionalText: [{ value: '', disabled: true }],
      isDosageMorning: [{ value: false, disabled: true }],
      isDosageNoon: [{ value: false, disabled: true }],
      isDosageEvening: [{ value: false, disabled: true }],
      isDosageNight: [{ value: false, disabled: true }],
      dosageMorning: [{ value: false, disabled: true }],
      dosageNoon: [{ value: false, disabled: true }],
      dosageEvening: [{ value: false, disabled: true }],
      dosageNight: [{ value: false, disabled: true }],
      dosageopenScheme: [{ value: false, disabled: true }],
      dosageUnitCode: [{ value: false, disabled: true }],
      dosageUnitText: [{ value: false, disabled: true }],
      drugIngredientsList: this.formBuilder.array([])
    });
  }
}
