import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { PatientsService } from '@app/pharmacist/Models/patients.service';
import { IgxDatePickerComponent } from 'igniteui-angular';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { PatientWizardSteps } from '@app/objects/configurations';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-edit-patient',
  templateUrl: './edit-patient.component.html',
  styleUrls: ['./edit-patient.component.scss']
})
export class EditPatientComponent implements OnInit, OnDestroy, AfterViewInit {
  patientsEditForm!: FormGroup;
  genderOptions: any;
  isLoading = false;
  genderSelected: string;
  urlPrefix = '';
  selectedDate: any;
  patientId: number;
  patientCaseId: number;
  patientData: any = null;
  public isPharmacist = false;
  public doctors: any;
  public caseObject: any = null;
  public parentData: any = {
    stepNumber: 0
  };

  @ViewChild(IgxDatePickerComponent, { static: false }) public datePicker: IgxDatePickerComponent;
  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private patientWizardService: PatientWizardService,
    private location: Location,
    private router: Router,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.parentData.stepNumber = PatientWizardSteps.PATIENT_DETAILS;
    this.isLoading = true;
    this.patientId = this.activatedRoute.snapshot.params.patientId;
    this.patientCaseId = this.activatedRoute.snapshot.params.caseId;

    this.createPatientForm();
    const genderRequest = this.patientsService.getGenderOptions();
    const getCaseRequest = this.patientWizardService.getCase(this.patientCaseId);

    forkJoin([genderRequest, getCaseRequest]).subscribe(results => {
      this.genderOptions = results[0];
      // @ts-ignore
      this.caseObject = results[1].body;

      this.patientData = this.caseObject;

      this.patientsEditForm.patchValue(this.patientData);
      this.patientsEditForm.patchValue({
        dateOfBirth: this.formatter(new Date(this.patientData.dateOfBirth))
      });
      this.genderSelected = this.genderOptions.filter(item => item.id === this.patientData.genderID)[0].code;
      this.selectedDate = new Date(this.patientData.dateOfBirth);
      this.isLoading = false;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {
    const applicationId = localStorage.getItem('applicationId');
    if (applicationId === 'pharmacist' || applicationId === 'Pharmacy') {
      this.isPharmacist = true;
    }
  }

  public formatter = (date: Date) => {
    const dateFormat = localStorage.getItem('dateFormat');
    return `${moment(date).format(dateFormat)}`;
  };

  goBack(): void {
    this.location.back();
  }

  goToPatients(): void {
    this.router.navigate(['pharmacist/patients']);
  }

  private createPatientForm() {
    this.patientsEditForm = this.formBuilder.group({
      firstName: [{ value: '', disabled: true }],
      lastName: [{ value: '', disabled: true }],
      dateOfBirth: [{ value: '', disabled: true }],
      genderID: [{ value: '', disabled: true }],
      insuranceNumber: [{ value: '', disabled: true }],
      pharmacyPatientID: [{ value: '', disabled: true }],
      email: [{ value: '', disabled: true }],
      phone: [{ value: '', disabled: true }]
    });
  }
}
