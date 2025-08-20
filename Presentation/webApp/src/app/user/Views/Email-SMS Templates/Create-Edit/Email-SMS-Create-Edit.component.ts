import { TranslateService } from '@ngx-translate/core';
import { UserAccountsService } from '@app/user/Models/user-accounts.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { CustomValidatorService } from '@app/shared/services/custom-validator.service';

@Component({
  selector: 'app-user-create',
  templateUrl: './Email-SMS-Create-Edit.component.html',
  styleUrls: ['./Email-SMS-Create-Edit.component.scss']
})
export class EmailSMSCreateEditComponent implements OnInit, OnDestroy, AfterViewInit {
  templateForm!: FormGroup;
  error: string | undefined;
  templateId: number = 0;
  isLoading = false;
  pageTitle: string = 'Create Template';
  templateTypes: any;
  templateData: any = null;
  gettingData: boolean = true;
  gettingNotificationTypes: boolean = false;
  gettingPlaceHolders: boolean = false;
  eventTypes: any;
  placeholders: any;
  public message: string;

  constructor(
    private formBuilder: FormBuilder,
    private customValidator: CustomValidatorService,
    private location: Location,
    private toastr: ToastrService,
    private activatedRoute: ActivatedRoute,
    private userService: UserAccountsService,
    private translateService: TranslateService,
    private router: Router
  ) {
    if (this.activatedRoute.snapshot.params['templateId']) {
      this.templateId = Number(this.activatedRoute.snapshot.params['templateId']);
    }
  }

  ngOnInit() {
    this.createTemplateForm();
    this.getEventTypes();

    // const templateTypesRequest = this.userService.getTemplateTypes();
    // const eventTypesRequest = this.userService.getEvenTypes();
    // forkJoin([templateTypesRequest, eventTypesRequest]).subscribe(results => {
    //   this.templateTypes = results[0];
    //   this.eventTypes = results[1];
    //   this.gettingData = false;
    // });
    if (this.templateId > 0) {
      this.pageTitle = 'Edit Template';
      this.isLoading = true;
      this.getOneTemplate();
    }
  }

  getEventTypes() {
    this.userService.getEvenTypes().subscribe(response => {
      this.eventTypes = response;
      this.gettingData = false;
    });
  }

  getotherTypes() {
    // alert("changed"+Number( this.templateForm.controls.eventTypeID.value));
    let selectedEventId = Number(this.templateForm.controls.eventTypeID.value);
    this.getNotificationTypesByEventId(selectedEventId);
    this.getPlaceHoldersByEventId(selectedEventId);
    if (Number(this.templateForm.controls.templateTypeID.value) > 0) {
      this.getOneTemplateByEventTypeAndTemplateType();
    }
  }

  getNotificationTypesByEventId(eventId: number) {
    this.gettingNotificationTypes = true;
    this.userService.getNotificationTypesByEventId(eventId).subscribe(result => {
      this.templateTypes = result;
      this.gettingNotificationTypes = false;
    });
  }

  getPlaceHoldersByEventId(eventId: number) {
    this.gettingPlaceHolders = true;
    this.userService.getPlaceHoldersByEventId(eventId).subscribe(result => {
      this.placeholders = result;
      this.gettingPlaceHolders = false;
    });
  }

  getOneTemplateByEventTypeAndTemplateType() {
    let eventTypeId = Number(this.templateForm.controls.eventTypeID.value);
    let templateTypeId = Number(this.templateForm.controls.templateTypeID.value);
    this.userService.getOneTemplateByEventIdAndType(eventTypeId, templateTypeId).subscribe(result => {
      if (result) {
        this.templateData = result;
        this.templateForm.patchValue(this.templateData);
        this.getPlaceHoldersByEventId(this.templateData.eventTypeID);
        this.getNotificationTypesByEventId(this.templateData.eventTypeID);
        this.isLoading = false;
        this.pageTitle = 'Edit Template';
      }
    });
  }

  getOneTemplate() {
    this.userService.getOneTemplate(this.templateId).subscribe(result => {
      this.templateData = result;
      this.templateForm.patchValue(this.templateData);
      this.getPlaceHoldersByEventId(this.templateData.eventTypeID);
      this.getNotificationTypesByEventId(this.templateData.eventTypeID);
      this.isLoading = false;
    });
  }

  ngOnDestroy() {}
  ngAfterViewInit() {}

  scrollToError(): void {
    const firstElementWithError = document.querySelector('.ng-invalid[formControlName]');
    firstElementWithError.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  public saveTemplate() {
    if (this.templateForm.invalid) {
      this.validateAllFormFields(this.templateForm);
      this.scrollToError();
    } else {
      if (this.templateForm.controls.id.value > 0) {
        this.update();
      } else {
        this.createNew();
      }
    }
  }

  public createNew() {
    let template = {
      templateTypeID: this.templateForm.controls.templateTypeID.value,
      eventTypeID: this.templateForm.controls.eventTypeID.value,
      code: this.templateForm.controls.code.value,
      subject: this.templateForm.controls.subject.value,
      message: this.templateForm.controls.message.value,
      // isActive: this.templateForm.controls.isActive.value,
      isActive: true
    };

    this.isLoading = true;
    this.userService.createTemplate(template).subscribe(
      response => {
        if (response.status === 200) {
          this.translateService.get('Saved Successfully').subscribe(text => {
            this.toastr.success(text);
            this.templateForm.reset();
          });
          this.isLoading = false;
          this.router.navigate(['/user/template/list']);
        }
        this.isLoading = false;
      },
      error => {
        this.translateService.get('Something went wrong.').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoading = false;
      }
    );
  }

  public update() {
    let template = {
      id: this.templateForm.controls.id.value,
      templateTypeID: this.templateForm.controls.templateTypeID.value,
      eventTypeID: this.templateForm.controls.eventTypeID.value,
      code: this.templateForm.controls.code.value,
      subject: this.templateForm.controls.subject.value,
      message: this.templateForm.controls.message.value,
      // isActive: this.templateForm.controls.isActive.value,
      isActive: true
    };
    this.isLoading = true;
    this.userService.updateTemplate(template).subscribe(
      response => {
        if (response.status === 200) {
          this.translateService.get('Saved Successfully').subscribe(text => {
            this.toastr.success(text);
            this.router.navigate(['/user/template/list']);
          });
          this.isLoading = false;
        }
        this.isLoading = false;
      },
      error => {
        this.translateService.get('Something went wrong.').subscribe(text => {
          this.toastr.error(text);
        });
        this.isLoading = false;
      }
    );
  }

  public appendPlaceHolder(placeholder) {
    this.message = this.templateForm.controls.message.value.trim() + ' ' + placeholder.code.trim();
    this.templateForm.controls.message.setValue(this.message.trim());
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

  private createTemplateForm() {
    this.templateForm = this.formBuilder.group({
      id: [''],
      templateTypeID: ['', Validators.required],
      eventTypeID: ['', Validators.required],
      code: ['', Validators.required],
      subject: ['', Validators.required],
      message: ['', Validators.required],
      isActive: []
    });
  }
}
