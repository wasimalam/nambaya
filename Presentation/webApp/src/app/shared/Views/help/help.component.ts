import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class HelpComponent implements OnInit {
  public applicationId: string;

  constructor(private location: Location) {
    if (localStorage.getItem('applicationId') !== 'undefined' && localStorage.getItem('applicationId') !== '') {
      this.applicationId = localStorage.getItem('applicationId');
    }
  }

  ngOnInit() {}

  goBack(): void {
    this.location.back();
  }
}
