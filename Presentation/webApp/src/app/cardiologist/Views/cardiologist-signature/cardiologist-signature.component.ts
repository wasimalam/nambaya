import {Component, OnInit} from '@angular/core';
import {FormBuilder} from '@angular/forms';
import {CustomValidatorService} from '@app/shared/services/custom-validator.service';
import {PatientsService} from '@app/pharmacist/Models/patients.service';
import {Location} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {NavigationEnd, Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {filter} from 'rxjs/operators';

@Component({
  selector: 'app-cardiologist-signature',
  templateUrl: './cardiologist-signature.component.html',
  styleUrls: ['./cardiologist-signature.component.scss']
})
export class CardiologistSignatureComponent implements OnInit {

  isLoading = false;
  public path: string;
  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private patientsService: PatientsService,
    private location: Location,
    private toastr: ToastrService,
    private router: Router,
    private translateService: TranslateService
  ) {
  }

  ngOnInit() {
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe(args => this.path = args['url']);
  }

  goBack(): void {
    this.location.back();
  }
}
