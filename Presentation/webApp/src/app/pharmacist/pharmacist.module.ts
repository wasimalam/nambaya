import { PharmacistDashboardComponent} from "@app/pharmacist/Views/Dashboard/pharmacist-dashboard.component";
import { CookieService } from 'ngx-cookie-service';
import { TranslateModule } from '@ngx-translate/core';
import { DeviceUnAssignComponent} from "@app/pharmacist/Devices/Device-UnAssign/device-unassign.component";
import { DeviceListComponent} from "@app/pharmacist/Devices/Device-list/device-list.component";
import { DeviceService} from "@app/pharmacist/Devices/device.service";
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@app/shared';
import { PharmacistRoutingModule } from './pharmacist-routing.module';
import { NgxFileDropModule } from 'ngx-file-drop';
import { ClickOutsideModule } from 'ng-click-outside';

import {
  IgxGridModule,
  IgxIconModule,
  IgxInputGroupModule,
  IgxButtonModule,
  IgxRippleModule,
  IgxDatePickerModule,
  IgxTimePickerModule,
  IgxComboModule,
  IgxDropDownModule,
  IgxSelectModule,
  IgxAutocompleteModule,
  IgxSwitchModule,
  IgxLayoutModule,
  IgxDialogModule,
  IgxColumnPinningModule
} from 'igniteui-angular';
import { PharmacistComponent} from "@app/pharmacist/pharmacist.component";
import { PharmacistAccountsComponent } from '@app/pharmacist/Views/pharmacist-accounts/pharmacist-accounts.component';
import { PharmacistAccountsService } from '@app/pharmacist/Models/pharmacist-accounts.service';
import { CreatePatientComponent} from "@app/pharmacist/Views/create-patient/create-patient.component";
import { EditPatientComponent} from "@app/pharmacist/Views/edit-patient/edit-patient.component";
import { PatientsService } from '@app/pharmacist/Models/patients.service';

import { PipesModule } from '@app/pipes/Pipes.module';
import { PharmacistEditComponent} from "@app/pharmacist/pharmacist-edit/pharmacist-edit.component";
import { PharmacistCreateComponent} from "@app/pharmacist/pharmacist-create/pharmacist-create.component";
import { DeviceCreateComponent} from "@app/pharmacist/Devices/Device-create/device-create.component";
import { DeviceEditComponent} from "@app/pharmacist/Devices/Device-edit/device-edit.component";
import { PharmacistListComponent} from "@app/pharmacist/Views/pharmacist-list/pharmacist-list.component";
import { PharmacyListComponent } from '@app/pharmacist/Views/pharmacy-list/pharmacy-list.component';
import { DeviceAssignComponent} from "@app/pharmacist/Devices/Device-Assign/device-assign.component";
import { OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { CityService } from '@app/shared/services/City.service';
import { PharmacistSettingsComponent} from "@app/pharmacist/Views/pharmacist-settings/pharmacist-settings.component";
import { ImportPatientComponent} from "@app/pharmacist/Views/import-patient/import-patient.component";

import { NgxChartsModule } from '@swimlane/ngx-charts';
import { PharmacistService} from "@app/pharmacist/Models/pharmacist.service";
import { OwlDateTimeModule, OWL_DATE_TIME_FORMATS } from 'ng-pick-datetime';
import { OwlMomentDateTimeModule } from 'ng-pick-datetime/date-time/adapter/moment-adapter/moment-date-time.module';
import {NgxIntlTelInputModule} from "ngx-intl-tel-input";

export const MY_MOMENT_FORMATS = {
  parseInput: 'l LT',
  fullPickerInput: 'l LT',
  datePickerInput: 'l',
  timePickerInput: 'LT',
  monthYearLabel: 'MMM YYYY',
  dateA11yLabel: 'LL',
  monthYearA11yLabel: 'MMMM YYYY'
};

export const MY_CUSTOM_FORMATS = {
  fullPickerInput: 'YYYY.MM.DD HH:mm:ss',
  parseInput: 'YYYY.MM.DD HH:mm:ss',
  datePickerInput: 'YYYY.MM.DD HH:mm:ss',
  timePickerInput: 'LT',
  monthYearLabel: 'MMM YYYY',
  dateA11yLabel: 'LL',
  monthYearA11yLabel: 'MMMM YYYY'
};

export const MY_CUSTOM_FORMATS2 = {
  fullPickerInput: `${
    localStorage.getItem('dateFormat')
      ? localStorage.getItem('dateFormat') + '' + localStorage.getItem('timeFormat')
      : 'DD.MM.YYYY HH:mm:ss'
  } `,
  parseInput: 'DD.MM.YYYY HH:mm:ss',
  datePickerInput: 'DD.MM.YYYY HH:mm:ss',
  timePickerInput: 'LT',
  monthYearLabel: 'MMM YYYY',
  dateA11yLabel: 'LL',
  monthYearA11yLabel: 'MMMM YYYY'
};

@NgModule({
  declarations: [
    PharmacistComponent,
    PharmacistAccountsComponent,
    CreatePatientComponent,
    EditPatientComponent,
    PharmacistCreateComponent,
    PharmacistEditComponent,
    DeviceCreateComponent,
    DeviceEditComponent,
    PharmacistListComponent,
    PharmacyListComponent,
    DeviceAssignComponent,
    DeviceListComponent,
    DeviceUnAssignComponent,
    PharmacistSettingsComponent,
    ImportPatientComponent,
    PharmacistDashboardComponent
  ],
  imports: [
    SharedModule,
    PharmacistRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    IgxGridModule,
    IgxIconModule,
    IgxInputGroupModule,
    IgxButtonModule,
    IgxRippleModule,
    IgxDatePickerModule,
    IgxTimePickerModule,
    IgxComboModule,
    IgxDropDownModule,
    IgxSelectModule,
    ReactiveFormsModule,
    ClickOutsideModule,
    IgxAutocompleteModule,
    PipesModule,
    IgxSwitchModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgxFileDropModule,
    TranslateModule,
    NgxChartsModule,
    IgxLayoutModule,
    OwlMomentDateTimeModule,
    IgxDialogModule,
    IgxColumnPinningModule,
    NgxIntlTelInputModule
  ],
  providers: [
    PharmacistAccountsService,
    PatientsService,
    DeviceService,
    CityService,
    CookieService,
    PharmacistService,
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_CUSTOM_FORMATS2 }
  ]
})
export class PharmacistModule {}
