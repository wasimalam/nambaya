import { ForgetPasswordComponent } from './../shared/forgetPassword/forgetPassword.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CountdownModule } from 'ngx-countdown';
import { LoginRoutingModule } from './Routes/login-routing.module';
import { LoginComponent } from './login.component';
import { IgxButtonModule, IgxIconModule, IgxInputGroupModule, IgxRippleModule } from 'igniteui-angular';
import { CookieService } from 'ngx-cookie-service';
import {
  AuthModule,
  OidcSecurityService,
} from 'angular-auth-oidc-client';
import { HomeAuthComponent } from '../shared/homeAuth.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    NgbModule,
    IgxInputGroupModule,
    IgxIconModule,
    IgxButtonModule,
    IgxRippleModule,
    ReactiveFormsModule,
    LoginRoutingModule,
    CountdownModule,
    AuthModule.forRoot()
  ],
  declarations: [LoginComponent, HomeAuthComponent, ForgetPasswordComponent],
  providers: [OidcSecurityService, CookieService]
})
export class LoginModule {
  constructor(public oidcSecurityService: OidcSecurityService) {}
}
