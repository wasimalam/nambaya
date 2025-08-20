import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DoctorContext, PatientsService } from '@app/pharmacist/Models/patients.service';

@Component({
  selector: 'app-doctor-edit',
  templateUrl: './doctor-edit.component.html',
  styleUrls: ['./doctor-edit.component.scss']
})
export class DoctorEditComponent implements OnInit, OnDestroy, AfterViewInit {
  doctorEditForm!: FormGroup;
  doctorData: any = null;
  error: string | undefined;
  isLoading = false;
  settings: any;
  doctorId: number;
  urlPrefix: string;
  private doctorContext: DoctorContext;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private translateService: TranslateService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.isLoading = true;
    this.doctorId = this.activatedRoute.snapshot.params.doctorId;
    this.createdoctorEditForm();

    this.patientsService.getDoctor(this.doctorId).subscribe(
      response => {
        if (response.status === 200) {
          this.doctorData = response.body;
          this.doctorEditForm.patchValue(this.doctorData);
          if (this.doctorData.doctorID) {
            this.doctorEditForm.controls.doctorID.disable();
          }
          this.isLoading = false;
        }
      },
      error => {}
    );
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateDoctor() {
    if (this.doctorEditForm.invalid) {
      this.validateAllFormFields(this.doctorEditForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.doctorContext = this.doctorEditForm.value;
      this.doctorContext.id = Number(this.doctorId); // PK-ID
      if (this.doctorData.doctorID) {
        this.doctorContext.doctorId = this.doctorData.doctorID;
      }

      this.patientsService.updateDoctor(this.doctorContext).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('doctor_save_success').subscribe(text => {
              this.toastr.success(text);
              this.isLoading = false;
              this.router.navigate([this.urlPrefix + '/doctors-list']);
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

          if (error.error === 'USER_ID_ALREADY_EXISTS') {
            this.translateService.get('Email already exists').subscribe(text => {
              this.toastr.error(text);
            });
          } else {
            this.translateService.get('Something went wrong').subscribe(text => {
              this.toastr.error(text);
            });
          }
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

  private createdoctorEditForm() {
    this.doctorEditForm = this.formBuilder.group({
      id: ['', Validators.required],
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      doctorID: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(9)])
      ],
      companyID: [
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
