import { ActivatedRoute, Router } from '@angular/router';
import { CardiologistService, NurseContext } from '@app/user/Views/Cardiologist/cardiologist.service';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from '@app/shared/OAuth.Service';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { SearchCountryField, CountryISO} from "ngx-intl-tel-input";

@Component({
  selector: 'app-add-edit-nurse',
  templateUrl: './add-edit-nurse.component.html',
  styleUrls: ['./add-edit-nurse.component.scss']
})
export class AddEditNurseComponent implements OnInit, OnDestroy, AfterViewInit {
  nurseAddEditForm!: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  settings: any;
  public isActivepp = true;
  public isLocked = false;
  urlPrefix = '';
  pageTitle = 'add_nurse';
  nurseId = 0;
  nurseData: any = null;
  private nurseContext: NurseContext;
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
    private translateService: TranslateService,
    private activatedRoute: ActivatedRoute,
    public oAuthService: OAuthService,
    public cardiologistAccountsService: CardiologistAccountsService
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {
    this.addEditNurseForm();

    if (this.activatedRoute.snapshot.params.nurseId) {
      this.nurseId = Number(this.activatedRoute.snapshot.params.nurseId);
    }

    if (this.nurseId > 0) {
      this.isLoading = true;
      this.pageTitle = 'edit_nurse';
      this.isLoading = true;
      this.cardiologistService.getNurse(this.nurseId).subscribe(response => {
        this.nurseData = response;
        this.isLoading = false;
        this.nurseAddEditForm.patchValue(this.nurseData);
        this.isActivepp = this.nurseData.isActive;
        this.isLocked = this.nurseData.isLocked;
      });
    }

    if (this.nurseId === 0) {
      const cardioId = Number(localStorage.getItem('appUserId'));
      this.cardiologistAccountsService.getData(cardioId, 'cardiologist').subscribe(response => {
        this.nurseAddEditForm.patchValue({
          companyID: response.companyID
        });
      });
    }
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  saveNurse() {
    if (this.nurseAddEditForm.invalid) {
      this.validateAllFormFields(this.nurseAddEditForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.nurseAddEditForm.controls.isLocked.setValue(this.isLocked);
      this.nurseContext = this.nurseAddEditForm.value;
      this.nurseContext.role = 'Nurse';
      this.nurseContext.CompanyId = this.nurseAddEditForm.value.companyId;
      let saveNurseCall: any = '';
      if (this.nurseId === 0) {
        this.nurseContext.CardiologistId = Number(this.oAuthService.userData.appuserid);
        if(this.nurseAddEditForm.value.phone) {
          this.nurseContext.phone = this.nurseAddEditForm.controls.phone.value.e164Number;
        }
        saveNurseCall = this.cardiologistService.saveNurse(this.nurseContext);
      } else {
        this.nurseContext.CardiologistId = Number(this.nurseData.cardiologistID);
        this.nurseContext.phone = this.nurseData.phone;
        saveNurseCall = this.cardiologistService.updateNurse(this.nurseContext);
      }

      saveNurseCall.subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('nurse_saved_successfully').subscribe(text => {
              this.toastr.success(text);
            });
            this.nurseAddEditForm.reset();
            this.router.navigate(['cardiologist/nurse/list']);
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

  private addEditNurseForm() {
    this.nurseAddEditForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      companyID: ['', Validators.required],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [''],
      street: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      address: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      zipCode: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(5)])
      ],
      isActive: [''],
      isLocked: [''],
      id: [0]
    });
  }
}
