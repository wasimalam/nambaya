import { TranslateModule } from '@ngx-translate/core';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LoaderComponent } from './loader/loader.component';
import { SharedRoutingModule } from '@app/shared/shared-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  IgxButtonModule,
  IgxCardModule,
  IgxDialogModule,
  IgxIconModule,
  IgxInputGroupModule,
  IgxSelectModule,
  IgxSwitchModule,
  IgxRadioModule,
  IgxProgressBarModule,
  IgxGridModule,
  IgxSliderModule,
  IgxExpansionPanelModule
} from 'igniteui-angular';
import { AdditionalMedicationComponent } from './Views/patient/additional-medication/additional-medication.component';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
import { QuickEvaluationComponent } from '@app/shared/Views/patient/quick-evaluation/quick-evaluation.component';
import { FeedbackTimelineComponent } from './Views/patient/feedback-timeline/feedback-timeline.component';
import { PatientWizardComponent } from './Views/wizard/patient-wizard/patient-wizard.component';
import { PatientWizardService } from '@app/shared/services/patient-wizard.service';
import { NgxFileDropModule } from 'ngx-file-drop';
import { DoctorListComponent } from '@app/shared/Views/patient/doctor-list/doctor-list.component';
import { DoctorCreateComponent } from '@app/shared/Views/patient/doctor-create/doctor-create.component';
import { DoctorEditComponent } from '@app/shared/Views/patient/doctor-edit/doctor-edit.component';
import { CardioWizardComponent } from '@app/shared/Views/wizard/cardio-wizard/cardio-wizard.component';
import { FaqComponent } from '@app/shared/Views/faq/faq.component';
import { HelpComponent } from '@app/shared/Views/help/help.component';
import { HelpCardiologistComponent } from '@app/shared/Views/help/help-cardiologist/help-cardiologist.component';
import { HelpPharmacistComponent } from '@app/shared/Views/help/help-pharmacist/help-pharmacist.component';
import { FaqCardiologistComponent } from '@app/shared/Views/faq/faq-cardiologist/faq-cardiologist.component';
import { FaqPharmacistComponent } from '@app/shared/Views/faq/faq-pharmacist/faq-pharmacist.component';
import { HelpUserComponent } from './Views/help/help-user/help-user.component';
import { PasswordResetComponent } from './Views/password/password-reset/password-reset.component';

@NgModule({
  imports: [
    CommonModule,
    SharedRoutingModule,
    ReactiveFormsModule,
    IgxInputGroupModule,
    IgxSelectModule,
    IgxSwitchModule,
    IgxCardModule,
    IgxIconModule,
    IgxDialogModule,
    IgxButtonModule,
    IgxRadioModule,
    NgbCollapseModule,
    TranslateModule,
    NgxFileDropModule,
    IgxProgressBarModule,
    IgxGridModule,
    IgxSliderModule,
    IgxExpansionPanelModule,
    FormsModule
  ],
  declarations: [
    HelpCardiologistComponent,
    HelpPharmacistComponent,
    FaqCardiologistComponent,
    FaqPharmacistComponent,
    LoaderComponent,
    AdditionalMedicationComponent,
    QuickEvaluationComponent,
    FeedbackTimelineComponent,
    PatientWizardComponent,
    DoctorListComponent,
    DoctorCreateComponent,
    DoctorEditComponent,
    CardioWizardComponent,
    HelpComponent,
    FaqComponent,
    HelpUserComponent,
    PasswordResetComponent
  ],

  exports: [LoaderComponent, PatientWizardComponent, CardioWizardComponent],
  providers: [PatientWizardService]
})
export class SharedModule {}
