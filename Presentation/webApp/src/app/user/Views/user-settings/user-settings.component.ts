import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { TranslateService } from '@ngx-translate/core';
import { CookieService } from 'ngx-cookie-service';
import { TIMEZONE_DATA } from '@app/jsonData/timezones';
import * as moment from 'moment';
import { now } from 'moment';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.scss']
})
export class UserSettingsComponent implements OnInit, OnDestroy, AfterViewInit {
  userSettingsForm!: FormGroup;
  userSettingsData: any;

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
    private userAccountsService: UserAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private translateService: TranslateService,
    private cookieService: CookieService
  ) {}

  ngOnInit() {
    this.createUserSettingsForm();

    const settingsRequest = this.userAccountsService.getSettingsData();
    const languagesRequest = this.userAccountsService.getLanguages();
    const Options2FRequest = this.userAccountsService.get2FOptions();

    this.isLoading = true;
    forkJoin([settingsRequest, languagesRequest, Options2FRequest]).subscribe(results => {
      this.userSettingsData = results[0];
      this.languages = results[1];
      this.options2F = results[2];

      this.userSettingsForm.controls.dateFormat.setValue('DD.MM.YYYY');
      this.userSettingsForm.controls.timeFormat.setValue('HH:mm');
      this.userSettingsForm.controls.timeZone.setValue('Arctic/Longyearbyen');

      for (const result of this.userSettingsData) {
        if (result.code === 'Language') {
          this.userSettingsForm.controls.languageCode.setValue(result.value);
        }
        if (result.code === '2FactorNotificationType') {
          this.userSettingsForm.controls.option2F.setValue(Number(result.value));
        }
        if (result.code === 'DateFormat') {
          this.userSettingsForm.controls.dateFormat.setValue(result.value);
        }
        if (result.code === 'TimeFormat') {
          this.userSettingsForm.controls.timeFormat.setValue(result.value);
        }
        if (result.code === 'TimeZone') {
          this.userSettingsForm.controls.timeZone.setValue(result.value);
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

  updateUserSettings() {
    if (this.userSettingsForm.invalid) {
      this.validateAllFormFields(this.userSettingsForm);
      this.scrollToError();
    } else {
      this.isLoading = true;

      const settingsObject = [];
      settingsObject.push(
        {
          code: 'Language',
          value: this.userSettingsForm.value.languageCode,
          dataType: 'Language'
        },
        {
          code: '2FactorNotificationType',
          value: this.userSettingsForm.value.option2F.toString(),
          dataType: 'Lookups'
        },
        {
          code: 'DateFormat',
          value: this.userSettingsForm.value.dateFormat.toString(),
          dataType: 'DateFormat'
        },
        {
          code: 'TimeFormat',
          value: this.userSettingsForm.value.timeFormat.toString(),
          dataType: 'TimeFormat'
        },
        {
          code: 'TimeZone',
          value: this.userSettingsForm.value.timeZone.toString(),
          dataType: 'TimeZone'
        }
      );

      localStorage.setItem('dateFormat', this.userSettingsForm.value.dateFormat.toString());
      localStorage.setItem('timeFormat', this.userSettingsForm.value.timeFormat.toString());
      localStorage.setItem('timeZone', this.userSettingsForm.value.timeZone.toString());

      this.userAccountsService.updateUserPreferences(settingsObject).subscribe(
        response => {
          if (response.status === 200) {
            this.cookieService.set('defaultLanguage', this.userSettingsForm.value.languageCode);
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

  private createUserSettingsForm() {
    this.userSettingsForm = this.formBuilder.group({
      languageCode: ['', Validators.required],
      option2F: ['', Validators.required],
      dateFormat: ['', Validators.required],
      timeFormat: ['', Validators.required],
      timeZone: ['', Validators.required]
    });
  }
}
