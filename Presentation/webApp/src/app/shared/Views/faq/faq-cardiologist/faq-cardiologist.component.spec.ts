import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FaqCardiologistComponent } from './faq-cardiologist.component';

describe('FaqCardiologistComponent', () => {
  let component: FaqCardiologistComponent;
  let fixture: ComponentFixture<FaqCardiologistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [FaqCardiologistComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FaqCardiologistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
