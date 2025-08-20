import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListNurseComponent } from './list-nurse.component';

describe('ListNurseComponent', () => {
  let component: ListNurseComponent;
  let fixture: ComponentFixture<ListNurseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListNurseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListNurseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
