import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ValidationErrors, AbstractControl } from '@angular/forms';
import { CustomAuthService } from 'src/app/services/custom-auth.service';
import { MatDialogRef } from '@angular/material';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';



// function onSignIn(googleUser) {
//   const profile = googleUser.getBasicProfile();
//   console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
//   console.log('Name: ' + profile.getName());
//   console.log('Image URL: ' + profile.getImageUrl());
//   console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
// }

@Component({
  selector: 'app-login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.css']
})

export class LoginDialogComponent implements OnInit {

  constructor(private authService: CustomAuthService,
              private router: Router,
              private dialogRef: MatDialogRef<LoginDialogComponent>) { }

  public loginForm: FormGroup;
  public serverError = new BehaviorSubject<string>('');

  ngOnInit() {
    this.buildLoginForm();
  }

  private buildLoginForm() {
    this.loginForm = new FormGroup({
      userName: new FormControl('', Validators.required),
      password: new FormControl(null, Validators.required),
    });
  }

  public navigateToRegister() {
    this.dialogRef.close();
    this.router.navigate(['home/register']);
  }

  public async login() {
    this.loginForm.markAsPristine();
    this.loginForm.markAsUntouched();
    this.serverError.next('');
    const res = await this.authService.login(this.loginForm.value);
    if (res && res instanceof HttpErrorResponse) {
      this.serverError.next(res.error);
    } else {
      let url = '';
      if (res) { url = res as string; }
      this.dialogRef.close();
      this.router.navigate([url]);
    }

  }

}
