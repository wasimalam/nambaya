import { TestBed } from '@angular/core/testing';

import { UserAccountsService } from './user-accounts.service';

describe('UserAccountsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UserAccountsService = TestBed.get(UserAccountsService);
    expect(service).toBeTruthy();
  });
});
