import { Router } from '@angular/router';
import { PharmacyService } from './../pharmacy.service';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { SearchCountryField, CountryISO} from "ngx-intl-tel-input";

@Component({
  selector: 'app-pharmacy-create',
  templateUrl: './pharmacy-create.component.html',
  styleUrls: ['./pharmacy-create.component.scss']
})
export class PharmacyCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  pharmacyAddForm!: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  towns: string[];
  settings: any;
  public isActivepp = true;
  public urlPrefix = '';
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private pharmacyService: PharmacyService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.createUserForm();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  createNewPharmacy() {
    if (this.pharmacyAddForm.invalid) {
      this.validateAllFormFields(this.pharmacyAddForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.pharmacyAddForm.controls.identification.setValue(
        this.pharmacyAddForm.controls.identification.value.toString()
      );
      if(this.pharmacyAddForm.value.phone) {
        this.pharmacyAddForm.controls.phone.setValue(
          this.pharmacyAddForm.controls.phone.value.e164Number
        );
      }
      this.pharmacyService.save(this.pharmacyAddForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('pharmacy_successfully_saved').subscribe(text => {
              this.toastr.success(text);
            });
            this.pharmacyAddForm.reset();
            this.isLoading = false;
            this.router.navigate([this.urlPrefix + '/pharmacy/list']);
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

  private createUserForm() {
    this.pharmacyAddForm = this.formBuilder.group({
      name: ['', Validators.required],
      email: ['', Validators.email],
      phone: [''],
      street: ['', Validators.required],
      zipCode: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]],
      identification: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]],
      fax: [''],
      contact: [''],
      isActive: [''],
      address: ['', Validators.required]
    });
  }
}
