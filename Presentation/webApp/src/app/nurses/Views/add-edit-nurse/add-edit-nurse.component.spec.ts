import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditNurseComponent } from './add-edit-nurse.component';

describe('AddEditNurseComponent', () => {
  let component: AddEditNurseComponent;
  let fixture: ComponentFixture<AddEditNurseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddEditNurseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditNurseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
