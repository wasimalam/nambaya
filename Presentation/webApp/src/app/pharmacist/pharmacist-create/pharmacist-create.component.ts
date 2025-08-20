import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { PharmacistAccountsService } from '../Models/pharmacist-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-pharmacist-create',
  templateUrl: './pharmacist-create.component.html'
})
export class PharmacistCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  pharmacistAddForm!: FormGroup;
  error: string | undefined;
  isLoading = false;
  phoneNumber = '^(+d{1,3}[- ]?)?d{10}$';
  settings: any;
  public isActivepp: boolean = true;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private pharmacistService: PharmacistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.createPharmacistAddForm();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  createNewPharmacist() {
    if (this.pharmacistAddForm.invalid) {
      this.validateAllFormFields(this.pharmacistAddForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.pharmacistAddForm.controls.isActive.setValue(this.isActivepp);
      this.pharmacistService.save(this.pharmacistAddForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Pharmacist Saved successfully').subscribe(text => {
              this.toastr.success(text);
            });

            this.isLoading = false;
            this.router.navigate(['/pharmacist/pharmacist-list']);
          }
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

  private createPharmacistAddForm() {
    this.pharmacistAddForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      pharmacyId: [''],
      email: ['', Validators.email],
      phone: ['', [Validators.required]],
      street: ['', Validators.required],
      zipCode: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]],
      isActive: [''],
      address: ['', Validators.required]
    });
  }
}
