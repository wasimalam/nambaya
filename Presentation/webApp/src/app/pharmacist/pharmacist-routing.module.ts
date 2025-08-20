import { PharmacistDashboardComponent} from "@app/pharmacist/Views/Dashboard/pharmacist-dashboard.component";
import { DeviceAssignComponent} from "@app/pharmacist/Devices/Device-Assign/device-assign.component";
import { PharmacistEditComponent} from "@app/pharmacist/pharmacist-edit/pharmacist-edit.component";
import { PharmacistCreateComponent} from "@app/pharmacist/pharmacist-create/pharmacist-create.component";
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract, AuthenticationGuard } from '@app/core';
import { Shell } from '@app/shell/shell.service';
import { PharmacistAccountsComponent } from '@app/pharmacist/Views/pharmacist-accounts/pharmacist-accounts.component';
import { DeviceCreateComponent} from "@app/pharmacist/Devices/Device-create/device-create.component";
import { DeviceEditComponent} from "@app/pharmacist/Devices/Device-edit/device-edit.component";
import { PharmacyListComponent } from '@app/pharmacist/Views/pharmacy-list/pharmacy-list.component';
import { DeviceListComponent} from "@app/pharmacist/Devices/Device-list/device-list.component";
import { DeviceUnAssignComponent} from "@app/pharmacist/Devices/Device-UnAssign/device-unassign.component";
import { PharmacistListComponent } from '@app/pharmacist/Views/pharmacist-list/pharmacist-list.component';
import { EditPatientComponent } from '@app/pharmacist/Views/edit-patient/edit-patient.component';
import { PharmacistSettingsComponent } from '@app/pharmacist/Views/pharmacist-settings/pharmacist-settings.component';
import { ImportPatientComponent } from '@app/pharmacist/Views/import-patient/import-patient.component';

const routes: Routes = [
  { path: 'pharmacist', redirectTo: '/homeAuth', pathMatch: 'full' },
  Shell.childRoutes([
    {
      path: 'pharmacist/accounts',
      component: PharmacistAccountsComponent,
      data: { title: extract('Account Settings'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/dashboard',
      component: PharmacistDashboardComponent,
      data: { title: extract('Dashboard'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/settings',
      component: PharmacistSettingsComponent,
      data: { title: extract('Settings'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/pharmacist-create',
      component: PharmacistCreateComponent,
      data: { title: extract('add_pharmacist'), roles: ['Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/pharmacist-list',
      component: PharmacistListComponent,
      data: { title: extract('Pharmacists'), roles: ['Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/device/create',
      component: DeviceCreateComponent,
      data: { title: extract('Add Device'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/device/list',
      component: DeviceListComponent,
      data: { title: extract('Devices'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/device/list/:filter',
      component: DeviceListComponent,
      data: { title: extract('Devices'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/device/edit/:deviceId',
      component: DeviceEditComponent,
      data: { title: extract('Edit Device'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/assigndevice/:patientCaseId/:pharmacyId',
      component: DeviceAssignComponent,
      data: { title: extract('Assign Device'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'unassigndevice/:deviceId/:patientId',
      component: DeviceUnAssignComponent,
      data: { title: extract('UnAssign Device'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/pharmacist-edit/:pharmacistId',
      component: PharmacistEditComponent,
      data: { title: extract('edit_pharmacist'), roles: ['Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/patient/import',
      component: ImportPatientComponent,
      data: { title: extract('Import Patient'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    /*{
      path: 'pharmacist/patient/create',
      component: CreatePatientComponent,
      data: { title: extract('Add Patient'), roles: ['Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },*/
    {
      path: 'pharmacist/patient/edit/:patientId',
      component: EditPatientComponent,
      data: { title: extract('Edit Patient'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/patient/edit/:patientId',
      component: EditPatientComponent,
      data: { title: extract('Edit Patient'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/patient/edit/:patientId/:caseId',
      component: EditPatientComponent,
      data: { title: extract('Edit Patient'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/patient/edit/:patientId/:caseId',
      component: EditPatientComponent,
      data: { title: extract('Edit Patient'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/pharmacy/list',
      component: PharmacyListComponent,
      data: { title: extract('Pharmacies'), roles: [
          'NambayaUser', 'CentralGroupUser', 'Pharmacist', 'Pharmacy', 'PharmacyTrainer'
        ]},
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PharmacistRoutingModule {}
