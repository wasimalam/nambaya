import { environment } from './../environments/environment';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute, NavigationStart } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';
import { Logger, I18nService, untilDestroyed } from '@app/core';
import { OAuthService } from '@app/shared/OAuth.Service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private titleService: Title,
    private translateService: TranslateService,
    private i18nService: I18nService,
    private oAuthService: OAuthService,
    private oidcSecurityService: OidcSecurityService
  ) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        window.localStorage.setItem('previousUrl', this.router.url);
        let client22 = window.location.pathname;
      }
    });

    this.oAuthService.setUpComponent();
    if (this.oidcSecurityService.getToken() != '') {
      this.oidcSecurityService.getUserData();
    }
  }

  ngOnInit() {
    // Setup logger
    if (environment.production) {
      Logger.enableProductionMode();
    }
    // Setup translations
    this.i18nService.init(environment.defaultLanguage, environment.supportedLanguages);

    const onNavigationEnd = this.router.events.pipe(filter(event => event instanceof NavigationEnd));

    // Change page title on navigation or language change, based on route data
    merge(this.translateService.onLangChange, onNavigationEnd)
      .pipe(
        map(() => {
          let route = this.activatedRoute;
          while (route.firstChild) {
            route = route.firstChild;
          }
          return route;
        }),
        filter(route => route.outlet === 'primary'),
        switchMap(route => route.data),
        untilDestroyed(this)
      )
      .subscribe(event => {
        const title = event.title;
        if (title) {
          this.titleService.setTitle(this.translateService.instant(title));
        }
      });
  }

  ngOnDestroy() {
    this.i18nService.destroy();
  }
}
