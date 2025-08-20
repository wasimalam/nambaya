import { ActivatedRoute, Router } from '@angular/router';
import { PharmacyService } from './../pharmacy.service';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-pharmacy-edit',
  templateUrl: './pharmacy-edit.component.html'
})
export class PharmacyEditComponent implements OnInit, OnDestroy, AfterViewInit {
  pharmacyEditForm!: FormGroup;
  pharmacyData: any = null;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  towns: string[];
  settings: any;
  pharmacyId: number;
  public urlPrefix = '';

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private pharmacyService: PharmacyService,
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
    this.pharmacyId = this.activatedRoute.snapshot.params.pharmacyId;
    this.createPharmacyEditForm();

    const pharmacyRequest = this.pharmacyService.getOne(this.pharmacyId);
    forkJoin([pharmacyRequest]).subscribe(results => {
      this.pharmacyData = results[0];
      this.pharmacyEditForm.patchValue(this.pharmacyData);
      this.isLoading = false;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updatePharmacy() {
    if (this.pharmacyEditForm.invalid) {
      this.validateAllFormFields(this.pharmacyEditForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.pharmacyEditForm.controls.phone.enable();
      this.pharmacyEditForm.controls.phone.setValue(this.pharmacyData.phone);
      this.pharmacyEditForm.controls.identification.setValue(
        this.pharmacyEditForm.controls.identification.value.toString()
      );

      this.pharmacyService.update(this.pharmacyEditForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('pharmacy_successfully_saved').subscribe(text => {
              this.toastr.success(text);
            });
            this.pharmacyEditForm.reset();
            this.isLoading = false;
            this.router.navigate([this.urlPrefix + '/pharmacy/list']);
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

  private createPharmacyEditForm() {
    this.pharmacyEditForm = this.formBuilder.group({
      id: ['', Validators.required],
      name: ['', Validators.required],
      email: ['', Validators.email],
      phone: [{value: '', disabled: true}],
      street: ['', Validators.required],
      address: ['', Validators.required],
      zipCode: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]],
      isActive: [''],
      isLocked: [''],
      identification: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]],
      fax: [''],
      contact: ['']
    });
  }
}
