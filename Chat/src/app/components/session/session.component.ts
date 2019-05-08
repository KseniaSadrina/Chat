import { Component, OnInit, OnDestroy } from '@angular/core';
import { SessionsService } from 'src/app/services/sessions.service';
import { Observable, Subscription, of } from 'rxjs';
import { Message } from 'src/app/models/Message';
import { FormControl, Validators } from '@angular/forms';
import { ChatSession } from 'src/app/models/chatSession';
import { map } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { ME } from 'src/app/helpers/mocks';
import { CustomAuthService } from 'src/app/services/custom-auth.service';
import { User } from 'src/app/models/User';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit {


  constructor(private sessionsService: SessionsService,
              private auth: CustomAuthService,
              private route: ActivatedRoute,
              private router: Router) { }

  message = new FormControl('',  Validators.required);
  currentSession: Observable<ChatSession>;
  currentUser: Observable<User>;
  messages: Observable<Message[]>;
  subscriptions: Subscription[] = [];

  ngOnInit() {
    this.currentUser = this.auth.currentUser;
    this.currentSession = this.sessionsService.currentSession$;
    this.messages = this.currentSession.pipe(
      map(session => {
        let res: Message[] = [];
        if (session && session.messages) { res = session.messages; }
        return res;
      })
    );

  }

  public sendMessage(): void {
      const data = new Message();
      data.sender = ME.userName;
      data.text = this.message.value;
      data.timestamp = new Date(Date.now());
      this.sessionsService.sendMessage(data);
      this.message.setValue('');
      this.message.markAsPristine();
  }

  onKeydown(event: KeyboardEvent) {
    if (event.keyCode === 13) { this.sendMessage(); }
  }
}
