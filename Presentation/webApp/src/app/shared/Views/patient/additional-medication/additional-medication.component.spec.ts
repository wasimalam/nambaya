import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdditionalMedicationComponent } from './additional-medication.component';

describe('AdditionalMedicationComponent', () => {
  let component: AdditionalMedicationComponent;
  let fixture: ComponentFixture<AdditionalMedicationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AdditionalMedicationComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdditionalMedicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
