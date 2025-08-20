import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactivatedPatientsComponent } from './deactivated-patients.component';

describe('DeactivatedPatientsComponent', () => {
  let component: DeactivatedPatientsComponent;
  let fixture: ComponentFixture<DeactivatedPatientsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DeactivatedPatientsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeactivatedPatientsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
