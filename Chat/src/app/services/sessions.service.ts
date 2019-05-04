import { Injectable, ɵConsole } from '@angular/core';
import { Observable, BehaviorSubject, zip, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Message } from '../models/Message';
import { ServiceBase } from './serviceBase';
import { ChatSession } from '../models/chatSession';
import { map, combineLatest } from 'rxjs/operators';
import { ME } from '../helpers/mocks';
import { User } from '../models/User';
import { ActivatedRoute } from '@angular/router';
import { TrainingsService } from './trainings.service';
import { Session } from 'protractor';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})

export class SessionsService extends ServiceBase {

  protected hubType = 'message';
  sessionURL = 'api/chatSessions';

  private currentUser: BehaviorSubject<User>;

  private currentSession = new BehaviorSubject<ChatSession>(null);
  public currentSession$ = this.currentSession.asObservable();

  private sessions = new BehaviorSubject<ChatSession[]>([]);
  public sessions$ = this.sessions.asObservable();

  constructor(private httpService: HttpClient,
              private trainings: TrainingsService,
              private authService: AuthService) {
    super();
    this.currentUser = this.authService.currentUser;
    this.initHub();

    // retrieve existing sessions from the server and re-join to them
    this.getUserSessionsFromServer().subscribe( res => {
      if (!res) { return; }
      res.forEach(item => this.joinToSession(item.trainingId));
    });

    this.sessions.pipe(
      combineLatest(this.trainings.currentTraining$, (sessions, training) => {
        // if the training is not null and the user has already joined the session in the past, switch the selected session
        if (training) {
          const session = sessions.find(s => s.trainingId === training.id);
          this.currentSession.next(session);
        }
      })
    ).subscribe();
  }

  // retrieves all the sessions of the user from the server by user id
  private getUserSessionsFromServer(): Observable<ChatSession[]> {
    const user = this.currentUser.getValue();
    if (!user) { return null; }
    const fullURL = this.sessionURL + '?userId=' + user.id;
    return this.httpService.get<ChatSession[]>(fullURL);
  }

  // retrieves all the sessions of the user from the server by user id
  private getSessionByTrainingIdFromServer(trainingId: number): Observable<ChatSession> {
    if (!this.currentUser.getValue()) { return null; }
    const fullURL = this.sessionURL + '?userId=' + this.currentUser.getValue().id + '&trainingId=' + trainingId;
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
        if (!session) { return; }
        const sessions = this.sessions.getValue();
        sessions.push(session);
        this.sessions.next(sessions);
      });

      this.hubConnection.on('groupLeave', (groupName: string) => {
          const sessions = this.sessions.getValue();
          const indx = sessions.findIndex(s => s.name === groupName);
          sessions.splice(indx);
          this.sessions.next(sessions);
      });

      // receive new messages
      this.hubConnection.on('message', (message: Message) => {
        console.log(message);
        if (!message) { return; }
        // Get all the session to which the new message belongs
        const sessions = this.sessions.getValue();
        const sessionIndx = sessions.findIndex(s => s.name === message.sessionName);
        // Get the current session
        const currentSession = this.currentSession.getValue();
        // in case the sender is not me and this is not an open session, update the number of unread messages
        // TBD
        // Update the messages of the current session
        if (!sessions[sessionIndx].messages) { sessions[sessionIndx].messages = []; }
        sessions[sessionIndx].messages.push(message);
        this.sessions.next(sessions);
      });
    }
  }

  public joinToSession(trainingId: number) {
    const currentUser = this.currentUser.getValue();
    if (this.hubConnection && currentUser) {
      this.hubConnection.invoke('AddToSession', trainingId, currentUser.id);
    }
  }

  public leaveSession(sessionName: string) {
    const currentUser = this.currentUser.getValue();
    if (this.hubConnection && currentUser) {
      this.hubConnection.invoke('RemoveFromSession', sessionName, currentUser.id);
    }
  }
}
