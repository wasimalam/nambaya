import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Logger } from '../logger.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';

const log = new Logger('AuthenticationGuard');

@Injectable({
  providedIn: 'root'
})
export class AuthenticationGuard implements CanActivate {
  userRole: string;
  loggedUser: any = null;
  isAllowed: boolean = false;
  isLogin = localStorage.getItem('login');
  requestCompleted: boolean = false;
  constructor(private router: Router, public oidcSecurityService: OidcSecurityService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    this.oidcSecurityService.getUserData().subscribe(data => {
      this.loggedUser = data;
      this.requestCompleted = true;
      if (this.loggedUser && this.requestCompleted) {
        if (route.data.roles && route.data.roles.indexOf(this.loggedUser.rolecode) === -1) {
          this.router.navigate(['/unauthorized']);
          this.isAllowed = false;
        }
        this.isAllowed = true;
      } else {
        if (this.isLogin !== 'true' && this.requestCompleted) {
          var _this = this;
          setTimeout(function() {
            if (!_this.isAllowed) {
              _this.router.navigate(['/unauthorized']);
            }
          }, 4000);
        }
        this.isAllowed = false;
      }
    });
    return this.isAllowed;
  }
}
