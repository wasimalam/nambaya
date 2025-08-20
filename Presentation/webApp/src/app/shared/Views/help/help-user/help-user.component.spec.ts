import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HelpUserComponent } from './help-user.component';

describe('HelpUserComponent', () => {
  let component: HelpUserComponent;
  let fixture: ComponentFixture<HelpUserComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [HelpUserComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HelpUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
