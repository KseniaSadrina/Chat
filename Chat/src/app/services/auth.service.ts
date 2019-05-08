import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User } from '../models/User';
import { ME } from '../helpers/mocks';
import { Router, RouterEvent } from '@angular/router';
import { filter, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  loginUrl = '/home/login';

  constructor(private router: Router) {
    this.router.events.subscribe(((event: RouterEvent) => {
      if (event.url === this.loginUrl)  {
        this.isLoggingIn.next(true);
      } else if (this.isLoggingIn.getValue()) {
        this.isLoggingIn.next(false);
      }
    }).bind(this));
  }
  public currentUser = new BehaviorSubject<User>(ME);

  private isLoggedIn = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedIn.asObservable();

  private isLoggingIn = new BehaviorSubject<boolean>(false);
  public isLoggingIn$ = this.isLoggingIn.asObservable();

  public login() {
  }

  public logout() {

  }

}
