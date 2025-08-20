import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacistAccountsComponent } from './pharmacist-accounts.component';

describe('PharmacistAccountsComponent', () => {
  let component: PharmacistAccountsComponent;
  let fixture: ComponentFixture<PharmacistAccountsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PharmacistAccountsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PharmacistAccountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
