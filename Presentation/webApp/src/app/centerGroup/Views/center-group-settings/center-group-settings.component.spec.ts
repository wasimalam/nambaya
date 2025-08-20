import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CenterGroupSettingsComponent } from './center-group-settings.component';

describe('CenterGroupSettingsComponent', () => {
  let component: CenterGroupSettingsComponent;
  let fixture: ComponentFixture<CenterGroupSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CenterGroupSettingsComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CenterGroupSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
