import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class LoggedUserInfoService {
  private loggedUserName = new BehaviorSubject(localStorage.getItem('loggedUserName'));
  currentLoggedUserName = this.loggedUserName.asObservable();
  constructor() {}
  updateLoggedUserName(message: string) {
    localStorage.setItem('loggedUserName     ', message);

    this.loggedUserName.next(message);
  }
}
