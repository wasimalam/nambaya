import { ForgetPasswordComponent } from './../../shared/forgetPassword/forgetPassword.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from '../login.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    pathMatch: 'full'
  },
  {
    path: 'forgetpassword/:returnUrl',
    component: ForgetPasswordComponent,
    pathMatch: 'full'
  },
  {
    path: 'changepassword/:returnUrl',
    component: ForgetPasswordComponent,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: []
})
export class LoginRoutingModule {}
