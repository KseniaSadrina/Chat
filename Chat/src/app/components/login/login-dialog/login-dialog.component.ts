import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { GoogleLoginProvider, AuthService } from 'angularx-social-login';
import { CustomAuthService } from 'src/app/services/custom-auth.service';



function onSignIn(googleUser) {
  const profile = googleUser.getBasicProfile();
  console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
  console.log('Name: ' + profile.getName());
  console.log('Image URL: ' + profile.getImageUrl());
  console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
}

@Component({
  selector: 'app-login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.css']
})

export class LoginDialogComponent implements OnInit {

  constructor(private authService: CustomAuthService) { }

  public loginForm = new FormGroup({
    userName: new FormControl('', Validators.required),
    password: new FormControl(null, Validators.required),
  });

  ngOnInit() {
  }


}
