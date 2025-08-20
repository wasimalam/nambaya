import { PhysicianComponent } from './physician.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract } from '@app/core';
import { Shell } from '@app/shell/shell.service';
import { PhysicianAccountsComponent } from '@app/physician/Views/physician-accounts/physician-accounts.component';

const routes: Routes = [
  Shell.childRoutes([
    {
      path: 'physician/accounts',
      component: PhysicianAccountsComponent,
      data: { title: extract('Physician Accounts') }
    },
    { path: 'physician', redirectTo: '/homeAuth', pathMatch: 'full' },
    { path: 'physiciandashboard', component: PhysicianComponent, pathMatch: 'full' }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PhysicianRoutingModule {}
