import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract, AuthenticationGuard } from '@app/core';
import { Shell } from '@app/shell/shell.service';
import { AdditionalMedicationComponent } from '@app/shared/Views/patient/additional-medication/additional-medication.component';
import { QuickEvaluationComponent } from '@app/shared/Views/patient/quick-evaluation/quick-evaluation.component';
import { FeedbackTimelineComponent } from '@app/shared/Views/patient/feedback-timeline/feedback-timeline.component';
import { DoctorListComponent } from '@app/shared/Views/patient/doctor-list/doctor-list.component';
import { DoctorCreateComponent } from '@app/shared/Views/patient/doctor-create/doctor-create.component';
import { DoctorEditComponent } from '@app/shared/Views/patient/doctor-edit/doctor-edit.component';
import { HelpComponent } from '@app/shared/Views/help/help.component';
import { FaqComponent } from '@app/shared/Views/faq/faq.component';
import {PasswordResetComponent} from "@app/shared/Views/password/password-reset/password-reset.component";

const routes: Routes = [
  Shell.childRoutes([
    {
      path: ':urlPrefix/additional-medication/:patientCaseId',
      component: AdditionalMedicationComponent,
      data: {
        title: extract('Medications'),
        roles: ['CentralGroupUser', 'Cardiologist', 'Pharmacist', 'Pharmacy', 'Nurse']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/patient/additional-medication/:patientCaseId/:patientId',
      component: AdditionalMedicationComponent,
      data: {
        title: extract('Medications'),
        roles: ['CentralGroupUser', 'Cardiologist', 'Pharmacist', 'Pharmacy', 'Nurse']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/patient/quick-evaluation/:patientCaseId',
      component: QuickEvaluationComponent,
      data: {
        title: extract('Quick Evaluation Report'),
        roles: ['CentralGroupUser', 'Cardiologist', 'Pharmacist', 'Pharmacy', 'Nurse']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/patient/quick-evaluation/:patientCaseId/:patientId',
      component: QuickEvaluationComponent,
      data: {
        title: extract('Quick Evaluation Report'),
        roles: ['CentralGroupUser', 'Cardiologist', 'Pharmacist', 'Pharmacy', 'Nurse']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/patient/feedback-timeline/:patientCaseId',
      component: FeedbackTimelineComponent,
      data: {
        title: extract('Patient Timeline'),
        roles: ['CentralGroupUser', 'Cardiologist', 'Pharmacist', 'Pharmacy']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/doctors-list',
      component: DoctorListComponent,
      data: { title: extract('doctors'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/doctors-list',
      component: DoctorListComponent,
      data: { title: extract('doctors'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/doctor/create',
      component: DoctorCreateComponent,
      data: { title: extract('add_doctor'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/doctor/create',
      component: DoctorCreateComponent,
      data: { title: extract('add_doctor'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'pharmacist/doctor/edit/:doctorId',
      component: DoctorEditComponent,
      data: { title: extract('edit_doctor'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: 'center/doctor/edit/:doctorId',
      component: DoctorEditComponent,
      data: { title: extract('edit_doctor'), roles: ['CentralGroupUser', 'Pharmacist', 'Pharmacy'] },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/help',
      component: HelpComponent,
      data: {
        title: extract('help'),
        roles: ['CentralGroupUser','Cardiologist','Pharmacist','Pharmacy','NambayaUser','Nurse','StakeHolder','PharmacyTrainer']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/faq',
      component: FaqComponent,
      data: {
        title: extract('faq'),
        roles: ['CentralGroupUser','Cardiologist','Pharmacist','Pharmacy','NambayaUser','Nurse','StakeHolder','PharmacyTrainer']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    },
    {
      path: ':urlPrefix/password/change',
      component: PasswordResetComponent,
      data: {
        title: extract('change_password'),
        roles: ['CentralGroupUser','Cardiologist','Pharmacist','Pharmacy','NambayaUser','Nurse','StakeHolder','PharmacyTrainer']
      },
      canActivate: [AuthenticationGuard],
      pathMatch: 'full'
    }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SharedRoutingModule {}
