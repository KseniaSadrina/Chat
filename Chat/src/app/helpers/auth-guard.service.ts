
import { CanActivate, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AutoHelper } from './auth-helper.service';
import { CustomAuthService } from '../services/custom-auth.service';
import { take } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})

export class AuthGuard implements CanActivate {

  constructor(
    private router: Router,
    private authenticationService: CustomAuthService
) {}

  async canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean>  {
      const promise = this.authenticationService
      .isLoggedIn$
      .pipe(take(1))
      .toPromise();

      const res = await promise;

      if (!res) {
        this.authenticationService.onNextLoginNavigateTo(state.url);
        this.router.navigate(['home/login']);
      }

      return res;

  }

}
