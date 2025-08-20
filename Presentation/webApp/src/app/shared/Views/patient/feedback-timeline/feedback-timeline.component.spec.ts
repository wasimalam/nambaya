import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FeedbackTimelineComponent } from './feedback-timeline.component';

describe('FeedbackTimelineComponent', () => {
  let component: FeedbackTimelineComponent;
  let fixture: ComponentFixture<FeedbackTimelineComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [FeedbackTimelineComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FeedbackTimelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
