import { CardiologistService } from './../cardiologist.service';
import { CardiologistModel } from './../cardiologist.model';
import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-cardiologist-edit',
  templateUrl: './cardiologist-edit.component.html'
})
export class CardiologistEditComponent implements OnInit, OnDestroy, AfterViewInit {
  cardiologistEditForm!: FormGroup;
  cardiologistData: any = null;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  settings: any;
  cardiologistId: number;
  urlPrefix = '';

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private cardiologistService: CardiologistService,
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
    this.cardiologistId = this.activatedRoute.snapshot.params.cardiologistId;
    this.createcardiologistEditForm();
    const cardiologistRequest = this.cardiologistService.getOne(this.cardiologistId);
    forkJoin([cardiologistRequest]).subscribe(results => {
      this.cardiologistData = results[0];
      this.cardiologistEditForm.patchValue(this.cardiologistData);
      this.isLoading = false;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateCardiologist() {
    if (this.cardiologistEditForm.invalid) {
      this.validateAllFormFields(this.cardiologistEditForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.cardiologistEditForm.controls.phone.enable();
      this.cardiologistEditForm.controls.phone.setValue(this.cardiologistData.phone);
      this.cardiologistService.update(this.cardiologistEditForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Cardiologist Saved Successfully').subscribe(text => {
              this.toastr.success(text);
            });
            this.cardiologistEditForm.reset();
            this.isLoading = false;
            this.router.navigate([this.urlPrefix + '/cardiologist/list']);
          }
          this.isLoading = false;
        },
        error => {
          if (error.error === 'USER_ID_ALREADY_EXISTS') {
            this.translateService.get('Email already exists').subscribe(text => {
              this.toastr.error(text);
            });
          } else {
            this.translateService.get('Something went wrong').subscribe(text => {
              this.toastr.error(text);
            });
          }
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

  private createcardiologistEditForm() {
    this.cardiologistEditForm = this.formBuilder.group({
      id: ['', Validators.required],
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      doctorID: [
        '',
        [
          Validators.required,
          Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(12)])
        ]
      ],
      companyID: ['', Validators.required],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [{value: '', disabled: true}],
      street: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      address: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      zipCode: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(5)])
      ],
      isActive: [''],
      isLocked: ['']
    });
  }
}
