import { TestBed } from '@angular/core/testing';

import { UsersListService } from './users-list.service';

describe('UsersListService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UsersListService = TestBed.get(UsersListService);
    expect(service).toBeTruthy();
  });
});
