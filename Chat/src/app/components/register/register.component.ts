import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { MatDialogRef, MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';
import { RegisterDialogComponent } from './register-dialog/register-dialog.component';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, AfterViewInit {

  constructor(private dialog: MatDialog,
              private router: Router,
              private route: ActivatedRoute) { }

  dialogRef: MatDialogRef<RegisterDialogComponent>;
  dialogRefSub: Subscription;

  ngOnInit() {
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.openLoginDialog());
  }

  openLoginDialog() {
    this.dialogRef = this.dialog.open(RegisterDialogComponent,
      { closeOnNavigation: true, disableClose: true, height: '500px', width: '430px' } );
  }
}
