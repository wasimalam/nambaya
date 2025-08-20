import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PhysicianAccountsComponent } from './physician-accounts.component';

describe('PhysicianAccountsComponent', () => {
  let component: PhysicianAccountsComponent;
  let fixture: ComponentFixture<PhysicianAccountsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PhysicianAccountsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhysicianAccountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
