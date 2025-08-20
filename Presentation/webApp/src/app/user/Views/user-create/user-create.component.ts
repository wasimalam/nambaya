import { Router } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { UserAccountsService} from '@app/user/Models/user-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { TranslateService } from '@ngx-translate/core';
import { SearchCountryField, CountryISO} from "ngx-intl-tel-input";
import {UsersListService} from "@app/user/Models/users-list.service";

@Component({
  selector: 'app-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user-create.component.scss']
})
export class UserCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  userAccountsForm!: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  public isActiveToggle: boolean = true;
  userRoles: any;
  separateDialCode = true;
  SearchCountryField = SearchCountryField;
  CountryISO = CountryISO;
  preferredCountries: CountryISO[] = [CountryISO.Germany, CountryISO.SouthAfrica];

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private userAccountsService: UserAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService,
    private usersListService: UsersListService,
  ) {}

  ngOnInit() {
    this.createUserForm();
    this.usersListService.getUserRoles().subscribe(response => {
      this.userRoles = response.body;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  createNewUser() {
    if (this.userAccountsForm.invalid) {
      this.validateAllFormFields(this.userAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      if(this.userAccountsForm.value.phone) {
        this.userAccountsForm.controls.phone.setValue(
          this.userAccountsForm.controls.phone.value.e164Number
        );
      }
      this.userAccountsService.createUser(this.userAccountsForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('User Saved Successfully').subscribe(text => {
              this.toastr.success(text);
            });
            this.isLoading = false;
            this.router.navigate(['/user/user-list']);
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
    this.userAccountsForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [''],
      role: ['', Validators.required],
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
