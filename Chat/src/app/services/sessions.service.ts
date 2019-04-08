import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, zip, throwError, combineLatest } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Message } from '../models/Message';
import { ServiceBase } from './serviceBase';
import { ChatSession } from '../models/chatSession';
import { map, catchError } from 'rxjs/operators';
import { ME } from '../helpers/mocks';
import { User } from '../models/User';
import { ActivatedRoute } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class SessionsService extends ServiceBase {
  
  protected hubType = 'message';
  sessionURL = 'api/chatSession';

  private currentUser: User = ME;
  private sessions = new BehaviorSubject<{ [sessionName: string] : ChatSession }>({});
  private currentSession = new BehaviorSubject<ChatSession>(null);
  
  public currentSession$ = this.currentSession.asObservable();
  public sessions$ = this.sessions.asObservable();
  public selected$ = new BehaviorSubject<number>(-1);
  
  constructor(private httpService: HttpClient) {
    super();
    this.initHub();
    
    // retrieve existing messages from server and unsubscribe
    let sub = this.getUserSessionsFromServer().subscribe( res => {
      res.forEach(item => this.joinSession(item.trainingId));
      sub.unsubscribe();
    });

    this.sessions.subscribe( (sessions: { [ name : string ] : ChatSession}) => {
      this.updateSelected(sessions);
    });

    this.selected$.subscribe(selected => {
      let sessions = this.sessions.getValue();
      this.updateSelected(sessions);
    });
  }

  // retrieves all the sessions of the user from the server by user id
  private getUserSessionsFromServer(): Observable<ChatSession[]> {
    let fullURL = this.sessionURL + "?userId=" + this.currentUser.id;
    return this.httpService.get<ChatSession[]>(fullURL);
  }

  // retrieves all the sessions of the user from the server by user id
  private getSessionByTrainingIdFromServer(trainingId: number): Observable<ChatSession> {
    let fullURL = this.sessionURL + "?userId=" + this.currentUser.id + "&trainingId=" + trainingId;
    return this.httpService.get<ChatSession>(fullURL);
  }

  public sendMessage(message: Message) {
    // send message 
    if (this.hubConnection) {
      message.sessionName = this.currentSession.getValue().name;
      this.hubConnection.invoke('SendToAll', message);
    }
  }

  // hub implementations
  protected subscribeToHub() {
    // first validate we have retrieved the messages from the server
    // then subscribe to new messages
    if (this.hubConnection) {

      this.hubConnection.on('group', (notification: string) => {
        console.log(notification);
      });

      // receive new notifications about group activity
      this.hubConnection.on('groupJoin', (session: ChatSession) => {
        if (!session) return;
        let sessions = this.sessions.getValue();
        sessions[session.name] = session;
        this.sessions.next(sessions);
      });

      this.hubConnection.on('groupLeave', (groupName: string) => {
          let sessions = this.sessions.getValue();
          delete sessions[groupName];
          this.sessions.next(sessions);
      });

      // receive new messages
      this.hubConnection.on('message', (message: Message) => {
        if (!message) return;
        let sessions = this.sessions.getValue();
        let session = sessions[message.sessionName];
        if(!session.messages) session.messages = [];
        session.messages.push(message);
        sessions[message.sessionName] = session;
        this.sessions.next(sessions);
      });
    }
  }

  public joinSession(trainingId: number) {
    if (this.hubConnection) {
      this.hubConnection.invoke('AddToSession', trainingId, this.currentUser.id);
    }
  }

  public leaveSession(sessionName: string) {
    if (this.hubConnection) {
      this.hubConnection.invoke('RemoveFromSession', sessionName, this.currentUser.id);
    }
  }

  updateSelected(sessions: {[name: string] : ChatSession}) {
    let curr = this.selected$.getValue();
    let selectedItem = Object.keys(sessions).find(item => sessions[item].id === curr);
    this.currentSession.next(sessions[selectedItem]);
  }

}
