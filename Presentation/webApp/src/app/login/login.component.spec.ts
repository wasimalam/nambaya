import { async, TestBed, ComponentFixture } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './login.component';
import { IgxInputGroupModule, IgxIconModule, IgxButtonModule, IgxRippleModule } from 'igniteui-angular';

const MAIL_GROUP_NAME = 'email';
const PASSWORD_GROUP_NAME = 'password';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [
        ReactiveFormsModule,
        NoopAnimationsModule,
        IgxInputGroupModule,
        IgxIconModule,
        IgxButtonModule,
        IgxRippleModule
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not allow login until valid', async () => {
    expect(component.loginForm.valid).toBeFalsy();
    component.loginForm.controls[MAIL_GROUP_NAME].setValue('test@example.com');
    expect(component.loginForm.valid).toBeFalsy();
    component.loginForm.controls[PASSWORD_GROUP_NAME].setValue('123456');
    expect(component.loginForm.valid).toBeTruthy();
  });
});
