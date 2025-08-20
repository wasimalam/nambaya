import { Router } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { PatientsService, DoctorContext } from '@app/pharmacist/Models/patients.service';

@Component({
  selector: 'app-doctor-create',
  templateUrl: './doctor-create.component.html',
  styleUrls: ['./doctor-create.component.scss']
})
export class DoctorCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  doctorCreateForm!: FormGroup;
  error: string | undefined;
  isLoading = false;
  towns: string[];
  settings: any;
  urlPrefix = '';
  private doctorContext: DoctorContext;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.createDoctorForm();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
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
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
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
