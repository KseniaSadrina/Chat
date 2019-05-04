import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User } from '../models/User';
import { ME } from '../helpers/mocks';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }
  public currentUser = new BehaviorSubject<User>(ME);

}
