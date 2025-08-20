import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {
  isAllowed: boolean;

  constructor(
    private router: Router,
    public oidcSecurityService: OidcSecurityService,
    public userService: UserAccountsService
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (route.data.type === 'systemLog') {
      if (localStorage.getItem('isTemplateAllowed')) {
        this.isAllowed = true;
      } else {
        this.userService.getTemplateAllowedDomains().subscribe(
          response => {
            const allowedDomains: any = response; // This will be array of allowed domains
            if (allowedDomains && allowedDomains.length > 0) {
              this.oidcSecurityService.getUserData().subscribe(data => {
                const currentUserDomain = data.email.split('@')[1];
                const allowedDomain = allowedDomains.filter(domain => domain === currentUserDomain);
                if (allowedDomain && allowedDomain.length > 0) {
                  localStorage.setItem('isTemplateAllowed', 'true');
                  this.isAllowed = true;
                } else {
                  this.router.navigate(['/unauthorized']);
                  return false;
                }
              });
            }
          },
          error => {}
        );
      }
    } else {
      if (localStorage.getItem('isLogAllowed')) {
        this.isAllowed = true;
      } else {
        this.userService.getLogAllowedUserDomains().subscribe(
          response => {
            const allowedDomains: any = response; // This will be array of allowed domains
            if (allowedDomains && allowedDomains.length > 0) {
              this.oidcSecurityService.getUserData().subscribe(data => {
                const currentUserDomain = data.email.split('@')[1];
                const allowedDomain = allowedDomains.filter(domain => domain === currentUserDomain);
                if (allowedDomain && allowedDomain.length > 0) {
                  localStorage.setItem('isLogAllowed', 'true');
                  this.isAllowed = true;
                } else {
                  this.router.navigate(['/unauthorized']);
                  return false;
                }
              });
            }
          },
          error => {}
        );
      }
    }

    return this.isAllowed;
  }
}
