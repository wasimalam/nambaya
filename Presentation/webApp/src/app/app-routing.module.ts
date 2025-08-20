import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { extract } from '@app/core';
import { HomeComponent } from '@app/home/home.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    data: { title: extract('Willkommen zu Nambaya'), roles: [] },
    canActivate: [],
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      anchorScrolling: 'enabled'
    })
  ],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule {}
