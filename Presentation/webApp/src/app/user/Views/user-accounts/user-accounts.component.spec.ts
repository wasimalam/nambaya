import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAccountsComponent } from './user-accounts.component';

describe('AccountsComponent', () => {
  let component: UserAccountsComponent;
  let fixture: ComponentFixture<UserAccountsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UserAccountsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserAccountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
