import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FaqPharmacistComponent } from './faq-pharmacist.component';

describe('FaqPharmacistComponent', () => {
  let component: FaqPharmacistComponent;
  let fixture: ComponentFixture<FaqPharmacistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [FaqPharmacistComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FaqPharmacistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
