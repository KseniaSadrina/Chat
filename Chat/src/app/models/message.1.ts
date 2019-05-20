import { UserType } from './enums/UserType';

export class Message {

    id: number;
    sender: string;
    senderType: UserType;
    text: string;
    sessionName: string;
    timestamp: Date;
    ChatSessionId: number;

    public constructor(init?: Partial<Message >) {
        Object.assign(this, init);
    }
}
