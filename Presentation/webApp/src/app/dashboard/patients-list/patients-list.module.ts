import { DeviceService } from './../../pharmacist/Devices/device.service';
import { PipesModule } from '@app/pipes/Pipes.module';
import { FileUploadComponent } from '@app/shared/fileUpload.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { PatientsListService } from '@app/dashboard/patients-list/patients-list.service';
import { CoreModule } from '@app/core';
import { SharedModule } from '@app/shared';
import { PatientsListRoutingModule } from './patients-list-routing.module';
import { PatientsListComponent } from './patients-list.component';
import { HttpClientModule } from '@angular/common/http';
import {
  IgxGridModule,
  IgxCheckboxModule,
  IgxBadgeModule,
  IgxToastModule,
  IgxProgressBarModule,
  IgxSelectModule,
  IgxCardModule,
  IgxIconModule,
  IgxDialogModule
} from 'igniteui-angular';
import { LogoutComponent } from '@app/shared/logout.component';
import { LoggedOutComponent } from '@app/shared/loggedOut.component';
import { UnAuthorized } from '@app/shared/unauthorized.component';
import { NgxFileDropModule } from 'ngx-file-drop';
import { IgxToggleModule, IgxButtonModule } from 'igniteui-angular';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    CoreModule,
    SharedModule,
    PatientsListRoutingModule,
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
    IgxCardModule,
    IgxIconModule,
    IgxDialogModule
  ],
  providers: [PatientsListService, DeviceService],
  declarations: [PatientsListComponent, LogoutComponent, LoggedOutComponent, UnAuthorized, FileUploadComponent]
})
export class PatientsListModule {}
