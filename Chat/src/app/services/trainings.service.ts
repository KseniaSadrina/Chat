import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { Training } from '../models/Training';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ServiceBase } from './serviceBase';
import { User } from '../models/User';
import { ME } from '../helpers/mocks';

@Injectable({
  providedIn: 'root'
})

export class TrainingsService extends ServiceBase {
  
  protected hubType = "training";
  
  private trainingsURL = 'api/trainings';
  private trainings = new BehaviorSubject<Training[]>([]);
  public trainings$ = this.trainings.asObservable();
  private subscription: Subscription;

  private currentUser: User =  ME; // till I add auth

  constructor(private httpService: HttpClient) { 
    super();

    this.initHub();
    this.subscription = this.getTrainingsFromServer().subscribe( res => {
        if (!res) res = [];
        this.trainings.next(res);
        this.subscription.unsubscribe();
      });
    }
    
  private getTrainingsFromServer() : Observable<Training[]> {
    return this.httpService.get<Training[]>(this.trainingsURL);
  }
  
  public getTraining(id: number | string): Observable<Training> {
    return this.trainings$.pipe(
      // (+) before `id` turns the string into a number
      map((trainings: Training[]) => trainings.find(hero => hero.id === +id))
      );
    }
    
  public startNewTraining(training: Training) {
    console.log("Saving new training.." + training.id);
    if (this.hubConnection) {
      this.hubConnection.invoke('NotifyAddAll', this.currentUser.id, training);
    }
  }

  protected subscribeToHub() {
    // first validate we have retrieved the messages from the server
    // then subscribe to training updates and addition
    if (this.hubConnection) {

      this.hubConnection.on('add', (training: Training) => {
        let arr = this.trainings.getValue();
        arr.push(training);
        this.trainings.next(arr);
      });

      this.hubConnection.on('update', (id: number, training: Training) => {
        let arr = this.trainings.getValue();
        let updatedItem = arr.find(tr => tr.id === id);
        let index = arr.indexOf(updatedItem);
        arr[index] = training;
        this.trainings.next(arr);
      });
    }
  }
  
}
