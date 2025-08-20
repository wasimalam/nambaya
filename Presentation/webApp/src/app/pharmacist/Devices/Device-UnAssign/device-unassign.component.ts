import { DeviceModel } from '@app/pharmacist/Devices/device.model';
import { DeviceAssignModel } from '@app/pharmacist/Devices/deviceassign.model';
import { ActivatedRoute } from '@angular/router';
import { DeviceService } from '@app/pharmacist/Devices/device.service';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { environment } from '@env/environment';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-device-unAssign',
  templateUrl: './device-unassign.component.html'
})
export class DeviceUnAssignComponent implements OnInit, OnDestroy, AfterViewInit {
  deviceUnAssignForm: FormGroup;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = true;
  patientId: number;
  pharmacyId: number;
  dateselected: Date;
  filterObject = [];
  public devices: any[];
  deviceSelected: any;
  deviceSeltedId: number;
  deviceAssign: DeviceAssignModel = new DeviceAssignModel();
  assignedDeviceId: number;
  assignedDevice: DeviceModel = new DeviceModel();
  gettingDevice: boolean = true;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private deviceService: DeviceService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService
  ) {
    this.assignedDeviceId = this.activatedRoute.snapshot.params['deviceId'];
    this.patientId = this.activatedRoute.snapshot.params['patientId'];
  }

  public getOneDevice(id: number) {
    this.deviceService.getOne(id).subscribe(data => {
      this.assignedDevice = data;
      this.gettingDevice = false;
      this.isLoading = false;
    });
  }

  ngOnInit() {
    this.createdeviceUnAssignForm();
    this.getOneDevice(this.assignedDeviceId);
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  unAssignDevice() {
    this.isLoading = true;
    this.deviceAssign.deviceId = Number(this.assignedDevice.id);
    this.deviceAssign.PatientCaseID = Number(this.patientId);
    this.deviceAssign.IsAssigned = false;
    this.deviceAssign.DeviceStatusID = 451;
    this.deviceService.assign(this.deviceAssign).subscribe(
      response => {
        if (response.status === 200) {
          this.translateService.get('Device UnAssigned Successfully').subscribe(text => {
            this.toastr.success(text);
            this.deviceUnAssignForm.reset();
            this.isLoading = false;
            this.location.back();
          });

          this.isLoading = false;
        }
      },
      error => {
        this.translateService.get('Something went wrong').subscribe(text => {
          this.toastr.error(text);
        });
      }
    );
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

  private createdeviceUnAssignForm() {
    this.deviceUnAssignForm = this.formBuilder.group({
      device: ['', Validators.required]
      //measurementStart: ['', Validators.required]
    });
  }
}
