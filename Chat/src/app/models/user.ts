import { ChatSession } from './chatSession';
import { UserType } from './enums/UserType';

export class User {

    id: number;
    fullName: string;
    userName: string;
    type: UserType;
}