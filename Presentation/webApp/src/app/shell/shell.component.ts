import { OidcSecurityService } from 'angular-auth-oidc-client';
import { OAuthService } from '@app/shared/OAuth.Service';
import { Component, OnInit, AfterViewInit, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent implements OnInit, AfterViewInit, AfterViewChecked {
  public userName: string;
  render: boolean = false;
  public role: string;
  public urlPrefix = '';
  public showNav: boolean = false;
  public mainStyle = '';
  public isMenuExpanded: boolean;

  constructor(
    public outhservice: OAuthService,
    public oidcService: OidcSecurityService,
    private cdRef: ChangeDetectorRef,
  ) {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngAfterViewChecked() {
    this.cdRef.detectChanges();
  }
  ngAfterViewInit() {
    const a = this.oidcService.getUserData();
    this.outhservice.observableUser.subscribe(user => {
      // tslint:disable-next-line:triple-equals
      if (user != undefined) {
        this.role = user.rolecode;
        this.showNav = true;
      }
    });
  }

  // tslint:disable-next-line:typedef
  adjustNavBar(isMenuExpanded) {
    this.isMenuExpanded = isMenuExpanded;
    if (isMenuExpanded) {
      this.mainStyle = '230';
    } else {
      this.mainStyle = '68';
    }
  }

  ngOnInit() {}
}
