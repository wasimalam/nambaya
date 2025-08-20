import { TestBed } from '@angular/core/testing';

import { PatientWizardService } from './patient-wizard.service';

describe('PatientWizardService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PatientWizardService = TestBed.get(PatientWizardService);
    expect(service).toBeTruthy();
  });
});
