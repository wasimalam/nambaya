import { Component, OnInit, OnDestroy, AfterViewInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { environment } from '@env/environment';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { PharmacistAccountsService } from '../Models/pharmacist-accounts.service';
import { ToastrService } from 'ngx-toastr';
import { ConnectedPositioningStrategy, VerticalAlignment } from 'igniteui-angular';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-pharmacist-edit',
  templateUrl: './pharmacist-edit.component.html'
})
export class PharmacistEditComponent implements OnInit, OnDestroy, AfterViewInit {
  pharmacistEditForm: FormGroup;
  pharmacistData: any = null;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  phoneNumber = '^(+d{1,3}[- ]?)?d{10}$';
  settings: any;
  renderForm: boolean = false;
  pharmacistId: any;
  public isActivepp: boolean;
  public isLocked: boolean;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private pharmacistService: PharmacistAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private translateService: TranslateService
  ) {
    this.settings = {
      positionStrategy: new ConnectedPositioningStrategy({
        closeAnimation: null,
        openAnimation: null,
        verticalDirection: VerticalAlignment.Top,
        verticalStartPoint: VerticalAlignment.Top
      })
    };
  }

  ngOnInit() {
    this.isLoading = true;
    this.pharmacistId = this.activatedRoute.snapshot.params['pharmacistId'];
    this.createPharmacistEditForm();
    const pharmacistRequest = this.pharmacistService.getOne(this.pharmacistId);
    forkJoin([pharmacistRequest]).subscribe(results => {
      this.pharmacistData = results[0];
      this.pharmacistEditForm.patchValue(this.pharmacistData);
      this.isActivepp = this.pharmacistData.isActive;
      this.renderForm = true;
      this.isLoading = false;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updatePharmacist() {
    if (this.pharmacistEditForm.invalid) {
      this.validateAllFormFields(this.pharmacistEditForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.pharmacistEditForm.controls.isActive.setValue(this.isActivepp);
      this.pharmacistEditForm.controls.isLocked.setValue(this.isLocked);
      this.pharmacistService.update(this.pharmacistEditForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Pharmacist Saved successfully').subscribe(text => {
              this.toastr.success(text);
            });

            this.isLoading = false;
            this.router.navigate(['/pharmacist/pharmacist-list']);
          }
        },
        error => {
          this.translateService.get('Something went wrong').subscribe(text => {
            this.toastr.error(text);
          });
        }
      );
    }
  }

  goBack(): void {
    this.location.back();
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }

  private createPharmacistEditForm() {
    this.pharmacistEditForm = this.formBuilder.group({
      id: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      pharmacyId: [''],
      email: ['', Validators.email],
      phone: ['', [Validators.required]],
      street: ['', Validators.required],
      zipCode: ['', [Validators.required, Validators.compose([this.customValidator.numberValidator()])]],
      isActive: [''],
      isLocked: [''],
      address: ['', Validators.required]
    });
  }
}
