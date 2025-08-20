import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-pharmacist',
  templateUrl: './pharmacist.component.html',
  styleUrls: ['./pharmacist.component.scss']
})
export class PharmacistComponent implements OnInit {
  constructor(private location: Location) {}

  ngOnInit() {}

  goBack(): void {
    this.location.back();
  }
}
