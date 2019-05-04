import { SessionType } from './enums/SessionType';
import { Message } from './Message';

export class ChatSession {
    id: number;
    name: string;
    type: SessionType;
    trainingId: number;
    messages: Message[];
}
