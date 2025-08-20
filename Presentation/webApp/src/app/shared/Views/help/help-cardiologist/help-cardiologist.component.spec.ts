import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HelpCardiologistComponent } from './help-cardiologist.component';

describe('HelpCardiologistComponent', () => {
  let component: HelpCardiologistComponent;
  let fixture: ComponentFixture<HelpCardiologistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [HelpCardiologistComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HelpCardiologistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
