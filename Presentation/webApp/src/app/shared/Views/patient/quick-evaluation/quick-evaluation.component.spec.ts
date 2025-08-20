import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QuickEvaluationComponent } from './quick-evaluation.component';

describe('QuickEvaluationComponent', () => {
  let component: QuickEvaluationComponent;
  let fixture: ComponentFixture<QuickEvaluationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [QuickEvaluationComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuickEvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
