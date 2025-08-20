import { error } from 'util';
import { Router } from '@angular/router';
import { OAuthService } from '@app/shared/OAuth.Service';
import { DeviceService } from '@app/pharmacist/Devices/device.service';
import { DeviceModel } from '@app/pharmacist/Devices/device.model';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-device-create',
  templateUrl: './device-create.component.html'
})
export class DeviceCreateComponent implements OnInit, OnDestroy, AfterViewInit {
  deviceAddForm: FormGroup;
  deviceData: Observable<DeviceModel>;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  pharmacyId: number;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private deviceService: DeviceService,
    private location: Location,
    private toastr: ToastrService,
    private oauthService: OAuthService,
    private router: Router,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.createdeviceAddForm();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  createNewDevice() {
    if (this.deviceAddForm.invalid) {
      this.validateAllFormFields(this.deviceAddForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      if (this.oauthService.userData.rolecode === 'Pharmacy') {
        this.pharmacyId = parseInt(this.oauthService.userData.appuserid);
      } else if (this.oauthService.userData.rolecode === 'Pharmacist') {
        this.pharmacyId = parseInt(localStorage.getItem('pharmacyId'));
      }

      this.deviceService.save(this.deviceAddForm.value, this.pharmacyId).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Device Saved Successfully').subscribe(text => {
              this.toastr.success(text);
              this.deviceAddForm.reset();
              this.isLoading = false;
              this.router.navigate(['/pharmacist/device/list']);
            });
          }
          this.isLoading = false;
        },
        error => {
          if (error.error === 'DEVICE_SERIAL_ALREADY_EXIST') {
            this.translateService.get('Device Already Exist').subscribe(text => {
              this.toastr.error(text);
            });
          } else {
            this.translateService.get('Something went wrong').subscribe(text => {
              this.toastr.error(text);
            });
          }
          this.isLoading = false;
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

  private createdeviceAddForm() {
    this.deviceAddForm = this.formBuilder.group({
      name: ['', Validators.required],
      serialNumber: [
        '',
        [
          Validators.required,
          Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(7)])
        ]
      ],
      manufacturer: ['', Validators.required],
      description: ['', Validators.required]
    });
  }
}
