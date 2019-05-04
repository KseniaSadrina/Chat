import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

export abstract class ServiceBase {

    protected hubConnection: HubConnection | undefined;

    private fullURL: string;
    private hubURL = 'http://localhost:5000';
    protected abstract hubType: string;
    // hub

    protected initHub() {
        this.createHub();
        this.startHub();
        this.subscribeToHub();
    }

    private createHub() {
        // create hub connection
        this.fullURL = [this.hubURL, this.hubType].join('/');
        this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(this.fullURL, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
        }).configureLogging(signalR.LogLevel.Information)
        .build();
    }

    private startHub() {
        if (this.hubConnection) {
            // start hub connection
          this.hubConnection.start()
          .then(() => console.log(['Connection with', this.hubType, 'hub has started.'].join(" ")))
          .catch(err => console.error(err.toString()));
        }
    }

    protected abstract subscribeToHub();
}
