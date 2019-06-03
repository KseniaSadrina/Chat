import { SessionType } from './enums/session-type';
import { Message } from './Message';

export class ChatSession {
    id: number;
    name: string;
    type: SessionType;
    trainingId: number;
    messages: Message[];
}
