import { CardiologistComponent } from './cardiologist.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { extract, AuthenticationGuard } from '@app/core';
import { Shell } from '@app/shell/shell.service';
import { CardiologistAccountsComponent } from './Views/cardiologist-accounts/cardiologist-accounts.component';
import { CardiologistPatientsListComponent } from '@app/cardiologist/Views/cardiologist-accounts/Cardiologist-PatientsList/cardiologist-patients-list.component';
import { EcgUploadComponent } from '@app/shared/UploadEcg/uploadEcg.component';
import { CardiologistSettingsComponent } from '@app/cardiologist/Views/cardiologist-settings/cardiologist-settings.component';
import { DrawComponent } from '@app/cardiologist/Views/cardiologist-signature/draw/draw.component';
import { UploadComponent } from '@app/cardiologist/Views/cardiologist-signature/upload/upload.component';
import { PreviewComponent } from '@app/cardiologist/Views/cardiologist-signature/preview/preview.component';
import { AddEditNurseComponent } from '@app/nurses/Views/add-edit-nurse/add-edit-nurse.component';
import { ListNurseComponent } from '@app/nurses/Views/list-nurse/list-nurse.component';

const routes: Routes = [
  { path: 'cardiologist', redirectTo: '/homeAuth', pathMatch: 'full' },

  Shell.childRoutes([
    {
      path: 'cardiologist/accounts',
      component: CardiologistAccountsComponent,
      data: { title: extract('Account Settings'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/settings',
      component: CardiologistSettingsComponent,
      data: { title: extract('Settings'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/dashboard',
      component: CardiologistComponent,
      data: { title: extract('Dashboard'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/patients/list/openCases',
      component: CardiologistPatientsListComponent,
      data: { title: extract('Patients'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/patients/list/assignedCases',
      component: CardiologistPatientsListComponent,
      data: { title: extract('Patients'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/patients/list',
      component: CardiologistPatientsListComponent,
      data: { title: extract('Patients'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/ecgupload/:patientcaseId',
      component: EcgUploadComponent,
      data: { title: extract('File Upload'), roles: ['Cardiologist', 'Nurse'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/signature/draw',
      component: DrawComponent,
      data: { title: extract('Draw'), roles: ['Cardiologist'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/signature/upload',
      component: UploadComponent,
      data: { title: extract('Upload'), roles: ['Cardiologist'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/signature/preview',
      component: PreviewComponent,
      data: { title: extract('Preview'), roles: ['Cardiologist'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/nurse',
      component: AddEditNurseComponent,
      data: { title: extract('add_nurse'), roles: ['Cardiologist'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/nurse/edit/:nurseId',
      component: AddEditNurseComponent,
      data: { title: extract('edit_nurse'), roles: ['Cardiologist'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'cardiologist/nurse/list',
      component: ListNurseComponent,
      data: { title: extract('Nurses'), roles: ['Cardiologist'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CardiologistRoutingModule {}
