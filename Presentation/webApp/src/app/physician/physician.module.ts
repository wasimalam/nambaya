import { TranslateModule } from '@ngx-translate/core';
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
  IgxAutocompleteModule
} from 'igniteui-angular';
import { PhysicianRoutingModule } from './physician-routing.module';
import { PhysicianAccountsComponent } from './Views/physician-accounts/physician-accounts.component';
import { PhysicianComponent } from './physician.component';
import { PhysicianAccountsService } from '@app/physician/Models/physician-accounts.service';
import { PipesModule } from '@app/pipes/Pipes.module';
import { CityService } from '@app/shared/services/City.service';

@NgModule({
  declarations: [PhysicianAccountsComponent, PhysicianComponent],
  imports: [
    SharedModule,
    PhysicianRoutingModule,
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
    TranslateModule
  ],
  providers: [PhysicianAccountsService, CityService]
})
export class PhysicianModule {}
