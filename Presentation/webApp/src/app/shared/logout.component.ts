import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '@app/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html'
})
export class LogoutComponent implements OnInit {
  constructor(private authenticationService: AuthenticationService, private activatedRoute: ActivatedRoute) {}

  private logoutfun() {
    var query = window.location.search;
    var logoutIdQuery = query && query.toLowerCase().indexOf('?logoutid=') == 0 && query;
    this.authenticationService.logout(logoutIdQuery).subscribe(res => {
      if (res.signOutIFrameUrl) {
        var iframe = document.createElement('iframe');
        iframe.src = res.signOutIFrameUrl;
        document.getElementById('logout_iframe').appendChild(iframe);
      }

      if (res.postLogoutRedirectUri) {
        window.location = res.postLogoutRedirectUri;
      } else {
        document.getElementById('bye').innerText = 'You can close this window. Bye!';
      }
    });
  }

  ngOnInit() {
    this.logoutfun();
  }
}
