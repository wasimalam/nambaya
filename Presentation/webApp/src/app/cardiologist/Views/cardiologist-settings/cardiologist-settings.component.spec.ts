import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardiologistSettingsComponent } from './cardiologist-settings.component';

describe('CardiologistSettingsComponent', () => {
  let component: CardiologistSettingsComponent;
  let fixture: ComponentFixture<CardiologistSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CardiologistSettingsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardiologistSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
