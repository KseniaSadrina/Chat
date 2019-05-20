import { UserType } from './enums/UserType';

export class User {

    id: number;
    fullName: string;
    userName: string;
    type: UserType;
    unreadMessages: { [sessionName: string]: number };
    accessToken: string;
    refreshToken: string;
    role: string;

    public constructor(init?: Partial<User>) {
      Object.assign(this, init);
  }
}

export class RegistrationUser {

  userName: string;
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  type: UserType;
  role: string;

  public constructor(init?: Partial<RegistrationUser>) {
    Object.assign(this, init);
}
}
