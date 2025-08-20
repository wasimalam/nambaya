import { APIUrls } from '@app/shared/Addresses';
import { Injectable } from '@angular/core';
import { Observable, throwError, from } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Addresses } from '@app/shared/Addresses';

const options = {
  withCredentials: true //this is required so that Angular returns the Cookies received from the server. The server sends cookies in Set-Cookie header. Without this, Angular will ignore the Set-Cookie header
};

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  stsServerAddress: string = new Addresses().stsServerAddress;
  private IdentityUrl = new APIUrls().IdentityServerBaseUrl;
  private urls = {
    loginUrl: `${this.IdentityUrl}/api/authenticate`,
    logOutUrl: `${this.IdentityUrl}/api/authenticate/logout/`,
    loginTwUrl: `${this.IdentityUrl}/api/authenticate/account`,
    verifyOtpUrl: `${this.IdentityUrl}/api/authenticate/account/otp`,
    getOtpForgetPasswordUrl: `${this.IdentityUrl}/api/password/account/`,
    verifyOtpForgetPasswordUrl: `${this.IdentityUrl}/api/password/account/otp`,
    saveNewPassword: `${this.IdentityUrl}/api/password/account/changepassword`
  };
  constructor(private http: HttpClient) {}

  getForgetPasswordOtp(context: any): Observable<any> {
    return this.http.post<any>(this.urls.getOtpForgetPasswordUrl, context, options).pipe(catchError(this.handleError));
  }

  verifyForgetPasswordOtop(context: any): Observable<any> {
    return this.http
      .post<any>(this.urls.verifyOtpForgetPasswordUrl, context, options)
      .pipe(catchError(this.handleError));
  }

  saveNewPassword(context: any): Observable<any> {
    return this.http.post<any>(this.urls.saveNewPassword, context, options).pipe(catchError(this.handleError));
  }

  login(context: any): Observable<any> {
    return this.http.post<any>(this.urls.loginTwUrl, context, options).pipe(catchError(this.handleError));
  }

  verifyOtop(context: any): Observable<any> {
    return this.http.post<any>(this.urls.verifyOtpUrl, context, options).pipe(catchError(this.handleError));
  }

  logout(logoutId: string): Observable<any> {
    return this.http.get<any>(this.urls.logOutUrl + logoutId, options).pipe(catchError(this.handleError));
  }

  public endSession(token: string, url: string) {
    return this.http.get(
      `http://identity.qtlife.com:5000/connect/endsession?id_token_hint=${token}&post_logout_redirect_uri='${url}'`,
      { observe: 'response' }
    );
  }

  destroySession(url): Observable<any> {
    return this.http.get<any>(url).pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    if (error.error instanceof Error) {
      return throwError(error.error.messgae);
    }

    return throwError(error || 'Node.js server error');
  }
}
