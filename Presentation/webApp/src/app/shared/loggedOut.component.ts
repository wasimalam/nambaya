import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { IgxDialogComponent, PositionSettings } from 'igniteui-angular';
import { Router } from '@angular/router';

@Component({
  selector: 'app-loggedOut',
  templateUrl: './loggedOut.component.html',
  styleUrls: ['./loggedout.component.scss']
})
export class LoggedOutComponent implements OnInit, AfterViewInit {
  initialClient: string;
  isSessionExpired: string = null;
  sessionExpiredDialogTitle = 'Session Expired';
  sessionExpiredDialogMessage = 'your_session_expired';
  leftButtonLabel = 'Login';

  // @ts-ignore
  @ViewChild('sessionExpiredDialog') public sessionExpiredDialog: IgxDialogComponent;

  public positionSettings: PositionSettings = {
    minSize: { height: 100, width: 500 }
  };
  constructor(private translateService: TranslateService, public router: Router) {}

  openSessionExpiredDialog() {
    this.sessionExpiredDialog.open();
  }

  closeDialog() {
    this.sessionExpiredDialog.close();
    localStorage.clear();
    sessionStorage.clear();
    if (this.initialClient === 'Pharmacist') {
      window.location.href = '/pharmacist';
    } else if (this.initialClient === 'Cardiologist') {
      window.location.href = '/cardiologist';
    } else if (this.initialClient === 'User') {
      window.location.href = '/user';
    } else if (this.initialClient === 'Center') {
      window.location.href = '/center';
    }
  }

  ngOnInit() {
    this.initialClient = localStorage.getItem('initialClient');
    this.isSessionExpired = localStorage.getItem('isSessionExpired');
    this.translateService.get(this.sessionExpiredDialogTitle).subscribe(text => {
      this.sessionExpiredDialogTitle = text;
    });
    this.translateService.get(this.sessionExpiredDialogMessage).subscribe(text => {
      this.sessionExpiredDialogMessage = text;
    });
    this.translateService.get(this.leftButtonLabel).subscribe(text => {
      this.leftButtonLabel = text;
    });
  }

  goToHome() {
    this.router.navigate(['/']);
  }

  ngAfterViewInit(): void {
    if (this.isSessionExpired === 'yes') {
      this.openSessionExpiredDialog();
    } else {
      localStorage.clear();
      sessionStorage.clear();
    }
  }
}
