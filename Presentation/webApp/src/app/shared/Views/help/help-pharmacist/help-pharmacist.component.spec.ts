import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HelpPharmacistComponent } from './help-pharmacist.component';

describe('HelpPharmacistComponent', () => {
  let component: HelpPharmacistComponent;
  let fixture: ComponentFixture<HelpPharmacistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [HelpPharmacistComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HelpPharmacistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
