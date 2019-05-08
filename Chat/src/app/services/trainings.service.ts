import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { Training } from '../models/Training';
import { HttpClient } from '@angular/common/http';
import { map, filter, combineLatest } from 'rxjs/operators';
import { ServiceBase } from './serviceBase';
import { User } from '../models/User';
import { ME } from '../helpers/mocks';
import { Router, RoutesRecognized } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class TrainingsService extends ServiceBase {

  protected hubType = 'training';
  private trainingsURL = 'api/trainings';

  private trainings = new BehaviorSubject<Training[]>([]);
  public trainings$ = this.trainings.asObservable();
  private subscription: Subscription;

  private currentUser: User =  ME; // till I add auth

  private currentIndex = new BehaviorSubject<number>(-1);
  public currentIndex$ = this.currentIndex.asObservable();

  private currentTraining = new BehaviorSubject<Training>(null);
  public currentTraining$ = this.currentTraining.asObservable();

  constructor(private httpService: HttpClient, private router: Router) {
    super();
    // initialize training hub
    // each time someone will change\ add training update from the server will be pushed to the client
    this.initHub();

    // retrieve all the trainings from the server
    this.getTrainingsFromServer().subscribe( res => {
        if (!res) { res = []; }
        this.trainings.next(res);
    });

    // get current selected training index
    this.router.events.pipe(
        filter(e => e instanceof RoutesRecognized),
        map(e => {
          console.log(e);
          return (e as RoutesRecognized).state.root.firstChild.params;

        }),
        map(params => {
          this.currentIndex.next(Number(params.id));
          console.log(params.id);
        } )
    ).subscribe();

    // update the selected training when the trainings change or the selected index changes
    this.currentIndex.pipe(
      combineLatest(this.trainings, (indx, items) => this.currentTraining.next(items[indx]))
    ).subscribe();
  }

  private getTrainingsFromServer(): Observable<Training[]> {
      return this.httpService.get<Training[]>(this.trainingsURL);
  }

  public startNewTraining(training: Training) {
    console.log('Saving new training..' + training.id);
    if (this.hubConnection) {
      this.hubConnection.invoke('NotifyAddAll', this.currentUser.id, training);
    }
  }

  protected subscribeToHub() {
    // first validate we have retrieved the messages from the server
    // then subscribe to training updates and addition
    if (this.hubConnection) {

      this.hubConnection.on('add', (training: Training) => {
        const arr = this.trainings.getValue();
        arr.push(training);
        this.trainings.next(arr);
      });

      this.hubConnection.on('update', (id: number, training: Training) => {
        const arr = this.trainings.getValue();
        const updatedItem = arr.find(tr => tr.id === id);
        const index = arr.indexOf(updatedItem);
        arr[index] = training;
        this.trainings.next(arr);
      });
    }
  }

}
