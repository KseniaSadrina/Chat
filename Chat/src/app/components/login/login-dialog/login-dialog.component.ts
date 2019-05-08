import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.css']
})

export class LoginDialogComponent implements OnInit {

  constructor(private authService: AuthService) { }

  public loginForm = new FormGroup({
    userName: new FormControl('', Validators.required),
    password: new FormControl(null, Validators.required),
  });

  ngOnInit() {
  }

  login() {
    this.authService.login();
  }
}
