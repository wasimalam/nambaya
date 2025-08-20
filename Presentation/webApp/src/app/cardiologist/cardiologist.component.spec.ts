import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardiologistComponent } from './cardiologist.component';

describe('CardiologistComponent', () => {
  let component: CardiologistComponent;
  let fixture: ComponentFixture<CardiologistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CardiologistComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardiologistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
