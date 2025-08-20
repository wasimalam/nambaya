import { TestBed } from '@angular/core/testing';

import { LoggedUserInfoService } from './logged-user-info.service';

describe('LoggedUserInfoService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LoggedUserInfoService = TestBed.get(LoggedUserInfoService);
    expect(service).toBeTruthy();
  });
});
