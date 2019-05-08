import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { User } from 'src/app/models/User';
import { map, filter } from 'rxjs/operators';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material';
import { LoginComponent } from 'src/app/components/login/login.component';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-head',
  templateUrl: './head.component.html',
  styleUrls: ['./head.component.css']
})
export class HeadComponent implements OnInit {

  title = 'Cyber Training ChatRoom';
  isLoggedIn: Observable<boolean>;
  loggedInUser: Observable<string>;
  isLoggingIn: Observable<boolean>;


  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.isLoggedIn = this.authService.isLoggedIn$;
    this.loggedInUser = this.authService.currentUser.pipe(map((user: User) => user.fullName));
    this.isLoggingIn = this.authService.isLoggingIn$;
  }

  logout() {
    this.authService.logout();
  }

}
