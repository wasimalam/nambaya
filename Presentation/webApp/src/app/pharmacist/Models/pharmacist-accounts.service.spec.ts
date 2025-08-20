import { TestBed } from '@angular/core/testing';

import { PharmacistAccountsService } from './pharmacist-accounts.service';

describe('PharmacistAccountsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PharmacistAccountsService = TestBed.get(PharmacistAccountsService);
    expect(service).toBeTruthy();
  });
});
