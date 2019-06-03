import { Component, OnInit, OnDestroy } from '@angular/core';
import { Validators, FormControl, FormGroup, AbstractControl, ValidationErrors } from '@angular/forms';
import { CustomAuthService } from 'src/app/services/custom-auth.service';
import { Router, RouterEvent, NavigationStart } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatDialogRef } from '@angular/material';
import { Subscription, BehaviorSubject } from 'rxjs';
import { User, RegistrationUser } from 'src/app/models/User';
import { HttpErrorResponse } from '@angular/common/http';
import { DFLTERROR } from 'src/app/helpers/mocks';
import { UserType } from 'src/app/models/enums/user-type';

@Component({
  selector: 'app-register-dialog',
  templateUrl: './register-dialog.component.html',
  styleUrls: ['./register-dialog.component.css']
})
export class RegisterDialogComponent implements OnInit, OnDestroy {

  constructor(private authService: CustomAuthService,
              private router: Router,
              private dialogRef: MatDialogRef<RegisterDialogComponent>) { }

  public registerForm: FormGroup;
  private sub: Subscription;
  public isLoading = new BehaviorSubject<boolean>(false);
  public serverError = new BehaviorSubject<string>(null);
  public roles = new BehaviorSubject<string[]>(['Trainee', 'Instructor']);

  // ***** Life-cycle hooks *****

  ngOnInit() {
    this.buildRegisterForm();
    this.sub = this.router.events.pipe(
      filter((event: RouterEvent) => event instanceof NavigationStart),
      filter(() => !!this.dialogRef)
    )
    .subscribe(() => {
      this.dialogRef.close();
    });
  }

  ngOnDestroy() {
    if (this.sub) { this.sub.unsubscribe(); }
  }

  // ***** Methods *****

  buildRegisterForm() {
    this.registerForm = new FormGroup({
      userName: new FormControl(
        '',
        [Validators.required, Validators.minLength(3)],
        [this.nameFree.bind(this)] ),
      firstName: new FormControl('', [Validators.required, Validators.minLength(3)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(3)]),
      credentials: new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(6)]),
        verify: new FormControl('', Validators.required)
      }, this.verifyPassword),
      email: new FormControl('', [Validators.required, Validators.email]),
      role: new FormControl(this.roles.getValue()[0], []),
    });
  }


  delay(millis: number): Promise<void>  {
    return new Promise((resolve, reject) => {
      setTimeout(resolve, millis);
    });
  }

  async register() {
    this.isLoading.next(true);
    const user: RegistrationUser = {
      userName: this.registerForm.controls.userName.value,
      firstName: this.registerForm.controls.firstName.value,
      lastName: this.registerForm.controls.lastName.value,
      password: this.registerForm.controls.credentials.get('password').value,
      type: UserType.Human,
      role: this.registerForm.controls.role.value,
      email: this.registerForm.controls.email.value
    };
    try {
      const res = await this.authService.register(user);
      this.isLoading.next(false);
      this.router.navigate(['home/login']);
    } catch (error) {
        this.isLoading.next(false);
        if (!error || !error.statusText) { this.serverError.next(DFLTERROR); }
        this.serverError.next(error.statusText);
    }

  }

  // ***** Validators *****

  async nameFree(control: AbstractControl): Promise<null | ValidationErrors> {
    const userName = control.value as string;
    if (!userName) { return null; }

    const res = await this.authService.doesUsernameExist(userName);
    if (!res) {
      return { nameFree: true };
    }

    return null;
  }

  verifyPassword(control: AbstractControl): null | ValidationErrors {
    try {
      const pw = (control.get('password') as FormControl).value;
      const vf = (control.get('verify') as FormControl).value;

      if (pw === vf) { return null; }
      return { verifyPassword: true};
    } catch {
      return null;
    }
  }

}
