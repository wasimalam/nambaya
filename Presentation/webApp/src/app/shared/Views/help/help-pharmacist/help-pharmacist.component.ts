import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-help-pharmacist',
  templateUrl: './help-pharmacist.component.html',
  styleUrls: ['./help-pharmacist.component.scss']
})
export class HelpPharmacistComponent implements OnInit {
  private fragment: string;
  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.fragment.subscribe(fragment => {
      this.fragment = fragment;
    });
  }

  scroll(el: HTMLElement) {
    el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  }
  ngAfterViewInit(): void {
    try {
      document.querySelector('#' + this.fragment).scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    } catch (e) {}
  }
}
