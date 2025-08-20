import { Addresses } from './Addresses';
import { Injectable, Injector, Inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable, of, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private oidcSecurityService: OidcSecurityService;
  initialClient: string;

  constructor(private injector: Injector, private toastrService: ToastrService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let requestToForward = req;
    if (this.oidcSecurityService === undefined) {
      this.oidcSecurityService = this.injector.get(OidcSecurityService);
    }
    if (this.oidcSecurityService !== undefined) {
      let token: string;
      if (this.oidcSecurityService.getToken()) {
        token = this.oidcSecurityService.getToken();
      } else {
        token = localStorage.getItem('token');
      }
      if (token !== '') {
        let tokenValue = 'Bearer ' + token;
        //  let tokenValue = 'Bearer ' + "";

        requestToForward = req.clone({ setHeaders: { Authorization: tokenValue } });
      }
    } else {
      console.debug('OidcSecurityService undefined: NO auth header!');
    }

    return next.handle(requestToForward).pipe(
      tap(evt => {
        if (evt instanceof HttpResponse) {
          // code on success
        }
      }),
      catchError((err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (
            err.status === 401 &&
            err.error !== 'USER_ID_PASSWORD_INVALID' &&
            err.error !== 'Value does not fall within the expected range.' &&
            err.error !== 'The given key was not present in the dictionary.' &&
            err.error !== 'OTP does not match' &&
            err.error !== 'USER_ID_INACTIVE' &&
            err.error !== 'USER_ID_LOCKED' &&
            err.error !== 'USER_ID_DELETED' &&
            err.error !== 'INVALID_USER_ID_PASSWORD'
          ) {
            if (!err.url.includes(new Addresses().stsServerAddress)) {
              this.oidcSecurityService.logoff();
              localStorage.setItem('isSessionExpired', 'yes');
            }
          }
        }
        return throwError(err);
      })
    );
  }
}
