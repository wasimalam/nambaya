import { LogListComponent } from './Views/Logging/logs-list.component';
import { CookieService } from 'ngx-cookie-service';
import { EmailSmsTemplateListComponent } from './Views/Email-SMS Templates/List/email-sms-template-list.component';
import { TranslateModule } from '@ngx-translate/core';
import { CityService } from '@app/shared/services/City.service';
import { CardiologistService } from '@app/user/Views/Cardiologist/cardiologist.service';
import { CardiologistEditComponent } from '@app/user/Views/Cardiologist/cardiologist-edit/cardiologist-edit.component';
import { CardiologistCreateComponent } from '@app/user/Views/Cardiologist/cardiologist-create/cardiologist-create.component';
import { PharmacyEditComponent } from './Views/pharmacy/pharmacy-edit/pharmacy-edit.component';
import { PharmacyCreateComponent } from '@app/user/Views/pharmacy/pharmacy-create/pharmacy-create.component';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@app/shared';
import { HttpClientModule } from '@angular/common/http';
import { UserRoutingModule } from '@app/user/user-routing.module';
import { UserComponent } from '@app/user/user.component';
import { UserAccountsComponent } from '@app/user/Views/user-accounts/user-accounts.component';
import {
  IgxGridModule,
  IgxCheckboxModule,
  IgxBadgeModule,
  IgxToastModule,
  IgxAutocompleteModule,
  IgxSwitchModule,
  IgxTooltipModule,
  IgxDialogModule
} from 'igniteui-angular';

import {
  IgxIconModule,
  IgxInputGroupModule,
  IgxButtonModule,
  IgxRippleModule,
  IgxDatePickerModule,
  IgxTimePickerModule,
  IgxComboModule,
  IgxDropDownModule,
  IgxSelectModule,
  IgxLayoutModule
} from 'igniteui-angular';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { UserListComponent } from '@app/user/Views/user-list/user-list.component';
import { UserEditComponent } from '@app/user/Views/user-edit/user-edit.component';
import { UserCreateComponent } from '@app/user/Views/user-create/user-create.component';
import { UsersListService } from '@app/user/Models/users-list.service';
import { PharmacyService } from '@app/user/Views/pharmacy/pharmacy.service';
import { CommonModule } from '@angular/common';
import { CardiologistListComponent } from '@app/user/Views/Cardiologist/cardiologist-list/cardiologist-list.component';
import { PipesModule } from '@app/pipes/Pipes.module';
import { UserSettingsComponent } from './Views/user-settings/user-settings.component';
import { EmailSMSCreateEditComponent } from './Views/Email-SMS Templates/Create-Edit/Email-SMS-Create-Edit.component';
import { ClickOutsideModule } from 'ng-click-outside';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { OwlDateTimeModule } from 'ng-pick-datetime';
import { EmailSmsLogsComponent } from './Views/email-sms-logs/email-sms-logs.component';
import {NgxIntlTelInputModule} from "ngx-intl-tel-input";

@NgModule({
  declarations: [
    UserComponent,
    UserAccountsComponent,
    UserListComponent,
    UserEditComponent,
    UserCreateComponent,
    PharmacyCreateComponent,
    PharmacyEditComponent,
    CardiologistCreateComponent,
    CardiologistEditComponent,
    CardiologistListComponent,
    EmailSMSCreateEditComponent,
    EmailSmsTemplateListComponent,
    LogListComponent,
    UserSettingsComponent,
    EmailSmsLogsComponent
  ],
  imports: [
    UserRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SharedModule,
    FormsModule,
    IgxIconModule,
    IgxInputGroupModule,
    IgxButtonModule,
    IgxRippleModule,
    IgxDatePickerModule,
    IgxTimePickerModule,
    IgxComboModule,
    IgxDropDownModule,
    IgxSelectModule,
    IgxLayoutModule,
    ReactiveFormsModule,
    IgxGridModule,
    IgxCheckboxModule,
    IgxBadgeModule,
    HttpClientModule,
    IgxToastModule,
    IgxAutocompleteModule,
    CommonModule,
    PipesModule,
    IgxSwitchModule,
    TranslateModule,
    ClickOutsideModule,
    IgxTooltipModule,
    NgxChartsModule,
    IgxLayoutModule,
    OwlDateTimeModule,
    IgxDialogModule,
    NgxIntlTelInputModule
  ],
  providers: [UserAccountsService, UsersListService, PharmacyService, CardiologistService, CityService, CookieService]
})
export class UserModule {}
