import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { MatDialogRef, MatDialog } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginDialogComponent } from './login-dialog/login-dialog.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, AfterViewInit {

  constructor(private dialog: MatDialog,
              private router: Router,
              private route: ActivatedRoute) { }

  dialogRef: MatDialogRef<LoginDialogComponent>;
  dialogRefSub: Subscription;

  ngOnInit() {
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.openLoginDialog());
  }

  openLoginDialog() {
    this.dialogRef = this.dialog.open(LoginDialogComponent, { closeOnNavigation: true, disableClose: true } );
  }


}
