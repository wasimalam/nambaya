import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { PharmacistAccountsService } from '@app/pharmacist/Models/pharmacist-accounts.service';
import { TranslateService } from '@ngx-translate/core';
import { CookieService } from 'ngx-cookie-service';
import { TIMEZONE_DATA } from '@app/jsonData/timezones';
import * as moment from 'moment';
import { now } from 'moment';

@Component({
  selector: 'app-pharmacist-settings',
  templateUrl: './pharmacist-settings.component.html',
  styleUrls: ['./pharmacist-settings.component.scss']
})
export class PharmacistSettingsComponent implements OnInit, OnDestroy, AfterViewInit {
  pharmacistSettingsForm!: FormGroup;
  pharmacistSettingsData: any;

  isLoading = false;
  public applicationId: any = 'center';
  public userId: any;
  options2F: any;
  languages: any;
  dateFormats: any = [];
  timeFormats: any = [];
  timeZones = TIMEZONE_DATA;

  constructor(
    private formBuilder: FormBuilder,
    private pharmacistAccountsService: PharmacistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private cookieService: CookieService
  ) {}

  ngOnInit() {
    this.isLoading = true;
    this.createPharmacistSettingsForm();

    const settingsRequest = this.pharmacistAccountsService.getSettingsData();
    const languagesRequest = this.pharmacistAccountsService.getLanguages();
    const Options2FRequest = this.pharmacistAccountsService.get2FOptions();

    this.isLoading = true;
    forkJoin([settingsRequest, languagesRequest, Options2FRequest]).subscribe(results => {
      this.pharmacistSettingsData = results[0];
      this.languages = results[1];
      this.options2F = results[2];

      this.pharmacistSettingsForm.controls.dateFormat.setValue('DD.MM.YYYY');
      this.pharmacistSettingsForm.controls.timeFormat.setValue('HH:mm');
      this.pharmacistSettingsForm.controls.timeZone.setValue('Arctic/Longyearbyen');

      for (const result of this.pharmacistSettingsData) {
        if (result.code === 'Language') {
          this.pharmacistSettingsForm.controls.languageCode.setValue(result.value);
        }
        if (result.code === '2FactorNotificationType') {
          this.pharmacistSettingsForm.controls.option2F.setValue(Number(result.value));
        }
        if (result.code === 'DateFormat') {
          this.pharmacistSettingsForm.controls.dateFormat.setValue(result.value);
        }
        if (result.code === 'TimeFormat') {
          this.pharmacistSettingsForm.controls.timeFormat.setValue(result.value);
        }
        if (result.code === 'TimeZone') {
          this.pharmacistSettingsForm.controls.timeZone.setValue(result.value);
        }
      }
      this.isLoading = false;

      this.prepareDateFormatValues();
      this.prepareTimeFormatValues();
    });
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

  updatePharmacistSettings() {
    if (this.pharmacistSettingsForm.invalid) {
      this.validateAllFormFields(this.pharmacistSettingsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;

      const settingsObject = [];
      settingsObject.push(
        {
          code: 'Language',
          value: this.pharmacistSettingsForm.value.languageCode,
          dataType: 'Language'
        },
        {
          code: '2FactorNotificationType',
          value: this.pharmacistSettingsForm.value.option2F.toString(),
          dataType: 'Lookups'
        },
        {
          code: 'DateFormat',
          value: this.pharmacistSettingsForm.value.dateFormat.toString(),
          dataType: 'DateFormat'
        },
        {
          code: 'TimeFormat',
          value: this.pharmacistSettingsForm.value.timeFormat.toString(),
          dataType: 'TimeFormat'
        },
        {
          code: 'TimeZone',
          value: this.pharmacistSettingsForm.value.timeZone.toString(),
          dataType: 'TimeZone'
        }
      );

      localStorage.setItem('dateFormat', this.pharmacistSettingsForm.value.dateFormat.toString());
      localStorage.setItem('timeFormat', this.pharmacistSettingsForm.value.timeFormat.toString());
      localStorage.setItem('timeZone', this.pharmacistSettingsForm.value.timeZone.toString());

      this.pharmacistAccountsService.updatePharmacistPreferences(settingsObject).subscribe(
        response => {
          if (response.status === 200) {
            this.cookieService.set('defaultLanguage', this.pharmacistSettingsForm.value.languageCode);

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

          this.translateService.get('Something went wrong').subscribe(text => {
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

  private createPharmacistSettingsForm() {
    this.pharmacistSettingsForm = this.formBuilder.group({
      languageCode: ['', Validators.required],
      option2F: ['', Validators.required],
      dateFormat: ['', Validators.required],
      timeFormat: ['', Validators.required],
      timeZone: ['', Validators.required]
    });
  }
}
