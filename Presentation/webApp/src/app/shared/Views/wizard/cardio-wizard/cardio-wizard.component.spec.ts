import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardioWizardComponent } from './cardio-wizard.component';

describe('CardioWizardComponent', () => {
  let component: CardioWizardComponent;
  let fixture: ComponentFixture<CardioWizardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CardioWizardComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardioWizardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
