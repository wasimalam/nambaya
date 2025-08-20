import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { CenterGroupAccountsService } from '@app/centerGroup/Models/center-group-accounts.service';
import { TranslateService } from '@ngx-translate/core';
import { CookieService } from 'ngx-cookie-service';
import * as moment from 'moment';
import { now } from 'moment';
import { TIMEZONE_DATA } from '@app/jsonData/timezones';

@Component({
  selector: 'app-center-group-settings',
  templateUrl: './center-group-settings.component.html',
  styleUrls: ['./center-group-settings.component.scss']
})
export class CenterGroupSettingsComponent implements OnInit, OnDestroy, AfterViewInit {
  centerGroupSettingsForm!: FormGroup;
  centerGroupSettingsData: any;

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
    private centerGroupAccountsService: CenterGroupAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private cookieService: CookieService
  ) {}

  ngOnInit() {
    this.createCenterGroupSettingsForm();

    const settingsRequest = this.centerGroupAccountsService.getSettingsData();
    const languagesRequest = this.centerGroupAccountsService.getLanguages();
    const Options2FRequest = this.centerGroupAccountsService.get2FOptions();

    this.isLoading = true;
    forkJoin([settingsRequest, languagesRequest, Options2FRequest]).subscribe(results => {
      this.centerGroupSettingsData = results[0];
      this.languages = results[1];
      this.options2F = results[2];

      this.centerGroupSettingsForm.controls.dateFormat.setValue('DD.MM.YYYY');
      this.centerGroupSettingsForm.controls.timeFormat.setValue('HH:mm');
      this.centerGroupSettingsForm.controls.timeZone.setValue('Arctic/Longyearbyen');

      for (const result of this.centerGroupSettingsData) {
        if (result.code === 'Language') {
          this.centerGroupSettingsForm.controls.languageCode.setValue(result.value);
        }
        if (result.code === '2FactorNotificationType') {
          this.centerGroupSettingsForm.controls.option2F.setValue(Number(result.value));
        }
        if (result.code === 'DateFormat') {
          this.centerGroupSettingsForm.controls.dateFormat.setValue(result.value);
        }
        if (result.code === 'TimeFormat') {
          this.centerGroupSettingsForm.controls.timeFormat.setValue(result.value);
        }
        if (result.code === 'TimeZone') {
          this.centerGroupSettingsForm.controls.timeZone.setValue(result.value);
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

  updateCenterGroupSettings() {
    if (this.centerGroupSettingsForm.invalid) {
      this.validateAllFormFields(this.centerGroupSettingsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;

      const settingsObject = [];
      settingsObject.push(
        {
          code: 'Language',
          value: this.centerGroupSettingsForm.value.languageCode,
          dataType: 'Language'
        },
        {
          code: '2FactorNotificationType',
          value: this.centerGroupSettingsForm.value.option2F.toString(),
          dataType: 'Lookups'
        },
        {
          code: 'DateFormat',
          value: this.centerGroupSettingsForm.value.dateFormat.toString(),
          dataType: 'DateFormat'
        },
        {
          code: 'TimeFormat',
          value: this.centerGroupSettingsForm.value.timeFormat.toString(),
          dataType: 'TimeFormat'
        },
        {
          code: 'TimeZone',
          value: this.centerGroupSettingsForm.value.timeZone.toString(),
          dataType: 'TimeZone'
        }
      );

      localStorage.setItem('dateFormat', this.centerGroupSettingsForm.value.dateFormat.toString());
      localStorage.setItem('timeFormat', this.centerGroupSettingsForm.value.timeFormat.toString());
      localStorage.setItem('timeZone', this.centerGroupSettingsForm.value.timeZone.toString());

      this.centerGroupAccountsService.updateCenterGroupPreferences(settingsObject).subscribe(
        response => {
          if (response.status === 200) {
            this.cookieService.set('defaultLanguage', this.centerGroupSettingsForm.value.languageCode);
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

  private createCenterGroupSettingsForm() {
    this.centerGroupSettingsForm = this.formBuilder.group({
      languageCode: ['', Validators.required],
      option2F: ['', Validators.required],
      dateFormat: ['', Validators.required],
      timeFormat: ['', Validators.required],
      timeZone: ['', Validators.required]
    });
  }
}
