import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacistSettingsComponent } from './pharmacist-settings.component';

describe('PharmacistSettingsComponent', () => {
  let component: PharmacistSettingsComponent;
  let fixture: ComponentFixture<PharmacistSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PharmacistSettingsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PharmacistSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
