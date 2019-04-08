import { Component, OnInit, OnDestroy } from '@angular/core';
import { SessionsService } from 'src/app/services/sessions.service';
import { Observable, Subscription, BehaviorSubject, zip } from 'rxjs';
import { Message } from 'src/app/models/Message';
import { FormControl, Validators } from '@angular/forms';
import { ChatSession } from 'src/app/models/chatSession';
import { switchMap, map, combineAll } from 'rxjs/operators';
import { ParamMap, Router, ActivatedRoute } from '@angular/router';
import { ME } from 'src/app/helpers/mocks';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit, OnDestroy {
  
  
  constructor(private sessionsService: SessionsService, private route: ActivatedRoute, private router: Router) { }
  
  message = new FormControl('',  Validators.required);;
  currentSession: Observable<ChatSession>;  
  messages = new BehaviorSubject<Message[]>([]);
  subscriptions: Subscription[] = [];

  ngOnInit() {
    
    this.currentSession = this.sessionsService.currentSession$;

    let sub = this.currentSession.subscribe(newSession => {
      if (!newSession) return;
      this.messages.next(newSession.messages);
    });


    let sub1 = this.route.paramMap.pipe(
      map((params: ParamMap) => {
        this.sessionsService.selected$.next(+params.get('id'))
      })
      ).subscribe();
      
    this.subscriptions.push(sub);
    this.subscriptions.push(sub1);
  } 

  public sendMessage(): void {
      const data = new Message();
      data.sender = ME.userName;
      data.text = this.message.value;
      data.timestamp = new Date(Date.now());
      this.sessionsService.sendMessage(data);
      this.message.setValue("");
      this.message.markAsPristine();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(item => {
      if (item) item.unsubscribe();
    });
  }

}
