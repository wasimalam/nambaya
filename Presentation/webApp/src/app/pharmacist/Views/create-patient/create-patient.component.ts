import { I18nService } from '@app/core';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { PatientsService, DoctorContext } from '@app/pharmacist/Models/patients.service';
import { CityService } from '@app/shared/services/City.service';
import {
  IComboSelectionChangeEventArgs,
  IgxComboComponent,
  IgxDatePickerComponent,
  IgxDialogComponent
} from 'igniteui-angular';
import { PatientContext } from '@app/pharmacist/Models/patients.service';
import * as moment from 'moment';
import { OAuthService } from '@app/shared/OAuth.Service';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-patient',
  templateUrl: './create-patient.component.html',
  styleUrls: ['./create-patient.component.scss']
})
export class CreatePatientComponent implements OnInit, OnDestroy, AfterViewInit {
  patientsCreateForm!: FormGroup;
  doctorCreateForm!: FormGroup;
  genderOptions: any;

  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  invalidDate = false;
  public pharmacyIdPrefix: string;
  public doctors: any;
  public doctorSelected: number[] = [];
  public date: Date = new Date(Date.now());
  public currentLanguage = 'de-DE';
  @ViewChild(IgxDatePickerComponent, { static: true }) public datePicker: IgxDatePickerComponent;
  // @ts-ignore
  @ViewChild('withValueKey', { read: IgxComboComponent }) public comboValueKey: IgxComboComponent;
  @ViewChild('doctorDialog', { static: true }) public doctorDialog: IgxDialogComponent;

  private patientContext: PatientContext;
  private doctorContext: DoctorContext;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private location: Location,
    private toastr: ToastrService,
    private cityService: CityService,
    private oAutheService: OAuthService,
    private translateService: TranslateService,
    private _changeDetectorRef: ChangeDetectorRef,
    private i18nService: I18nService,
    private router: Router
  ) {
    this.currentLanguage = this.i18nService.language;
  }

  ngOnInit() {
    this.isLoading = true;
    this.getPharmacyIdPrefix();
    this.createPatientForm();
    this.createDoctorForm();
    this.datePicker.mask = 'dd.mm.yyyy';
    this.datePicker.format = 'd.M.yyyy';
    this.genderOptions = this.patientsService.getGenderOptions();
    this.getDoctors();
  }

  public getDoctors() {
    this.patientsService.getDoctors().subscribe(
      response => {
        // @ts-ignore
        this.doctors = response.data;
      },
      error => {}
    );
  }

  public getPharmacyIdPrefix() {
    this.patientsService.getPharmacyIdPrefix().subscribe(
      result => {
        this.pharmacyIdPrefix = result;
        this.createPatientForm();
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
      }
    );
  }

  public singleSelection(event: IComboSelectionChangeEventArgs) {
    if (this.comboValueKey && event.added.length) {
      event.newSelection = event.added;
      this.doctorSelected = event.added[0];
      this.comboValueKey.close();
    }
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  openDoctorDialog() {
    this.createDoctorForm();
    this.doctorDialog.open();
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  // tslint:disable-next-line:typedef
  validateDate(date) {
    this.invalidDate = true;
    if (date !== null) {
      const currentDate = new Date();
      const selectedDate = new Date(date);
      if (currentDate > selectedDate) {
        this.invalidDate = false;
      }
    }
  }

  createNewPatient() {
    if (!this.datePicker.value) {
      this.invalidDate = true;
    }

    if (this.patientsCreateForm.invalid || this.invalidDate) {
      this.validateAllFormFields(this.patientsCreateForm);
      this.scrollToError();
    } else {
      this.patientContext = this.patientsCreateForm.value;
      const pharmacyPatientId = this.pharmacyIdPrefix + '-' + this.patientsCreateForm.controls.PharmacyPatientID.value;
      this.patientContext.PharmacyPatientID = pharmacyPatientId;
      let dob = moment(this.datePicker.value).format('YYYY-MM-DD');
      this.patientContext.dateOfBirth = moment.utc(dob).toISOString();
      this.patientContext.caseID = 0;
      this.patientContext.doctorId = this.comboValueKey.selectedItems()[0];

      if (this.oAutheService.userData.rolecode === 'Pharmacy') {
        this.patientContext.PharmacyID = Number(localStorage.getItem('appUserId'));
      } else {
        // Pharmacist
        this.patientContext.PharmacyID = Number(localStorage.getItem('pharmacyId'));
      }

      this.isLoading = true;
      this.patientsService.createPatient(this.patientContext).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('patient_save_success').subscribe(text => {
              this.toastr.success(text);
              this.isLoading = false;
              let patientData = response.body;
              // sessionStorage.setItem('wiz_patientCaseID', patientData.caseId.toString());
              // sessionStorage.setItem('wiz_patientID', patientData.id);
              this.router.navigate(['/pharmacist/patient/edit/' + patientData.id + '/' + patientData.caseID]);
            });
          }
        },
        error => {
          this.isLoading = false;
          if (error.status === 400 && error.error === 'PHARMACY_PATIENT_ID_ALREADY_EXISTS') {
            this.translateService.get(error.error).subscribe(text => {
              this.toastr.error(text);
            });
            return;
          }

          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  createNewDoctor() {
    if (this.doctorCreateForm.invalid) {
      this.validateAllFormFields(this.doctorCreateForm);
      this.scrollToError();
    } else {
      this.doctorContext = this.doctorCreateForm.value;
      this.isLoading = true;
      this.patientsService.createDoctor(this.doctorContext).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('doctor_save_success').subscribe(text => {
              this.toastr.success(text);
              this.isLoading = false;
              this.doctorDialog.close();
              this.getDoctors();
            });
          }
        },
        error => {
          this.isLoading = false;
          if (error.status === 400 && error.error === 'DOCTOR_ID_ALREADY_EXISTS') {
            this.translateService.get(error.error).subscribe(text => {
              this.toastr.error(text);
            });
            return;
          }
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  public formatter = (date: Date) => {
    const dateFormat = localStorage.getItem('dateFormat');
    const timeZone = localStorage.getItem('timeZone');
    return `${moment
      .utc(date)
      .tz(timeZone)
      .format(dateFormat)}`;
  };

  goBack(): void {
    this.location.back();
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if ((field === 'password' || field === 'confirmPassword') && control.value !== '') {
        control.markAsTouched({ onlySelf: true });
      } else {
        control.markAsTouched({ onlySelf: true });
      }
    });
  }

  private createPatientForm() {
    this.patientsCreateForm = this.formBuilder.group({
      FirstName: ['', Validators.required],
      LastName: ['', Validators.required],
      Email: ['', Validators.email],
      dateOfBirth: ['', Validators.required],
      GenderID: ['', Validators.required],
      InsuranceNumber: [''],
      Phone: [''],
      Street: [''],
      zipCode: ['', Validators.compose([this.customValidator.numberValidator()])],
      Address: [''],
      PharmacyPatientID: [
        '',
        [
          Validators.required,
          Validators.compose([this.customValidator.pharmacyIdMaxLengthValidator(20, this.pharmacyIdPrefix)])
        ]
      ],
      doctorId: ['']
    });
  }

  private createDoctorForm() {
    this.doctorCreateForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      doctorId: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(9)])
      ],
      companyId: [
        '',
        [Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(7)])]
      ],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(20)])
      ],
      street: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      address: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      zipCode: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(5)])
      ]
    });
  }
}
