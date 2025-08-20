import { CookieService } from 'ngx-cookie-service';
import { CenterEditComponent } from './Views/center-edit/center-edit.component';
import { CenterListComponent } from './Views/center-list/center-list.component';
import { CenterCreateComponent } from './Views/center-create/center-create.component';
import { SendCompletedStudyComponent } from './Views/send-Completed-Study/send-CompletedStudy.component';
import { CompletedStudesListComponent } from './Views/Completed-Studies/completed-studies-list.component';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@app/shared';

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
  IgxAutocompleteModule,
  IgxCheckboxModule,
  IgxGridModule,
  IgxToastModule,
  IgxToggleModule,
  IgxBadgeModule,
  IgxSwitchModule,
  IgxLayoutModule,
  IgxDialogModule
} from 'igniteui-angular';
import { CenterGroupRoutingModule } from './center-group-routing.module';
import { CenterGroupAccountsComponent } from './Views/center-group-accounts/center-group-accounts.component';
import { CenterGroupComponent } from './center-group.component';
import { CenterGroupAccountsService } from '@app/centerGroup/Models/center-group-accounts.service';
import { PipesModule } from '@app/pipes/Pipes.module';
import { CityService } from '@app/shared/services/City.service';
import { CenterGroupSettingsComponent } from './Views/center-group-settings/center-group-settings.component';
import { CenterPatientsListComponent } from './Views/Center-PatientList/center-patients-list.component';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CoreModule } from '@app/core';
import { HttpClientModule } from '@angular/common/http';
import { NgxFileDropModule } from 'ngx-file-drop';
import { PharmacyService } from '@app/user/Views/pharmacy/pharmacy.service';
import { PatientsService } from '@app/pharmacist/Models/patients.service';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { PharmacistService } from '@app/pharmacist/Models/pharmacist.service';
import { DeactivatedPatientsComponent } from './Views/deactivated-patients/deactivated-patients.component';
import { ClickOutsideModule } from 'ng-click-outside';
import {NgxIntlTelInputModule} from "ngx-intl-tel-input";

@NgModule({
  declarations: [
    CenterGroupAccountsComponent,
    CenterGroupComponent,
    CenterGroupSettingsComponent,
    CenterPatientsListComponent,
    CompletedStudesListComponent,
    SendCompletedStudyComponent,
    CenterCreateComponent,
    CenterListComponent,
    CenterEditComponent,
    DeactivatedPatientsComponent
  ],
  imports: [
    SharedModule,
    CenterGroupRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
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
    IgxAutocompleteModule,
    PipesModule,
    ReactiveFormsModule,
    ClickOutsideModule,
    CommonModule,
    TranslateModule,
    CoreModule,
    IgxGridModule,
    IgxCheckboxModule,
    IgxBadgeModule,
    HttpClientModule,
    IgxToastModule,
    NgxFileDropModule,
    IgxToggleModule,
    IgxSwitchModule,
    NgxChartsModule,
    IgxLayoutModule,
    IgxDialogModule,
    NgxIntlTelInputModule
  ],
  providers: [
    CenterGroupAccountsService,
    CityService,
    PatientsListService,
    PharmacyService,
    PatientsService,
    CookieService,
    PharmacistService
  ]
})
export class CenterGroupModule {}
