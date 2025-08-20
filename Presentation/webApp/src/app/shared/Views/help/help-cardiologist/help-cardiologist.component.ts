import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-help-cardiologist',
  templateUrl: './help-cardiologist.component.html',
  styleUrls: ['./help-cardiologist.component.scss']
})
export class HelpCardiologistComponent implements OnInit {
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
