import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { Training } from '../models/Training';
import { HttpClient } from '@angular/common/http';
import { map, filter, combineLatest } from 'rxjs/operators';
import { ServiceBase } from './serviceBase';
import { User } from '../models/User';
import { Router, ActivationEnd } from '@angular/router';
import { CustomAuthService } from './custom-auth.service';
import { basename } from 'path';

@Injectable({
  providedIn: 'root'
})

export class TrainingsService extends ServiceBase {

  private trainingsURL = 'api/trainings';

  private trainings = new BehaviorSubject<Training[]>([]);
  public trainings$ = this.trainings.asObservable();

  private currentIndex = new BehaviorSubject<number>(-1);
  public currentIndex$ = this.currentIndex.asObservable();

  private currentTraining = new BehaviorSubject<Training>(null);
  public currentTraining$ = this.currentTraining.asObservable();

  constructor(private httpService: HttpClient,
              private router: Router,
              authService: CustomAuthService) {
    super(authService,
      'training',
      { add: 'add', update: 'update' } ,
      []);
    this.hubType = 'training';
  }

  private getTrainingsFromServer(): Observable<Training[]> {
    return this.httpService.get<Training[]>(this.trainingsURL);
  }

  public startNewTraining(training: Training) {
    console.log('Saving new training..' + training.id);
    if (this.hubConnection && this.authService.currentUserValue) {
      this.hubConnection.invoke('NotifyAddAll', this.authService.currentUserValue.id, training);
    }
  }

  protected subscribeToHub() {
    // first validate we have retrieved the messages from the server
    // then subscribe to training updates and addition
    if (this.hubConnection) {

      this.hubConnection.on(this.hubHandlers.add, (training: Training) => {
        const arr = this.trainings.getValue();
        arr.push(training);
        this.trainings.next(arr);
      });

      this.hubConnection.on(this.hubHandlers.update, (id: number, training: Training) => {
        const arr = this.trainings.getValue();
        const updatedItem = arr.find(tr => tr.id === id);
        const index = arr.indexOf(updatedItem);
        arr[index] = training;
        this.trainings.next(arr);
      });
    }
  }

  protected activateService() {

    this.getTrainingsFromServer()
    .toPromise()
    .then(res => {
      if (!res) { res = []; }
      this.trainings.next(res);
     });

    const routeSub = this.router.events.pipe(
    filter(e => e instanceof ActivationEnd && !e.snapshot.firstChild),
    map((e: ActivationEnd ) => e.snapshot.params),
    map(params => {
      this.currentIndex.next(Number(params.id));
    })
    ).subscribe();

    // update the selected training when the trainings change or the selected index changes
    const currentIndxSub = this.currentIndex.pipe(
      combineLatest(this.trainings, (indx, items) => this.currentTraining.next(items[indx]))
    ).subscribe();

    this.subscriptions.push(routeSub);
    this.subscriptions.push(currentIndxSub);

  }

  protected deActivateService() {
    super.deActivateService();
    if (this.currentIndex) { this.currentIndex.next(-1) };
    if (this.trainings) { this.trainings.next([]); }
    if (this.currentTraining) { this.currentTraining.next(null); }
  }

}
