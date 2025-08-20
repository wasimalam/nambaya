import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { Observable } from 'rxjs';
import { PhysicianAccountsService, PhysicianAccountsData } from '@app/physician/Models/physician-accounts.service';
import { finalize, tap } from 'rxjs/operators';
import { untilDestroyed } from '@app/core';
import { ToastrService } from 'ngx-toastr';
import { CityService } from '@app/shared/services/City.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-physician-accounts',
  templateUrl: './physician-accounts.component.html',
  styleUrls: ['./physician-accounts.component.scss']
})
export class PhysicianAccountsComponent implements OnInit, OnDestroy, AfterViewInit {
  physicianAccountsForm!: FormGroup;
  physicianAccountsData: Observable<PhysicianAccountsData>;

  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  public applicationId: any = 'physician';
  public userId: any;
  gettingCities = true;
  cities: any[];
  townSelected: string;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private physicianAccountsService: PhysicianAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private cityService: CityService,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
    }
    if (localStorage.getItem('appUserId') !== 'undefined') {
      this.userId = localStorage.getItem('appUserId');
    }

    this.createPhysicianSettingsForm();
    this.getCities();
    this.physicianAccountsData = this.physicianAccountsService.getData(this.userId, this.applicationId).pipe(
      finalize(() => {
        this.isLoading = false;
      }),
      untilDestroyed(this),
      tap(data => {
        this.physicianAccountsForm.patchValue(data);
        // @ts-ignore
        this.townSelected = data.city;
      })
    );
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  public getCities() {
    this.cityService.getCities().subscribe(res => {
      this.cities = res;
      this.gettingCities = false;
    });
  }

  goBack(): void {
    this.location.back();
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updatePhysicianSettings() {
    if (this.physicianAccountsForm.invalid) {
      this.validateAllFormFields(this.physicianAccountsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.physicianAccountsService.updatePhysicianSettings(this.physicianAccountsForm.value, this.userId).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Settings successfully saved').subscribe(text => {
              this.toastr.success(text);
            });

            this.isLoading = false;
          }
        },
        error => {
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if ((field === 'password' || field === 'confirmPassword') && control.value !== '') {
        control.markAsTouched({ onlySelf: true });
      } else {
        control.markAsTouched({ onlySelf: true });
      }
    });
  }

  private createPhysicianSettingsForm() {
    this.physicianAccountsForm = this.formBuilder.group(
      {
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', Validators.email],
        doctorID: ['', Validators.required],
        phone: [''],
        street: [''],
        zipCode: [''],
        city: [''],
        password: ['', Validators.compose([this.customValidator.patternValidator()])],
        confirmPassword: ['']
      },
      {
        validator: this.customValidator.MatchPassword('password', 'confirmPassword')
      }
    );
  }
}
