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
export class LoginComponent implements OnInit, OnDestroy, AfterViewInit {

  constructor(private dialog: MatDialog,
              private router: Router,
              private route: ActivatedRoute) { }

  isOpen = false;
  dialogRef: MatDialogRef<LoginDialogComponent>;
  dialogRefSub: Subscription;

  ngOnInit() {
  }

  ngAfterViewInit(): void {
    if (this.isOpen) { return; }
    setTimeout(() => this.openLoginDialog());
  }

  ngOnDestroy(): void {
    if (this.dialogRefSub)  { this.dialogRefSub.unsubscribe(); }
  }

  openLoginDialog() {
    this.dialogRef = this.dialog.open(LoginDialogComponent, { closeOnNavigation: true, disableClose: true } );
    this.isOpen = true;
    this.dialogRefSub =  this.dialogRef.afterClosed().subscribe((() => {
      this.isOpen = false;
    }).bind(this));

  }


}
