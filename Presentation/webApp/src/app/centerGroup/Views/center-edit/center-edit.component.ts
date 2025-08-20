import { CenterGroupAccountsService } from '@app/centerGroup/Models/center-group-accounts.service';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { environment } from '@env/environment';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-center-edit',
  templateUrl: './center-edit.component.html',
  styleUrls: ['./center-edit.component.scss']
})
export class CenterEditComponent implements OnInit, OnDestroy, AfterViewInit {
  editCenterFrom!: FormGroup;
  centerData: any = null;
  renderForm: boolean = false;
  version: string | null = environment.version;
  error: string | undefined;
  isLoading = false;
  public centerId: number;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private centerService: CenterGroupAccountsService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService
  ) {}

  ngOnInit() {
    this.isLoading = true;
    this.centerId = this.activatedRoute.snapshot.params.centerId;
    this.createEditCenterForm();

    const getCenter = this.centerService.getUserData(this.centerId);

    forkJoin([getCenter]).subscribe(results => {
      this.centerData = results[0];
      this.editCenterFrom.patchValue(this.centerData);
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

  updateCenter() {
    if (this.editCenterFrom.invalid) {
      this.validateAllFormFields(this.editCenterFrom);
      this.scrollToError();
    } else {
      this.isLoading = true;
      this.editCenterFrom.controls.phone.enable();
      this.editCenterFrom.controls.phone.setValue(this.centerData.phone);
      this.centerService.updateCenter(this.editCenterFrom.value).subscribe(
        response => {
          if (response.status === 200) {
            this.translateService.get('Center Group User successfully saved').subscribe(text => {
              this.toastr.success(text);
              this.isLoading = false;
              this.location.back();
            });
          }
        },
        error => {
          if (error.error === 'USER_ID_ALREADY_EXISTS') {
            this.translateService.get('Email already exists').subscribe(text => {
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

  private createEditCenterForm() {
    this.editCenterFrom = this.formBuilder.group({
      id: [''],
      firstName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(45)])]],
      lastName: ['', [Validators.required, Validators.compose([this.customValidator.maxLengthValidator(45)])]],
      email: ['', [Validators.email, Validators.compose([this.customValidator.maxLengthValidator(80)])]],
      phone: [{value: '', disabled: true}],
      street: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      address: ['', Validators.compose([this.customValidator.maxLengthValidator(200)])],
      zipCode: [
        '',
        Validators.compose([this.customValidator.numberValidator(), this.customValidator.maxLengthValidator(5)])
      ],
      isActive: [''],
      isLocked: ['']
    });
  }
}
