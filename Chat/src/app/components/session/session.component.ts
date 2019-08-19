import { Component, OnInit, ViewChild, ElementRef, AfterViewChecked, ChangeDetectionStrategy, Input } from '@angular/core';
import { SessionsService } from 'src/app/services/sessions.service';
import { Observable, Subscription, of } from 'rxjs';
import { Message } from 'src/app/models/Message';
import { FormControl, Validators } from '@angular/forms';
import { ChatSession } from 'src/app/models/chat-session';
import { map, startWith } from 'rxjs/operators';
import { CustomAuthService } from 'src/app/services/custom-auth.service';
import { User } from 'src/app/models/User';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SessionComponent implements OnInit, AfterViewChecked {
  disableScrollDown: any;

  constructor(private sessionsService: SessionsService,
              private auth: CustomAuthService) { }

  @ViewChild('scrollMe') private myScrollContainer: ElementRef;
  message = new FormControl('',  Validators.required);
  currentSession: Observable<ChatSession>;
  currentUser: Observable<User>;
  typingMessage: Observable<string>;
  messages: Observable<Message[]>;
  subscriptions: Subscription[] = [];
  myControl = new FormControl();
  tags: string[] = [
    '@Marley',
    '#abstract'
  ];
  filteredTags: Observable<string[]>;


  ngOnInit() {
    this.currentUser = this.auth.currentUser$;
    this.currentSession = this.sessionsService.currentSession$;
    this.messages = this.currentSession.pipe(
      map(session => {
        let res: Message[] = null;
        if (session && session.messages && session.messages.length > 0) {
          res = session.messages;
          this.disableScrollDown = false;
        }
        return res;
      })
    );
    this.filteredTags = this.message.valueChanges
    .pipe(
      startWith(''),
      map(value => this._filter(value))
    );
    this.typingMessage = this.sessionsService.currentTypes$.pipe(
      map(typer => typer ? `${typer} is typing..` : null)
    );
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.tags.filter(option => option.toLowerCase().includes(filterValue));
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

  private onScroll() {
    const element = this.myScrollContainer.nativeElement;
    const atBottom = element.scrollHeight - element.scrollTop === element.clientHeight;
    if (this.disableScrollDown && atBottom) {
        this.disableScrollDown = false;
    } else {
        this.disableScrollDown = true;
    }
  }

  scrollToBottom(): void {
    if (this.disableScrollDown) {
      return;
    }
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}
