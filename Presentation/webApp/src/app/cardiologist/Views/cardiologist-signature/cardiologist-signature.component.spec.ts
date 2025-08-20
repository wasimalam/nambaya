import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardiologistSignatureComponent } from './cardiologist-signature.component';

describe('CardiologistSignatureComponent', () => {
  let component: CardiologistSignatureComponent;
  let fixture: ComponentFixture<CardiologistSignatureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardiologistSignatureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardiologistSignatureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
