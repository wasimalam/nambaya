import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailSmsLogsComponent } from './email-sms-logs.component';

describe('EmailSmsLogsComponent', () => {
  let component: EmailSmsLogsComponent;
  let fixture: ComponentFixture<EmailSmsLogsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EmailSmsLogsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmailSmsLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
