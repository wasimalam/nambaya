import { CenterEditComponent } from './Views/center-edit/center-edit.component';
import { CenterListComponent } from './Views/center-list/center-list.component';
import { CenterCreateComponent } from './Views/center-create/center-create.component';
import { SendCompletedStudyComponent } from './Views/send-Completed-Study/send-CompletedStudy.component';
import { CompletedStudesListComponent } from './Views/Completed-Studies/completed-studies-list.component';
import { CenterPatientsListComponent } from './Views/Center-PatientList/center-patients-list.component';
import { CenterGroupComponent } from './center-group.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract, AuthenticationGuard } from '@app/core';
import { Shell } from '@app/shell/shell.service';
import { CenterGroupAccountsComponent } from '@app/centerGroup/Views/center-group-accounts/center-group-accounts.component';
import { CenterGroupSettingsComponent } from '@app/centerGroup/Views/center-group-settings/center-group-settings.component';
import { DeactivatedPatientsComponent } from '@app/centerGroup/Views/deactivated-patients/deactivated-patients.component';

const routes: Routes = [
  // {path:'', pathMatch:'full', redirectTo:environment.startingScreen},
  //   // {path:'', pathMatch:'full', redirectTo:'home'},
  //   {path:'*', pathMatch:'full', redirectTo:environment.startingScreen},
  { path: 'center', redirectTo: '/homeAuth', pathMatch: 'full' },

  Shell.childRoutes([
    {
      path: 'center/accounts',
      component: CenterGroupAccountsComponent,
      data: { title: extract('Account Settings') }
    },
    {
      path: 'center/settings',
      component: CenterGroupSettingsComponent,
      data: { title: extract('Settings') }
    },
    {
      path: 'center/dashboard',
      component: CenterGroupComponent,
      data: { title: extract('Dashboard'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/patients',
      component: CenterPatientsListComponent,
      data: { title: extract('Case Studies'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/completedCases',
      component: CompletedStudesListComponent,
      data: { title: extract('Completed Case Studies'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/dispatched-cases',
      component: CompletedStudesListComponent,
      data: { title: extract('dispatched_cases'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/deactivated-patients',
      component: DeactivatedPatientsComponent,
      data: { title: extract('deactivated_patients'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/sendCompletedStudy/:caseId',
      component: SendCompletedStudyComponent,
      data: { title: extract('send_report_to_doctor'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/center/create',
      component: CenterCreateComponent,
      data: { title: extract('add_center_group_user'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/center/edit/:centerId',
      component: CenterEditComponent,
      data: { title: extract('edit_center_group_user'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/center/list',
      component: CenterListComponent,
      data: { title: extract('CENTER GROUP USERS'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CenterGroupRoutingModule {}
