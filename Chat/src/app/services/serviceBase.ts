import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable, BehaviorSubject, Subscription, Subject } from 'rxjs';
import { User } from '../models/User';
import { CustomAuthService } from './custom-auth.service';
import { map } from 'rxjs/operators';

export abstract class ServiceBase {
  protected hubConnection: HubConnection | undefined;

  private fullURL: string;
  private hubURL = 'http://localhost:5000';
  protected currentUser: Observable<User>;

  protected isHubActive = new BehaviorSubject<boolean>(false);

  public constructor( protected authService: CustomAuthService,
                      protected hubType: string,
                      protected hubHandlers,
                      protected subscriptions: Subscription[]) {

    this.isHubActive.subscribe(change => this.onHubActiveChanged(change));
    this.currentUser = this.authService.currentUser$.pipe(
      map((user: User) => this.onUserUpdate(user)).bind(this)
    );

    this.currentUser.subscribe();
  }

  private onHubActiveChanged(isActive: boolean) {
    if (isActive) { this.activateService(); } else { this.deActivateService(); }
  }

  protected abstract activateService();

  protected deActivateService() {
    if (this.subscriptions) {
      this.subscriptions.forEach(sub => sub.unsubscribe());
      this.subscriptions = new Array();
    }
  }

  /// This method decides what to do with the hub connection every time the user logs in and out.
  /// It returns the user
  private onUserUpdate(user: User): User {
    const isActive = this.isHubActive.getValue();
    // logged out - stop the hub connection
    if (!user && isActive) {
      this.stopHub();
      // logged in
    } else if (user && !isActive) {
      this.initHub(user.accessToken);
    } else {
      console.log('user is changed while the hub was already activated?');
      console.log(user);
    }

    return user;
  }

  // hub destruction - clear executes all the other methods

  protected clearHub() {
    this.unSubscribeFromHub();
    this.stopHub();
    this.destroyHub();
  }

  private destroyHub() {
    // create hub connection
    this.hubConnection = null;
  }

  private stopHub() {
    if (this.hubConnection) {
      // stop hub connection
      this.hubConnection
        .stop()
        .then(() => {
          console.log(
            ["Connection with", this.hubType, "hub has stopped."].join(" ")
          );
          this.isHubActive.next(false);
        })
        .catch(err => {
          if (!err) {
            return;
          }
          console.error(err.toString());
        });
    }
  }

  protected unSubscribeFromHub() {
    if (this.hubConnection) {
      Object.keys(this.hubHandlers).forEach(key => this.hubConnection.off(this.hubHandlers[key]));
    }
  }

  // hub init - initHub executes all the other methods

  protected initHub(token: string) {
    this.createHub(token);
    this.startHub();
    this.subscribeToHub();
  }

  private createHub(token: string) {

    // create hub connection
    this.fullURL = [this.hubURL, this.hubType].join("/");
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.fullURL, {
        accessTokenFactory: () => token,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

  private startHub() {
    if (this.hubConnection) {
      // start hub connection
      this.hubConnection
        .start()
        .then(() => {
          console.log(
            ["Connection with", this.hubType, "hub has started."].join(" ")
          );
          this.isHubActive.next(true);
        })
        .catch(err => {
          if (!err) {
            return;
          }
          console.error(err.toString());
        });
    }
  }

  protected abstract subscribeToHub();

}
