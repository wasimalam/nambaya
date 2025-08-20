import { Component, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { IgxExpansionPanelComponent } from 'igniteui-angular';

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.scss']
})
export class FaqComponent implements OnInit {
  @ViewChild(IgxExpansionPanelComponent, { read: IgxExpansionPanelComponent, static: true })
  public panel: IgxExpansionPanelComponent;
  public applicationId: string;

  constructor(private location: Location) {
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
      /*if (this.applicationId === 'pharmacist') {
        this.settingsLink = '/pharmacist/accounts';
        this.preferencesLink = '/pharmacist/settings';
      } else if (this.applicationId === 'cardiologist') {
        this.settingsLink = '/cardiologist/accounts';
        this.preferencesLink = '/cardiologist/settings';
        this.isCardiologist = true;
      } else if (this.applicationId === 'centralgroupuser') {
        this.settingsLink = '/center/accounts';
        this.preferencesLink = '/center/settings';
      }*/
    }
  }

  ngOnInit(): void {}

  goBack(): void {
    this.location.back();
  }
}
