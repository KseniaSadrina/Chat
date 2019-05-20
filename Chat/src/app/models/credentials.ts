export class Credentials {
  userName: string;
  password: string;

  public constructor(init?: Partial<Credentials>) {
    Object.assign(this, init);
}
}
