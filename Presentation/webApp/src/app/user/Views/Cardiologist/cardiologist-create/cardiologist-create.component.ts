import { Router } from '@angular/router';
import { CardiologistService } from './../cardiologist.service';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { SearchCountryField, CountryISO} from "ngx-intl-tel-input";

@Component({
  selector: 'app-cardiologist-create',
  templateUrl: './cardiologist-create.component.html'
})
export class CardiologistCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  cardiologistAddForm!: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  towns: string[];
  settings: any;
  public isActivepp: boolean = true;
  urlPrefix = '';
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private cardiologistService: CardiologistService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.createCardiologstForm();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  createNewCardiologist() {
    if (this.cardiologistAddForm.invalid) {
      this.validateAllFormFields(this.cardiologistAddForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      if(this.cardiologistAddForm.value.phone) {
        this.cardiologistAddForm.controls.phone.setValue(
          this.cardiologistAddForm.controls.phone.value.e164Number
        );
      }

      this.cardiologistService.save(this.cardiologistAddForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Cardiologist Saved Successfully').subscribe(text => {
              this.toastr.success(text);
            });
            this.cardiologistAddForm.reset();
            this.router.navigate([this.urlPrefix + '/cardiologist/list']);
          }
          this.isLoading = false;
        },
        error => {
          this.translateService.get(error.error).subscribe(text => {
            this.toastr.error(text);
          });
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

  private createCardiologstForm() {
    this.cardiologistAddForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      doctorId: [
        '',
        [
          Validators.required,
          Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(12)])
        ]
      ],
      companyID: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(50)])]],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [''],
      street: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      address: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      zipCode: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(5)])
      ],
      isActive: ['']
    });
  }
}
