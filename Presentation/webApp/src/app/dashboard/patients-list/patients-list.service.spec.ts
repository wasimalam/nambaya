import { TestBed } from '@angular/core/testing';

import { PatientsListService } from './patients-list.service';

describe('PatientsListService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PatientsListService = TestBed.get(PatientsListService);
    expect(service).toBeTruthy();
  });
});
