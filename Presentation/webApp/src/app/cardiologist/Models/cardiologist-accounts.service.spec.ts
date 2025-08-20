import { TestBed } from '@angular/core/testing';

import { CardiologistAccountsService } from './cardiologist-accounts.service';

describe('CardiologistAccountsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CardiologistAccountsService = TestBed.get(CardiologistAccountsService);
    expect(service).toBeTruthy();
  });
});
