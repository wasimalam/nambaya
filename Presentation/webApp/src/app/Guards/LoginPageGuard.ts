import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoginPageGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let url: string = state.url;
    return this.checkLogin(url);
  }

  checkLogin(url: string): boolean {
    // if (this.authService.isLoggedIn) { return true; }
    // // Store the attempted URL for redirecting
    // this.authService.redirectUrl = url;
    // // Navigate to the login page with extras
    // this.router.navigate(['/login']);
    return true;
  }
}
