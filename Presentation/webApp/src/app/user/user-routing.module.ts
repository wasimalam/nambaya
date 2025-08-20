import { LogListComponent } from './Views/Logging/logs-list.component';
import { EmailSmsTemplateListComponent } from './Views/Email-SMS Templates/List/email-sms-template-list.component';
import { PharmacyCreateComponent } from '@app/user/Views/pharmacy/pharmacy-create/pharmacy-create.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { extract, AuthenticationGuard } from '@app/core';
import { UserComponent } from '@app/user/user.component';
import { UserAccountsComponent } from '@app/user/Views/user-accounts/user-accounts.component';
import { Shell } from '@app/shell/shell.service';
import { UserEditComponent } from '@app/user/Views/user-edit/user-edit.component';
import { UserCreateComponent } from '@app/user/Views/user-create/user-create.component';
import { UserListComponent } from '@app/user/Views/user-list/user-list.component';
import { PharmacyEditComponent } from './Views/pharmacy/pharmacy-edit/pharmacy-edit.component';
import { CardiologistCreateComponent } from '@app/user/Views/Cardiologist/cardiologist-create/cardiologist-create.component';
import { CardiologistEditComponent } from '@app/user/Views/Cardiologist/cardiologist-edit/cardiologist-edit.component';
import { CardiologistListComponent } from '@app/user/Views/Cardiologist/cardiologist-list/cardiologist-list.component';
import { UserSettingsComponent } from '@app/user/Views/user-settings/user-settings.component';
import { EmailSMSCreateEditComponent } from './Views/Email-SMS Templates/Create-Edit/Email-SMS-Create-Edit.component';
import { EmailSmsLogsComponent } from '@app/user/Views/email-sms-logs/email-sms-logs.component';

const routes: Routes = [
  Shell.childRoutes([
    {
      path: 'user/accounts',
      component: UserAccountsComponent,
      data: { title: extract('Account Settings'), roles: ['NambayaUser','StakeHolder','PharmacyTrainer'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/settings',
      component: UserSettingsComponent,
      data: { title: extract('Settings'), roles: ['NambayaUser','StakeHolder','PharmacyTrainer'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/user-edit/:userId',
      component: UserEditComponent,
      data: { title: extract('Edit User'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/user-create',
      component: UserCreateComponent,
      data: { title: extract('Add User'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/user-list',
      component: UserListComponent,
      data: { title: extract('Users'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    { path: 'user', redirectTo: '/homeAuth', pathMatch: 'full' },
    {
      path: 'user/dashboard',
      component: UserComponent,
      data: { title: extract('Dashboard'), roles: ['NambayaUser', 'PharmacyTrainer', 'StakeHolder'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/pharmacy/create',
      component: PharmacyCreateComponent,
      data: { title: extract('Add Pharmacy'), roles: [
        'NambayaUser', 'CentralGroupUser', 'PharmacyTrainer'
        ]},
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/pharmacy/edit/:pharmacyId',
      component: PharmacyEditComponent,
      data: { title: extract('Edit Pharmacy'), roles: [
        'NambayaUser', 'CentralGroupUser', 'PharmacyTrainer'
        ]},
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/cardiologist/create',
      component: CardiologistCreateComponent,
      data: { title: extract('Add Cardiologist'), roles: ['NambayaUser', 'CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/cardiologist/create',
      component: CardiologistCreateComponent,
      data: { title: extract('Add Cardiologist'), roles: ['NambayaUser', 'CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/cardiologist/edit/:cardiologistId',
      component: CardiologistEditComponent,
      data: { title: extract('Edit Cardiologist'), roles: ['NambayaUser', 'CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/cardiologist/edit/:cardiologistId',
      component: CardiologistEditComponent,
      data: { title: extract('Edit Cardiologist'), roles: ['NambayaUser', 'CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/cardiologist/list',
      component: CardiologistListComponent,
      data: { title: extract('Cardiologists'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/cardiologist/list',
      component: CardiologistListComponent,
      data: { title: extract('Cardiologists'), roles: ['CentralGroupUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/template/create',
      component: EmailSMSCreateEditComponent,
      data: { title: extract('Create Template') },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/template/edit/:templateId',
      component: EmailSMSCreateEditComponent,
      data: { title: extract('Edit Template'), roles: ['NambayaUser'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/template/list',
      component: EmailSmsTemplateListComponent,
      data: { title: extract('Template List'), roles: ['NambayaUser']},
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/logs/list',
      component: LogListComponent,
      data: { title: extract('Logs List'), roles: ['NambayaUser']},
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'user/email-sms-logs/list',
      component: EmailSmsLogsComponent,
      data: { title: extract('email-sms-logs'), roles: ['NambayaUser']},
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule {}
