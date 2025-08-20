import { Component, OnInit } from '@angular/core';
import { OAuthService } from '@app/shared/OAuth.Service';

@Component({
  selector: 'app-physician',
  templateUrl: './physician.component.html',
  styleUrls: ['./physician.component.scss']
})
export class PhysicianComponent implements OnInit {
  constructor(private oAuthService: OAuthService) {
    this.oAuthService.checkIfAuthenticated();
  }

  ngOnInit() {}
}
