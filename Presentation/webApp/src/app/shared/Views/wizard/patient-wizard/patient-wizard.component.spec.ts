import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientWizardComponent } from './patient-wizard.component';

describe('PatientWizardComponent', () => {
  let component: PatientWizardComponent;
  let fixture: ComponentFixture<PatientWizardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PatientWizardComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PatientWizardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
