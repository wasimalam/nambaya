import { TestBed } from '@angular/core/testing';

import { PhysicianAccountsService } from './physician-accounts.service';

describe('PhysicianAccountsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PhysicianAccountsService = TestBed.get(PhysicianAccountsService);
    expect(service).toBeTruthy();
  });
});
