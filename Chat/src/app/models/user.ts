import { UserType } from './enums/UserType';

export class User {

    id: number;
    fullName: string;
    userName: string;
    type: UserType;
    unreadMessages: { [sessionName: string]: number };
}
