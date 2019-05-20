import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User, RegistrationUser } from '../models/User';
import { Router, RouterEvent } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { HttpResponse } from '@aspnet/signalr';
import { Credentials } from '../models/credentials';

@Injectable({
  providedIn: 'root'
})
export class CustomAuthService {

  loginUrl = '/home/login';

  serverLoginUrl = 'api/login';
  serverRegisterUrl = 'api/register';
  currentUserStr = 'currentUser';

  constructor(private router: Router,
              private httpClient: HttpClient) {

    // subscribe to routing event to know when you navigate to the login page
    // then, update the "isloggingIn" var properly.
    this.router.events.subscribe(((event: RouterEvent) => {
      if (event.url === this.loginUrl)  {
        this.isLoggingIn.next(true);
      } else if (this.isLoggingIn.getValue()) {
        this.isLoggingIn.next(false);
      }
    }).bind(this));

    // update the is logged in when the loggedin user is changed
    this.currentUser.subscribe(change => {
      this.currentUserValue = change;
      if (change) {
        this.isLoggedIn.next(true);
      } else {
        this.isLoggedIn.next(false);
      }
    });
  }

  private currentUser = new BehaviorSubject<User>(JSON.parse(localStorage.getItem(this.currentUserStr)));
  public currentUser$ = this.currentUser.asObservable();
  public currentUserValue: User = null;

  private targetUrl: string | null;

  private isLoggedIn = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedIn.asObservable();

  private isLoggingIn = new BehaviorSubject<boolean>(false);
  public isLoggingIn$ = this.isLoggingIn.asObservable();

  public async login(credentials: Credentials): Promise<HttpErrorResponse | string> {
    try {
      const res = await this.httpClient.post<User>(this.serverLoginUrl, credentials).toPromise();
      if (res) {
        localStorage.setItem(this.currentUserStr, JSON.stringify(res));
        this.currentUser.next(res);
        if (this.targetUrl) {
          const url = this.targetUrl;
          this.targetUrl = null;
          return url;
        }
        return null;
      } else { this.router.navigateByUrl(''); }
    } catch (error) {
      return error as HttpErrorResponse;
    }
  }

  // returns whether this username is taken from the db
  public register(userRegistration: RegistrationUser): Promise<HttpResponse> {
    return this.httpClient.post<HttpResponse>(this.serverRegisterUrl, userRegistration).toPromise();
  }

  // returns whether this username is taken from the db
  public async doesUsernameExist(username: string): Promise<boolean> {
    const fullUrl = this.serverRegisterUrl + '?userName=' + username;
    return this.httpClient.get<boolean>(fullUrl).toPromise();
  }

  public logout(): Promise<void> {
    localStorage.removeItem(this.currentUserStr);
    this.currentUser.next(null);
    this.router.navigate([this.loginUrl]);
    return Promise.resolve();
  }

  public onNextLoginNavigateTo(url: string) {
    this.targetUrl = url;
  }
}
