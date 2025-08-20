import { CookieService } from 'ngx-cookie-service';
import { EcgUploadComponent } from '@app/shared/UploadEcg/uploadEcg.component';
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
  IgxProgressBarModule,
  IgxGridModule,
  IgxCheckboxModule,
  IgxBadgeModule,
  IgxToastModule,
  IgxToggleModule,
  IgxLayoutModule,
  IgxDialogModule,
  IgxTabsModule
} from 'igniteui-angular';
import { CardiologistRoutingModule } from './cardiologist-routing.module';
import { CardiologistComponent } from './cardiologist.component';
import { CardiologistAccountsComponent } from '@app/cardiologist/Views/cardiologist-accounts/cardiologist-accounts.component';
import { CardiologistAccountsService } from '@app/cardiologist/Models/cardiologist-accounts.service';
import { PipesModule } from '@app/pipes/Pipes.module';
import { CityService } from '@app/shared/services/City.service';
import { CardiologistPatientsListComponent } from '@app/cardiologist/Views/cardiologist-accounts/Cardiologist-PatientsList/cardiologist-patients-list.component';
import { NgxFileDropModule } from 'ngx-file-drop';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CoreModule } from '@app/core';
import { HttpClientModule } from '@angular/common/http';
import { CardiologistSettingsComponent } from './Views/cardiologist-settings/cardiologist-settings.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { CardiologistSignatureComponent } from './Views/cardiologist-signature/cardiologist-signature.component';
import { DrawComponent } from './Views/cardiologist-signature/draw/draw.component';
import { UploadComponent } from './Views/cardiologist-signature/upload/upload.component';
import { PreviewComponent } from './Views/cardiologist-signature/preview/preview.component';
import { SignaturePadModule } from 'angular2-signaturepad';
import {NgxIntlTelInputModule} from "ngx-intl-tel-input";

@NgModule({
  declarations: [
    CardiologistComponent,
    CardiologistAccountsComponent,
    EcgUploadComponent,
    CardiologistPatientsListComponent,
    CardiologistSettingsComponent,
    CardiologistSignatureComponent,
    DrawComponent,
    UploadComponent,
    PreviewComponent
  ],
  imports: [
    SharedModule,
    CardiologistRoutingModule,
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
    ReactiveFormsModule,
    PipesModule,
    NgxFileDropModule,
    IgxProgressBarModule,
    CommonModule,
    TranslateModule,
    CoreModule,
    SharedModule,
    IgxGridModule,
    IgxCheckboxModule,
    IgxBadgeModule,
    HttpClientModule,
    IgxToastModule,
    NgxFileDropModule,
    IgxProgressBarModule,
    IgxSelectModule,
    IgxToggleModule,
    IgxButtonModule,
    FormsModule,
    ReactiveFormsModule,
    PipesModule,
    NgxChartsModule,
    IgxLayoutModule,
    IgxDialogModule,
    IgxTabsModule,
    SignaturePadModule,
    NgxIntlTelInputModule
  ],
  providers: [CardiologistAccountsService, CityService, CookieService]
})
export class CardiologistModule {}
