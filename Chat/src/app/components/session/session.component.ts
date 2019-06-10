import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, AfterViewInit } from '@angular/core';
import { SessionsService } from 'src/app/services/sessions.service';
import { Observable, Subscription, of } from 'rxjs';
import { Message } from 'src/app/models/Message';
import { FormControl, Validators } from '@angular/forms';
import { ChatSession } from 'src/app/models/chat-session';
import { map } from 'rxjs/operators';
import { CustomAuthService } from 'src/app/services/custom-auth.service';
import { User } from 'src/app/models/User';
import { type } from 'os';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit, AfterViewChecked {

  constructor(private sessionsService: SessionsService,
              private auth: CustomAuthService) { }

  @ViewChild('scrollMe') private myScrollContainer: ElementRef;
  message = new FormControl('',  Validators.required);
  currentSession: Observable<ChatSession>;
  currentUser: Observable<User>;
  typingMessage: Observable<string>;
  messages: Observable<Message[]>;
  subscriptions: Subscription[] = [];

  ngOnInit() {
    this.currentUser = this.auth.currentUser$;
    this.currentSession = this.sessionsService.currentSession$;
    this.messages = this.currentSession.pipe(
      map(session => {
        let res: Message[] = null;
        if (session && session.messages && session.messages.length > 0) { res = session.messages; }
        return res;
      })
      );

    this.typingMessage = this.sessionsService.currentTypes$.pipe(
      map(typer => typer ? `${typer} is typing..` : null)
    );

  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  public sendMessage(currentUser: User): void {
    const data = new Message();
    data.sender = currentUser.userName;
    data.senderType = currentUser.type;
    data.text = this.message.value;
    data.timestamp = new Date(Date.now());
    this.sessionsService.sendMessage(data);
    this.message.setValue('');
    this.message.markAsPristine();
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) {

    }
  }
}
