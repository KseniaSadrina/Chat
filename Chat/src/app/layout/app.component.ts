import { Component } from '@angular/core';
import { Observable, Subscription, BehaviorSubject } from 'rxjs';
import { Message } from './../models/Message';
import { SessionsService } from './../services/sessions.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = 'chat-client';
  public authentication = true;
  constructor() { }


  ngOnInit() {

  }


}
