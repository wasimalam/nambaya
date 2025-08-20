import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ShellComponent } from './shell.component';
import { HeaderComponent } from './header/header.component';
import { AvatarModule } from 'ngx-avatar';
import {
  IgxButtonModule,
  IgxIconModule,
  IgxLayoutModule,
  IgxNavigationDrawerModule,
  IgxRippleModule,
  IgxToggleModule,
  IgxDropDownModule,
  IgxSwitchModule
} from 'igniteui-angular';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    NgbModule,
    RouterModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    IgxButtonModule,
    IgxIconModule,
    IgxLayoutModule,
    IgxNavigationDrawerModule,
    IgxRippleModule,
    IgxToggleModule,
    AvatarModule,
    IgxDropDownModule
  ],
  declarations: [HeaderComponent, ShellComponent]
})
export class ShellModule {}
