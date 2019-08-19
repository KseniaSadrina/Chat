import { Injectable, ɵConsole } from '@angular/core';
import { Observable, BehaviorSubject, zip, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Message } from '../models/Message';
import { ServiceBase } from './serviceBase';
import { ChatSession } from '../models/chat-session';
import { combineLatest } from 'rxjs/operators';
import { TrainingsService } from './trainings.service';
import { CustomAuthService } from './custom-auth.service';
import { Session } from 'protractor';


@Injectable({
  providedIn: 'root'
})

export class SessionsService extends ServiceBase {

  sessionURL = 'api/chatSessions';

  private currentSession = new BehaviorSubject<ChatSession>(null);
  public currentSession$ = this.currentSession.asObservable();

  private sessions = new BehaviorSubject<ChatSession[]>([]);
  public sessions$ = this.sessions.asObservable();

  private currentTyper = new BehaviorSubject<string>(null);
  public currentTypes$ = this.currentTyper.asObservable();

  private unreadMessages = new BehaviorSubject< {[trainingId: number]: number}>({});
  public unreadMessages$ = this.unreadMessages.asObservable();

  constructor(private httpService: HttpClient,
              private trainings: TrainingsService,
              authService: CustomAuthService) {
    super(authService,
          'message',
          {
            group: 'group',
            groupJoin: 'groupJoin',
            groupLeave: 'groupLeave',
            message: 'message',
            startTyping: 'startTyping',
            stoppedTyping: 'stoppedTyping'
          } ,
          []);
  }

  // retrieves all the sessions of the user from the server by user id
  private getUserSessionsFromServer(): Observable<ChatSession[]> {
    const user = this.authService.currentUserValue;
    if (!user) { return null; }
    const fullURL = `${this.sessionURL}?userId=${user.id}`;
    return this.httpService.get<ChatSession[]>(fullURL);
  }

  public sendMessage(message: Message) {
    // send message
    if (this.hubConnection) {
      const session = this.currentSession.getValue();
      message.sessionName = session.name;
      message.chatSessionId = session.id;
      this.hubConnection.invoke('SendToAll', message);
    }
  }

  // ***** hub implementations *****

  protected subscribeToHub() {
    // first validate we have retrieved the messages from the server
    // then subscribe to new messages
    if (this.hubConnection) {

      this.hubConnection.on(this.hubHandlers.group, (notification: string) => {
        console.log(notification);
      });

      // receive new notifications about group activity
      this.hubConnection.on(this.hubHandlers.groupJoin, (session: ChatSession) => {
        if (!session) { return; }
        const sessions = this.sessions.getValue();
        sessions.push(session);
        this.sessions.next(sessions);
      });

      // receive training progress updates
      this.hubConnection.on(this.hubHandlers.groupLeave, (groupName: string) => {
          const sessions = this.sessions.getValue();
          const indx = sessions.findIndex(s => s.name === groupName);
          sessions.splice(indx);
          this.sessions.next(sessions);
      });

      // receive new messages
      this.hubConnection.on(this.hubHandlers.message, (message: Message) => {
        if (!message) { return; }
        // Get all the session to which the new message belongs
        const sessions = this.sessions.getValue();
        const sessionIndx = sessions.findIndex(s => s.name === message.sessionName);
        this.handleNewMessage(message);
        if (!sessions[sessionIndx].messages) { sessions[sessionIndx].messages = []; }
        sessions[sessionIndx].messages.push(message);
        this.sessions.next(sessions);
      });

       // receive start typing message
      this.hubConnection.on(this.hubHandlers.startTyping, (groupName: string, userName: string) => {
        const currSession = this.currentSession.getValue();
        console.log(`${userName} is typing`);
        if (currSession.name === groupName && this.authService.currentUserValue.userName !== userName) {
          console.log(`${userName} is typing and you should see that`);
          this.currentTyper.next(userName);
        }
      });

        // receive stopped typing message
      this.hubConnection.on(this.hubHandlers.stoppedTyping, (groupName: string) => {
          const currSession = this.currentSession.getValue();
          console.log(`stopped typing`);
          if (currSession.name === groupName) {
            this.currentTyper.next(null);
          }
        });

    }
  }

  protected unSubscribeFromHub() {
    this.sessions.getValue().forEach(session => this.leaveSession(session.name));
    super.unSubscribeFromHub();
  }

  public joinToSession(trainingId: number) {
    const currentUser = this.authService.currentUserValue;
    if (this.hubConnection && currentUser) {
      this.hubConnection.invoke('AddToSession', trainingId, currentUser.id);
    }
  }

  public leaveSession(sessionName: string) {
    const currentUser = this.authService.currentUserValue;
    if (this.hubConnection && currentUser) {
      this.hubConnection.invoke('RemoveFromSession', sessionName, currentUser.id);
    }
  }

  /// ***** Service activation and deactivation *****

  protected activateService() {
    // retrieve existing sessions from the server and re-join to them
    this.getUserSessionsFromServer()
    .toPromise()
    .then( res => {
      if (!res) { return; }
      res.forEach(item => this.joinToSession(item.trainingId));
    });

    const sub = this.sessions.pipe(
      combineLatest(this.trainings.currentTraining$, (sessions, training) => {
        // if the training is not null and the user has already joined the session in the past, switch the selected session
        if (training) {
          const session = sessions.find(s => s.trainingId === training.id);
          this.currentSession.next(session);
        }
      })
    ).subscribe();

    const currentSessionSub = this.currentSession.subscribe( res => {
      if (!res) { return; }
      this.removeAllUnreadMessages(res.trainingId);
    });

    this.subscriptions.push(sub);
    this.subscriptions.push(currentSessionSub);
  }

  protected deActivateService() {
    super.deActivateService();
    if (this.sessions) { this.sessions.next([]); }
    if (this.currentSession) { this.currentSession.next(null); }
  }

  // unread messages

  handleNewMessage(message: Message) {
    // in case the sender is not me and this is not an open session, update the number of unread messages
   const currentSession = this.currentSession.getValue();
   if (currentSession.name === message.sessionName ||
     message.sender === this.authService.currentUserValue.userName) { return; }
   const val = this.unreadMessages.getValue();
   const session = this.sessions.getValue().find(item => item.id === message.chatSessionId);
   if (!session) { return; }
   const currentValue = val[session.trainingId];
   if (!currentValue) { val[session.trainingId] = 1; } else {   val[session.trainingId] = currentValue + 1; }

   this.unreadMessages.next(val);
 }

  removeAllUnreadMessages(trainingId: number) {
    // in case the sender is not me and this is not an open session, update the number of unread messages
   const val = this.unreadMessages.getValue();
   val[trainingId] = 0;
   this.unreadMessages.next(val);
 }

}
