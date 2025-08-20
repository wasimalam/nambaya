import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { TranslateService } from '@ngx-translate/core';
import {UsersListService} from "@app/user/Models/users-list.service";

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.scss']
})
export class UserEditComponent implements OnInit, OnDestroy, AfterViewInit {
  userAccountsForm!: FormGroup;
  userAccountsData: any = null;
  userRoles: any = null;
  error: string | undefined;
  isLoading = false;
  public userId: number;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private userAccountsService: UserAccountsService,
    private usersListService: UsersListService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService
  ) {}
  ngOnInit() {
    this.isLoading = true;
    this.userId = this.activatedRoute.snapshot.params.userId;
    this.createUserSettingsForm();

    const userRequest = this.userAccountsService.getUserData(this.userId);
    const userRolesRequest = this.usersListService.getUserRoles()
    forkJoin([userRequest, userRolesRequest]).subscribe(results => {
      this.userAccountsData = results[0];
      this.userRoles = results[1].body;
      this.userAccountsForm.patchValue(this.userAccountsData);
      this.isLoading = false;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateUser() {
    if (this.userAccountsForm.invalid) {
      this.validateAllFormFields(this.userAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.userAccountsForm.controls.phone.enable();
      this.userAccountsForm.controls.phone.setValue(this.userAccountsData.phone);
      this.userAccountsService.updateUser(this.userAccountsForm.value, this.userId).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('user_successfully_saved').subscribe(text => {
              this.toastr.success(text);
            });
            this.location.back();
            this.isLoading = false;
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

  private createUserSettingsForm() {
    this.userAccountsForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [{value: '', disabled: true}],
      role: ['', Validators.required],
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
