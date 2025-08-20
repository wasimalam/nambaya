import { FileUploadComponent } from '@app/shared/fileUpload.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { extract, AuthenticationGuard } from '@app/core';
import { PatientsListComponent } from './patients-list.component';
import { Shell } from '@app/shell/shell.service';
import { LogoutComponent } from '@app/shared/logout.component';
import { LoggedOutComponent } from '@app/shared/loggedOut.component';
import { HomeAuthComponent } from '@app/shared/homeAuth.component';
import { UnAuthorized } from '@app/shared/unauthorized.component';

const routes: Routes = [
  { path: 'loggedout/:user', component: LoggedOutComponent, pathMatch: 'full' },
  { path: 'logout', component: LogoutComponent, pathMatch: 'full' },
  { path: 'unauthorized', component: UnAuthorized, pathMatch: 'full' },
  // { path: '', redirectTo: '/unauthorized', pathMatch: 'full' },
  { path: 'homeAuth', component: HomeAuthComponent, pathMatch: 'full' },

  Shell.childRoutes([
    {
      path: 'pharmacist/patients',
      component: PatientsListComponent,
      data: { title: extract('Patients'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/patients/:filter',
      component: PatientsListComponent,
      data: { title: extract('Patients'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/edfupload/:patientCaseId',
      component: FileUploadComponent,
      data: { title: extract('EDF File Upload'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/edfupload/:patientCaseId/:deviceId',
      component: FileUploadComponent,
      data: { title: extract('EDF File Upload'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PatientsListRoutingModule {}
