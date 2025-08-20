import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '@app/pharmacist/Devices/device.service';
import { DeviceModel } from '@app/pharmacist/Devices/device.model';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { tap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-device-edit',
  templateUrl: './device-edit.component.html'
})
export class DeviceEditComponent implements OnInit, OnDestroy, AfterViewInit {
  deviceEditForm: FormGroup;
  deviceData: Observable<DeviceModel>;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  renderForm = false;
  deviceId: number;
  gettingStatuses = true;
  statuses: any[];
  public disableStatusChange = false;
  public newStatus = 0;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private deviceService: DeviceService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private translateService: TranslateService
  ) {}

  getStatuses() {
    this.deviceService.getStatuses().subscribe(data => {
      this.statuses = data;
      this.gettingStatuses = false;
    });
  }

  ngOnInit() {
    this.deviceId = this.activatedRoute.snapshot.params.deviceId;
    this.createdeviceEditForm();
    this.getOneDevice(this.deviceId);
    this.getStatuses();
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  updateDevice() {
    if (this.deviceEditForm.invalid) {
      this.validateAllFormFields(this.deviceEditForm);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.deviceService.update(this.deviceEditForm.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Device Saved Successfully').subscribe(text => {
              this.toastr.success(text);
              this.deviceEditForm.reset();
              this.isLoading = false;
              this.router.navigate(['/pharmacist/device/list']);
            });

            this.isLoading = false;
          }
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

  public handleSelection(event) {
    this.newStatus = Number(event.newSelection.value);
    if (event.newSelection.value === 452) {
      this.translateService.get('Status cannot be changed to assigned').subscribe(text => {
        this.toastr.error(text);
      });
    }
  }

  public getOneDevice(id: number) {
    this.deviceService
      .getOne(id)
      .pipe(tap(data => this.deviceEditForm.patchValue(data)))
      .subscribe(data => {
        if (data.statusID === 452) {
          this.disableStatusChange = true;
        } else {
          this.disableStatusChange = false;
        }
        this.renderForm = true;
      });
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

  private createdeviceEditForm() {
    this.deviceEditForm = this.formBuilder.group({
      id: [''],
      pharmacyID: [''],
      name: ['', Validators.required],
      serialNumber: ['', Validators.required],
      manufacturer: ['', Validators.required],
      description: ['', Validators.required],
      createdBy: [''],
      createdOn: [''],
      statusID: ['', Validators.required]
    });
  }
}
