import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-faq-pharmacist',
  templateUrl: './faq-pharmacist.component.html',
  styleUrls: ['./faq-pharmacist.component.scss']
})
export class FaqPharmacistComponent implements OnInit {
  public urlPrefix = '';
  constructor() {
    this.urlPrefix = localStorage.getItem('APP_URL_PREFIX');
  }

  ngOnInit() {}
}
