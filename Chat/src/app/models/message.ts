import { UserType } from './enums/user-type';

export class Message {

    id: number;
    sender: string;
    senderType: UserType;
    text: string;
    sessionName: string;
    timestamp: Date;
    chatSessionId: number;

    public constructor(init?: Partial<Message >) {
        Object.assign(this, init);
    }
}
