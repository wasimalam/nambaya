import { UserConfig } from './shared/userConfigs';
import { NgModule, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ServiceWorkerModule } from '@angular/service-worker';
import { TranslateModule } from '@ngx-translate/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { environment } from '@env/environment';
import { CoreModule } from '@app/core';
import { SharedModule } from '@app/shared';
import { HomeModule } from '@app/home/home.module';
import { PatientsListModule } from '@app/dashboard/patients-list/patients-list.module';
import { ShellModule } from '@app/shell/shell.module';
import { AboutModule } from '@app/about/about.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginModule } from '@app/login/login.module';
import { AppComponent } from '@app/app.component';
import { AppRoutingModule } from '@app/app-routing.module';
import { ToastrModule } from 'ngx-toastr';

import {
  IgxInputGroupModule,
  IgxIconModule,
  IgxButtonModule,
  IgxRippleModule,
  IgxGridModule,
  IgxCheckboxModule,
  IgxAvatarModule,
  IgxBadgeModule,
  IgxProgressBarModule,
  IgxSwitchModule,
  IgxToggleModule,
  IgxLayoutModule,
  IgxSelectModule
} from 'igniteui-angular';
import { UserModule } from '@app/user/user.module';
import { PharmacistModule } from '@app/pharmacist/pharmacist.module';
import { CardiologistModule } from '@app/cardiologist/cardiologist.module';
import { OAuthService } from './shared/OAuth.Service';
import { ActivatedRoute } from '@angular/router';
import { AuthInterceptor } from '@app/shared/AuthIntercepter';
import { PipesModule } from '@app/pipes/Pipes.module';
import { CenterGroupModule } from '@app/centerGroup/center-group.module';
import { CookieService } from 'ngx-cookie-service';
import { AddEditNurseComponent } from '@app/nurses/Views/add-edit-nurse/add-edit-nurse.component';
import { ListNurseComponent } from '@app/nurses/Views/list-nurse/list-nurse.component';
import { NgxIntlTelInputModule } from 'ngx-intl-tel-input';

@NgModule({
  imports: [
    AppRoutingModule, // must be imported as the last module as it contains the fallback route
    BrowserModule,
    ServiceWorkerModule.register('./ngsw-worker.js', { enabled: environment.production }),
    FormsModule,
    HttpClientModule,
    TranslateModule.forRoot(),
    NgbModule,
    CoreModule,
    SharedModule,
    ShellModule,
    PatientsListModule,
    HomeModule,
    AboutModule,
    NgxIntlTelInputModule,
    BrowserAnimationsModule,
    LoginModule,
    UserModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-center',
      toastClass: 'toastr-global-position ngx-toastr'
      /*disableTimeOut:true,*/
    }),
    PharmacistModule,
    CardiologistModule,
    CenterGroupModule,
    IgxInputGroupModule,
    IgxIconModule,
    IgxButtonModule,
    IgxRippleModule,
    ReactiveFormsModule,
    IgxGridModule,
    IgxCheckboxModule,
    IgxAvatarModule,
    IgxBadgeModule,
    IgxProgressBarModule,
    IgxSwitchModule,
    IgxToggleModule,
    IgxLayoutModule,
    PipesModule,
    IgxSelectModule
  ],
  declarations: [AppComponent, AddEditNurseComponent, ListNurseComponent],
  providers: [OAuthService, { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }, CookieService],
  bootstrap: [AppComponent]
})
export class AppModule implements OnInit {
  userConfigs: UserConfig = new UserConfig();

  constructor(private oAutheService: OAuthService, private route: ActivatedRoute) {
    const client = window.location.pathname;
    this.route.queryParams.subscribe(params => {
      if (params.ReturnUrl) {
        let clientid = params.ReturnUrl.split('client_id=')[1].split('&')[0];

        if (client === '/login') {
          if (clientid === this.userConfigs.pharmacistConfig.client_id) {
            localStorage.setItem('initialClient', 'Pharmacist');
          } else if (clientid === this.userConfigs.cardiologistConfig.client_id) {
            localStorage.setItem('initialClient', 'Cardiologist');
          } else if (clientid === this.userConfigs.nambayauserConfig.client_id) {
            localStorage.setItem('initialClient', 'User');
          } else if (clientid === this.userConfigs.centerGroupConfig.client_id) {
            localStorage.setItem('initialClient', 'Center');
          }
        }
      }
    });

    if (client === '/pharmacist') {
      localStorage.setItem('initialClient', 'Pharmacist');
    } else if (client === '/cardiologist') {
      localStorage.setItem('initialClient', 'Cardiologist');
    } else if (client === '/user') {
      localStorage.setItem('initialClient', 'User');
    } else if (client === '/center') {
      localStorage.setItem('initialClient', 'Center');
    }

    if (client !== '/pharmacist' && localStorage.getItem('initialClient') === 'Pharmacist') {
      this.oAutheService.setUpModule(this.userConfigs.pharmacistConfig);
      localStorage.setItem('clientId', this.userConfigs.pharmacistConfig.client_id);
      localStorage.setItem('applicationId', 'pharmacist');
      localStorage.setItem('APP_URL_PREFIX', '/pharmacist');
      localStorage.setItem('loginPageTitle', 'Pharmacist');
      localStorage.setItem('applicationCode', 'Pharma');
    } else if (client !== '/cardiologist' && localStorage.getItem('initialClient') === 'Cardiologist') {
      this.oAutheService.setUpModule(this.userConfigs.cardiologistConfig);
      localStorage.setItem('clientId', this.userConfigs.cardiologistConfig.client_id);
      localStorage.setItem('loginPageTitle', 'Cardiologist');
      localStorage.setItem('applicationId', 'cardiologist');
      localStorage.setItem('APP_URL_PREFIX', '/cardiologist');
      localStorage.setItem('applicationCode', 'Cardio');
    } else if (client !== '/user' && localStorage.getItem('initialClient') === 'User') {
      this.oAutheService.setUpModule(this.userConfigs.nambayauserConfig);
      localStorage.setItem('clientId', this.userConfigs.nambayauserConfig.client_id);
      localStorage.setItem('loginPageTitle', 'Nambaya User');
      localStorage.setItem('applicationId', 'nambayauser');
      localStorage.setItem('APP_URL_PREFIX', '/user');
      localStorage.setItem('applicationCode', 'User');
    } else if (client !== '/center' && localStorage.getItem('initialClient') === 'Center') {
      this.oAutheService.setUpModule(this.userConfigs.centerGroupConfig);
      localStorage.setItem('clientId', this.userConfigs.centerGroupConfig.client_id);
      localStorage.setItem('loginPageTitle', 'Center Group');
      localStorage.setItem('applicationId', 'centralgroupuser');
      localStorage.setItem('applicationCode', 'CentralGroupApp');
      localStorage.setItem('APP_URL_PREFIX', '/center');
    }
  }

  ngOnInit(): void {}
}
