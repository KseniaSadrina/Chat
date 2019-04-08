import { User } from './User';

export class Message {
    
    id: number;
    sender: string;
    text: string;
    sessionName: string;
    timestamp: Date;

    public constructor(init?: Partial<Message >) {
        Object.assign(this, init);
    }
}