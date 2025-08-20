import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { Location } from '@angular/common';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { CookieService } from 'ngx-cookie-service';
import * as moment from 'moment';
import { now } from 'moment';
import { TIMEZONE_DATA } from '@app/jsonData/timezones';

@Component({
  selector: 'app-cardiologist-settings',
  templateUrl: './cardiologist-settings.component.html',
  styleUrls: ['./cardiologist-settings.component.scss']
})
export class CardiologistSettingsComponent implements OnInit, OnDestroy, AfterViewInit {
  cardiologistSettingsForm!: FormGroup;
  cardiologistSettingsData: any;

  isLoading = false;
  public applicationId: any = 'cardiologist';
  public userId: any;
  options2F: any;
  languages: any;
  roleCode = '';
  dateFormats: any = [];
  timeFormats: any = [];
  timeZones = TIMEZONE_DATA;

  constructor(
    private formBuilder: FormBuilder,
    private cardiologistAccountsService: CardiologistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private cookieService: CookieService
  ) {}

  ngOnInit() {
    this.createCardiologistSettingsForm();

    this.roleCode = localStorage.getItem('roleCode');

    const settingsRequest = this.cardiologistAccountsService.getSettingsData(this.roleCode);
    const languagesRequest = this.cardiologistAccountsService.getLanguages();
    const Options2FRequest = this.cardiologistAccountsService.get2FOptions();

    this.isLoading = true;
    forkJoin([settingsRequest, languagesRequest, Options2FRequest]).subscribe(results => {
      this.cardiologistSettingsData = results[0];
      this.languages = results[1];
      this.options2F = results[2];

      this.cardiologistSettingsForm.controls.dateFormat.setValue('DD.MM.YYYY');
      this.cardiologistSettingsForm.controls.timeFormat.setValue('HH:mm');
      this.cardiologistSettingsForm.controls.timeZone.setValue('Arctic/Longyearbyen');

      for (const result of this.cardiologistSettingsData) {
        if (result.code === 'Language') {
          this.cardiologistSettingsForm.controls.languageCode.setValue(result.value);
        }
        if (result.code === '2FactorNotificationType') {
          this.cardiologistSettingsForm.controls.option2F.setValue(Number(result.value));
        }
        if (result.code === 'DateFormat') {
          this.cardiologistSettingsForm.controls.dateFormat.setValue(result.value);
        }
        if (result.code === 'TimeFormat') {
          this.cardiologistSettingsForm.controls.timeFormat.setValue(result.value);
        }
        if (result.code === 'TimeZone') {
          this.cardiologistSettingsForm.controls.timeZone.setValue(result.value);
        }
      }
      this.isLoading = false;
    });
    this.prepareDateFormatValues();
    this.prepareTimeFormatValues();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  goBack(): void {
    this.location.back();
  }

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateCardiologistSettings() {
    if (this.cardiologistSettingsForm.invalid) {
      this.validateAllFormFields(this.cardiologistSettingsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;

      const settingsObject = [];
      settingsObject.push(
        {
          code: 'Language',
          value: this.cardiologistSettingsForm.value.languageCode,
          dataType: 'Language'
        },
        {
          code: '2FactorNotificationType',
          value: this.cardiologistSettingsForm.value.option2F.toString(),
          dataType: 'Lookups'
        },
        {
          code: 'DateFormat',
          value: this.cardiologistSettingsForm.value.dateFormat.toString(),
          dataType: 'DateFormat'
        },
        {
          code: 'TimeFormat',
          value: this.cardiologistSettingsForm.value.timeFormat.toString(),
          dataType: 'TimeFormat'
        },
        {
          code: 'TimeZone',
          value: this.cardiologistSettingsForm.value.timeZone.toString(),
          dataType: 'TimeZone'
        }
      );

      localStorage.setItem('dateFormat', this.cardiologistSettingsForm.value.dateFormat.toString());
      localStorage.setItem('timeFormat', this.cardiologistSettingsForm.value.timeFormat.toString());
      localStorage.setItem('timeZone', this.cardiologistSettingsForm.value.timeZone.toString());

      this.cardiologistAccountsService.updateCardiologistPreferences(settingsObject, this.roleCode).subscribe(
        response => {
          if (response.status === 200) {
            this.cookieService.set('defaultLanguage', this.cardiologistSettingsForm.value.languageCode);
            this.translateService.get('Settings successfully saved').subscribe(text => {
              this.toastr.success(text);
            });
          }
          this.isLoading = false;
        },
        error => {
          this.isLoading = false;
          if (error.status === 400 && error.error === 'PHONE_IS_UNVERIFIED') {
            this.translateService.get(error.error).subscribe(text => {
              this.toastr.error(text);
            });
            return;
          }

          this.translateService.get('Something went wrong.').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  prepareDateFormatValues() {
    const dateFormatsArray = [
      {
        text: `${moment(now()).format('DD.MM.YYYY')} (DD.MM.YYYY)`.toString(),
        value: 'DD.MM.YYYY'
      },
      {
        text: `${moment(now()).format('MM.DD.YYYY')} (MM.DD.YYYY)`.toString(),
        value: 'MM.DD.YYYY'
      },
      {
        text: `${moment(now()).format('DD/MM/YYYY')} (DD/MM/YYYY)`.toString(),
        value: 'DD/MM/YYYY'
      },
      {
        text: `${moment(now()).format('MM/DD/YYYY')} (MM/DD/YYYY)`.toString(),
        value: 'MM/DD/YYYY'
      }
    ];

    this.dateFormats = dateFormatsArray;
  }

  prepareTimeFormatValues() {
    const timeFormatsArray = [
      {
        text: `${moment(now()).format('HH:mm')} (HH:mm)`.toString(),
        value: 'HH:mm'
      },
      {
        text: `${moment(now()).format('hh:mm A')} (hh:mm t)`.toString(),
        value: 'hh:mm A'
      }
    ];

    this.timeFormats = timeFormatsArray;
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createCardiologistSettingsForm() {
    this.cardiologistSettingsForm = this.formBuilder.group({
      languageCode: ['', Validators.required],
      option2F: ['', Validators.required],
      dateFormat: ['', Validators.required],
      timeFormat: ['', Validators.required],
      timeZone: ['', Validators.required]
    });
  }
}
