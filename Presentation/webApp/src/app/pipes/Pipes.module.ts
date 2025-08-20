import { EmailSMSTypePipe } from './emailSMStypes.pipe';
import { SortPipe } from './sorting.pipe';
import { StatusPipe } from './status.pipe';
import { AutocompletePipeStartsWith } from './startswith.pipe';
import { NgModule } from '@angular/core';
import { AutocompleteDeviceStartsWith } from './devicefilter.pipe';
import { GenderPipe } from './gender.pipe';
import { EvaluationStatusPipe } from './evaluationStatus.pipe';
import { AgePipe } from './age.pipe';
import { PHarmacyNamePipe } from './pharmacyNameFilter.pipe';
import { PharmaciesStartsWithPipe } from './pharmaciesStartWith.pipe';
import { DeviceAssignedCheck } from './assignedfilter.pipe';
import { QeStatusesPipe } from '@app/pipes/qestatuses.pipe';
import {PharmacyIdentificationPipe} from '@app/pipes/pharmacyIdentificationFilter.pipe';

@NgModule({
  imports: [],
  declarations: [
    AutocompletePipeStartsWith,
    AutocompleteDeviceStartsWith,
    StatusPipe,
    SortPipe,
    GenderPipe,
    EvaluationStatusPipe,
    AgePipe,
    PHarmacyNamePipe,
    PharmacyIdentificationPipe,
    PharmaciesStartsWithPipe,
    DeviceAssignedCheck,
    EmailSMSTypePipe,
    QeStatusesPipe
  ],
  exports: [
    AutocompletePipeStartsWith,
    AutocompleteDeviceStartsWith,
    StatusPipe,
    SortPipe,
    GenderPipe,
    EvaluationStatusPipe,
    AgePipe,
    PHarmacyNamePipe,
    PharmacyIdentificationPipe,
    PharmaciesStartsWithPipe,
    DeviceAssignedCheck,
    EmailSMSTypePipe,
    QeStatusesPipe
  ]
})
export class PipesModule {}
