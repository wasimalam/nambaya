import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardiologistAccountsComponent } from './cardiologist-accounts.component';

describe('CardiologistAccountsComponent', () => {
  let component: CardiologistAccountsComponent;
  let fixture: ComponentFixture<CardiologistAccountsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CardiologistAccountsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardiologistAccountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
