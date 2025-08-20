import { Router } from '@angular/router';
import { CenterGroupAccountsService } from '@app/centerGroup/Models/center-group-accounts.service';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { TranslateService } from '@ngx-translate/core';
import { SearchCountryField, CountryISO} from "ngx-intl-tel-input";

@Component({
  selector: 'app-center-create',
  templateUrl: './center-create.component.html',
  styleUrls: ['./center-create.component.scss']
})
export class CenterCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  centerAccountsForm!: FormGroup;
  error: string | undefined;
  isLoading = false;
  public isActiveToggle = true;
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private userAccountsService: UserAccountsService,
    private centerService: CenterGroupAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.createCenterForm();
  }

  ngOnDestroy() {}

  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  createNewCenter() {
    if (this.centerAccountsForm.invalid) {
      this.validateAllFormFields(this.centerAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      if(this.centerAccountsForm.value.phone) {
        this.centerAccountsForm.controls.phone.setValue(
          this.centerAccountsForm.controls.phone.value.e164Number
        );
      }
      this.centerService.createCenter(this.centerAccountsForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Center Group User successfully saved').subscribe(text => {
              this.toastr.success(text);
              this.isLoading = false;
              this.router.navigate(['/user/center/list']);
            });
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

  private createCenterForm() {
    this.centerAccountsForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(45)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(45)])]],
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
