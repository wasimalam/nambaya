import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportPatientComponent } from './import-patient.component';

describe('ImportPatientComponent', () => {
  let component: ImportPatientComponent;
  let fixture: ComponentFixture<ImportPatientComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ImportPatientComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportPatientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
